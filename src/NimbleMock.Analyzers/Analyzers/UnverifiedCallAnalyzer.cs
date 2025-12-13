using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NimbleMock.Analyzers.Analyzers;

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

