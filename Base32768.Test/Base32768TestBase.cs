using System;
using System.Text;
using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768TestBase
    {
        public static TheoryData<string, string, byte[]> SimpleData { get; } = new TheoryData<string, string, byte[]>
        {
            {
                "ꡟ",
                "ꡟ",
                new byte[]{255}
            },
            {
                "ꡟꡟꡟꡟꡟꡟꡟꡟꡟ",
                "ꡟꡟꡟꡟꡟꡟꡟꡟꡟ",
                new byte[]{
                    255, 255, 255, 255,
                    255, 255, 255, 255,
                    255, 255, 255, 255,
                    255, 255, 255, 255
                }
            },
            {
                "ݠ暠䙠㙐▨ᖄቢႡဟ",
                "ݠ暠䙠㙐▨ᖄቢႡဟ",
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1
                }
            },
            {
                "ݠ暠䙠㙐▨ᖄቢႡݠʟ",
                "ݠ暠䙠㙐▨ᖄቢႡݠʟ",
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1, 1
                }
            },
            {
                "ݠ暠䙠㙐▨ᖄቢႡݠ曟",
                "ݠ暠䙠㙐▨ᖄቢႡݠ曟",
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1
                }
            }
        };

        public static TheoryData<string, string, byte[]> EnumerateRandomBytes { get; } = Enumerable.Repeat(new Random(227), 100)
            .Select((rnd, i) =>
            {
                var bytes = new byte[rnd.Next(1, 1000)];
                rnd.NextBytes(bytes);
                return ($"Random:{i}", Base32768.Encode(bytes), bytes);
            })
            .ToTheoryData();

        public static TheoryData<string, string, byte[]> EnumeratePairTestData { get; } = TestUtil.TestData
            .Where(p => p.Key.StartsWith("test-data/pairs") && p.Key.EndsWith(".txt"))
            .Select(p => (p.Key, Encoding.UTF8.GetString(p.Value), TestUtil.TestData[System.IO.Path.ChangeExtension(p.Key, "bin")]))
            .ToTheoryData();
        public static IEnumerable<object[]> AllPairData { get; }
            = SimpleData
            .Concat(EnumerateRandomBytes)
            .Concat(EnumeratePairTestData)
            .ToArray();

        public static TheoryData<string> EnumerateTestDataBad { get; } = TestUtil.TestData
            .Where(p => p.Key.StartsWith("test-data/bad") && p.Key.EndsWith(".txt"))
            .Select(p => Encoding.UTF8.GetString(p.Value))
            .ToTheoryData();
    }
}
