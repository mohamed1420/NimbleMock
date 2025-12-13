using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NimbleMock.Analyzers.Tests;

public class UnverifiedCallAnalyzerTests
{
    [Fact]
    public async Task Analyzer_ReportsUnverifiedCall()
    {
        var test = @"
using NimbleMock;

public interface IRepo { int Get(int id); }

public class Test
{
    public void Method()
    {
        var mock = Mock.Of<IRepo>()
            .Setup(x => x.Get(1), 42)
            .Build();
        
        mock.Object.Get(1); // Should warn: call not verified
    }
}
";

        // TODO: Implement analyzer test using Microsoft.CodeAnalysis.Testing
        // This requires additional setup with DiagnosticResult
        await Task.CompletedTask;
    }
}

