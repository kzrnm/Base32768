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

            int result = 0;

            if (previousBytesCacheCount > 0)
            {
                if (previousBytesCacheCount <= count)
                {
                    Array.Copy(previousBytesCache, previousBytesCacheOffset, buffer, offset, previousBytesCacheCount);
                    result = previousBytesCacheCount;
                    offset += previousBytesCacheCount;
                    count -= previousBytesCacheCount;
                    previousBytesCacheOffset = 0;
                    previousBytesCacheCount = 0;
                }
                else
                {
                    Array.Copy(previousBytesCache, previousBytesCacheOffset, buffer, offset, count);
                    previousBytesCacheOffset += count;
                    previousBytesCacheCount -= count;
                    return count;
                }
            }


            Debug.Assert(previousBytesCacheOffset == 0);
            Debug.Assert(previousBytesCacheCount == 0);

            int nextCharSize = (count + BITS_PER_CHAR - 1) / BITS_PER_CHAR * BITS_PER_BYTE;

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
                    return result;
                }
                int readingBytesSize = CalculateByteLength(readingCharSize, charBuffer[readingCharSize - 1]);

                if (readingBytesSize <= count)
                {
                    ClearBufferAndDecodeCore(charBuffer, 0, readingCharSize, buffer, offset, readingBytesSize);
                    return result + readingBytesSize;
                }

                int buffferdCharSize = readingCharSize & ~0b111;
                int bufferedSize = CalculateByteLength(buffferdCharSize);
                Debug.Assert(bufferedSize <= count);
                ClearBufferAndDecodeCore(charBuffer, 0, buffferdCharSize, buffer, offset, bufferedSize);
                count -= bufferedSize;
                offset += bufferedSize;
                result += bufferedSize;

                int remainingCharSize = readingCharSize - buffferdCharSize;
                if (remainingCharSize <= 0)
                    return result;

                Debug.Assert(remainingCharSize > 0);
                Debug.Assert(remainingCharSize <= BITS_PER_BYTE);

                previousBytesCacheCount = CalculateByteLength(remainingCharSize, charBuffer[readingCharSize - 1]);
                ClearBufferAndDecodeCore(charBuffer, buffferdCharSize, remainingCharSize, previousBytesCache, 0, previousBytesCacheCount);

                if (previousBytesCacheCount <= count)
                {
                    Array.Copy(previousBytesCache, previousBytesCacheOffset, buffer, offset, previousBytesCacheCount);
                    result += previousBytesCacheCount;
                    previousBytesCacheOffset = 0;
                    previousBytesCacheCount = 0;
                    return result;
                }
                else
                {
                    Array.Copy(previousBytesCache, previousBytesCacheOffset, buffer, offset, count);
                    previousBytesCacheOffset += count;
                    previousBytesCacheCount -= count;
                    result += count;
                    return result;
                }
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
