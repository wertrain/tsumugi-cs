using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tsumugi.Script.Lexing
{
    /// <summary>
    /// 字句解析用の StringReader クラス
    /// 通常の StringReader との違いは、Seek が使用できること
    /// </summary>
    public class LexingStringReader : TextReader
    {
        /// <summary>
        /// シーク位置
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// 解析行
        /// </summary>
        public int Lines { get; set; }

        /// <summary>
        /// 解析列
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text"></param>
        public LexingStringReader(string text)
        {
            _disposed = false;
            _string = new StringBuilder(text);
            Position = 0;
            Lines = Columns = 0;
        }

        public override void Close()
        {
            base.Close();
        }

        public override int Peek()
        {
            return Peek(0);
        }

        public int Peek(int offset)
        {
            int position = Position + offset;
            if (0 <= position && _string.Length > position)
            {
                return _string[position];
            }
            return -1;
        }

        public char PeekChar()
        {
            return (char)Peek();
        }

        public char PeekChar(int offset)
        {
            return (char)Peek(offset);
        }

        public override int Read()
        {
            if (_string.Length > Position)
            {
                CheckNewLine();
                return _string[Position++];
            }
            return -1;
        }

        public char ReadChar()
        {
            return (char)Read();
        }

        public int Seek(int offset)
        {
            return Seek(offset, SeekOrigin.Begin);
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            // TODO: 行数、列数の再計算

            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (0 > _string.Length + offset || _string.Length <= offset)
                    {
                        return Position;
                    }
                    Position = offset;
                    break;

                case SeekOrigin.Current:
                    if (0 > _string.Length + offset || _string.Length <= Position + offset)
                    {
                        return Position;
                    }
                    Position = Position + offset;
                    break;

                case SeekOrigin.End:
                    if (0 > _string.Length + offset || _string.Length <= _string.Length + offset)
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
        /// 解析位置を取得
        /// </summary>
        /// <returns>解析位置</returns>
        public LexingPosition GetLexingPosition()
        {
            return new LexingPosition()
            {
                Position = Position,
                Lines = Lines,
                Columns = Columns
            };
        }

        /// <summary>
        /// 改行のチェック
        /// </summary>
        private void CheckNewLine()
        {
            var newline = string.Empty;
            for (int p = Position; p < _string.Length && p < Position + System.Environment.NewLine.Length; ++p)
            {
                newline += _string[p];
            }
            if (newline.CompareTo(System.Environment.NewLine) == 0)
            {
                ++Lines;
                Columns = 0;
            }
            else
            {
                ++Columns;
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