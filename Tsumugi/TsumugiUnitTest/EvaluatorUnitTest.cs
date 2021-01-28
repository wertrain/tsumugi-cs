using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tsumugi.Script.Objects;
using Tsumugi.Script.Lexing;
using Tsumugi.Script.Parsing;
using Tsumugi.Script.Evaluating;

namespace TsumugiUnitTest
{
    /// <summary>
    /// EvaluatorUnitTest の概要の説明
    /// </summary>
    [TestClass]
    public class EvaluatorUnitTest
    {
        [TestMethod]
        public void TestEvalIntegerExpression()
        {
            var tests = new (string, int)[]
            {
                ("1", 1),
                ("12", 12),
                ("-1", -1),
                ("-12", -12),
                ("1 + 2 - 3", 0),
                ("1 + 2 * 3", 7),
                ("3 * 4 / 2 + 10 - 8", 8),
                ("(1 + 2) * 3 - -1", 10),
                ("-1 * -1", 1),
                ("-10 + -1 * 2", -12),
                ("(10 + 20) / (10 - 0)", 3),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testIntegerObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestEvalDoubleExpression()
        {
            var tests = new (string, double)[]
            {
                ("1.0", 1.0),
                ("12.0", 12.0),
                ("-1.0", -1.0),
                ("-12.0", -12.0),
                ("1.0 + 2.0 - 3.0", 0.0),
                ("1.0 + 2.0 * 3.0", 7.0),
                ("3.0 * 4.0 / 2.0 + 10.0 - 8.0", 8.0),
                ("(1.0 + 2.0) * 3.0 - -1.0", 10.0),
                ("-1.0 * -1.0", 1.0),
                ("-10.0 + -1.0 * 2.0", -12.0),
                ("(10.0 + 20.0) / (10.0 - 0.0)", 3.0),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testDoubleObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestEvalStringExpression()
        {
            var tests = new (string, string)[]
            {
                ("\"str\"", "str"),
                ("\"str\" + \"str\"", "strstr"),
                ("\"str\" * 2", "strstr"),
                ("\"str\" * 4", "strstrstrstr"),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testStringObject(evaluated, expected);
            }
        }

        private IObject testEval(string input)
        {
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            var evaluator = new Evaluator();
            var enviroment = new Enviroment();
            return evaluator.Eval(root, enviroment);
        }

        private void testIntegerObject(IObject obj, int expected)
        {
            var result = obj as IntegerObject;
            if (result == null)
            {
                Assert.Fail("object が Integer ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }

        private void testDoubleObject(IObject obj, double expected)
        {
            var result = obj as DoubleObject;
            if (result == null)
            {
                Assert.Fail("object が Double ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }

        private void testStringObject(IObject obj, string expected)
        {
            var result = obj as StringObject;
            if (result == null)
            {
                Assert.Fail("object が String ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }

        [TestMethod]
        public void TestEvalBooleanExpression()
        {
            var tests = new (string, bool)[]
            {
                ("1 <= 2", true),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testBooleanObject(evaluated, expected);
            }
        }

        private void testBooleanObject(IObject obj, bool expected)
        {
            var result = obj as BooleanObject;
            if (result == null)
            {
                Assert.Fail("object が Boolean ではありません。");
            }

            Assert.AreEqual(expected, result.Value);
        }

        [TestMethod]
        public void TestEvalBangOperator()
        {
            var tests = new (string, bool)[]
            {
                ("!true", false),
                ("!false", true),
                ("!5", false),
                ("!!true", true),
                ("!!!true", false),
                ("!!5", true),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testBooleanObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestEvalIfExpression()
        {
            var tests = new (string, int?)[]
            {
                ("if (true) { 1 }", 1),
                ("if (false) { 1 }", null),
                ("if (true) { 1 } else { 2 }", 1),
                ("if (false) { 1 } else { 2 }", 2),
                ("if (5) { 1 } else { 2 }", 1),
                ("if (!5) { 1 } else { 2 }", 2),
                ("if (1 < 2) { 1 } else { 2 }", 1),
                ("if (1 > 2) { 1 } else { 2 }", 2),
                ("if (1 > 2) { 1 }", null),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                if (expected.HasValue)
                {
                    testIntegerObject(evaluated, expected.Value);
                }
                else
                {
                    testNullObject(evaluated);
                }
            }
        }

        private void testNullObject(object obj)
        {
            var nullObject = obj as NullObject;
            if (nullObject == null)
            {
                Assert.Fail($"Object が Null ではありません。{obj.GetType()}");
            }
        }

        [TestMethod]
        public void TestEvalReturnStatement()
        {
            var tests = new (string, int)[]
            {
                ("return 10;", 10),
                ("return 100/10", 10),
                ("return 10; 1234;", 10),
                ("2*3; return 10; 1234;", 10),
                (@"if (true) {
                        if (true) {
                            return 10;
                        }
                        0;
                   }", 10),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testIntegerObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestErrorHandling()
        {
            var tests = new (string, string)[]
            {
                ("5 + true;", "型のミスマッチ：Integer + Boolean"),
                ("5 + true; 5;", "型のミスマッチ：Integer + Boolean"),
                ("-true", "未知の演算子：-Boolean"),
                ("true + false", "未知の演算子：Boolean + Boolean"),
                ("if (true) { true * false; }", "未知の演算子：Boolean * Boolean"),
                (@"if (true) {
                        if (true) {
                            return false / false;
                        }
                        0;
                    }", "未知の演算子：Boolean / Boolean"),
                ("-true + 100", "未知の演算子：-Boolean"),
                ("foo", "未定義の識別子 foo です。"),
            };

            var logger = new Tsumugi.Log.Logger();
            foreach (var (input, expected) in tests)
            {
                var evaluated = this.testEval(input);
                var error = evaluated as Error;
                if (error == null)
                {
                    Assert.Fail($"エラーオブジェクトではありません。({evaluated.GetType()})");
                }

                Assert.AreEqual(expected, error.Message);
            }
        }

        [TestMethod]
        public void TestEvalLetStatement()
        {
            var tests = new (string, int)[]
            {
                ("let a = 1; a;", 1),
                ("let a = 1 + 2 * 3; a;", 7),
                ("let a = 1; let b = a; b;", 1),
                ("let a = 1; let b = 2; let c = a + b; c;", 3),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testIntegerObject(evaluated, expected);
            }
        }

        [TestMethod]
        public void TestEvalFunctionObject()
        {
            var input = "function(x) { x + 2; }";
            var evaluated = testEval(input);

            var fn = evaluated as FunctionObject;
            if (fn == null)
            {
                Assert.Fail($"オブジェクトが関数ではありません。({fn?.GetType()})");
            }

            Assert.AreEqual(fn.Parameters.Count, 1);
            Assert.AreEqual(fn.Parameters[0].ToCode(), "x");
            Assert.AreEqual(fn.Body.ToCode(), "{(x + 2)}");
        }

        [TestMethod]
        public void TestFunctionApplication()
        {
            var tests = new (string, int)[]
            {
                ("let identity = function(x) { x }; identity(10);", 10),
                ("let identity = function(x) { return x; }; identity(10);", 10),
                ("let double = function(x) { x * 2; }; double(10);", 20),
                ("let add = function(x, y) { x + y; }; add(10, 20);", 30),
                ("let add = function(x, y) { x + y; }; add(add(10, 20), 30 + 40);", 100),
                ("function(x) { x; }(10);", 10),
            };

            foreach (var (input, expected) in tests)
            {
                var evaluated = testEval(input);
                testIntegerObject(evaluated, expected);
            }
        }
    }
}
