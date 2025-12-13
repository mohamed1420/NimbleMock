using NimbleMock;
using NimbleMock.Tests.Fixtures;
using Xunit;

namespace NimbleMock.Tests;

public class PartialMockTests
{
    [Fact]
    public void PartialMock_OnlyMocksSpecifiedMethods()
    {
        var expectedUser = new User { Id = 1, Name = "Test" };
        
        var mock = Mock.Partial<IUserRepository>()
            .Only(x => x.GetById(1), expectedUser)
            .Build();

        var user = mock.Object.GetById(1);
        Assert.NotNull(user);
        Assert.Equal(expectedUser, user);
    }

    [Fact]
    public void PartialMock_UnmockedMethod_ThrowsNotImplementedException()
    {
        var mock = Mock.Partial<IUserRepository>()
            .Only(x => x.GetById(1), new User { Id = 1 })
            .Build();

        // Unmocked methods should throw NotImplementedException
        Assert.Throws<NotImplementedException>(() => mock.Object.Delete(1));
    }
}

