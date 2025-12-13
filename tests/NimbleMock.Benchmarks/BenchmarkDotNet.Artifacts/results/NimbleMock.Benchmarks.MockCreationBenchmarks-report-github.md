```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26100.7462)
Unknown processor
.NET SDK 9.0.308
  [Host] : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
| Method         | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|--------------- |-----:|------:|------:|--------:|------------:|
| Moq_CreateMock |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  MockCreationBenchmarks.Moq_CreateMock: Job-WGLWPJ(IterationCount=5, WarmupCount=3)
