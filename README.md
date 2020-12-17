# Base32768

[![NuGet version (Base32768)](https://img.shields.io/nuget/v/Base32768.svg?style=flat-square)](https://www.nuget.org/packages/Base32768/)
![test](https://github.com/naminodarie/Base32768/workflows/Build-Release-Publish/badge.svg?branch=master)

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

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.101
  [Host]   : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  ShortRun : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method |     Toolchain |      N |       Mean |      Error |    StdDev |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------- |-------------- |------- |-----------:|-----------:|----------:|--------:|--------:|--------:|----------:|
| **Encode** | **.NET Core 5.0** |   **1000** |   **2.093 μs** |  **0.7857 μs** | **0.0431 μs** |  **0.5341** |       **-** |       **-** |   **2.19 KB** |
| Encode |        net472 |   1000 |   2.512 μs |  0.4415 μs | 0.0242 μs |  0.5608 |       - |       - |   2.31 KB |
| **Encode** | **.NET Core 5.0** | **100000** | **201.801 μs** |  **5.6550 μs** | **0.3100 μs** | **66.6504** | **66.6504** | **66.6504** | **208.44 KB** |
| Encode |        net472 | 100000 | 243.293 μs | 35.6847 μs | 1.9560 μs | 66.6504 | 66.6504 | 66.6504 | 208.93 KB |
|        |               |        |            |            |           |         |         |         |           |
| **Decode** | **.NET Core 5.0** |   **1000** |   **1.675 μs** |  **0.2148 μs** | **0.0118 μs** |  **0.2441** |       **-** |       **-** |      **1 KB** |
| Decode |        net472 |   1000 |   1.804 μs |  0.5857 μs | 0.0321 μs |  0.2441 |       - |       - |      1 KB |
| **Decode** | **.NET Core 5.0** | **100000** | **171.527 μs** | **31.7089 μs** | **1.7381 μs** | **31.0059** | **31.0059** | **31.0059** |  **97.68 KB** |
| Decode |        net472 | 100000 | 184.791 μs | 16.9163 μs | 0.9272 μs | 31.0059 | 31.0059 | 31.0059 |  97.68 KB |
