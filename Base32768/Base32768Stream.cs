using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using static Kzrnm.Convert.Base32768.Base32768;

namespace Kzrnm.Convert.Base32768
{
    /// <summary>
    /// Stream that encode/decode Base32768.
    /// </summary>
    public class Base32768Stream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base32768Stream"/>. the stream is for decoding and read only.
        /// </summary>
        /// <param name="reader">Base32768 text reader</param>
        public Base32768Stream(TextReader reader)
        {
            this.reader = reader;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Base32768Stream"/>. the stream is for encoding and write only.
        /// </summary>
        /// <param name="writer">Base32768 text writer</param>
        public Base32768Stream(TextWriter writer)
        {
            this.writer = writer;
        }

        private TextReader reader;
        private TextWriter writer;

        private byte[] previousBytesCache = new byte[BITS_PER_CHAR];
        private int previousBytesCacheOffset, previousBytesCacheCount;

        /// <inheritdoc/>
        public override bool CanRead => reader != null;

        /// <inheritdoc/>
        public override bool CanWrite => writer != null;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        #region NotSupported
        /// <inheritdoc/>
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush() => throw new NotSupportedException();

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();
        #endregion NotSupported

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

            int readCacheSize = ReadCache(buffer, offset, count);
            offset += readCacheSize;
            count -= readCacheSize;

            return ReadCore(buffer, offset, count) + readCacheSize;
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
        private int ReadCore(byte[] buffer, int offset, int count)
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

        private static void ClearBufferAndDecodeCore(char[] str, int offset, int count, byte[] result, int resultOffset, int resultCount)
        {
            Array.Clear(result, resultOffset, resultCount);
            DecodeCore(str, offset, count, result, resultOffset, resultCount);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }


        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                reader?.Dispose();
                reader = null;

                writer?.Dispose();
                writer = null;

                previousBytesCache = null;
            }
            base.Dispose(disposing);
        }

        private void EnsureNotDisposed()
        {
            if (reader == null && writer == null)
                ThrowObjectDisposedException();

            static void ThrowObjectDisposedException() =>
                throw new ObjectDisposedException(nameof(Base32768Stream));
        }
        private void EnsureWritable()
        {
            if (writer == null)
                ThrowInvalidOperationException();

            static void ThrowInvalidOperationException() =>
                throw new InvalidOperationException("The stream is not writable.");
        }
        private void EnsureReadable()
        {
            if (reader == null)
                ThrowInvalidOperationException();

            static void ThrowInvalidOperationException() =>
                throw new InvalidOperationException("The stream is not readable.");
        }
    }
}
