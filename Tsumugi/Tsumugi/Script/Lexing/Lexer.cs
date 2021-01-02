using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tsumugi.Script.Lexing
{
    public class Lexer
    {
        private StringReader Reader { get; set; }

        public Lexer(string script)
        {
            Reader = new StringReader(script);
        }

        public Token NextToken()
        {
            int next = -1;
            while ((next = Reader.Read()) >= 0)
            {
                char c = (char)next;
                switch (c)
                {
                    case '=':
                        return new Token(TokenType.Assign, c.ToString());

                    case '+':
                        return new Token(TokenType.Plus, c.ToString());

                    case ',':
                        return new Token(TokenType.Comma, c.ToString());

                    case ';':
                        return new Token(TokenType.Semicolon, c.ToString());

                    case '(':
                        return new Token(TokenType.LeftParenthesis, c.ToString());

                    case ')':
                        return new Token(TokenType.RightParenthesis, c.ToString());

                    case '{':
                        return new Token(TokenType.LeftBraces, c.ToString());
   
                    case '}':
                        return new Token(TokenType.RightBraces, c.ToString());

                    case '[':
                        return new Token(TokenType.LeftBrackets, c.ToString());

                    case ']':
                        return new Token(TokenType.RightBrackets, c.ToString());
                }
            }

            return new Token(TokenType.EOF, "");
        }
    }
}
