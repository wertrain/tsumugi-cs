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
        /// <summary>
        /// Let 文のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserLetStatement()
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

        /// <summary>
        /// 正しくない Let 文でのテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserLetStatementError()
        {
            var script = "" +
                        "let x 5;" +
                        "let = 10;" +
                        "let;" +
                        "";

            var lexer = new Lexer(script);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();

            Assert.IsTrue(parser.Logger.Count(Logger.Categories.Error) >= 3, parser.Logger.GetHistory(Logger.Categories.Error));
        }

        /// <summary>
        /// Let 文のテスト
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="name"></param>
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
                "識別子が間違っています。"
            );

            Assert.AreEqual(
                letStatement.Name.TokenLiteral(), name,
                "識別子のリテラルが間違っています。"
            );

        }

        /// <summary>
        /// Return 文のテスト
        /// </summary>
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

        /// <summary>
        /// ToCode によるコード復元のテスト
        /// </summary>
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

        /// <summary>
        /// 識別子式のテスト
        /// </summary>
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

        /// <summary>
        /// 整数値式のテスト
        /// </summary>
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
                Assert.Fail("integerLiteral.Value が 12345 ではありません。");
            }
            if (integerLiteral.TokenLiteral() != "12345")
            {
                Assert.Fail("integerLiteral.TokenLiteral が 12345 ではありません。");
            }
        }

        /// <summary>
        /// 前置演算子式のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParsertPrefixExpression()
        {
            var tests = new[] {
                ("!5", "!", 5),
                ("-15", "-", 15),
            };

            foreach (var (input, op, value) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                checkParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0] as ExpressionStatement;
                if (statement == null)
                {
                    Assert.Fail("statement が ExpressionStatement ではありません。");
                }

                var expression = statement.Expression as PrefixExpression;
                if (expression == null)
                {
                    Assert.Fail("expression が PrefixExpression ではありません。");
                }

                if (expression.Operator != op)
                {
                    Assert.Fail($"Operator が {expression.Operator} ではありません。({op})");
                }

                testIntegerLiteral(expression.Right, value);
            }
        }

        /// <summary>
        /// 中置演算子式のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParsertInfixExpression()
        {
            var tests = new[] {
                ("1 + 1;", 1, "+", 1),
                ("1 - 1;", 1, "-", 1),
                ("1 * 1;", 1, "*", 1),
                ("1 / 1;", 1, "/", 1),
                ("1 < 1;", 1, "<", 1),
                ("1 > 1;", 1, ">", 1),
                ("1 == 1;", 1, "==", 1),
                ("1 != 1;", 1, "!=", 1),
            };

            foreach (var (input, leftValue, op, rightValue) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                checkParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0] as ExpressionStatement;
                if (statement == null)
                {
                    Assert.Fail("statement が ExpressionStatement ではありません。");
                }
                
                testInfixExpression(statement.Expression, leftValue, op, rightValue);
            }
        }

        /// <summary>
        /// パースでエラーが発生すれば Fail とする
        /// </summary>
        /// <param name="parser">チェックするパーサー</param>
        private void checkParserErrors(Parser parser)
        {
            if (parser.Logger.Count(Logger.Categories.Error) == 0) return;
            Assert.Fail(parser.Logger.GetHistory(Logger.Categories.Error));
        }

        /// <summary>
        /// リテラルごとの式テスト
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="expected"></param>
        private void testLiteralExpression(IExpression expression, object expected)
        {
            switch (expected)
            {
                case int intValue:
                    testIntegerLiteral(expression, intValue);
                    break;
                case string stringValue:
                    testIdentifier(expression, stringValue);
                    break;
                default:
                    Assert.Fail("予期せぬ型です。");
                    break;
            }
        }

        /// <summary>
        /// 識別子式のテスト
        /// </summary>
        /// <param name="expression">チェックする式</param>
        /// <param name="value">式の値</param>
        private void testIdentifier(IExpression expression, string value)
        {
            var identifier = expression as Identifier;

            if (identifier == null)
            {
                Assert.Fail("Expression が Identifier ではありません。");
            }
            if (identifier.Value != value)
            {
                Assert.Fail($"identifier.Value が {value} ではありません。({identifier.Value})");
            }
            if (identifier.TokenLiteral() != value)
            {
                Assert.Fail($"identifier.TokenLiteral が {value} ではありません。({identifier.TokenLiteral()})");
            }
        }

        /// <summary>
        /// 整数値式のテスト
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        public void testIntegerLiteral(IExpression expression, int value)
        {
            var integerLiteral = expression as IntegerLiteral;

            if (integerLiteral == null)
            {
                Assert.Fail("Expression が IntegerLiteral ではありません。");
            }
            if (integerLiteral.Value != value)
            {
                Assert.Fail($"integerLiteral.Value が {value} ではありません。");
            }
            if (integerLiteral.TokenLiteral() != $"{value}")
            {
                Assert.Fail($"ident.TokenLiteral が {value} ではありません。");
            }
        }

        /// <summary>
        /// 中置演算子式のテスト
        /// </summary>
        /// <param name="expression">式</param>
        /// <param name="left">左辺</param>
        /// <param name="op">演算子</param>
        /// <param name="right">右辺</param>
        private void testInfixExpression(IExpression expression, object left, string op, object right)
        {
            var infixExpression = expression as InfixExpression;
            if (infixExpression == null)
            {
                Assert.Fail("expression が InfixExpression ではありません。");
            }

            testLiteralExpression(infixExpression.Left, left);

            if (infixExpression.Operator != op)
            {
                Assert.Fail($"Operator が {infixExpression.Operator} ではありません。({op})");
            }

            testLiteralExpression(infixExpression.Right, right);
        }
    }
}
