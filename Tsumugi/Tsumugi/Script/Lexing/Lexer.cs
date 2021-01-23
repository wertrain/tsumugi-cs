using System.IO;
using System.Text;

namespace Tsumugi.Script.Lexing
{
    /// <summary>
    /// 解析位置
    /// </summary>
    public class LexingPosition
    {
        /// <summary>
        /// 文字位置
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 行
        /// </summary>
        public int Lines { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public int Columns { get; set; }
    }

    /// <summary>
    /// 字句解析
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// 字句解析用の文字列リーダー
        /// </summary>
        private LexingStringReader Reader { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script"></param>
        public Lexer(string script)
        {
            Reader = new LexingStringReader(script);
        }

        /// <summary>
        /// 次のトークンを取り出す
        /// </summary>
        /// <returns></returns>
        public Token NextToken()
        {
            SkipWhiteSpace();

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
                    case '=':
                        // 次の文字によって、トークンの意味が変わるのでチェックする
                        if (Reader.PeekChar(1) == '=')
                        {
                            // = が二回続けば比較演算子
                            token = CreateToken(TokenType.Equal, "==");
                            Reader.ReadChar();
                        }
                        // そうでなければ代入演算子
                        else
                        {
                            token = CreateToken(TokenType.Assign, c.ToString());
                        }
                        break;

                    case '+':
                        token = CreateToken(TokenType.Plus, c.ToString());
                        break;

                    case '-':
                        token = CreateToken(TokenType.Minus, c.ToString());
                        break;

                    case '*':
                        token = CreateToken(TokenType.Asterisk, c.ToString());
                        break;

                    case '/':
                        token = CreateToken(TokenType.Slash, c.ToString());
                        break;

                    case '!':
                        if (Reader.PeekChar(1) == '=')
                        {
                            token = CreateToken(TokenType.NotEqual, "!=");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Bang, c.ToString());
                        }
                        break;

                    case '>':
                        if (Reader.PeekChar(1) == '=')
                        {
                            token = CreateToken(TokenType.GreaterThanOrEqual, ">=");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = CreateToken(TokenType.GreaterThan, c.ToString());
                        }
                        break;

                    case '<':
                        if (Reader.PeekChar(1) == '=')
                        {
                            token = CreateToken(TokenType.LessThanOrEqual, "<=");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = CreateToken(TokenType.LessThan, c.ToString());
                        }
                        break;

                    case '&':
                        if (Reader.PeekChar(1) == '&')
                        {
                            token = CreateToken(TokenType.And, "&&");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = CreateToken(TokenType.LogicalConjunction, c.ToString());
                        }
                        break;


                    case '|':
                        if (Reader.PeekChar(1) == '|')
                        {
                            token = CreateToken(TokenType.Or, "||");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = CreateToken(TokenType.LogicalDisjunction, c.ToString());
                        }
                        break;

                    case ',':
                        token = CreateToken(TokenType.Comma, c.ToString());
                        break;

                    case ';':
                        token = CreateToken(TokenType.Semicolon, c.ToString());
                        break;

                    case '(':
                        token = CreateToken(TokenType.LeftParenthesis, c.ToString());
                        break;

                    case ')':
                        token = CreateToken(TokenType.RightParenthesis, c.ToString());
                        break;

                    case '{':
                        token = CreateToken(TokenType.LeftBraces, c.ToString());
                        break;

                    case '}':
                        token = CreateToken(TokenType.RightBraces, c.ToString());
                        break;

                    case '[':
                        token = CreateToken(TokenType.LeftBrackets, c.ToString());
                        break;

                    case ']':
                        token = CreateToken(TokenType.RightBrackets, c.ToString());
                        break;

                    case '"':
                        token = TryReadStringToken();
                        break;

                    default:
                        if (IsLetter(c))
                        {
                            var identifier = ReadIdentifier();
                            var type = Token.LookupIdentifier(identifier);
                            token = CreateToken(type, identifier);
                        }
                        else if (IsDigit(c))
                        {
                            token = ReadNumberToken();
                        }
                        else
                        {
                            token = CreateToken(TokenType.Illegal, c.ToString());
                        }
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
        /// 文字列が連続する間、数字として読み出す
        /// </summary>
        /// <returns>読みだした数字</returns>
        private string ReadNumber()
        {
            var number = new StringBuilder(Reader.ReadChar().ToString());

            while (IsDigit(Reader.PeekChar()))
            {
                number.Append(Reader.ReadChar());
            }

            Reader.Seek(-1, SeekOrigin.Current);

            return number.ToString();
        }

        /// <summary>
        /// 文字列が連続する間、識別子として読み出す
        /// </summary>
        /// <returns>読みだした識別子</returns>
        private string ReadIdentifier()
        {
            var identifier = new StringBuilder(Reader.ReadChar().ToString());

            while (IsLetter(Reader.PeekChar()))
            {
                identifier.Append(Reader.ReadChar());
            }

            Reader.Seek(-1, SeekOrigin.Current);

            return identifier.ToString();
        }

        /// <summary>
        /// 数値として読み出す
        /// </summary>
        /// <returns></returns>
        private Token ReadNumberToken()
        {
            var numberBuffer = new StringBuilder(Reader.ReadChar().ToString());

            while (IsDigit(Reader.PeekChar()) || '.' == Reader.PeekChar())
            {
                numberBuffer.Append(Reader.ReadChar());
            }

            Reader.Seek(-1, SeekOrigin.Current);

            var number = numberBuffer.ToString();

            if (number.IndexOf('.') > 0)
            {
                if (double.TryParse(number, out var result))
                {
                    return new Token(TokenType.Double, number);
                }
            }
            else
            {
                if (int.TryParse(number, out var result))
                {
                    return new Token(TokenType.Integer, number);
                }
            }

            return CreateToken(TokenType.Illegal, number);
        }

        /// <summary>
        /// ダブルクォーテーションまで、文字列として読み出す
        /// </summary>
        /// <returns></returns>
        private Token TryReadStringToken()
        {
            var str = new StringBuilder();

            // 最初のダブルクォーテーションを読み飛ばす（ついでに c を初期化）
            char c = Reader.ReadChar();

            while ((c = Reader.PeekChar()) != '"')
            {
                // エスケープシーケンス
                if (c == '\\')
                {
                    // 読み飛ばす
                    Reader.ReadChar();
                }
                // 改行または終端が先に見つかった場合は、不正なトークン
                else if (c == '\r' || c == '\n' || c == char.MaxValue)
                {
                    return CreateToken(TokenType.Illegal, c.ToString());
                }

                str.Append(Reader.ReadChar());
            }

            return CreateToken(TokenType.String, str.ToString());
        }

        /// <summary>
        /// 引数が数字かチェックする
        /// </summary>
        /// <param name="c">対象の文字</param>
        /// <returns>引数が数字なら true</returns>
        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        /// <summary>
        /// 引数が（識別子として有効な）文字かをチェックする
        /// </summary>
        /// <param name="c"></param>
        /// <returns>引数が文字なら true</returns>
        private bool IsLetter(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || c == '_';
        }

        /// <summary>
        /// 空白・タブ・改行などの間、リーダーを進める
        /// </summary>
        private void SkipWhiteSpace()
        {
            var next = Reader.PeekChar();

            while (next == ' '
                || next == '\t'
                || next == '\r'
                || next == '\n')
            {
                Reader.ReadChar();
                next = Reader.PeekChar();
            }
        }
    }
}
