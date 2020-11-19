using System;
using System.Collections.Generic;
using System.Text;
/**
Base32768 is a binary-to-text encoding optimised for UTF-16-encoded text.
(e.g. Windows, Java, JavaScript)

original is https://github.com/qntm/base32768

MIT License

Copyright (c) 2020 naminodarie

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
namespace Kzrnm.Convert.Base32768
{
    public static partial class Base32768
    {
        internal const int BITS_PER_CHAR = 15;// Base32768 is a 15-bit encoding
        internal const int BITS_PER_BYTE = 8;

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

                var (numZBits, z) = lookupD[chr];
                switch (numZBits)
                {
                    case 15:
                        break;
                    case 7:
                        if (i != str.Length - 1)
                            throw new FormatException($"Unrecognised Base32768 character: {chr}");
                        break;
                    default:
                        throw new FormatException($"Unrecognised Base32768 character: {chr}");
                }

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