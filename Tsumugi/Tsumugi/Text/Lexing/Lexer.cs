using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tsumugi.Localize;
using Tsumugi.Text.Commanding;

namespace Tsumugi.Text.Lexing
{
    public class Lexer
    {
        /// <summary>
        /// 字句解析用の文字列リーダー
        /// </summary>
        private Script.Lexing.LexingStringReader Reader { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script"></param>
        public Lexer(string script)
        {
            Reader = new Script.Lexing.LexingStringReader(script);
        }

        /// <summary>
        /// 次のトークンを取り出す
        /// </summary>
        /// <returns></returns>
        public Token NextToken()
        {
            SkipNewLine();

            Token token = null;

            int next = Reader.Peek();

            if (next < 0)
            {
                token = CreateToken(TokenType.EOF, string.Empty);
            }
            else
            {
                char c = (char)next;

                switch (c)
                {
                    case ':':
                        var label = ReadLabelText();
                        token = CreateToken(TokenType.Label, label);
                        break;

                    case '[':
                        token = ReadTag();
                        break;

                    case '@':
                        token = ReadTagLine();
                        break;

                    default:
                        token = ReadText();
                        break;
                }
            }

            Reader.Read();

            return token;
        }

        /// <summary>
        /// トークン作成
        /// </summary>
        /// <param name="type">トークンのタイプ</param>
        /// <param name="literal">トークンのリテラル</param>
        /// <returns>トークン</returns>
        private Token CreateToken(TokenType type, string literal)
        {
            return new Token(type, literal, Reader.GetLexingPosition());
        }

        /// <summary>
        /// ラベルの読み込み
        /// </summary>
        /// <returns></returns>
        private string ReadLabelText()
        {
            Reader.ReadChar();

            var label = new StringBuilder();

            while (!IsWhiteSpace(Reader.PeekChar()))
            {
                label.Append(Reader.ReadChar());
            }

            Reader.Seek(-1, SeekOrigin.Current);

            return label.ToString();
        }

        /// <summary>
        /// タグの読み込み
        /// </summary>
        /// <returns></returns>
        private Token ReadTag()
        {
            var tag = new StringBuilder();

            // 最初の [ を読み飛ばす（ついでに c を初期化）
            char c = Reader.ReadChar();

            while ((c = Reader.PeekChar()) != ']')
            {
                // 改行または終端が先に見つかった場合は、不正なトークン
                if (c == '\r' || c == '\n' || c == char.MaxValue)
                {
                    return CreateToken(TokenType.Illegal, c.ToString());
                }

                tag.Append(Reader.ReadChar());
            }

            return CreateToken(TokenType.Tag, tag.ToString());
        }

        /// <summary>
        /// タグの読み込み
        /// </summary>
        /// <returns></returns>
        private Token ReadTagLine()
        {
            var tag = new StringBuilder();

            // 最初の @ を読み飛ばす（ついでに c を初期化）
            char c = Reader.ReadChar();

            while ((c = Reader.PeekChar()) != char.MaxValue)
            {
                // 改行または終端が先に見つかった場合は終了
                if (c == '\r' || c == '\n')
                {
                    break;
                }

                tag.Append(Reader.ReadChar());
            }

            return CreateToken(TokenType.Tag, tag.ToString());
        }

        /// <summary>
        /// テキストとして読み込み
        /// </summary>
        /// <returns></returns>
        private Token ReadText()
        {
            var text = new StringBuilder();

            char c = char.MinValue;

            while ((c = Reader.PeekChar()) != char.MaxValue)
            {
                // エスケープシーケンス
                if (c == '\\' && Reader.PeekChar(1) == '[')
                {
                    // 読み飛ばす
                    Reader.ReadChar();
                }
                else if (c == '[')
                {
                    break;
                }

                text.Append(Reader.ReadChar());

                SkipNewLine();
            }

            return CreateToken(TokenType.Text, text.ToString());
        }

        /// <summary>
        /// 空白文字・改行文字かを判定
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsWhiteSpace(char c)
        {
            return (c == ' ' || c == '\t' || c == '\r'|| c == '\n');
        }

        /// <summary>
        /// 改行の間、リーダーを進める
        /// </summary>
        private void SkipNewLine()
        {
            var next = Reader.PeekChar();

            while (next == '\r'
                || next == '\n')
            {
                Reader.ReadChar();
                next = Reader.PeekChar();
            }
        }
    }
}
