```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26100.7462)
Unknown processor
.NET SDK 9.0.308
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2


```
| Method             | Mean       | Error    | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------- |-----------:|---------:|----------:|------:|--------:|-------:|----------:|------------:|
| Moq_Verify         | 1,795.0 ns | 35.56 ns | 100.29 ns |  1.00 |    0.00 | 0.2575 |    2168 B |        1.00 |
| NSubstitute_Verify | 2,163.3 ns | 42.72 ns |  70.20 ns |  1.20 |    0.08 | 0.3433 |    2889 B |        1.33 |
| NimbleMock_Verify  |   584.5 ns | 19.31 ns |  56.93 ns |  0.33 |    0.04 | 0.0648 |     544 B |        0.25 |
