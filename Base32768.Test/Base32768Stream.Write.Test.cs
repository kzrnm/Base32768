using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768StreamWriteTest
    {
        [Fact]
        public void NullArgumentConstructor_WriteMode()
        {
            new Action(() => _ = new Base32768Stream((TextWriter)null))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("writer");
        }

        [Fact]
        public void WriteReadStream()
        {
            using var st = new Base32768Stream(new StringReader("foo"));
            st.Invoking(st => st.Write(null, 0, 0))
                .Should().Throw<InvalidOperationException>().WithMessage("The stream is not writable.");
        }

        [Fact]
        public void WriteAndFlush()
        {
            using var writer = new StringWriter();
            using (var st = new Base32768Stream(writer))
            {
                st.Write(Enumerable.Range(0, 20).Select(n => ((byte)n)).ToArray(), 0, 20);
                writer.ToString().Should().Be("Ҡ曠蛠盀庠䩨㱘Ⳏ");
                st.Flush();
                writer.ToString().Should().Be("Ҡ曠蛠盀庠䩨㱘Ⳏ┨ᗄ棟");
                st.Flush();
                writer.ToString().Should().Be("Ҡ曠蛠盀庠䩨㱘Ⳏ┨ᗄ棟");
            }
        }

        [Fact]
        public void WriteSizeZero()
        {
            using var writer = new StringWriter();
            using (var st = new Base32768Stream(writer))
                st.Write(new byte[0], 0, 0);
            writer.ToString().Should().BeEmpty();
        }

        [Fact]
        public void WriteDisposedStream()
        {
            var st = new Base32768Stream(new StringWriter());
            st.Dispose();
            st.Invoking(st => st.Write(null, 0, 0))
                .Should().Throw<ObjectDisposedException>()
                .Which.ObjectName.Should().Be("Base32768Stream");
        }

        [Fact]
        public void WriteNullBuffer()
        {
            using var st = new Base32768Stream(new StringWriter());
            st.Invoking(st => st.Write(null, 0, 0))
                .Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("buffer");
        }

        [Fact]
        public void WriteNegativeOffset()
        {
            using var st = new Base32768Stream(new StringWriter());
            st.Invoking(st => st.Write(new byte[0], -1, 0))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("offset");
        }

        [Fact]
        public void WriteTooLargeCount()
        {
            using var st = new Base32768Stream(new StringWriter());
            st.Invoking(st => st.Write(new byte[1], 0, 2))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
            st.Invoking(st => st.Write(new byte[2], 0, 3))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
            st.Invoking(st => st.Write(new byte[2], 1, 2))
                .Should().Throw<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("count");
        }
    }
}
