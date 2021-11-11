using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
/*
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
    /// <summary>
    /// Base32768 is a binary-to-text encoding optimised for UTF-16-encoded text.
    /// </summary>
    public static partial class Base32768
    {
        /// <summary>
        /// Decodes a Base32768.
        /// </summary>
        /// <param name="str">Base32768 encoded data</param>
        /// <returns>original binary data</returns>
        public static byte[] Decode(string str)
#if NETSTANDARD2_1_OR_GREATER
            => Decode(str.AsSpan());
        /// <summary>
        /// Decodes a Base32768.
        /// </summary>
        /// <param name="str">Base32768 encoded data</param>
        /// <returns>original binary data</returns>
        public static byte[] Decode(ReadOnlySpan<char> str)
#endif
        {
            if (str.Length == 0)
                return
#if NETSTANDARD1_0
                    new byte[0];
#else
                    Array.Empty<byte>();
#endif
            var res = new byte[CalculateByteLength(str.Length, str[str.Length - 1])];

#if NETSTANDARD2_1_OR_GREATER
            DecodeCore(str, res, 0, res.Length);
#else
            DecodeCore(str, 0, str.Length, res, 0, res.Length);
#endif
            return res;
        }

        /// <summary>
        /// Calculate byte array length from char length. This overload does not consider the last character.
        /// </summary>
        internal static int CalculateByteLength(int charLength)
            => charLength * BITS_PER_CHAR / BITS_PER_BYTE;

        /// <summary>
        /// Calculate byte array length from char length.
        /// </summary>
        internal static int CalculateByteLength(int charLength, char lastChar)
        {
            var byteLength = CalculateByteLength(charLength);
            if (lastChar < ZBits15Start)
                --byteLength;
            return byteLength;
        }

        static void ThrowFormatException(char c)
           => throw new FormatException($"Unrecognised Base32768 character: {c}");
        static ushort ValidateCharAndLookupD(char c)
        {
            if (c < ZBits15Start || lookupD[c] is not ushort z)
            {
                ThrowFormatException(c);
                return default;
            }
            return z;
        }

        private static unsafe void DecodeCore(char* str, int count, byte[] result, int resultOffset, int resultCount)
        {
            int resultEnd = resultOffset + resultCount;
            Debug.Assert(resultEnd <= result.Length);
            var numUint8s = resultOffset;
            int i;

            unchecked
            {
                for (i = 0; i + 9 < count;)
                {
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 7);
                        result[numUint8s] = (byte)(z << 1);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 14);
                        result[numUint8s++] = (byte)(z >> 6);
                        result[numUint8s] = (byte)(z << 2);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 13);
                        result[numUint8s++] = (byte)(z >> 5);
                        result[numUint8s] = (byte)(z << 3);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 12);
                        result[numUint8s++] = (byte)(z >> 4);
                        result[numUint8s] = (byte)(z << 4);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 11);
                        result[numUint8s++] = (byte)(z >> 3);
                        result[numUint8s] = (byte)(z << 5);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 10);
                        result[numUint8s++] = (byte)(z >> 2);
                        result[numUint8s] = (byte)(z << 6);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 9);
                        result[numUint8s++] = (byte)(z >> 1);
                        result[numUint8s] = (byte)(z << 7);
                    }
                    {
                        var chr = str[i++];
                        var z = ValidateCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 8);
                        result[numUint8s++] = (byte)z;
                    }
                }

                var numUint8Remaining = 8;
                for (; i < count; i++)
                {
                    var chr = str[i];

                    int numZBits = 15;
                    if (lookupD[chr] is ushort z)
                    {
                        if (chr < ZBits15Start)
                        {
                            if (i + 1 != count)
                            {
                                ThrowFormatException(chr);
                                return;
                            }
                            numZBits = 7;
                        }
                    }
                    else
                    {
                        ThrowFormatException(chr);
                        return;
                    }

                    do
                    {
                        var mask = (1 << numZBits) - 1;
                        var zz = z & mask;
                        if (numZBits < numUint8Remaining)
                        {
                            numUint8Remaining -= numZBits;
                            result[numUint8s] |= (byte)(zz << numUint8Remaining);
                            numZBits = 0;
                        }
                        else
                        {
                            numZBits -= numUint8Remaining;
                            result[numUint8s++] |= (byte)(zz >> numZBits);
                            numUint8Remaining = 8;
                        }
                    } while (numZBits > 0 && numUint8s < resultEnd);
                }
            }
        }

#if NETSTANDARD2_1_OR_GREATER
        internal static unsafe void DecodeCore(ReadOnlySpan<char> str, byte[] result, int resultOffset, int resultCount)
        {
            fixed (char* p = str)
            {
                DecodeCore(p, str.Length, result, resultOffset, resultCount);
            }
        }
#else
        internal static unsafe void DecodeCore(string str, int offset, int count, byte[] result, int resultOffset, int resultCount)
        {
            if (offset + count > str.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (char* p = str)
            {
                DecodeCore(p + offset, count, result, resultOffset, resultCount);
            }
        }
#endif
        internal static unsafe void DecodeCore(char[] str, int offset, int count, byte[] result, int resultOffset, int resultCount)
        {
            if (offset + count > str.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (char* p = str)
            {
                DecodeCore(p + offset, count, result, resultOffset, resultCount);
            }
        }
    }
}
