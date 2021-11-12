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
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();
        #endregion NotSupported

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reader is not null)
                {
                    DisposeReader();
                }

                if (writer is not null)
                {
                    DisposeWriter();
                }

                Debug.Assert(reader is null);
                Debug.Assert(writer is null);

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
