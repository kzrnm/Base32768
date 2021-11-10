using System;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768Test : Base32768TestBase
    {
        [Theory]
        [MemberData(nameof(Simple_Data))]
        public void Simple(string str, byte[] bytes)
        {
            Base32768.Encode(bytes).Should().Be(str);
            Base32768.Decode(str).Should().Equal(bytes);
        }

        [Theory]
        [MemberData(nameof(Simple_Data))]
        public void SimpleStream(string str, byte[] bytes)
        {
            Base32768.Encode(bytes.ToStream()).Should().Be(str);
        }

#if NETCOREAPP3_1_OR_GREATER
        [Theory]
        [MemberData(nameof(Simple_Data))]
        public void SimpleSpan(string str, byte[] bytes)
        {
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
        }
#endif

#if !DEBUG
        [Theory]
        [MemberData(nameof(EnumerateRandomBytes))]
        public void RandomBytes(byte[] bytes)
        {
            var str = Base32768.Encode(bytes);
            str.IsNormalized().Should().BeTrue();

            Base32768.Encode(bytes.ToStream()).Should().Be(str);
            Base32768.Decode(str).Should().Equal(bytes);
#if NETCOREAPP3_1_OR_GREATER
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
#endif
        }

        [Theory]
        [MemberData(nameof(EnumeratePairTestData))]
        public void PairTestData(string str, byte[] bytes)
        {
            Base32768.Encode(bytes).Should().Be(str);
            Base32768.Decode(str).Should().Equal(bytes);
#if NETCOREAPP3_1_OR_GREATER
            Base32768.Encode(bytes.ToStream()).Should().Be(str);
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
#endif
        }
#endif

        [Theory]
        [MemberData(nameof(EnumerateTestDataBad))]
        public void Bad(string str)
        {
            str.Invoking(str => Base32768.Decode(str)).Should().Throw<FormatException>();
#if NETCOREAPP3_1_OR_GREATER
            str.Invoking(str => Base32768.Decode(str.AsSpan())).Should().Throw<FormatException>();
#endif
        }
    }
}
