using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.SaveIO
{
    /// <summary>
    /// Decorates a Stream object but offsets any seeks by a specified base offset.
    /// </summary>
    class OffsetStream : Stream
    {
        public OffsetStream(Stream baseStream)
        {
            _stream = baseStream;
            _baseOffset = baseStream.Position;
        }

        public OffsetStream(Stream baseStream, long baseOffset)
        {
            _stream = baseStream;
            _baseOffset = baseOffset;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Length
        {
            get { return _stream.Length - _baseOffset; }
        }

        public override long Position
        {
            get
            {
                return _stream.Position - _baseOffset;
            }
            set
            {
                _stream.Position = value + _baseOffset;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin != SeekOrigin.Current)
                return _stream.Seek(offset + _baseOffset, origin);
            else
                return _stream.Seek(offset, SeekOrigin.Current);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value + _baseOffset);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        private Stream _stream;
        private long _baseOffset;
    }
}
