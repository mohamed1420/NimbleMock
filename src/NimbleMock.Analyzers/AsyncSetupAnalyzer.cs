using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NimbleMock.Analyzers;

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

