using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using Kzrnm.Convert.Base32768;

_ = BenchmarkRunner.Run(typeof(Base32768Benchmark).Assembly);


public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        AddJob(Job.ShortRun.WithToolchain(CsProjCoreToolchain.NetCoreApp60));
        AddJob(Job.ShortRun.WithToolchain(CsProjClassicNetToolchain.Net472));
    }
}

[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(
    BenchmarkLogicalGroupRule.ByJob,
    BenchmarkLogicalGroupRule.ByCategory,
    BenchmarkLogicalGroupRule.ByMethod)]
public class Base32768Benchmark
{
    [Params(1_00_000, 10_000_000)]
    public int ByteSize;

    public byte[] bytes;
    public string str;

    [GlobalSetup]
    public void Setup()
    {
        bytes = new byte[ByteSize];
        new Random(42).NextBytes(bytes);

        str = Base32768.Encode(bytes);
    }

    [Benchmark]
    public string Encode() => Base32768.Encode(bytes);

    [Benchmark]
    public byte[] Decode() => Base32768.Decode(str);
}
