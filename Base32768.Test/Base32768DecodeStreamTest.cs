using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768DecodeStreamTest : Base32768TestBase
    {
        [Theory]
        [MemberData(nameof(AllPairData))]
        public void DecodeStream(string name, string str, byte[] bytes)
        {
            name.Should().NotBeNull();
            using var decodedMemoryStream = new MemoryStream();
            using (var textReader = new StringReader(str))
            using (var decoder = new Base32768Stream(textReader))
            {
                decoder.CopyTo(decodedMemoryStream);
            }
            decodedMemoryStream.ToArray().Should().Equal(bytes);
        }
    }
}
