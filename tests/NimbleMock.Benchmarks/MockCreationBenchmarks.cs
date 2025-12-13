using BenchmarkDotNet.Attributes;
using Moq;
using NSubstitute;
using NimbleMock;
using NimbleMock.Benchmarks;

namespace NimbleMock.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class MockCreationBenchmarks
{
    private User _testUser = new() { Id = 1, Name = "Test" };

    [Benchmark(Baseline = true)]
    public IUserRepository Moq_CreateMock()
    {
        var mock = new Mock<IUserRepository>();
        mock.Setup(x => x.GetById(1)).Returns(_testUser);
        mock.Setup(x => x.SaveAsync(It.IsAny<User>())).ReturnsAsync(true);
        return mock.Object;
    }

    [Benchmark]
    public IUserRepository NSubstitute_CreateMock()
    {
        var mock = Substitute.For<IUserRepository>();
        mock.GetById(1).Returns(_testUser);
        mock.SaveAsync(Arg.Any<User>()).Returns(true);
        return mock;
    }

    [Benchmark]
    public IUserRepository NimbleMock_CreateMock()
    {
        return Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), _testUser)
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build()
            .Object;
    }

    [Benchmark]
    public IUserRepository NimbleMock_PartialMock()
    {
        return Mock.Partial<IUserRepository>()
            .Only(x => x.GetById(1), _testUser)
            .Build()
            .Object;
    }
}


