# Base32768

Base32768 is a binary encoding optimised for UTF-16-encoded text.

C# port of [Base32768](https://github.com/qntm/base32768)

## Usage

TODO:

## Benchmark

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1198 (1909/November2018Update/19H2)
Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]   : .NET Core 3.1.10 (CoreCLR 4.700.20.51601, CoreFX 4.700.20.51901), X64 RyuJIT
  ShortRun : .NET Core 3.1.10 (CoreCLR 4.700.20.51601, CoreFX 4.700.20.51901), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

| Method |      N |         Mean |       Error |     StdDev |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------- |------- |-------------:|------------:|-----------:|--------:|--------:|--------:|----------:|
| Encode |   1000 |     3.875 us |   0.1575 us |  0.0086 us |  0.5341 |       - |       - |   2.19 KB |
| Encode |  10000 |    29.217 us |  14.0340 us |  0.7693 us |  5.1270 |  0.0305 |       - |  20.94 KB |
| Encode | 100000 |   329.112 us | 158.9030 us |  8.7100 us | 62.0117 | 62.0117 | 62.0117 |  208.9 KB |
|        |        |              |             |            |         |         |         |           |
| Decode |   1000 |    12.293 us |   5.1508 us |  0.2823 us |  0.2441 |       - |       - |      1 KB |
| Decode |  10000 |   146.029 us |  74.8056 us |  4.1003 us |  2.1973 |       - |       - |   9.79 KB |
| Decode | 100000 | 1,490.583 us | 968.7149 us | 53.0985 us | 29.2969 | 29.2969 | 29.2969 |  97.68 KB |