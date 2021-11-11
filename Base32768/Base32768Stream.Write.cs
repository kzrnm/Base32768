using System;
using System.Text;
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
            throw new NotImplementedException();
        }
    }
}
