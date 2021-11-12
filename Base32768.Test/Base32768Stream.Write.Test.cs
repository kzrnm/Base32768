using System;
using System.IO;
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
    }
}
