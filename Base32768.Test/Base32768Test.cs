using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Kzrnm.Convert.Base32768.Models;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768Test
    {
        public static TheoryData<PairTestData> SimpleData { get; } = new TheoryData<PairTestData>
        {
            new(
                "ꡟ",
                "ꡟ",
                new byte[]{255}
            ),
            new(
                "ꡟꡟꡟꡟꡟꡟꡟꡟꡟ",
                "ꡟꡟꡟꡟꡟꡟꡟꡟꡟ",
                new byte[]{
                    255, 255, 255, 255,
                    255, 255, 255, 255,
                    255, 255, 255, 255,
                    255, 255, 255, 255
                }
            ),
            new(
                "ݠ暠䙠㙐▨ᖄቢႡဟ",
                "ݠ暠䙠㙐▨ᖄቢႡဟ",
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1
                }
            ),
            new(
                "ݠ暠䙠㙐▨ᖄቢႡݠʟ",
                "ݠ暠䙠㙐▨ᖄቢႡݠʟ",
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1, 1
                }
            ),
            new(
                "ݠ暠䙠㙐▨ᖄቢႡݠ曟",
                "ݠ暠䙠㙐▨ᖄቢႡݠ曟",
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1
                }
            )
        };

        public static TheoryData<PairTestData> NormalPairTestData { get; } = TestUtil.TestData
            .Select(p => p.Key)
            .Where(s => s.StartsWith("pairs.") && !s.StartsWith("pairs.single_bytes.") && s.EndsWith(".txt"))
            .Select(PairTestData.FromStringFileName)
            .ToTheoryData();


        public static TheoryData<string> EnumerateTestDataBad { get; } = TestUtil.TestData
            .Where(p => p.Key.StartsWith("bad.") && p.Key.EndsWith(".txt"))
            .Select(p => Encoding.UTF8.GetString(p.Value))
            .ToTheoryData();

        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void Encode(PairTestData data)
        {
            Base32768.Encode(data.Bytes).Should().Be(data.String);
        }
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void Decode(PairTestData data)
        {
            Base32768.Decode(data.String).Should().Equal(data.Bytes);
        }
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void EncodeStream(PairTestData data)
        {
            Base32768.Encode(data.Bytes.ToStream()).Should().Be(data.String);
        }


        [Theory]
        [MemberData(nameof(EnumerateTestDataBad))]
        public void BadDecode(string str)
        {
            str.Invoking(str => Base32768.Decode(str)).Should().Throw<FormatException>();
        }

        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void Base32768StreamDecode(PairTestData data)
        {
            using var decodedMemoryStream = new MemoryStream();
            using (var textReader = new StringReader(data.String))
            using (var decoder = new Base32768Stream(textReader))
            {
                decoder.CopyTo(decodedMemoryStream);
            }
            var gotBytes = decodedMemoryStream.ToArray();
            gotBytes.Should().Equal(data.Bytes);
        }
        [Fact]
        public void Large()
        {
            var str = Encoding.UTF8.GetString(TestUtil.TestData["pairs.every-pair-of-bytes.txt"]);
            var bytes = TestUtil.TestData["pairs.every-pair-of-bytes.bin"];
            using var decodedMemoryStream = new MemoryStream();
            using (var textReader = new StringReader(str))
            using (var decoder = new Base32768Stream(textReader))
            {
                decoder.CopyTo(decodedMemoryStream);
            }
            var gotBytes = decodedMemoryStream.ToArray();
            gotBytes.Should().Equal(bytes);
        }
#if NETCOREAPP3_1_OR_GREATER
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void EncodeSpan(PairTestData data)
        {
            Base32768.Encode(data.Bytes.AsSpan()).Should().Be(data.String);
        }
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void DecodeSpan(PairTestData data)
        {
            Base32768.Decode(data.String.AsSpan()).Should().Equal(data.Bytes);
        }

        [Theory]
        [MemberData(nameof(EnumerateTestDataBad))]
        public void BadDecodeSpan(string str)
        {
            str.Invoking(str => Base32768.Decode(str.AsSpan())).Should().Throw<FormatException>();
        }
#endif
    }
}
