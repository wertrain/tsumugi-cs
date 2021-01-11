using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Text.Lexing;

namespace TsumugiUnitTest
{
    [TestClass]
    public class TsumugiUnitTest
    {
        [TestMethod]
        public void TestMethodLexerBasic()
        {
            var tokens = new List<Token>();
            tokens.Add(new Token(TokenType.Label, "label"));
            tokens.Add(new Token(TokenType.Tag, "tag attr1=0 attr2=0"));
            tokens.Add(new Token(TokenType.Text, "テキスト"));

            var lexer = new Lexer(
                ":label" + Environment.NewLine +
                "[tag attr1=0 attr2=0]" + Environment.NewLine +
                "テキスト" + Environment.NewLine +
            "");
            foreach (var testToken in tokens)
            {
                var token = lexer.NextToken();
                Assert.AreEqual(testToken.Type, token.Type, string.Format("{0} token type is wrong.", testToken.Type.ToString()));
                Assert.AreEqual(testToken.Literal, token.Literal, string.Format("{0} literal is wrong.", testToken.Literal));
            }
        }

        [TestMethod]
        public void TestMethodParsingBasic()
        {
            var parser = new Tsumugi.Text.Parsing.Parser();
            parser.Parse("Hello, Tsumugi!");

            Assert.AreNotEqual(parser.CommandQueue.Dequeue(), null);
        }

        [TestMethod]
        public void TestMethodParsingError()
        {
            var parser = new Tsumugi.Text.Parsing.Parser();

            var script = "" +
                ":start|開始位置" +
                "[var wtime=1000][jump target=notfound]" +
                "こんにちは[r]" +
                "これは Tsumugi のテスト[wait time=notdefine]です。[l][cm]" +
                "ページをクリアしました。[l][r][cm][jump target=start]" +
                "[l]";
            parser.Parse(script);

            Assert.IsTrue(parser.Logger.Count(Tsumugi.Script.Logger.Categories.Error) == 2);
        }
    }
}
