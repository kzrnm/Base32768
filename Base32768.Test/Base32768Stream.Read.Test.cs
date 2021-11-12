using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768StreamReadTest
    {
        [Fact]
        public void NullArgumentConstructor_ReadMode()
        {
            new Action(() => _ = new Base32768Stream((TextReader)null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("reader");
        }

        [Fact]
        public void ReadSizeZero()
        {
            using var sr = new Base32768Stream(new StringReader("foo"));
            sr.Read(new byte[0], 0, 0).Should().Be(0);
        }

        [Fact]
        public void ReadDisposedStream()
        {
            var sr = new Base32768Stream(new StringWriter());
            sr.Dispose();
            sr.Invoking(sr => sr.Read(null, 0, 0))
                .Should().Throw<ObjectDisposedException>()
                .Which.ObjectName.Should().Be("Base32768Stream");
        }

        [Fact]
        public void ReadWriteStream()
        {
            using var sr = new Base32768Stream(new StringWriter());
            sr.Invoking(sr => sr.Read(null, 0, 0))
                .Should().Throw<InvalidOperationException>().WithMessage("The stream is not readable.");
        }

        [Fact]
        public void ReadNullBuffer()
        {
            using var sr = new Base32768Stream(new StringReader("foo"));
            sr.Invoking(sr => sr.Read(null, 0, 0))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("buffer");
        }

        [Fact]
        public void ReadNegativeOffset()
        {
            using var sr = new Base32768Stream(new StringReader("foo"));
            sr.Invoking(sr => sr.Read(new byte[0], -1, 0))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("offset");
        }

        [Fact]
        public void ReadTooLargeCount()
        {
            using var sr = new Base32768Stream(new StringReader("foo"));
            sr.Invoking(sr => sr.Read(new byte[1], 0, 2))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
            sr.Invoking(sr => sr.Read(new byte[2], 0, 3))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
            sr.Invoking(sr => sr.Read(new byte[2], 1, 2))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
        }
    }
}
