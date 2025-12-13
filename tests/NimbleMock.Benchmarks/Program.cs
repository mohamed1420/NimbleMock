using BenchmarkDotNet.Running;
using NimbleMock.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

