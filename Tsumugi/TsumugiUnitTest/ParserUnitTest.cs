using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Script.AbstractSyntaxTree;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script;
using Tsumugi.Script.Lexing;
using Tsumugi.Script.Parsing;
using System.Collections.Generic;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;

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
            checkParserErrors(parser);

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
            var script = "" +
                        "return 5;" +
                        "return 10;" +
                        "return = 993322;" +
                        "";
            var lexer = new Lexer(script);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            checkParserErrors(parser);

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

        private void checkParserErrors(Parser parser)
        {
            if (parser.Logger.Count(Logger.Categories.Error) == 0) return;
            Assert.Fail(parser.Logger.GetHistory(Logger.Categories.Error));
        }

        [TestMethod]
        public void TestMethodParserToCode()
        {
            var code = "let x = abc;";

            var root = new Root();
            root.Statements = new List<IStatement>();

            root.Statements.Add(
                new LetStatement()
                {
                    Token = new Token(TokenType.Let, "let"),
                    Name = new Identifier(
                        new Token(TokenType.Identifier, "x"),
                        "x"
                    ),
                    Value = new Identifier(
                        new Token(TokenType.Identifier, "abc"),
                        "abc"
                    ),
                }
            );

            Assert.AreEqual(code, root.ToCode(), "Root.ToCode() の結果が間違っています。");
        }

        [TestMethod]
        public void TestMethodParserIdentifierExpression()
        {
            var input = @"foobar;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this.checkParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            var ident = statement.Expression as Identifier;
            if (ident == null)
            {
                Assert.Fail("Expression が Identifier ではありません。");
            }
            if (ident.Value != "foobar")
            {
                Assert.Fail("ident.Value が foobar ではありません。");
            }
            if (ident.TokenLiteral() != "foobar")
            {
                Assert.Fail("ident.TokenLiteral が foobar ではありません。");
            }
        }

        [TestMethod]
        public void TestMethodParserIntegerExpression()
        {
            var input = @"12345;";

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            this.checkParserErrors(parser);

            Assert.AreEqual(
                root.Statements.Count, 1,
                "Root.Statementsの数が間違っています。"
            );

            var statement = root.Statements[0] as ExpressionStatement;
            if (statement == null)
            {
                Assert.Fail("statement が ExpressionStatement ではありません。");
            }

            var integerLiteral = statement.Expression as IntegerLiteral;
            if (integerLiteral == null)
            {
                Assert.Fail("Expression が IntegerLiteral ではありません。");
            }
            if (integerLiteral.Value != 12345)
            {
                Assert.Fail("ident.Value が 12345 ではありません。");
            }
            if (integerLiteral.TokenLiteral() != "12345")
            {
                Assert.Fail("ident.TokenLiteral が 12345 ではありません。");
            }
        }
    }
}
