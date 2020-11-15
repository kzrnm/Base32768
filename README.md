# Base32768

[![NuGet version (ac-library-csharp)](https://img.shields.io/nuget/v/Base32768.svg?style=flat-square)](https://www.nuget.org/packages/ac-library-csharp/)
![test](https://github.com/naminodarie/Base32768/workflows/test/badge.svg?branch=master)

Base32768 is a binary encoding optimised for UTF-16-encoded text.

C# port of [Base32768](https://github.com/qntm/base32768)

## Installation

```
Install-Package Base32768
```

## Usage

```C#
using System;
using Kzrnm.Convert.Base32768;

var byteArray = new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 };
var str = Base32768.Encode(byteArray);
Console.WriteLine(str);
//// 6 code points, "媒腻㐤┖ꈳ埳"

var byteArray2 = Base32768.Decode(str);
Console.WriteLine(string.Join(", ", byteArray2));
//// [104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100]

// .NET Standard 2.1
str = Base32768.Encode(stackalloc byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 });
Console.WriteLine(str);
//// 6 code points, "媒腻㐤┖ꈳ埳"

byteArray2 = Base32768.Decode(str.AsSpan());
Console.WriteLine(string.Join(", ", byteArray2));
//// [104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100]
```

## Benchmark

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1198 (1909/November2018Update/19H2)
Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]   : .NET Core 3.1.10 (CoreCLR 4.700.20.51601, CoreFX 4.700.20.51901), X64 RyuJIT
  ShortRun : .NET Core 3.1.10 (CoreCLR 4.700.20.51601, CoreFX 4.700.20.51901), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

| Method |      N |         Mean |        Error |     StdDev |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------- |------- |-------------:|-------------:|-----------:|--------:|--------:|--------:|----------:|
| Encode |   1000 |     3.521 us |     2.569 us |  0.1408 us |  0.5341 |       - |       - |   2.19 KB |
| Encode |  10000 |    36.773 us |    11.450 us |  0.6276 us |  5.1270 |  0.0610 |       - |  20.94 KB |
| Encode | 100000 |   373.452 us |   130.075 us |  7.1299 us | 62.0117 | 62.0117 | 62.0117 | 208.87 KB |
|        |        |              |              |            |         |         |         |           |
| Decode |   1000 |    12.640 us |     4.268 us |  0.2339 us |  0.2441 |       - |       - |      1 KB |
| Decode |  10000 |   147.647 us |    13.668 us |  0.7492 us |  2.1973 |       - |       - |   9.79 KB |
| Decode | 100000 | 1,534.777 us | 1,081.132 us | 59.2605 us | 29.2969 | 29.2969 | 29.2969 |  97.68 KB |