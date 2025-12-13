using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace NimbleMock.Analyzers.Tests;

public class AsyncSetupAnalyzerTests
{
    [Fact]
    public async Task Analyzer_ReportsWrongSetupForAsyncMethod()
    {
        var test = @"
using NimbleMock;
using System.Threading.Tasks;

public interface IRepo { Task<int> GetAsync(int id); }

public class Test
{
    public void Method()
    {
        var mock = Mock.Of<IRepo>()
            .Setup(x => x.GetAsync(1), 42) // Should error: use SetupAsync
            .Build();
    }
}
";

        // TODO: Implement analyzer test using Microsoft.CodeAnalysis.Testing
        // This requires additional setup with DiagnosticResult
        await Task.CompletedTask;
    }
}

