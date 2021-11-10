using System;
using System.Text;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class Base32768TestBase
    {
        public static TheoryData Simple_Data = new TheoryData<string, byte[]>
        {
            {
                "ꡟ",
                new byte[]{255}
            },
            {
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
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1
                }
            },
            {
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
                new byte[]{
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1
                }
            }
        };
        public static TheoryData EnumerateRandomBytes()
        {
            var rnd = new Random();
            var theoryData = new TheoryData<byte[]>();
            for (int i = 0; i < 100; i++)
            {
                var bytes = new byte[rnd.Next(1, 1000)];
                rnd.NextBytes(bytes);
                theoryData.Add(bytes);
            }
            return theoryData;
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

        public static TheoryData EnumerateTestDataBad()
        {
            var testData = TestUtil.TestData;
            var theoryData = new TheoryData<string>();
            foreach (var (name, val) in testData)
            {
                if (!name.StartsWith("test-data/bad"))
                    continue;
                if (!name.EndsWith(".txt"))
                    continue;
                theoryData.Add(Encoding.UTF8.GetString(val));
            }
            return theoryData;
        }
    }
}
