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
        public void ReadWriteStream()
        {
            using var st = new Base32768Stream(new StringWriter());
            st.Invoking(st => st.Read(null, 0, 0))
                .Should().Throw<InvalidOperationException>().WithMessage("The stream is not readable.");
        }

        [Fact]
        public void ReadSizeZero()
        {
            using var st = new Base32768Stream(new StringReader("foo"));
            st.Read(new byte[0], 0, 0).Should().Be(0);
        }

        [Fact]
        public void ReadDisposedStream()
        {
            var st = new Base32768Stream(new StringWriter());
            st.Dispose();
            st.Invoking(st => st.Read(null, 0, 0))
                .Should().Throw<ObjectDisposedException>()
                .Which.ObjectName.Should().Be("Base32768Stream");
        }

        [Fact]
        public void ReadNullBuffer()
        {
            using var st = new Base32768Stream(new StringReader("foo"));
            st.Invoking(st => st.Read(null, 0, 0))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("buffer");
        }

        [Fact]
        public void ReadNegativeOffset()
        {
            using var st = new Base32768Stream(new StringReader("foo"));
            st.Invoking(st => st.Read(new byte[0], -1, 0))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("offset");
        }

        [Fact]
        public void ReadTooLargeCount()
        {
            using var st = new Base32768Stream(new StringReader("foo"));
            st.Invoking(st => st.Read(new byte[1], 0, 2))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
            st.Invoking(st => st.Read(new byte[2], 0, 3))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
            st.Invoking(st => st.Read(new byte[2], 1, 2))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
        }
    }
}
