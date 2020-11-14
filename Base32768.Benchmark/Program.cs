using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Kzrnm.Convert.Base32768;
using System;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.ShortRun);
    }
}
class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}

[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByMethod)]
public class Md5VsSha256
{
    [Params(1000, 10000, 100000)]
    public int N;

    public byte[] bytes;
    public string str;

    [GlobalSetup]
    public void Setup()
    {
        bytes = new byte[N];
        new Random(42).NextBytes(bytes);

        str = Base32768.Encode(bytes);
    }

    [Benchmark]
    public string Encode() => Base32768.Encode(bytes);

    [Benchmark]
    public byte[] Decode() => Base32768.Decode(str);
}