using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Kzrnm.Convert.Base32768.Models;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768EncodeDecodeTest
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
            var (_, str, bytes) = data;
            Base32768.Encode(bytes).Should().Be(str);
        }
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void Decode(PairTestData data)
        {
            var (_, str, bytes) = data;
            Base32768.Decode(str).Should().Equal(bytes);
        }
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void EncodeStream(PairTestData data)
        {
            var (_, str, bytes) = data;
            Base32768.Encode(bytes.ToStream()).Should().Be(str);
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
            var (_, str, bytes) = data;
            using var decodedMemoryStream = new MemoryStream();
            using (var textReader = new StringReader(str))
            using (var decoder = new Base32768Stream(textReader))
            {
                decoder.CopyTo(decodedMemoryStream);
            }
            var gotBytes = decodedMemoryStream.ToArray();
            gotBytes.Should().Equal(bytes);
        }

        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void Base32768StreamDecodeReadByte(PairTestData data)
        {
            var (_, str, bytes) = data;
            using var reader = new StringReader(str);
            using var st = new Base32768Stream(reader);
            int b;
            var list = new List<byte>();
            while ((b = st.ReadByte()) >= 0)
            {
                list.Add((byte)b);
            }
            list.Should().Equal(bytes);
        }

        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void Base32768StreamDecodeRead(PairTestData data)
        {
            var (_, str, bytes) = data;
            foreach (int bufferSize in new[] { 1, 2, 3, 5, 7, 8, 9, 13, 14, 15, 16, 17, 20 })
            {
                using var reader = new StringReader(str);
                using var st = new Base32768Stream(reader);
                var buffer = new byte[bufferSize];
                int len;
                var list = new List<byte>();
                while ((len = st.Read(buffer, 0, buffer.Length)) > 0)
                {
                    list.AddRange(buffer.Take(len));
                }
                list.Should().Equal(bytes, "buffer size: {0} is positive", bufferSize);
            }
        }

#if NETCOREAPP3_1_OR_GREATER
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void EncodeSpan(PairTestData data)
        {
            var (_, str, bytes) = data;
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
        }
        [Theory]
        [MemberData(nameof(SimpleData))]
        [MemberData(nameof(NormalPairTestData))]
        public void DecodeSpan(PairTestData data)
        {
            var (_, str, bytes) = data;
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
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
