using NimbleMock;
using NimbleMock.Exceptions;
using NimbleMock.Tests.Fixtures;
using Xunit;

namespace NimbleMock.Tests;

public class VerificationTests
{
    [Fact]
    public void Verify_Once_PassesWhenCalledOnce()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();

        mock.Object.GetById(1);

        mock.Verify(x => x.GetById(1)).Once();
    }

    [Fact]
    public void Verify_Once_FailsWhenNotCalled()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();

        Assert.Throws<VerificationException>(() => mock.Verify(x => x.GetById(1)).Once());
    }

    [Fact]
    public void Verify_Times_PassesWhenCalledCorrectTimes()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();

        mock.Object.GetById(1);
        mock.Object.GetById(1);
        mock.Object.GetById(1);

        mock.Verify(x => x.GetById(1)).Times(3);
    }

    [Fact]
    public void Verify_Never_PassesWhenNotCalled()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.Delete(1), null)
            .Build();

        mock.Verify(x => x.Delete(1)).Never();
    }

    [Fact]
    public void Verify_AtLeast_PassesWhenCalledEnoughTimes()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();

        mock.Object.GetById(1);
        mock.Object.GetById(1);

        mock.Verify(x => x.GetById(1)).AtLeast(2);
    }

    [Fact]
    public void VerifyNoOtherCalls_PassesWhenNoOtherCalls()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();

        mock.Object.GetById(1);

        mock.Verify(x => x.GetById(1)).Once();
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public void VerifyNoOtherCalls_FailsWhenOtherCallsExist()
    {
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();

        mock.Object.GetById(1);
        mock.Object.GetById(2);

        Assert.Throws<VerificationException>(() => mock.VerifyNoOtherCalls());
    }
}

