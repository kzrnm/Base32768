using System;
using System.Diagnostics;
using System.IO;
using static Kzrnm.Convert.Base32768.Base32768;

namespace Kzrnm.Convert.Base32768
{
    /// <summary>
    /// Stream that encode/decode Base32768.
    /// </summary>
    public partial class Base32768Stream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base32768Stream"/>. the stream is for decoding and read only.
        /// </summary>
        /// <param name="reader">The reader for Base32768 text.</param>
        public Base32768Stream(TextReader reader)
        {
            Utils.ThrowArgumentNullExceptionIfNull(reader);
            this.reader = reader;
        }
        private TextReader reader;

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            EnsureNotDisposed();
            EnsureReadable();
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset must not be negative.");
            if ((uint)count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));

            return ReadCore(buffer, offset, count);
        }
        private int ReadCore(byte[] buffer, int offset, int count)
        {
            Debug.Assert(buffer is not null);
            Debug.Assert(offset >= 0);
            Debug.Assert((uint)count <= buffer.Length - offset);

            int readCacheSize = ReadCache(buffer, offset, count);
            offset += readCacheSize;
            count -= readCacheSize;

            return ReadText(buffer, offset, count) + readCacheSize;
        }
        private int ReadText(byte[] buffer, int offset, int count)
        {
            Debug.Assert(buffer is not null);
            Debug.Assert(offset >= 0);
            Debug.Assert((uint)count <= buffer.Length - offset);
            Debug.Assert(previousBytesCacheCount == 0);

            if (count == 0)
                return 0;

            int nextByteSize = MathUtil.CeilingNth(count, BITS_PER_CHAR);
            int nextCharSize = nextByteSize * BITS_PER_BYTE / BITS_PER_CHAR;
            Debug.Assert(nextByteSize % BITS_PER_CHAR == 0);

            var charBuffer
#if NETSTANDARD2_1_OR_GREATER
                = System.Buffers.ArrayPool<char>.Shared.Rent(nextCharSize);
#else
                = new char[nextCharSize];
#endif
            try
            {
                int readingCharSize = reader.ReadBlock(charBuffer, 0, nextCharSize);
                if (readingCharSize == 0)
                {
                    return 0;
                }

                int readingBytesSize = CalculateByteLength(readingCharSize, charBuffer[readingCharSize - 1]);
                if (readingBytesSize <= count)
                {
                    ClearBufferAndDecodeCore(charBuffer, 0, readingCharSize, buffer, offset, readingBytesSize);
                    return readingBytesSize;
                }
                int result = count;
                int bufferedBytesSize = MathUtil.FloorNth(readingBytesSize - 1, BITS_PER_CHAR);
                int bufferedCharSize = bufferedBytesSize / BITS_PER_CHAR * BITS_PER_BYTE;
                Debug.Assert(bufferedBytesSize % BITS_PER_CHAR == 0);
                Debug.Assert(bufferedBytesSize <= count);

                ClearBufferAndDecodeCore(charBuffer, 0, bufferedCharSize, buffer, offset, bufferedBytesSize);
                count -= bufferedBytesSize;
                offset += bufferedBytesSize;
                int remainingCharCount = readingCharSize - bufferedCharSize;

                Debug.Assert(remainingCharCount > 0);
                Debug.Assert(count <= BITS_PER_CHAR);
                Debug.Assert(remainingCharCount <= BITS_PER_BYTE);

                previousBytesCacheOffset = 0;
                previousBytesCacheCount = CalculateByteLength(remainingCharCount, charBuffer[readingCharSize - 1]);
                ClearBufferAndDecodeCore(charBuffer, bufferedCharSize, remainingCharCount, previousBytesCache, 0, previousBytesCacheCount);
                ReadCache(buffer, offset, count);
                return result;
            }
            finally
            {
#if NETSTANDARD2_1_OR_GREATER
                System.Buffers.ArrayPool<char>.Shared.Return(charBuffer);
#endif
            }
        }
        private int ReadCache(byte[] buffer, int offset, int count)
        {
            int result = 0;
            if (previousBytesCacheCount > 0)
            {
                if (previousBytesCacheCount < count)
                {
                    Array.Copy(previousBytesCache, previousBytesCacheOffset, buffer, offset, previousBytesCacheCount);
                    result = previousBytesCacheCount;
                    previousBytesCacheOffset = 0;
                    previousBytesCacheCount = 0;
                }
                else
                {
                    Array.Copy(previousBytesCache, previousBytesCacheOffset, buffer, offset, count);
                    previousBytesCacheOffset += count;
                    previousBytesCacheCount -= count;
                    result = count;
                }
            }
            return result;
        }
        private static void ClearBufferAndDecodeCore(char[] str, int offset, int count, byte[] result, int resultOffset, int resultCount)
        {
            Array.Clear(result, resultOffset, resultCount);
            DecodeCore(str, offset, count, result, resultOffset, resultCount);
        }
    }
}
