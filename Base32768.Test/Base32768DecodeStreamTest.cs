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
        [MemberData(nameof(Simple_Data))]
#if !DEBUG
        [MemberData(nameof(EnumerateRandomBytes))]
        [MemberData(nameof(EnumeratePairTestData))]
#endif
        public void DecodeStream(string str, byte[] bytes)
        {
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
