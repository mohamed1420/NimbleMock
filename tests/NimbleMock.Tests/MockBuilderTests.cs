using NimbleMock;
using NimbleMock.Tests.Fixtures;
using Xunit;

namespace NimbleMock.Tests;

public class MockBuilderTests
{
    [Fact]
    public void Setup_ReturnsConfiguredValue()
    {
        var expectedUser = new User { Id = 1, Name = "John" };
        
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), expectedUser)
            .Build();

        var result = mock.Object.GetById(1);

        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task SetupAsync_ReturnsConfiguredValue()
    {
        var mock = Mock.Of<IUserRepository>()
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build();

        var result = await mock.Object.SaveAsync(new User());

        Assert.True(result);
    }

    [Fact]
    public async Task Throws_ThrowsConfiguredException()
    {
        var exception = new TimeoutException("API timeout");
        
        var mock = Mock.Of<IExternalApi>()
            .Throws(x => x.FetchDataAsync("health"), exception)
            .Build();

        var thrown = await Assert.ThrowsAsync<TimeoutException>(() => mock.Object.FetchDataAsync("health"));
        Assert.Equal("API timeout", thrown.Message);
    }
}

