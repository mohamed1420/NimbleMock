using NimbleMock;
using Moq;

namespace NimbleMock.Examples;

public class MigrationFromMoq
{
    public static void Comparison()
    {
        // Moq approach
        var moqMock = new Mock<IUserRepository>();
        moqMock.Setup(x => x.GetById(1)).Returns(new User { Id = 1 });
        moqMock.Setup(x => x.SaveAsync(It.IsAny<User>())).ReturnsAsync(true);
        var moqRepo = moqMock.Object;
        moqRepo.GetById(1);
        moqMock.Verify(x => x.GetById(1), Times.Once);

        // NimbleMock approach (more concise)
        var nimbleMock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build();
        nimbleMock.Object.GetById(1);
        nimbleMock.Verify(x => x.GetById(1)).Once();
    }
}

