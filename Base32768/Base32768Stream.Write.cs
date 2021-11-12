using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static Kzrnm.Convert.Base32768.Base32768;

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

            // TODO
            throw new NotImplementedException();
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
                // TODO
                throw new NotImplementedException();
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
