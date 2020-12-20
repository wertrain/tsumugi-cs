using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tsumugi.Parser
{
    /// <summary>
    /// Tsumugi 用の StringReader クラス
    /// 通常の StringReader との違いは、Seek が使用できること
    /// </summary>
    class TsumugiStringReader : TextReader
    {
        /// <summary>
        /// 
        /// </summary>
        public int Position { get; private set; }

        public TsumugiStringReader(string text)
        {
            _disposed = false;
            _string = new StringBuilder(text);
            Position = 0;
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override int Peek()
        {
            if (_string.Length > Position)
            {
                return _string[Position];
            }
            return -1;
        }

        public override int Read()
        {
            if (_string.Length > Position)
            {
                return _string[Position++];
            }
            return -1;
        }

        public int Seek(int offset)
        {
            return Seek(offset, SeekOrigin.Begin);
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    if (_string.Length <= offset)
                    {
                        return Position;
                    }
                    Position = offset;
                    break;

                case SeekOrigin.Current:
                    if (_string.Length <= Position + offset)
                    {
                        return Position;
                    }
                    Position = Position + offset;
                    break;

                case SeekOrigin.End:
                    if (0 > _string.Length + offset && _string.Length <= _string.Length + offset)
                    {
                        return Position;
                    }
                    Position = _string.Length + offset;
                    break;

            }

            return Position;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        public override Task<string> ReadLineAsync()
        {
            throw new NotImplementedException();
        }

        public override string ReadToEnd()
        {
            throw new NotImplementedException();
        }

        public override Task<string> ReadToEndAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _string.Clear();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        private StringBuilder _string;
    }
}
