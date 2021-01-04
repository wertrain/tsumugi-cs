using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Script.AbstractSyntaxTree;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script.Lexing;
using Tsumugi.Script.Parsing;

namespace TsumugiUnitTest
{
    [TestClass]
    public class ParserUnitTest
    {
        [TestMethod]
        public void TestMethodParserBasic()
        {
            var script = "" +
                        "let x = 5;" +
                        "let y = 10;" +
                        "let xyz = 838383;" +
                        "";

            var lexer = new Lexer(script);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();

            Assert.AreEqual(
                root.Statements.Count, 3,
                "Root.Statementsの数が間違っています。"
            );

            var tests = new string[] { "x", "y", "xyz" };
            for (int i = 0; i < tests.Length; i++)
            {
                var name = tests[i];
                var statement = root.Statements[i];
                letStatement(statement, name);
            }
        }

        [TestMethod]
        public void TestMethodParserBasicError()
        {
            var script = "" +
                        "let x 5;" +
                        "let = 10;" +
                        "let;" +
                        "";

            var lexer = new Lexer(script);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();

            Assert.IsTrue(parser.Logger.Count(Logger.Categories.Error) == 3, parser.Logger.GetHistory(Logger.Categories.Error));
        }

        private void letStatement(IStatement statement, string name)
        {
            Assert.AreEqual(
                statement.TokenLiteral(), "let",
                "TokenLiteral が let ではありません。"
            );

            var letStatement = statement as LetStatement;
            if (letStatement == null)
            {
                Assert.Fail("statement が LetStatement ではありません。");
            }

            Assert.AreEqual(
                letStatement.Name.Value, name,
                $"識別子が間違っています。"
            );

            Assert.AreEqual(
                letStatement.Name.TokenLiteral(), name,
                $"識別子のリテラルが間違っています。"
            );

        }

        [TestMethod]
        public void TestMethodParserReturnStatement()
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this._CheckParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 3,
                "Root.Statementsの数が間違っています。"
            );

            foreach (var statement in root.Statements)
            {
                var returnStatement = statement as ReturnStatement;
                if (returnStatement == null)
                {
                    Assert.Fail("statement が ReturnStatement ではありません。");
                }

                Assert.AreEqual(
                    returnStatement.TokenLiteral(), "return",
                    $"return のリテラルが間違っています。"
                );
            }
        }
    }
}
