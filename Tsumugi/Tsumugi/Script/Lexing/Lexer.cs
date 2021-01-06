using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tsumugi.Script.Lexing
{
    /// <summary>
    /// 字句解析
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// 字句解析用の文字列リーダー
        /// </summary>
        private TsumugiStringReader Reader { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="script"></param>
        public Lexer(string script)
        {
            Reader = new TsumugiStringReader(script);
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
                token = new Token(TokenType.EOF, string.Empty);
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
                            token = new Token(TokenType.Equal, "==");
                            Reader.ReadChar();
                        }
                        // そうでなければ代入演算子
                        else
                        {
                            token = new Token(TokenType.Assign, c.ToString());
                        }
                        break;

                    case '+':
                        token = new Token(TokenType.Plus, c.ToString());
                        break;

                    case '-':
                        token = new Token(TokenType.Minus, c.ToString());
                        break;

                    case '*':
                        token = new Token(TokenType.Asterisk, c.ToString());
                        break;

                    case '/':
                        token = new Token(TokenType.Slash, c.ToString());
                        break;

                    case '!':
                        if (Reader.PeekChar(1) == '=')
                        {
                            token = new Token(TokenType.NotEqual, "!=");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = new Token(TokenType.Bang, c.ToString());
                        }
                        break;

                    case '>':
                        if (Reader.PeekChar(1) == '=')
                        {
                            token = new Token(TokenType.GreaterThanOrEqual, ">=");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = new Token(TokenType.GreaterThan, c.ToString());
                        }
                        break;

                    case '<':
                        if (Reader.PeekChar(1) == '=')
                        {
                            token = new Token(TokenType.LessThanOrEqual, "<=");
                            Reader.ReadChar();
                        }
                        else
                        {
                            token = new Token(TokenType.LessThan, c.ToString());
                        }
                        break;

                    case ',':
                        token = new Token(TokenType.Comma, c.ToString());
                        break;

                    case ';':
                        token = new Token(TokenType.Semicolon, c.ToString());
                        break;

                    case '(':
                        token = new Token(TokenType.LeftParenthesis, c.ToString());
                        break;

                    case ')':
                        token = new Token(TokenType.RightParenthesis, c.ToString());
                        break;

                    case '{':
                        token = new Token(TokenType.LeftBraces, c.ToString());
                        break;

                    case '}':
                        token = new Token(TokenType.RightBraces, c.ToString());
                        break;

                    case '[':
                        token = new Token(TokenType.LeftBrackets, c.ToString());
                        break;

                    case ']':
                        token = new Token(TokenType.RightBrackets, c.ToString());
                        break;

                    default:
                        if (IsLetter(c))
                        {
                            var identifier = ReadIdentifier();
                            var type = Token.LookupIdentifier(identifier);
                            token = new Token(type, identifier);
                        }
                        else if (IsDigit(c))
                        {
                            var number = ReadNumber();
                            token = new Token(TokenType.Integer32, number);
                        }
                        else
                        {
                            token = new Token(TokenType.Illegal, c.ToString());
                        }
                        break;
                }
            }

            Reader.Read();

            return token;
        }

        /// <summary>
        /// 文字列が連続する間、数字として読み出す
        /// </summary>
        /// <returns>読みだした数字</returns>
        private string ReadNumber()
        {
            var number = Reader.ReadChar().ToString();

            while (IsDigit(Reader.PeekChar()))
            {
                number += Reader.ReadChar();
            }

            Reader.Seek(-1, SeekOrigin.Current);

            return number;
        }

        /// <summary>
        /// 文字列が連続する間、識別子として読み出す
        /// </summary>
        /// <returns>読みだした識別子</returns>
        private string ReadIdentifier()
        {
            var identifier = Reader.ReadChar().ToString();

            while (IsLetter(Reader.PeekChar()))
            {
                identifier += Reader.ReadChar();
            }

            Reader.Seek(-1, SeekOrigin.Current);

            return identifier;
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
