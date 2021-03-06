﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Script.AbstractSyntaxTree;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script;
using Tsumugi.Script.Lexing;
using Tsumugi.Script.Parsing;
using System.Collections.Generic;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;
using Tsumugi.Log;

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
                testLetStatement(statement, name);
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

        public void TestMethodParserLetStatement2()
        {
            var tests = new (string, string, object)[]
            {
                ("let x = 5;", "x", 5),
                ("let y = true;", "y", true),
                ("let z = x;", "z", "x"),
            };

            foreach (var (input, name, expected) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                checkParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var statement = root.Statements[0];
                testLetStatement(statement, name);

                var value = (statement as LetStatement).Value;
                testLiteralExpression(value, expected);
            }
        }

        /// <summary>
        /// Let 文のテスト
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="name"></param>
        private void testLetStatement(IStatement statement, string name)
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
            var tests = new (string, object)[]
            {
                ("return 5;", 5),
                ("return true;", true),
                ("return x;", "x"),
            };

            foreach (var (input, expected) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                checkParserErrors(parser);

                Assert.AreEqual(
                    root.Statements.Count, 1,
                    "Root.Statementsの数が間違っています。"
                );

                var returnStatement = root.Statements[0] as ReturnStatement;
                if (returnStatement == null)
                {
                    Assert.Fail("statement が ReturnStatement ではありません。");
                }

                Assert.AreEqual(
                    returnStatement.TokenLiteral(), "return",
                    $"return のリテラルが間違っています。"
                );

                testLiteralExpression(returnStatement.ReturnValue, expected);
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
        /// 演算式のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserOperatorPrecedenceParsing()
        {
            var tests = new[]
            {
                ("a + b", "(a + b)"),
                ("!-a", "(!(-a))"),
                ("a + b - c", "((a + b) - c)"),
                ("a * b / c", "((a * b) / c)"),
                ("a + b * c", "(a + (b * c))"),
                ("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
                ("1 + 2; -3 * 4", "(1 + 2)\r\n((-3) * 4)"),
                ("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
                ("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
                ("true", "true"),
                ("true == false", "(true == false)"),
                ("1 > 2 == false", "((1 > 2) == false)"),
                ("(1 + 2) * 3", "((1 + 2) * 3)"),
                ("1 + (2 - 3)", "(1 + (2 - 3))"),
                ("-(1 + 2)", "(-(1 + 2))"),
                ("!(true == true)", "(!(true == true))"),
                ("1 + (2 - 3) * 4", "(1 + ((2 - 3) * 4))"),
                ("(1 + -(2 + 3)) * 4", "((1 + (-(2 + 3))) * 4)"),
                // 関数呼び出し式の ( も演算子として扱うのでここでテストする
                ("add(1, 2) + 3 > 4", "((add(1, 2) + 3) > 4)"),
                ("add(x, y, 1, 2*3, 4+5, add(z) )", "add(x, y, 1, (2 * 3), (4 + 5), add(z))"),
                ("add(1 + 2 - 3 * 4 / 5 + 6)", "add((((1 + 2) - ((3 * 4) / 5)) + 6))"),
            };

            foreach (var (input, code) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();
                checkParserErrors(parser);

                var actual = root.ToCode();
                Assert.AreEqual(code, actual);
            }
        }

        /// <summary>
        /// 前置演算子式のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserPrefixExpression()
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
        public void TestMethodParserInfixExpression()
        {
            var tests = new (string, object, string, object)[] {
                ("1 + 1;", 1, "+", 1),
                ("1 - 1;", 1, "-", 1),
                ("1 * 1;", 1, "*", 1),
                ("1 / 1;", 1, "/", 1),
                ("1 < 1;", 1, "<", 1),
                ("1 > 1;", 1, ">", 1),
                ("1 == 1;", 1, "==", 1),
                ("1 != 1;", 1, "!=", 1),
                ("true == false", true, "==", false),
                ("false != false", false, "!=", false),
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
                case bool boolValue:
                    testBooleanLiteral(expression, boolValue);
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

        /// <summary>
        /// 真偽式のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserBooleanLiteralExpression()
        {
            var tests = new[]
            {
                ("true;", true),
                ("false;", false),
            };

            foreach (var (input, value) in tests)
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

                testBooleanLiteral(statement.Expression, value);
            }
        }

        /// <summary>
        /// 真偽式のテスト
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        private void testBooleanLiteral(IExpression expression, bool value)
        {
            var booleanLiteral = expression as BooleanLiteral;
            if (booleanLiteral == null)
            {
                Assert.Fail("Expression が BooleanLiteral ではありません。");
            }
            if (booleanLiteral.Value != value)
            {
                Assert.Fail($"booleanLiteral.Value が {value} ではありません。({booleanLiteral.Value})");
            }
            // bool値をToString()すると "True", "False" になるので小文字化してます
            if (booleanLiteral.TokenLiteral() != value.ToString().ToLower())
            {
                Assert.Fail($"booleanLiteral.TokenLiteral が {value.ToString().ToLower()} ではありません。({booleanLiteral.TokenLiteral()})");
            }
        }

        /// <summary>
        /// 関数式のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserFunctionLiteral()
        {
            var input = "function(x, y) { x + y; }";
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

            var expression = statement.Expression as FunctionLiteral;
            if (expression == null)
            {
                Assert.Fail("expression が FunctionLiteral ではありません。");
            }

            Assert.AreEqual(
                expression.Parameters.Count, 2,
                "関数リテラルの引数の数が間違っています。"
            );
            testIdentifier(expression.Parameters[0], "x");
            testIdentifier(expression.Parameters[1], "y");

            Assert.AreEqual(
                expression.Body.Statements.Count, 1,
                "関数リテラルの本文の式の数が間違っています。"
            );

            var bodyStatement = expression.Body.Statements[0] as ExpressionStatement;
            if (bodyStatement == null)
            {
                Assert.Fail("bodyStatement が ExpressionStatement ではありません。");
            }
            testInfixExpression(bodyStatement.Expression, "x", "+", "y");
        }

        /// <summary>
        /// 関数式の引数のテスト
        /// </summary>
        [TestMethod]
        public void TestMethodParserFunctionParameter()
        {
            var tests = new[]
            {
                ("function() {};", new string[] { }),
                ("function(x) {};", new string[] { "x" }),
                ("function(x, y, z) {};", new string[] { "x", "y", "z" }),
            };

            foreach (var (input, parameters) in tests)
            {
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var root = parser.ParseProgram();

                var statement = root.Statements[0] as ExpressionStatement;
                var fn = statement.Expression as FunctionLiteral;

                Assert.AreEqual(
                    fn.Parameters.Count, parameters.Length,
                    "関数リテラルの引数の数が間違っています。"
                );
                for (int i = 0; i < parameters.Length; i++)
                {
                    testIdentifier(fn.Parameters[i], parameters[i]);
                }
            }
        }

        /// <summary>
        /// 関数呼び出し式のテスト
        /// </summary>
        [TestMethod]
        public void TestCallExpression()
        {
            var input = "add(1, 2 * 3, 4 + 5);";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            parser.Logger = new Logger();
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

            var expression = statement.Expression as CallExpression;
            if (expression == null)
            {
                Assert.Fail("expression が CallExpression ではありません。");
            }

            testIdentifier(expression.Function, "add");

            Assert.AreEqual(
                expression.Arguments.Count, 3,
                "関数リテラルの引数の数が間違っています。"
            );

            testLiteralExpression(expression.Arguments[0], 1);
            testInfixExpression(expression.Arguments[1], 2, "*", 3);
            testInfixExpression(expression.Arguments[2], 4, "+", 5);
        }
    }
}
