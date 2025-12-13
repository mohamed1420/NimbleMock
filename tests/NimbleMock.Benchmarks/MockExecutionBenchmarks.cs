using BenchmarkDotNet.Attributes;
using Moq;
using NSubstitute;
using NimbleMock;

namespace NimbleMock.Benchmarks;

[MemoryDiagnoser]
public class MockExecutionBenchmarks
{
    private readonly IUserRepository _moqMock;
    private readonly IUserRepository _nsubstituteMock;
    private readonly IUserRepository _nimbleMock;
    private readonly User _user = new() { Id = 1, Name = "Test" };

    public MockExecutionBenchmarks()
    {
        var moq = new Mock<IUserRepository>();
        moq.Setup(x => x.GetById(1)).Returns(_user);
        _moqMock = moq.Object;

        _nsubstituteMock = Substitute.For<IUserRepository>();
        _nsubstituteMock.GetById(1).Returns(_user);

        _nimbleMock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), _user)
            .Build()
            .Object;
    }

    [Benchmark(Baseline = true)]
    public User Moq_ExecuteCall() => _moqMock.GetById(1);

    [Benchmark]
    public User NSubstitute_ExecuteCall() => _nsubstituteMock.GetById(1);

    [Benchmark]
    public User NimbleMock_ExecuteCall() => _nimbleMock.GetById(1);
}

