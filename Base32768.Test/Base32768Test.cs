using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768Test
    {
        [Fact]
        public void DecodeNullString()
        {
            new Action(() => Base32768.Decode(null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("str");
        }
        [Fact]
        public void EncodeNullStream()
        {
            new Action(() => Base32768.Encode((Stream)null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("stream");
        }
        [Fact]
        public void EncodeNullBytes()
        {
            new Action(() => Base32768.Encode((byte[])null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("bytes");
        }
    }
}
