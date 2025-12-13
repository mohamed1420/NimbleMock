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
    public void SetupAsync_ReturnsConfiguredValue()
    {
        var mock = Mock.Of<IUserRepository>()
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build();

        var result = mock.Object.SaveAsync(new User()).Result;

        Assert.True(result);
    }

    [Fact]
    public void Throws_ThrowsConfiguredException()
    {
        var exception = new TimeoutException("API timeout");
        
        var mock = Mock.Of<IExternalApi>()
            .Throws(x => x.HealthCheck(), exception)
            .Build();

        var thrown = Assert.Throws<TimeoutException>(() => mock.Object.HealthCheck());
        Assert.Equal("API timeout", thrown.Message);
    }
}

