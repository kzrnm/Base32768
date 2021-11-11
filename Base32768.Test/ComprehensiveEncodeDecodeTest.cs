using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class ComprehensiveEncodeDecodeTest
    {
        public static IEnumerable<byte[]> RandomBytesData { get; } = Enumerable.Repeat(new Random(227), 1000)
            .Select((rnd, i) =>
            {
                var bytes = new byte[rnd.Next(1, 1000)];
                rnd.NextBytes(bytes);
                return bytes;
            })
            .ToArray();

        public static IEnumerable<(string, byte[])> SingleBytesPairTestData { get; } = TestUtil.TestData
            .Where(p => p.Key.StartsWith("pairs.single_bytes.") && p.Key.EndsWith(".txt"))
            .Select(p => (Encoding.UTF8.GetString(p.Value), TestUtil.TestData[System.IO.Path.ChangeExtension(p.Key, "bin")]))
            .ToArray();


        [Fact]
        public void RandomBytes()
        {
            foreach (var bytes in RandomBytesData)
            {
                bytes.Should().NotBeNull($"Item of {nameof(RandomBytesData)} must not be null");
                var str = Base32768.Encode(bytes);
                str.IsNormalized().Should().BeTrue();

                EncodeAndDecode(str, bytes);
            }
        }

        [Fact]
        public void SingleBytesPair()
        {
            foreach (var (str, bytes) in SingleBytesPairTestData)
            {
                EncodeAndDecode(str, bytes);
            }
        }

        private void EncodeAndDecode(string str, byte[] bytes)
        {
            Base32768.Encode(bytes).Should().Be(str);
            Base32768.Decode(str).Should().Equal(bytes);
            Base32768.Encode(bytes.ToStream()).Should().Be(str);
#if NETCOREAPP3_1_OR_GREATER
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
#endif
        }
    }
}
