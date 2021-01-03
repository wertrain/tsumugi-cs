using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Tsumugi.Script.Lexing;

namespace TsumugiUnitTest
{
    [TestClass]
    public class LexerUnitTest
    {
        [TestMethod]
        public void TestMethodLexerBasic()
        {
            var tokens = new List<Token>();
            tokens.Add(new Token(TokenType.Assign, "="));
            tokens.Add(new Token(TokenType.Plus, "+"));
            tokens.Add(new Token(TokenType.LeftParenthesis, "("));
            tokens.Add(new Token(TokenType.RightParenthesis, ")"));
            tokens.Add(new Token(TokenType.LeftBraces, "{"));
            tokens.Add(new Token(TokenType.RightBraces, "}"));
            tokens.Add(new Token(TokenType.LeftBrackets, "["));
            tokens.Add(new Token(TokenType.RightBrackets, "]"));
            tokens.Add(new Token(TokenType.Comma, ","));
            tokens.Add(new Token(TokenType.Semicolon, ";"));
            tokens.Add(new Token(TokenType.EOF, ""));

            var lexer = new Lexer("=+(){}[],;");
            foreach (var testToken in tokens)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(testToken.Type, token.Type, string.Format("{0} token type is wrong.", testToken.Type.ToString()));
                Assert.AreEqual(testToken.Literal, token.Literal, string.Format("{0} literal is wrong.", testToken.Literal));
            }
        }

        [TestMethod]
        public void TestMethodLexerBasicCode()
        {
            var tokens = new List<Token>();
            tokens.Add(new Token(TokenType.Let, "let"));
            tokens.Add(new Token(TokenType.Identifier, "five"));
            tokens.Add(new Token(TokenType.Assign, "="));
            tokens.Add(new Token(TokenType.Integer32, "5"));
            tokens.Add(new Token(TokenType.Semicolon, ";"));
            tokens.Add(new Token(TokenType.Let, "let"));
            tokens.Add(new Token(TokenType.Identifier, "ten"));
            tokens.Add(new Token(TokenType.Assign, "="));
            tokens.Add(new Token(TokenType.Integer32, "10"));
            tokens.Add(new Token(TokenType.Semicolon, ";"));
            // let add = function(x, y) { x + y; };
            tokens.Add(new Token(TokenType.Let, "let"));
            tokens.Add(new Token(TokenType.Identifier, "add"));
            tokens.Add(new Token(TokenType.Assign, "="));
            tokens.Add(new Token(TokenType.Function, "function"));
            tokens.Add(new Token(TokenType.LeftParenthesis, "("));
            tokens.Add(new Token(TokenType.Identifier, "x"));
            tokens.Add(new Token(TokenType.Comma, ","));
            tokens.Add(new Token(TokenType.Identifier, "y"));
            tokens.Add(new Token(TokenType.RightParenthesis, ")"));
            tokens.Add(new Token(TokenType.LeftBraces, "{"));
            tokens.Add(new Token(TokenType.Identifier, "x"));
            tokens.Add(new Token(TokenType.Plus, "+"));
            tokens.Add(new Token(TokenType.Identifier, "y"));
            tokens.Add(new Token(TokenType.Semicolon, ";"));
            tokens.Add(new Token(TokenType.RightBraces, "}"));
            tokens.Add(new Token(TokenType.Semicolon, ";"));
            // let result = add(five, ten);
            tokens.Add(new Token(TokenType.Let, "let"));
            tokens.Add(new Token(TokenType.Identifier, "result"));
            tokens.Add(new Token(TokenType.Assign, "="));
            tokens.Add(new Token(TokenType.Identifier, "add"));
            tokens.Add(new Token(TokenType.LeftParenthesis, "("));
            tokens.Add(new Token(TokenType.Identifier, "five"));
            tokens.Add(new Token(TokenType.Comma, ","));
            tokens.Add(new Token(TokenType.Identifier, "ten"));
            tokens.Add(new Token(TokenType.RightParenthesis, ")"));
            tokens.Add(new Token(TokenType.Semicolon, ";"));
            tokens.Add(new Token(TokenType.EOF, ""));

            var script = "" +
                        "let five = 5;" +
                        "let ten = 10;" +
                        "" +
                        "let add = function(x, y) {" +
                        "    x + y;" +
                        "};" +
                        "" +
                        "let result = add(five, ten);" +
                        "";
            var lexer = new Lexer(script);
            foreach (var testToken in tokens)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(testToken.Type, token.Type, string.Format("{0} token type is wrong.", testToken.Type.ToString()));
                Assert.AreEqual(testToken.Literal, token.Literal, string.Format("{0} literal is wrong.", testToken.Literal));
            }
        }
    }
}
