// NimbleMock.Analyzers/UnverifiedCallAnalyzer.cs
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NimbleMock.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnverifiedCallAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "NMOCK001",
        title: "Mock call not verified",
        messageFormat: "Call to '{0}' on mock is not verified",
        category: "Testing",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All mock calls should be verified with .Verify()");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        
        // Find mock variables
        var mocks = FindMockVariables(method);
        if (mocks.Count == 0) return;

        // Find mock calls
        var calls = FindMockCalls(method, mocks);
        
        // Find verifications
        var verifications = FindVerifications(method, mocks);

        // Report unverified calls
        foreach (var call in calls)
        {
            if (!verifications.Contains(call.MethodName))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    call.Location,
                    call.MethodName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private HashSet<string> FindMockVariables(MethodDeclarationSyntax method)
    {
        var mocks = new HashSet<string>();
        
        foreach (var variable in method.DescendantNodes()
            .OfType<VariableDeclaratorSyntax>())
        {
            if (variable.Initializer?.Value is InvocationExpressionSyntax invocation &&
                IsMockBuildCall(invocation))
            {
                mocks.Add(variable.Identifier.ValueText);
            }
        }
        
        return mocks;
    }

    private List<(string MethodName, Location Location)> FindMockCalls(
        MethodDeclarationSyntax method,
        HashSet<string> mocks)
    {
        var calls = new List<(string, Location)>();
        
        foreach (var memberAccess in method.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>())
        {
            if (memberAccess.Expression is MemberAccessExpressionSyntax objectAccess &&
                objectAccess.Name.Identifier.ValueText == "Object" &&
                objectAccess.Expression is IdentifierNameSyntax mockVar &&
                mocks.Contains(mockVar.Identifier.ValueText))
            {
                calls.Add((
                    memberAccess.Name.Identifier.ValueText,
                    memberAccess.GetLocation()));
            }
        }
        
        return calls;
    }

    private HashSet<string> FindVerifications(
        MethodDeclarationSyntax method,
        HashSet<string> mocks)
    {
        var verified = new HashSet<string>();
        
        foreach (var invocation in method.DescendantNodes()
            .OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Verify" &&
                memberAccess.Expression is IdentifierNameSyntax mockVar &&
                mocks.Contains(mockVar.Identifier.ValueText))
            {
                // Extract method name from lambda
                if (invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression 
                    is SimpleLambdaExpressionSyntax lambda &&
                    lambda.Body is InvocationExpressionSyntax lambdaInvocation &&
                    lambdaInvocation.Expression is MemberAccessExpressionSyntax lambdaMember)
                {
                    verified.Add(lambdaMember.Name.Identifier.ValueText);
                }
            }
        }
        
        return verified;
    }

    private bool IsMockBuildCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return false;
            
        return memberAccess.Name.Identifier.ValueText == "Build";
    }
}

// NimbleMock.Analyzers/AsyncSetupAnalyzer.cs
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncSetupAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "NMOCK002",
        title: "Use SetupAsync for async methods",
        messageFormat: "Method '{0}' returns Task/ValueTask, use SetupAsync instead of Setup",
        category: "Testing",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSetupCall, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeSetupCall(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess ||
            memberAccess.Name.Identifier.ValueText != "Setup")
            return;

        var semanticModel = context.SemanticModel;
        var symbolInfo = semanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        
        if (symbolInfo.Symbol is not IMethodSymbol setupMethod)
            return;

        // Check if lambda returns Task/ValueTask
        if (invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression 
            is SimpleLambdaExpressionSyntax lambda)
        {
            var lambdaSymbol = semanticModel.GetSymbolInfo(lambda, context.CancellationToken);
            if (lambdaSymbol.Symbol is IMethodSymbol { ReturnType: INamedTypeSymbol returnType })
            {
                var isTask = returnType.Name is "Task" or "ValueTask";
                if (isTask)
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        memberAccess.Name.GetLocation(),
                        lambda.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}

// NimbleMock.Analyzers/PartialMockAnalyzer.cs
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PartialMockAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "NMOCK003",
        title: "Consider using Partial mock",
        messageFormat: "Interface has {0} methods but only {1} are setup, consider Mock.Partial()",
        category: "Testing",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Large interfaces with few setups benefit from partial mocks");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMockCreation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeMockCreation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        
        if (!IsMockOfCall(invocation))
            return;

        var semanticModel = context.SemanticModel;
        var symbolInfo = semanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        
        if (symbolInfo.Symbol is not IMethodSymbol mockOfMethod)
            return;

        var interfaceType = mockOfMethod.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
        if (interfaceType is null)
            return;

        var totalMethods = interfaceType.GetMembers()
            .OfType<IMethodSymbol>()
            .Count(m => m.MethodKind == MethodKind.Ordinary);

        if (totalMethods < 5) return; // Only suggest for larger interfaces

        // Count setups in the chain
        var setupCount = CountSetups(invocation.Parent);

        if (setupCount < totalMethods / 2)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                invocation.GetLocation(),
                totalMethods,
                setupCount);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private bool IsMockOfCall(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax
        {
            Name.Identifier.ValueText: "Of",
            Expression: IdentifierNameSyntax { Identifier.ValueText: "Mock" }
        };
    }

    private int CountSetups(SyntaxNode? node)
    {
        var count = 0;
        var current = node;
        
        while (current is not null)
        {
            if (current is InvocationExpressionSyntax inv &&
                inv.Expression is MemberAccessExpressionSyntax member &&
                member.Name.Identifier.ValueText is "Setup" or "SetupAsync")
            {
                count++;
            }
            
            current = current.Parent;
        }
        
        return count;
    }
}

// NimbleMock.Analyzers/CodeFixes/UseSetupAsyncCodeFix.cs
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class UseSetupAsyncCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds 
        => ImmutableArray.Create("NMOCK002");

    public override FixAllProvider GetFixAllProvider() 
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var invocation = root?.FindToken(diagnosticSpan.Start)
            .Parent?.AncestorsAndSelf()
            .OfType<InvocationExpressionSyntax>()
            .First();

        if (invocation is null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Use SetupAsync",
                createChangedDocument: ct => UseSetupAsync(context.Document, invocation, ct),
                equivalenceKey: "UseSetupAsync"),
            diagnostic);
    }

    private async Task<Document> UseSetupAsync(
        Document document,
        InvocationExpressionSyntax invocation,
        CancellationToken ct)
    {
        var root = await document.GetSyntaxRootAsync(ct);
        if (root is null) return document;

        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
        var newMemberAccess = memberAccess.WithName(
            SyntaxFactory.IdentifierName("SetupAsync"));

        var newInvocation = invocation.WithExpression(newMemberAccess);
        var newRoot = root.ReplaceNode(invocation, newInvocation);

        return document.WithSyntaxRoot(newRoot);
    }
}

// Diagnostic IDs:
// NMOCK001: Unverified mock call
// NMOCK002: Use SetupAsync for async methods  
// NMOCK003: Consider using Partial mock
// NMOCK004: Duplicate setup (future)
// NMOCK005: Setup never called (future)