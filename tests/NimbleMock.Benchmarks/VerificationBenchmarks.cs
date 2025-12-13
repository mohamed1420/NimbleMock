using BenchmarkDotNet.Attributes;
using Moq;
using NSubstitute;
using NimbleMock;

namespace NimbleMock.Benchmarks;

[MemoryDiagnoser]
public class VerificationBenchmarks
{
    private readonly Mock<IUserRepository> _moqMock;
    private readonly IUserRepository _nsubstituteMock;
    private readonly VerifiableMock<IUserRepository> _nimbleMock;

    public VerificationBenchmarks()
    {
        _moqMock = new Mock<IUserRepository>();
        _moqMock.Setup(x => x.GetById(1)).Returns(new User { Id = 1 });
        _moqMock.Object.GetById(1);

        _nsubstituteMock = Substitute.For<IUserRepository>();
        _nsubstituteMock.GetById(1).Returns(new User { Id = 1 });
        _nsubstituteMock.GetById(1);

        _nimbleMock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1 })
            .Build();
        _nimbleMock.Object.GetById(1);
    }

    [Benchmark(Baseline = true)]
    public void Moq_Verify()
    {
        _moqMock.Verify(x => x.GetById(1), Times.Once);
    }

    [Benchmark]
    public void NSubstitute_Verify()
    {
        _nsubstituteMock.Received(1).GetById(1);
    }

    [Benchmark]
    public void NimbleMock_Verify()
    {
        _nimbleMock.Verify(x => x.GetById(1)).Once();
    }
}

