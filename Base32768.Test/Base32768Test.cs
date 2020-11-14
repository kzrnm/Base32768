using System;
using System.Text;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768Test
    {
        Random rnd = new Random();

        [Fact]
        public void Simple255()
        {
            Assert.Equal("ꡟ", Base32768.Encode(stackalloc byte[] { 255 }));
            Assert.Equal("ꡟʟ", Base32768.Encode(stackalloc byte[] { 255, 255 }));
            Assert.Equal("ꡟꡟ", Base32768.Encode(stackalloc byte[] { 255, 255, 255 }));

            Assert.Equal(new byte[] { 255 }, Base32768.Decode("ꡟ"));
            Assert.Equal(new byte[] { 255, 255 }, Base32768.Decode("ꡟʟ"));
            Assert.Equal(new byte[] { 255, 255, 255 }, Base32768.Decode("ꡟꡟ"));
        }

        [Fact]
        public void RandomBytes()
        {
            for (int i = 0; i < 1000; i++)
            {
                var bytes = new byte[rnd.Next(1, 1000)];
                rnd.NextBytes(bytes);

                var str = Base32768.Encode(bytes);
                Assert.Equal(str, Base32768.Encode(bytes.AsSpan()));
                Assert.True(str.IsNormalized());

                Assert.Equal(bytes, Base32768.Decode(str));
                Assert.Equal(bytes, Base32768.Decode(str.AsSpan()));
            }
        }

        public static TheoryData EnumeratePairTestData()
        {
            var testData = TestUtil.TestData;
            var theoryData = new TheoryData<string, byte[]>();
            foreach (var (name, val) in testData)
            {
                if (!name.StartsWith("test-data/pairs"))
                    continue;
                if (!name.EndsWith(".txt"))
                    continue;
                var binName = System.IO.Path.ChangeExtension(name, "bin");
                theoryData.Add(Encoding.UTF8.GetString(val), testData[binName]);
            }
            return theoryData;
        }

        [Theory]
        [MemberData(nameof(EnumeratePairTestData))]
        public void PairTestData(string str, byte[] bytes)
        {
            Assert.Equal(str, Base32768.Encode(bytes));
            Assert.Equal(bytes, Base32768.Decode(str));
        }
    }
}
