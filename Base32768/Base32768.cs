using System;
using System.Collections.Generic;
using System.Text;
/**
Base32768 is a binary-to-text encoding optimised for UTF-16-encoded text.
(e.g. Windows, Java, JavaScript)
*/
namespace Base32768
{
    public static class Base32768
    {
        public const int BITS_PER_CHAR = 15;// Base32768 is a 15-bit encoding
        public const int BITS_PER_BYTE = 8;
        private static readonly Dictionary<int, (int numZBits, char z)> lookupD
            = new Dictionary<int, (int numZBits, char z)>();
        private static readonly char[] lookupE7;
        private static readonly char[] lookupE15;
        static Base32768()
        {
            lookupE15 = Build(new (int from, int to)[] {
                (1184, 1216), (1280, 1312), (1664, 1728), (1888, 1952),
                (1984, 2016), (4096, 4128), (4256, 4288), (4352, 4448),
                (4480, 4512), (4576, 4672), (4704, 4736), (4832, 4864),
                (4896, 4928), (5024, 5088), (5152, 5728), (5792, 5856),
                (6016, 6048), (6176, 6240), (6336, 6368), (6528, 6560),
                (6624, 6656), (6688, 6720), (7104, 7136), (7168, 7200),
                (7424, 7456), (8672, 8704), (8896, 8928), (9024, 9184),
                (9216, 9248), (9472, 10080), (10112, 10176), (10240, 10624),
                (10656, 10688), (10784, 10848), (10880, 10944), (10976, 11104),
                (11264, 11296), (11392, 11488), (11520, 11552), (11584, 11616),
                (11936, 12000), (12736, 12768), (13312, 19872), (19904, 40896),
                (40960, 42112), (42144, 42176), (42240, 42496), (42560, 42592),
                (42656, 42720), (42752, 42848), (42880, 42912), (43072, 43104),
            }, 32768, 0);
            lookupE7 = Build(new (int from, int to)[] {
                (384, 416), (576, 672),
            }, 128, 1);
        }
        private static char[] Build((int from, int to)[] pairString, int size, int r)
        {
            var numZBits = BITS_PER_CHAR - BITS_PER_BYTE * r;
            var encodeRepertoire = new char[size];
            var ix = 0;
            foreach (var (from, to) in pairString)
                for (int i = from; i < to; i++)
                {
                    lookupD[i] = (numZBits, (char)ix);
                    encodeRepertoire[ix++] = (char)i;
                }
            return encodeRepertoire;
        }

        public static string Encode(byte[] bytes)
#if NETSTANDARD2_1
            => Encode(bytes.AsSpan());
        public static string Encode(ReadOnlySpan<byte> bytes)
#endif
        {
            var sb = new StringBuilder((BITS_PER_BYTE * bytes.Length + (BITS_PER_CHAR - 1)) / BITS_PER_CHAR);

            const int mask = (1 << 15) - 1;
            var z = 0;
            var numOBits = BITS_PER_CHAR;
            foreach (var by in bytes)
            {
                if (numOBits > 8)
                {
                    numOBits -= 8;
                    z |= by << numOBits;
                }
                else
                {
                    z |= by >> (8 - numOBits);
                    sb.Append(lookupE15[z]);
                    numOBits += 7;
                    z = (by << numOBits) & mask;
                }
            }
            if (numOBits != BITS_PER_CHAR)
            {
                var numZBits = BITS_PER_CHAR - numOBits;
                var c = 7 ^ (numZBits & 0b111);
                if (numZBits > 7)
                {
                    z |= (1 << c) - 1;
                    sb.Append(lookupE15[z]);
                }
                else
                {
                    z >>= 8;
                    z |= (1 << c) - 1;
                    sb.Append(lookupE7[z]);
                }
            }

            return sb.ToString();
        }

        public static byte[] Decode(string str)
#if NETSTANDARD2_1
            => Decode(str.AsSpan());
        public static byte[] Decode(ReadOnlySpan<char> str)
#endif
        {
            var length = str.Length * BITS_PER_CHAR / BITS_PER_BYTE;
            if (length == 0)
                return Array.Empty<byte>();

            if (str[str.Length - 1] < 1184)
                --length;
            var res = new byte[length];
            var numUint8s = 0;
            var numUint8Remaining = 8;

            for (int i = 0; i < str.Length; i++)
            {
                var chr = str[i];
                if (!lookupD.TryGetValue(chr, out var tup))
                    throw new FormatException($"Unrecognised Base32768 character: {chr}");

                var (numZBits, z) = tup;
                if (numZBits != BITS_PER_CHAR && i != str.Length - 1)
                    throw new FormatException($"Unrecognised Base32768 character: {chr}");
                do
                {
                    var mask = (1 << numZBits) - 1;
                    var zz = z & mask;
                    if (numZBits < numUint8Remaining)
                    {
                        numUint8Remaining -= numZBits;
                        res[numUint8s] |= (byte)(zz << numUint8Remaining);
                        numZBits = 0;
                    }
                    else
                    {
                        numZBits -= numUint8Remaining;
                        res[numUint8s++] |= (byte)(zz >> numZBits);
                        numUint8Remaining = 8;
                    }
                } while (numZBits > 0 && numUint8s < res.Length);
            }

            return res;
        }
    }
}