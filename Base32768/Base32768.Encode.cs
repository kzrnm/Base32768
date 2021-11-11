﻿using System;
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
    }
}
