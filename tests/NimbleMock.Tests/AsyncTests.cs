using NimbleMock;
using NimbleMock.Tests.Fixtures;
using Xunit;

namespace NimbleMock.Tests;

public class AsyncTests
{
    [Fact]
    public async Task SetupAsync_Task_ReturnsConfiguredValue()
    {
        var mock = Mock.Of<IUserRepository>()
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build();

        var result = await mock.Object.SaveAsync(new User());

        Assert.True(result);
    }

    [Fact]
    public async Task SetupAsync_ValueTask_ReturnsConfiguredValue()
    {
        var expectedUser = new User { Id = 1, Name = "Test" };
        
        var mock = Mock.Of<IAsyncRepository>()
            .SetupAsync(x => x.GetAsync(1), expectedUser)
            .Build();

        var result = await mock.Object.GetAsync(1);

        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task Verify_AsyncMethod_WorksCorrectly()
    {
        var mock = Mock.Of<IUserRepository>()
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build();

        await mock.Object.SaveAsync(new User());

        mock.Verify(x => x.SaveAsync(default!)).Once();
    }
}

