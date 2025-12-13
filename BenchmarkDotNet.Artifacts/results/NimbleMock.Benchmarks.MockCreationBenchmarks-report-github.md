```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26100.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-IPAGEW : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
| Method                 | Mean        | Error       | StdDev      | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|----------------------- |------------:|------------:|------------:|------:|-------:|-------:|----------:|------------:|
| Moq_CreateMock         | 48,811.7 ns | 16,101.7 ns | 4,181.57 ns |  1.00 | 1.2207 | 1.0986 |  10.37 KB |        1.00 |
| NSubstitute_CreateMock |  9,936.7 ns |    983.5 ns |   255.41 ns |  0.20 | 1.4648 |      - |  12.36 KB |        1.19 |
| NimbleMock_CreateMock  |  1,415.0 ns |    134.8 ns |    35.01 ns |  0.03 | 0.4215 |      - |   3.45 KB |        0.33 |
| NimbleMock_PartialMock |    849.2 ns |    219.9 ns |    57.12 ns |  0.02 | 0.2623 |      - |   2.15 KB |        0.21 |
