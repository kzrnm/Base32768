﻿using System;
using System.Diagnostics;
using static Kzrnm.Convert.Base32768.Utils;

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
        {
            ThrowArgumentNullExceptionIfNull(str);
#if NETSTANDARD2_1_OR_GREATER
            return Decode(str.AsSpan());
        }

        /// <summary>
        /// Decodes a Base32768.
        /// </summary>
        /// <param name="str">Base32768 encoded data</param>
        /// <returns>original binary data</returns>
        public static byte[] Decode(ReadOnlySpan<char> str)
        {
#endif
            if (str.Length == 0)
                return
#if NETSTANDARD1_3_OR_GREATER
                    Array.Empty<byte>();
#else
                    new byte[0];
#endif
            var res = new byte[CalculateByteLength(str.Length, str[str.Length - 1])];

            unsafe
            {
                fixed (char* cp = str)
                fixed (byte* bp = res)
                {
                    DecodeCore(cp, str.Length, bp, res.Length);
                }
            }
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

        private static unsafe void DecodeCore(char* str, int count, byte* result, int resultCount)
        {
            var numUint8s = 0;
            int i;

            unchecked
            {
                for (i = 0; i + 9 < count;)
                {
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 7);
                        result[numUint8s] = (byte)(z << 1);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 14);
                        result[numUint8s++] = (byte)(z >> 6);
                        result[numUint8s] = (byte)(z << 2);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 13);
                        result[numUint8s++] = (byte)(z >> 5);
                        result[numUint8s] = (byte)(z << 3);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 12);
                        result[numUint8s++] = (byte)(z >> 4);
                        result[numUint8s] = (byte)(z << 4);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 11);
                        result[numUint8s++] = (byte)(z >> 3);
                        result[numUint8s] = (byte)(z << 5);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 10);
                        result[numUint8s++] = (byte)(z >> 2);
                        result[numUint8s] = (byte)(z << 6);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 9);
                        result[numUint8s++] = (byte)(z >> 1);
                        result[numUint8s] = (byte)(z << 7);
                    }
                    {
                        var chr = str[i++];
                        var z = Validate15BitCharAndLookupD(chr);
                        result[numUint8s++] |= (byte)(z >> 8);
                        result[numUint8s++] = (byte)z;
                    }
                }

                var numUint8Remaining = 8;
                for (; i < count; i++)
                {
                    int numZBits = 15;
                    var chr = str[i];
                    var z = ValidateCharAndLookupD(chr, out var is7BitChar);
                    if (is7BitChar)
                    {
                        if (i + 1 != count)
                            ThrowFormatException(chr);
                        numZBits = 7;
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
                    } while (numZBits > 0 && numUint8s < resultCount);
                }
            }
        }

        internal static void DecodeCore(char[] str, int offset, int count, byte[] result, int resultOffset, int resultCount)
        {
            Debug.Assert(resultOffset + resultCount <= result.Length);
            if ((uint)count > str.Length - offset)
                ThrowArgumentOutOfRangeException(nameof(count));

            unsafe
            {
                fixed (char* cp = str)
                fixed (byte* bp = result)
                {
                    DecodeCore(cp + offset, count, bp + resultOffset, resultCount);
                }
            }
        }
    }
}
