using System;
using Xunit;
using NimbleMock;
using NimbleMock.Exceptions;

namespace NimbleMock.Tests;

public class StaticMockTests
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
    
    [Fact]
    public void StaticMock_CanCreateBuilder()
    {
        var builder = Mock.Static<IDateTimeProvider>();
        var _ = builder;
        Assert.True(true);
    }
    
    [Fact]
    public void StaticMock_CanSetupAndBuild()
    {
        var fixedDate = new DateTime(2024, 1, 1, 12, 0, 0);
        var mock = Mock.Static<IDateTimeProvider>()
            .Returns(d => d.Now, fixedDate)
            .Build();
        
        Assert.NotNull(mock);
    }
    
    [Fact]
    public void StaticMock_ThrowsWhenAccessingWrapped()
    {
        var fixedDate = new DateTime(2024, 1, 1, 12, 0, 0);
        var mock = Mock.Static<IDateTimeProvider>()
            .Returns(d => d.Now, fixedDate)
            .Build();
        
        Assert.Throws<InvalidOperationException>(() => _ = mock.Wrapped);
    }
    
    [Fact]
    public void StaticMock_CanVerifyCalls()
    {
        var fixedDate = new DateTime(2024, 1, 1, 12, 0, 0);
        var mock = Mock.Static<IDateTimeProvider>()
            .Returns(d => d.Now, fixedDate)
            .Build();
        
        mock.Verify(d => d.Now).Never();
    }
}

