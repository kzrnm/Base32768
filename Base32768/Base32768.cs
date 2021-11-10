#if NETSTANDARD2_1_OR_GREATER
global using StringOrReadOnlySpanChar = System.ReadOnlySpan<char>;
#else
global using StringOrReadOnlySpanChar = System.String;
#endif
using System;
using System.Collections.Generic;
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
    public static class Base32768
    {
        internal const int BITS_PER_CHAR = 15;// Base32768 is a 15-bit encoding
        internal const int BITS_PER_BYTE = 8;

        internal const char ZBits15Start = '\u04a0';

        internal static readonly ushort?[] lookupD = new ushort?[0xA860];
        internal static readonly char[] lookupE7 = Build(
            "\u0180\u01a0" + "\u0240\u02a0"
        , 128);
        internal static readonly char[] lookupE15 = Build(
            "\u04a0\u04c0" + "\u0500\u0520" + "\u0680\u06c0" + "\u0760\u07a0" +
            "\u07c0\u07e0" + "\u1000\u1020" + "\u10a0\u10c0" + "\u1100\u1160" +
            "\u1180\u11a0" + "\u11e0\u1240" + "\u1260\u1280" + "\u12e0\u1300" +
            "\u1320\u1340" + "\u13a0\u13e0" + "\u1420\u1660" + "\u16a0\u16e0" +
            "\u1780\u17a0" + "\u1820\u1860" + "\u18c0\u18e0" + "\u1980\u19a0" +
            "\u19e0\u1a00" + "\u1a20\u1a40" + "\u1bc0\u1be0" + "\u1c00\u1c20" +
            "\u1d00\u1d20" + "\u21e0\u2200" + "\u22c0\u22e0" + "\u2340\u23e0" +
            "\u2400\u2420" + "\u2500\u2760" + "\u2780\u27c0" + "\u2800\u2980" +
            "\u29a0\u29c0" + "\u2a20\u2a60" + "\u2a80\u2ac0" + "\u2ae0\u2b60" +
            "\u2c00\u2c20" + "\u2c80\u2ce0" + "\u2d00\u2d20" + "\u2d40\u2d60" +
            "\u2ea0\u2ee0" + "\u31c0\u31e0" + "\u3400\u4da0" + "\u4dc0\u9fc0" +
            "\ua000\ua480" + "\ua4a0\ua4c0" + "\ua500\ua600" + "\ua640\ua660" +
            "\ua6a0\ua6e0" + "\ua700\ua760" + "\ua780\ua7a0" + "\ua840\ua860"
        , 32768);
        private static char[] Build(string pairString, int size)
        {
            var encodeRepertoire = new char[size];
            ushort ix = 0;
            for (int p = 0; p < pairString.Length; p += 2)
            {
                var from = pairString[p];
                var to = pairString[p + 1];
                for (char i = from; i < to; i++)
                {
                    lookupD[i] = ix;
                    encodeRepertoire[ix++] = i;
                }
            }
            System.Diagnostics.Debug.Assert(size == ix);
            return encodeRepertoire;
        }

        #region Encode
        /// <summary>
        /// Encode a binary data to Base32768.
        /// </summary>
        /// <param name="bytes">original binary data</param>
        /// <returns>Base32768 encoded data</returns>
        public static string Encode(byte[] bytes)
        {
            var sb = new StringBuilder((BITS_PER_BYTE * bytes.Length + (BITS_PER_CHAR - 1)) / BITS_PER_CHAR);
#if NETSTANDARD2_1_OR_GREATER
            EncodeCore(bytes, sb);
#else
            EncodeCore(new MemoryStream(bytes), sb);
#endif
            return sb.ToString();
        }

        /// <summary>
        /// Encode a binary data to Base32768.
        /// </summary>
        /// <param name="stream">original binary data</param>
        /// <returns>Base32768 encoded data</returns>
        public static string Encode(Stream stream)
        {
            var sb = new StringBuilder();
            EncodeCore(stream, sb);
            return sb.ToString();
        }
        private static void EncodeCore(Stream stream, StringBuilder sb)
        {
            const int mask = (1 << 15) - 1;

            int len;
            var bytes = new byte[15];
            while ((len = stream.Read(bytes, 0, 15)) >= 15)
            {
                uint u;

                u = ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);
                sb.Append(lookupE15[(u >> (32 - 30)) & mask]);

                u = (u << 30) | ((uint)bytes[4] << 22) | ((uint)bytes[5] << 14) | ((uint)bytes[6] << 6);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[7] << 13) | ((uint)bytes[8] << 5);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[9] << 12) | ((uint)bytes[10] << 4);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[11] << 11) | ((uint)bytes[12] << 3);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[13] << 10) | ((uint)bytes[14] << 2);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);
                sb.Append(lookupE15[(u >> (32 - 30)) & mask]);
            }

            var z = 0;
            var numOBits = BITS_PER_CHAR;
            for (int i = 0; i < len; i++)
            {
                var by = bytes[i];
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
        }

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Encode a binary data to Base32768.
        /// </summary>
        /// <param name="bytes">original binary data</param>
        /// <returns>Base32768 encoded data</returns>
        public static string Encode(ReadOnlySpan<byte> bytes)
        {
            var sb = new StringBuilder((BITS_PER_BYTE * bytes.Length + (BITS_PER_CHAR - 1)) / BITS_PER_CHAR);
            EncodeCore(bytes, sb);
            return sb.ToString();
        }
        private static void EncodeCore(ReadOnlySpan<byte> bytes, StringBuilder sb)
        {
            const int mask = (1 << 15) - 1;

            while (bytes.Length >= 15)
            {
                uint u;

                u = ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);
                sb.Append(lookupE15[(u >> (32 - 30)) & mask]);

                u = (u << 30) | ((uint)bytes[4] << 22) | ((uint)bytes[5] << 14) | ((uint)bytes[6] << 6);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[7] << 13) | ((uint)bytes[8] << 5);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[9] << 12) | ((uint)bytes[10] << 4);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[11] << 11) | ((uint)bytes[12] << 3);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);

                u = (u << 15) | ((uint)bytes[13] << 10) | ((uint)bytes[14] << 2);
                sb.Append(lookupE15[(u >> (32 - 15)) & mask]);
                sb.Append(lookupE15[(u >> (32 - 30)) & mask]);
                bytes = bytes[15..];
            }

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
        }
#endif

        #endregion Encode

        #region Decode
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
            var res = new byte[CalculateByteLength(str)];
            DecodeCore(str, res);
            return res;
        }

        internal static int CalculateByteLength(StringOrReadOnlySpanChar str)
        {
            var length = str.Length * BITS_PER_CHAR / BITS_PER_BYTE;
            if (str[str.Length - 1] < ZBits15Start)
                --length;
            return length;
        }

        internal static void DecodeCore(
#if NETSTANDARD2_1_OR_GREATER
            StringOrReadOnlySpanChar str, ReadOnlySpan<byte> result
#else
            StringOrReadOnlySpanChar str, byte[] result
#endif
            )
        {
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

            var numUint8s = 0;
            var numUint8Remaining = 8;
            int i;

            unchecked
            {
                for (i = 0; i + 9 < str.Length;)
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

                for (; i < str.Length; i++)
                {
                    var chr = str[i];

                    int numZBits = 15;
                    if (lookupD[chr] is ushort z)
                    {
                        if (chr < ZBits15Start)
                        {
                            if (i + 1 != str.Length)
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
                    } while (numZBits > 0 && numUint8s < result.Length);
                }
            }
        }
#endregion Decode
    }
}
