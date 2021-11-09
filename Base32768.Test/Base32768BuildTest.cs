using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768BuildTest
    {
        [Fact]
        public void ValidLookupD()
        {
            var wantKeys = Encoding.UTF8.GetString(TestUtil.TestData["test-data/wantKeys.txt"]);

            var lookupD = Base32768.lookupD;
            var keys = new List<char>();
            for (int i = 0; i < lookupD.Length; i++)
                if (lookupD[i].HasValue)
                    keys.Add((char)i);

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
