using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace NimbleMock.Analyzers;

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

        var interfaceType = mockOfMethod.TypeArguments.OfType<INamedTypeSymbol>().FirstOrDefault();
        if (interfaceType is null)
            return;

        var totalMethods = interfaceType.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary)
            .Count();

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

