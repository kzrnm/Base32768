using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using Kzrnm.Convert.Base32768;
using System;

_ = BenchmarkRunner.Run(typeof(Base32768Benchmark).Assembly);


public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        AddJob(Job.ShortRun.WithToolchain(CsProjCoreToolchain.NetCoreApp50));
        AddJob(Job.ShortRun.WithToolchain(CsProjClassicNetToolchain.Net472));
    }
}

[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByMethod)]
public class Base32768Benchmark
{
    [Params(1000, 100000)]
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