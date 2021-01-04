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
        private TsumugiStringReader Reader { get; set; }

        public Lexer(string script)
        {
            Reader = new TsumugiStringReader(script);
        }

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
                        token = new Token(TokenType.Assign, c.ToString());
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

        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        private bool IsLetter(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || c == '_';
        }

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
