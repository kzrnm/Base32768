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
| **Encode** |             **.NET 6.0** |   **100000** |    **110.4 μs** |     **42.13 μs** |     **2.31 μs** |  **66.6504** |  **66.6504** |  **66.6504** |    **208 KB** |
| **Encode** |             **.NET 6.0** | **10000000** | **22,626.8 μs** | **28,393.61 μs** | **1,556.35 μs** | **187.5000** | **187.5000** | **187.5000** | **20,834 KB** |
|        |                      |          |             |              |             |          |          |          |           |
| **Decode** |             **.NET 6.0** |   **100000** |    **128.2 μs** |     **92.19 μs** |     **5.05 μs** |  **31.1279** |  **31.1279** |  **31.1279** |     **98 KB** |
| **Decode** |             **.NET 6.0** | **10000000** | **18,278.7 μs** | **22,586.75 μs** | **1,238.06 μs** | **156.2500** | **156.2500** | **156.2500** |  **9,766 KB** |
|        |                      |          |             |              |             |          |          |          |           |
| **Encode** | **.NET Framework 4.7.2** |   **100000** |    **158.4 μs** |     **74.28 μs** |     **4.07 μs** |  **66.6504** |  **66.6504** |  **66.6504** |    **209 KB** |
| **Encode** | **.NET Framework 4.7.2** | **10000000** | **24,085.8 μs** | **29,145.78 μs** | **1,597.58 μs** | **187.5000** | **187.5000** | **187.5000** | **20,834 KB** |
|        |                      |          |             |              |             |          |          |          |           |
| **Decode** | **.NET Framework 4.7.2** |   **100000** |    **154.4 μs** |      **8.75 μs** |     **0.48 μs** |  **31.0059** |  **31.0059** |  **31.0059** |     **98 KB** |
| **Decode** | **.NET Framework 4.7.2** | **10000000** | **17,264.4 μs** |  **5,327.63 μs** |   **292.03 μs** | **125.0000** | **125.0000** | **125.0000** |  **9,766 KB** |

## Sample

- Web Assembly sample  
https://kzrnm.github.io/Base32768/