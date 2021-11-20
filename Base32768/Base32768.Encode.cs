using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        /// Encode a binary data to Base32768.
        /// </summary>
        /// <param name="bytes">original binary data</param>
        /// <returns>Base32768 encoded data</returns>
        public static unsafe string Encode(byte[] bytes)
        {
            ThrowArgumentNullExceptionIfNull(bytes);
            var sb = new StringBuilder((BITS_PER_BYTE * bytes.Length + (BITS_PER_CHAR - 1)) / BITS_PER_CHAR);
            fixed (byte* p = bytes)
            {
                EncodeCore(p, bytes.Length, sb);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Encode a binary data to Base32768.
        /// </summary>
        /// <param name="stream">original binary data</param>
        /// <returns>Base32768 encoded data</returns>
        public static string Encode(Stream stream)
        {
            ThrowArgumentNullExceptionIfNull(stream);
            using var writer = new StringWriter();
            using (var st = new Base32768Stream(writer))
            {
                stream.CopyTo(st);
            }
            return writer.ToString();
        }

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Encode a binary data to Base32768.
        /// </summary>
        /// <param name="bytes">original binary data</param>
        /// <returns>Base32768 encoded data</returns>
        public static unsafe string Encode(ReadOnlySpan<byte> bytes)
        {
            var sb = new StringBuilder((BITS_PER_BYTE * bytes.Length + (BITS_PER_CHAR - 1)) / BITS_PER_CHAR);
            fixed (byte* p = bytes)
            {
                EncodeCore(p, bytes.Length, sb);
            }
            return sb.ToString();
        }
#endif

        internal static unsafe void EncodeCore(byte[] bytes, int offset, int count, TextWriter writer)
        {
            if ((uint)count > bytes.Length - offset)
                ThrowArgumentOutOfRangeException(nameof(count));
            unsafe
            {
                fixed (byte* p = bytes)
                {
                    EncodeCore(p + offset, count, writer);
                }
            }
        }
    }
}
