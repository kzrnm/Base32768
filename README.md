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

| Method |      N |       Mean |      Error |     StdDev |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------- |------- |-----------:|-----------:|-----------:|--------:|--------:|--------:|----------:|
| Encode |   1000 |   3.756 us |   2.003 us |  0.1098 us |  0.5341 |       - |       - |   2.19 KB |
| Encode |  10000 |  37.181 us |   7.361 us |  0.4035 us |  5.1270 |  0.0610 |       - |  20.94 KB |
| Encode | 100000 | 372.515 us | 116.843 us |  6.4046 us | 66.4063 | 66.4063 | 66.4063 | 208.44 KB |
|        |        |            |            |            |         |         |         |           |
| Decode |   1000 |   5.539 us |   3.804 us |  0.2085 us |  0.2441 |       - |       - |      1 KB |
| Decode |  10000 |  54.333 us |  11.788 us |  0.6461 us |  2.3804 |       - |       - |   9.79 KB |
| Decode | 100000 | 553.683 us | 192.352 us | 10.5435 us | 30.2734 | 30.2734 | 30.2734 |  97.68 KB |