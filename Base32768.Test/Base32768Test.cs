using System;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768Test : Base32768TestBase
    {
        [Theory]
        [MemberData(nameof(AllPairData))]
        public void Encode(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
            Base32768.Encode(bytes).Should().Be(str);
        }
        [Theory]
        [MemberData(nameof(AllPairData))]
        public void Decode(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
            Base32768.Decode(str).Should().Equal(bytes);
        }
        [Theory]
        [MemberData(nameof(AllPairData))]
        public void Stream(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
            Base32768.Encode(bytes.ToStream()).Should().Be(str);
        }

        [Theory]
        [MemberData(nameof(EnumerateRandomBytes))]
        public void RandomBytes(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
            bytes.Should().NotBeNull();
            str.IsNormalized().Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(EnumerateTestDataBad))]
        public void BadDecode(string str)
        {
            str.Invoking(str => Base32768.Decode(str)).Should().Throw<FormatException>();
        }

#if NETCOREAPP3_1_OR_GREATER
        [Theory]
        [MemberData(nameof(AllPairData))]
        public void EncodeSpan(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
        }
        [Theory]
        [MemberData(nameof(AllPairData))]
        public void DecodeSpan(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
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
