using System;
using System.Diagnostics;
using System.IO;
using static Kzrnm.Convert.Base32768.Base32768;
using static Kzrnm.Convert.Base32768.Utils;

namespace Kzrnm.Convert.Base32768
{
    /// <summary>
    /// Stream that encode/decode Base32768.
    /// </summary>
    public partial class Base32768Stream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base32768Stream"/>. the stream is for encoding and write only.
        /// </summary>
        /// <param name="writer">The writer for Base32768 text.</param>
        public Base32768Stream(TextWriter writer)
        {
            Utils.ThrowArgumentNullExceptionIfNull(writer);
            this.writer = writer;
        }
        private TextWriter writer;

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureNotDisposed();
            EnsureWritable();
            if (buffer is null)
                ThrowArgumentNullExceptionIfNull(buffer);
            if (offset < 0)
                ThrowArgumentOutOfRangeException(nameof(offset), "offset must not be negative.");
            if ((uint)count > buffer.Length - offset)
                ThrowArgumentOutOfRangeException(nameof(count));

            WriteCore(buffer, offset, count);
        }

        private void WriteCore(byte[] buffer, int offset, int count)
        {
            Debug.Assert(buffer is not null);
            Debug.Assert(offset >= 0);
            Debug.Assert((uint)count <= buffer.Length - offset);

            int writeCacheSize = WriteCacheAndBytes(buffer, offset, count);
            offset += writeCacheSize;
            count -= writeCacheSize;

            if (count <= 0)
                return;

            WriteBytes(buffer, offset, count);
        }
        private int WriteCacheAndBytes(byte[] buffer, int offset, int count)
        {
            Debug.Assert(previousBytesCacheOffset == 0);
            if (previousBytesCacheCount == 0)
                return 0;
            var remaining = BITS_PER_CHAR - previousBytesCacheCount;
            if (count < remaining)
            {
                Array.Copy(buffer, offset, previousBytesCache, previousBytesCacheCount, count);
                previousBytesCacheCount += count;
                return count;
            }
            Array.Copy(buffer, offset, previousBytesCache, previousBytesCacheCount, remaining);
            EncodeCore(previousBytesCache, 0, BITS_PER_CHAR, writer);
            previousBytesCacheCount = 0;
            return remaining;
        }
        private void WriteBytes(byte[] buffer, int offset, int count)
        {
            int writingSize = MathUtil.FloorNth(count, BITS_PER_CHAR);
            Debug.Assert(previousBytesCacheOffset == 0);
            Debug.Assert(previousBytesCacheCount == 0);
            Debug.Assert(writingSize <= count);
            Debug.Assert(count - writingSize < BITS_PER_CHAR);

            EncodeCore(buffer, offset, writingSize, writer);

            // Copy remaining to cache
            Array.Copy(buffer, offset + writingSize, previousBytesCache, 0, previousBytesCacheCount = count - writingSize);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            EnsureNotDisposed();
            if (writer is not null)
                FlushBuffer();
        }

        private void FlushBuffer()
        {
            Debug.Assert(previousBytesCacheOffset == 0);
            if (previousBytesCacheCount > 0)
            {
                EncodeCore(previousBytesCache, 0, previousBytesCacheCount, writer);
                previousBytesCacheCount = 0;
            }
            writer.Flush();
        }

        private void DisposeWriter()
        {
            FlushBuffer();
            writer.Dispose();
            writer = null;
        }
    }
}
