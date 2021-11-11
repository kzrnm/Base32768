using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768BuildTest
    {
        [Fact]
        public void ValidLookupD()
        {
            static Dictionary<char, ushort> GetExpectedLookupD()
            {
                var serializer = new JsonSerializer();
                using var ms = new MemoryStream(TestUtil.TestData["test-data/expectedLookupD.json"]);
                using var sr = new StreamReader(ms);
                using var jsonTextReader = new JsonTextReader(sr);
                return serializer.Deserialize<Dictionary<char, ushort>>(jsonTextReader);
            }
            var wantKeys = Encoding.UTF8.GetString(TestUtil.TestData["test-data/wantKeys.txt"]);
            var expectedLookupD = GetExpectedLookupD();

            var lookupD = Base32768.lookupD;
            var dicLookupD = new Dictionary<char, ushort>();
            var keys = new List<char>();
            for (int i = 0; i < lookupD.Length; i++)
                if (lookupD[i] is ushort value)
                {
                    keys.Add((char)i);
                    dicLookupD.Add((char)i, value);
                }

            dicLookupD.Should().Equal(expectedLookupD);

            keys.Sort();
            keys.Should().HaveCount(32768 + 128);
            keys.Should().Equal(wantKeys);
            foreach (var c in keys)
                c.ToString().IsNormalized().Should().BeTrue();
            string.Join("", keys).IsNormalized().Should().BeTrue();
            keys.Reverse();
            string.Join("", keys).IsNormalized().Should().BeTrue();
        }

    }
}
