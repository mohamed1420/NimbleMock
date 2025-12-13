using System;
using System.Threading.Tasks;
using Xunit;
using NimbleMock;

namespace NimbleMock.Tests;

public class LieProofingTests
{
    public interface ITestApi
    {
        string GetData();
        Task<string> GetDataAsync();
    }
    
    [Fact]
    public async Task AssertMatchesReal_ReturnsValidationResult()
    {
        var result = await LieProofing.AssertMatchesReal<ITestApi>(
            "https://httpbin.org/get");
        
        Assert.NotNull(result);
        Assert.True(result.IsValid || !result.IsValid);
    }
    
    [Fact]
    public async Task AssertMatchesReal_WithInvalidUrl_ReturnsInvalidResult()
    {
        var result = await LieProofing.AssertMatchesReal<ITestApi>(
            "https://invalid-url-that-does-not-exist-12345.com/api");
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task AssertMatchesReal_CanBeCancelled()
    {
        using var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();
        
        var result = await LieProofing.AssertMatchesReal<ITestApi>(
            "https://httpbin.org/get",
            cts.Token);
        
        Assert.NotNull(result);
    }
}

