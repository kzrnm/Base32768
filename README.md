# Base32768

[![NuGet version (Base32768)](https://img.shields.io/nuget/v/Base32768.svg?style=flat-square)](https://www.nuget.org/packages/Base32768/)
![build](https://github.com/kzrnm/Base32768/workflows/Build-Release-Publish/badge.svg?branch=master)

Base32768 is a binary encoding optimised for UTF-16-encoded text.

C# port of [Base32768](https://github.com/qntm/base32768)

## Installation

```
Install-Package Base32768
```

## Usage

```C#
using System;
using System.IO;
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


// Stream API
{
    using var textWriter = new StringWriter();
    using (var stream = new Base32768Stream(textWriter))
    {
        stream.Write(stackalloc byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 });
    }
    textWriter.ToString();
//// 6 code points, "媒腻㐤┖ꈳ埳"
}
{
    using var ms = new MemoryStream();
    using var textReader = new StringReader("媒腻㐤┖ꈳ埳");
    using (var stream = new Base32768Stream(textReader))
    {
        stream.CopyTo(ms);
    }
    Console.WriteLine(string.Join(", ", ms.ToArray()));
//// [104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100]
}


```

## Benchmark
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.100
  [Host]   : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  ShortRun : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method |            Toolchain | ByteSize |        Mean |        Error |      StdDev |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------- |--------------------- |--------- |------------:|-------------:|------------:|---------:|---------:|---------:|----------:|
| **Encode** |             **.NET 6.0** |   **100000** |    **156.8 μs** |     **10.20 μs** |     **0.56 μs** |  **66.6504** |  **66.6504** |  **66.6504** |    **209 KB** |
| **Encode** |             **.NET 6.0** | **10000000** | **21,763.1 μs** | **14,153.04 μs** |   **775.78 μs** | **187.5000** | **187.5000** | **187.5000** | **20,834 KB** |
|        |                      |          |             |              |             |          |          |          |           |
| **Decode** |             **.NET 6.0** |   **100000** |    **121.5 μs** |     **22.30 μs** |     **1.22 μs** |  **31.0059** |  **31.0059** |  **31.0059** |     **98 KB** |
| **Decode** |             **.NET 6.0** | **10000000** | **14,056.3 μs** | **15,942.99 μs** |   **873.89 μs** | **125.0000** | **125.0000** | **125.0000** |  **9,766 KB** |
|        |                      |          |             |              |             |          |          |          |           |
| **Encode** | **.NET Framework 4.7.2** |   **100000** |    **208.2 μs** |     **23.38 μs** |     **1.28 μs** |  **66.6504** |  **66.6504** |  **66.6504** |    **209 KB** |
| **Encode** | **.NET Framework 4.7.2** | **10000000** | **28,246.3 μs** | **25,344.77 μs** | **1,389.23 μs** | **250.0000** | **250.0000** | **250.0000** | **20,836 KB** |
|        |                      |          |             |              |             |          |          |          |           |
| **Decode** | **.NET Framework 4.7.2** |   **100000** |    **148.0 μs** |     **20.15 μs** |     **1.10 μs** |  **31.0059** |  **31.0059** |  **31.0059** |     **98 KB** |
| **Decode** | **.NET Framework 4.7.2** | **10000000** | **16,726.1 μs** | **23,495.08 μs** | **1,287.84 μs** | **187.5000** | **187.5000** | **187.5000** |  **9,766 KB** |
