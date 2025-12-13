```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26100.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-WASLGY : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
| Method                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|----------------------- |---------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| Moq_CreateMock         | 45.10 μs | 5.204 μs | 0.805 μs |  1.00 |    0.00 | 1.2207 | 1.0986 |  10.37 KB |        1.00 |
| NSubstitute_CreateMock | 10.47 μs | 3.654 μs | 0.949 μs |  0.23 |    0.02 | 1.4648 |      - |  12.36 KB |        1.19 |
| NimbleMock_CreateMock  |       NA |       NA |       NA |     ? |       ? |     NA |     NA |        NA |           ? |
| NimbleMock_PartialMock |       NA |       NA |       NA |     ? |       ? |     NA |     NA |        NA |           ? |

Benchmarks with issues:
  MockCreationBenchmarks.NimbleMock_CreateMock: Job-WASLGY(IterationCount=5, WarmupCount=3)
  MockCreationBenchmarks.NimbleMock_PartialMock: Job-WASLGY(IterationCount=5, WarmupCount=3)
