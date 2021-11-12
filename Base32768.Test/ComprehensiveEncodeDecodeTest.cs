using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Kzrnm.Convert.Base32768.Models;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public class ComprehensiveEncodeDecodeTest
    {
        public static IEnumerable<(string, byte[])> SingleBytesPairTestData { get; } = TestUtil.TestData
            .Where(p => p.Key.StartsWith("pairs.single_bytes.") && p.Key.EndsWith(".txt"))
            .Select(p => (Encoding.UTF8.GetString(p.Value), TestUtil.TestData[System.IO.Path.ChangeExtension(p.Key, "bin")]))
            .ToArray();


        [Fact]
        public void RandomBytes()
        {
            foreach (var bytes in Enumerable.Repeat(new Random(227), 1000)
            .Select((rnd, i) =>
            {
                var bytes = new byte[rnd.Next(1, 1000)];
                rnd.NextBytes(bytes);
                return bytes;
            }))
            {
                var str = Base32768.Encode(bytes);
                str.IsNormalized().Should().BeTrue();

                EncodeAndDecode(str, bytes);
            }
        }

        [Fact]
        public void SingleBytesPair()
        {
            foreach (var (str, bytes) in SingleBytesPairTestData)
            {
                EncodeAndDecode(str, bytes);
            }
        }

        private static void EncodeAndDecode(string str, byte[] bytes)
        {
            Base32768.Encode(bytes).Should().Be(str);
            Base32768.Decode(str).Should().Equal(bytes);
            Base32768.Encode(bytes.ToStream()).Should().Be(str);
            Base32768StreamReadByte(str, bytes);
            Base32768StreamRead(str, bytes);
            Base32768StreamWriteByte(str, bytes);
            Base32768StreamWrite(str, bytes);
#if NETCOREAPP3_1_OR_GREATER
            Base32768.Encode(bytes.AsSpan()).Should().Be(str);
            Base32768.Decode(str.AsSpan()).Should().Equal(bytes);
#endif
        }

        private static void Base32768StreamReadByte(string str, byte[] bytes)
        {
            using var reader = new StringReader(str);
            using var st = new Base32768Stream(reader);
            int b;
            var list = new List<byte>();
            while ((b = st.ReadByte()) >= 0)
            {
                list.Add((byte)b);
            }
            list.Should().Equal(bytes);
        }

        private static void Base32768StreamRead(string str, byte[] bytes)
        {
            foreach (int bufferSize in new[] { 1, 2, 3, 5, 7, 8, 9, 13, 14, 15, 16, 17, 20 })
            {
                using var reader = new StringReader(str);
                using var st = new Base32768Stream(reader);
                var buffer = new byte[bufferSize];
                int len;
                var list = new List<byte>();
                while ((len = st.Read(buffer, 0, buffer.Length)) > 0)
                {
                    list.AddRange(buffer.Take(len));
                }
                list.Should().Equal(bytes, "buffer size: {0} is positive", bufferSize);
            }
        }


        private static void Base32768StreamWriteByte(string str, byte[] bytes)
        {
            using var writer = new StringWriter();
            using (var st = new Base32768Stream(writer))
            {
                foreach (var b in bytes)
                    st.WriteByte(b);
            }
            writer.ToString().Should().Be(str);
        }

        private static void Base32768StreamWrite(string str, byte[] bytes)
        {
            foreach (int bufferSize in new[] { 1, 2, 3, 5, 7, 8, 9, 13, 14, 15, 16, 17, 20 })
            {
                using var writer = new StringWriter();
                using (var st = new Base32768Stream(writer))
                {
                    for (int offset = 0; offset < bytes.Length; offset += bufferSize)
                    {
                        st.Write(bytes, offset, Math.Min(bufferSize, bytes.Length - offset));
                    }
                }
                writer.ToString().Should().Be(str, "buffer size: {0} is positive", bufferSize);
            }
        }
    }
}
