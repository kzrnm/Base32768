using FluentAssertions;
using System;
using System.Text;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768Test
    {
        [Fact]
        public void Simple255()
        {
            Base32768.Encode(stackalloc byte[] { 255 }).Should().Be("ꡟ");
            Base32768.Encode(stackalloc byte[] { 255, 255 }).Should().Be("ꡟʟ");
            Base32768.Encode(stackalloc byte[] { 255, 255, 255 }).Should().Be("ꡟꡟ");
        }

        public static TheoryData EnumerateRandomBytes()
        {
            var rnd = new Random();

            var theoryData = new TheoryData<byte[]>();
            for (int i = 0; i < 1000; i++)
            {
                var bytes = new byte[rnd.Next(1, 100)];
                rnd.NextBytes(bytes);
                theoryData.Add(bytes);
            }
            return theoryData;
        }

        [Theory]
        [MemberData(nameof(EnumerateRandomBytes))]
        public void RandomBytes(byte[] bytes)
        {
            var str = Base32768.Encode(bytes);
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
            str.IsNormalized().Should().BeTrue();

            Base32768.Decode(str).Should().Equal(bytes);
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
        }

        public static TheoryData EnumeratePairTestData()
        {
            var testData = TestUtil.TestData;
            var theoryData = new TheoryData<string, byte[]>();
            foreach (var (name, val) in testData)
            {
                if (!name.StartsWith("test-data/pairs"))
                    continue;
                if (!name.EndsWith(".txt"))
                    continue;
                var binName = System.IO.Path.ChangeExtension(name, "bin");
                theoryData.Add(Encoding.UTF8.GetString(val), testData[binName]);
            }
            return theoryData;
        }

        [Theory]
        [MemberData(nameof(EnumeratePairTestData))]
        public void PairTestData(string str, byte[] bytes)
        {
            Base32768.Encode(bytes).Should().Be(str);
            Base32768.Decode(str).Should().Equal(bytes);
        }

        public static TheoryData EnumerateTestDataBad()
        {
            var testData = TestUtil.TestData;
            var theoryData = new TheoryData<string>();
            foreach (var (name, val) in testData)
            {
                if (!name.StartsWith("test-data/bad"))
                    continue;
                if (!name.EndsWith(".txt"))
                    continue;
                theoryData.Add(Encoding.UTF8.GetString(val));
            }
            return theoryData;
        }

        [Theory]
        [MemberData(nameof(EnumerateTestDataBad))]
        public void Bad(string str)
        {
            str.Invoking(str => Base32768.Decode(str)).Should().Throw<FormatException>();
        }
    }
}
