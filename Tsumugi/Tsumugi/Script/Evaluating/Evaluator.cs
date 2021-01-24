using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsumugi.Localize;
using Tsumugi.Script.AbstractSyntaxTree;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script.Objects;

namespace Tsumugi.Script.Evaluating
{
    /// <summary>
    /// 評価器
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// 評価
        /// </summary>
        /// <param name="node">評価するノード</param>
        /// <param name="enviroment">環境</param>
        /// <returns>評価結果オブジェクト</returns>
        public IObject Eval(INode node, Enviroment enviroment)
        {
            switch (node)
            {
                // 文
                case Root root:
                    return EvalRootProgram(root.Statements, enviroment);
                case ExpressionStatement statement:
                    return Eval(statement.Expression, enviroment);
                case BlockStatement blockStatement:
                    return EvalBlockStatement(blockStatement, enviroment);
                case ReturnStatement returnStatement:
                    var value = Eval(returnStatement.ReturnValue, enviroment);
                    if (IsError(value)) return value;
                    return new ReturnValue(value);
                case LetStatement letStatement:
                    var letValue = Eval(letStatement.Value, enviroment);
                    if (IsError(letValue)) return letValue;
                    enviroment.Set(letStatement.Name.Value, letValue);
                    break;
                // 式
                case Identifier identifier:
                    return EvalIdentifier(identifier, enviroment);
                case AssignExpression assignExpression:
                    return EvalAssignExpression(assignExpression, enviroment);
                case IntegerLiteral integerLiteral:
                    return new IntegerObject(integerLiteral.Value);
                case DoubleLiteral doubleLiteral:
                    return new DoubleObject(doubleLiteral.Value);
                case StringLiteral stringLiteral:
                    return new StringObject(stringLiteral.Value);
                case BooleanLiteral booleanLiteral:
                    return ToBooleanObject(booleanLiteral.Value);
                case PrefixExpression prefixExpression:
                    var right = Eval(prefixExpression.Right, enviroment);
                    return EvalPrefixExpression(prefixExpression.Operator, right);
                case InfixExpression infixExpression:
                    var ifLeft = Eval(infixExpression.Left, enviroment);
                    if (IsError(ifLeft)) return ifLeft;
                    var ifRight = Eval(infixExpression.Left, enviroment);
                    if (IsError(ifRight)) return ifRight;
                    return EvalInfixExpression(infixExpression.Operator, Eval(infixExpression.Left, enviroment), Eval(infixExpression.Right, enviroment));
                case IfExpression ifExpression:
                    return EvalIfExpression(ifExpression, enviroment);
                case FunctionLiteral functionLiteral:
                    return new FunctionObject()
                    {
                        Parameters = functionLiteral.Parameters,
                        Body = functionLiteral.Body,
                        Enviroment = enviroment,
                    };
                case CallExpression callExpression:
                    var fn = Eval(callExpression.Function, enviroment);
                    if (IsError(fn)) return fn;
                    var args = EvalExpressions(callExpression.Arguments, enviroment);
                    if (IsError(args.FirstOrDefault())) return args.First();
                    return ApplyFunction(fn, args);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="enviroment">環境</param>
        /// <returns></returns>
        public IObject EvalRootProgram(List<IStatement> statements, Enviroment enviroment)
        {
            IObject result = null;
            foreach (var statement in statements)
            {
                result = Eval(statement, enviroment);

                switch (result)
                {
                    case ReturnValue returnValue:
                        return returnValue.Value;
                    case Error error:
                        return result;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// ブロック文の評価
        /// </summary>
        /// <param name="blockStatement">評価するブロック文</param>
        /// <param name="enviroment">環境</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalBlockStatement(BlockStatement blockStatement, Enviroment enviroment)
        {
            IObject result = null;
            foreach (var statement in blockStatement.Statements)
            {
                result = Eval(statement, enviroment);

                if (result.Type() == ObjectType.Return || result.Type() == ObjectType.Error) return result;
            }
            return result;
        }

        /// <summary>
        /// すべての文を順番に評価
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="enviroment">環境</param>
        /// <returns></returns>
        public IObject EvalStatements(List<IStatement> statements, Enviroment enviroment)
        {
            IObject result = null;
            foreach (var statement in statements)
            {
                result = Eval(statement, enviroment);

                // 以降の文の評価を中断することで Return を実現する
                if (result is ReturnValue returnValue)
                {
                    return returnValue.Value;
                }
            }
            return result;
        }

        /// <summary>
        /// 識別子の評価
        /// </summary>
        /// <param name="identifier">評価する識別子</param>
        /// <param name="enviroment">環境</param>
        /// <returns>評価オブジェクト</returns>
        public IObject EvalIdentifier(Identifier identifier, Enviroment enviroment)
        {
            var (value, ok) = enviroment.Get(identifier.Value);
            if (ok) return value;

            ok = Builtins.Functions.TryGetValue(identifier.Value, out value);
            if (ok) return value;

            return new Error(string.Format(LocalizationTexts.UndefinedIdentifier.Localize(), identifier.Value));
        }

        /// <summary>
        /// 代入式の評価
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="enviroment"></param>
        /// <returns></returns>
        public IObject EvalAssignExpression(AssignExpression expression, Enviroment enviroment)
        {
            var identifier = expression.Identifier;
            var (value, ok) = enviroment.Get(identifier.Value);

            if (ok)
            {
                var result = Eval(expression.Right, enviroment);
                enviroment.Set(expression.Identifier.Value, result);
                return result;
            }

            return new Error(string.Format(LocalizationTexts.UndefinedIdentifier.Localize(), identifier.Value));
        }

        /// <summary>
        /// 前置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="right">右辺</param>
        /// <returns>評価結果オブジェクト</returns>
        public IObject EvalPrefixExpression(string op, IObject right)
        {
            switch (op)
            {
                case "!":
                    return EvalBangOperator(right);
                case "-":
                    return EvalMinusPrefixOperatorExpression(right);
            }

            return new Error(string.Format(LocalizationTexts.UnknownOperatorPrefix.Localize(), op, right.Type()));
        }

        /// <summary>
        /// ! 前置演算子の評価
        /// </summary>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalBangOperator(IObject right)
        {
            if (right == True) return False;
            if (right == False) return True;
            if (right == Null) return True;
            return False;
        }

        /// <summary>
        /// - 前置演算子の評価
        /// </summary>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalMinusPrefixOperatorExpression(IObject right)
        {
            switch (right)
            {
                case IntegerObject value:
                    return new IntegerObject(-value.Value);
                case DoubleObject value:
                    return new DoubleObject(-value.Value);
            }
            return new Error(string.Format(LocalizationTexts.UnknownOperatorPrefix.Localize(), "-", right.Type()));
        }

        /// <summary>
        /// 中置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalInfixExpression(string op, IObject left, IObject right)
        {
            if (left is IntegerObject leftIntegerObject && right is IntegerObject rightIntegerObject)
            {
                return EvalIntegerInfixExpression(op, leftIntegerObject, rightIntegerObject);
            }
            else if (left is DoubleObject leftDoubleObject && right is DoubleObject rightDoubleObject)
            {
                return EvalDoubleInfixExpression(op, leftDoubleObject, rightDoubleObject);
            }
            else if (left is StringObject leftStringObject && right is StringObject rightStringObject)
            {
                return EvalStringInfixExpression(op, leftStringObject, rightStringObject);
            }
            else if (left is StringObject stringObject && right is IntegerObject integerObject)
            {
                return EvalStringIntegerInfixExpression(op, stringObject, integerObject);
            }
            else if (left is BooleanObject leftBooleanObject && right is BooleanObject rightBooleanObject)
            {
                return EvalBooleanInfixExpression(op, leftBooleanObject, rightBooleanObject);
            }

            switch (op)
            {
                case "==": return ToBooleanObject(left == right);
                case "!=": return ToBooleanObject(left != right);
            }

            if (left.Type() != right.Type()) return new Error(string.Format(LocalizationTexts.TypeMismatch.Localize(), left.Type(), op, right.Type()));

            return new Error(string.Format(LocalizationTexts.UnknownOperatorInfix.Localize(), left.Type(), op, right.Type()));
        }

        /// <summary>
        /// 整数値の中置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalIntegerInfixExpression(string op, IntegerObject left, IntegerObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+": return new IntegerObject(leftValue + rightValue);
                case "-": return new IntegerObject(leftValue - rightValue);
                case "*": return new IntegerObject(leftValue * rightValue);
                case "/": return new IntegerObject(leftValue / rightValue);
                case "<": return ToBooleanObject(leftValue < rightValue);
                case ">": return ToBooleanObject(leftValue > rightValue);
                case "<=": return ToBooleanObject(leftValue <= rightValue);
                case ">=": return ToBooleanObject(leftValue >= rightValue);
                case "==": return ToBooleanObject(leftValue == rightValue);
                case "!=": return ToBooleanObject(leftValue != rightValue);
            }
            return Null;
        }

        /// <summary>
        /// 倍精度浮動小数点数値の中置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalDoubleInfixExpression(string op, DoubleObject left, DoubleObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+": return new DoubleObject(leftValue + rightValue);
                case "-": return new DoubleObject(leftValue - rightValue);
                case "*": return new DoubleObject(leftValue * rightValue);
                case "/": return new DoubleObject(leftValue / rightValue);
                case "<": return ToBooleanObject(leftValue < rightValue);
                case ">": return ToBooleanObject(leftValue > rightValue);
                case "<=": return ToBooleanObject(leftValue <= rightValue);
                case ">=": return ToBooleanObject(leftValue >= rightValue);
                case "==": return ToBooleanObject(leftValue == rightValue);
                case "!=": return ToBooleanObject(leftValue != rightValue);
            }
            return Null;
        }

        /// <summary>
        /// 文字列の中置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalStringInfixExpression(string op, StringObject left, StringObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+": return new StringObject(leftValue + rightValue);
                case "==": return ToBooleanObject(leftValue.CompareTo(rightValue) == 0);
                case "!=": return ToBooleanObject(leftValue.CompareTo(rightValue) != 0);
            }
            return Null;
        }

        /// <summary>
        /// 文字列と整数値の中置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalStringIntegerInfixExpression(string op, StringObject left, IntegerObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+": return new StringObject(leftValue + rightValue);
                case "*":
                    {
                        var result = new StringBuilder();
                        for (int i = 0; i < rightValue; ++i)
                        {
                            result.Append(leftValue);
                        }
                        return new StringObject(result.ToString());
                    }
            }
            return Null;
        }

        /// <summary>
        /// 真偽値と真偽値の中置演算子式の評価
        /// </summary>
        /// <param name="op">演算子</param>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalBooleanInfixExpression(string op, BooleanObject left, BooleanObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "&&": return ToBooleanObject(left.Value && right.Value);
                case "||": return ToBooleanObject(left.Value || right.Value);
            }
            return new Error(string.Format(LocalizationTexts.UnknownOperatorInfix.Localize(), left.Type(), op, right.Type()));
        }

        /// <summary>
        /// If 式の評価
        /// </summary>
        /// <param name="ifExpression">評価する If 式</param>
        /// <param name="enviroment">環境</param>
        /// <returns>評価後のオブジェクト</returns>
        public IObject EvalIfExpression(IfExpression ifExpression, Enviroment enviroment)
        {
            var condition = Eval(ifExpression.Condition, enviroment);
            if (IsError(condition)) return condition;

            if (IsTruthly(condition))
            {
                // List<Statement> ではなく BlockStatement を渡す
                return EvalBlockStatement(ifExpression.Consequence, enviroment);
            }
            else if (ifExpression.Alternative != null)
            {
                // List<Statement> ではなく BlockStatement を渡す
                return EvalBlockStatement(ifExpression.Alternative, enviroment);
            }
            return Null;
        }

        /// <summary>
        /// 式の評価
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="enviroment"></param>
        /// <returns></returns>
        public List<IObject> EvalExpressions(List<IExpression> arguments, Enviroment enviroment)
        {
            var result = new List<IObject>();

            foreach (var arg in arguments)
            {
                var evaluated = Eval(arg, enviroment);
                if (IsError(evaluated)) return new List<IObject>() { evaluated };
                result.Add(evaluated);
            }

            return result;
        }

        /// <summary>
        /// 関数内部で使う新しい環境を作成し、関数ブロックを評価
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IObject ApplyFunction(IObject obj, List<IObject> args)
        {
            switch(obj)
            {
                case FunctionObject fn:
                    var extendedEnviroment = ExtendEnviroment(fn, args);
                    var evaluated = EvalBlockStatement(fn.Body, extendedEnviroment);
                    return UnwrapReturnValue(evaluated);

                case BuiltinObject builtinObject:
                    return builtinObject.Function(args);

                default:
                    return new Error(string.Format(LocalizationTexts.NotFunction.Localize(), obj.GetType()));
            }
        }
        
        /// <summary>
        /// 関数オブジェクト用の新しい環境を作成
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Enviroment ExtendEnviroment(FunctionObject fn, List<IObject> args)
        {
            var enviroment = Enviroment.CreateNewEnclosedEnviroment(fn.Enviroment);

            for (int i = 0; i < fn.Parameters.Count; i++)
            {
                // 呼び出し引数が不足していた場合は NULL を設定しておく
                enviroment.Set(fn.Parameters[i].Value, args.Count > i ? args[i] : Null);
            }

            return enviroment;
        }

        /// <summary>
        /// 評価結果が Return の場合、アンラップして値を取り出す
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IObject UnwrapReturnValue(IObject obj)
        {
            if (obj is ReturnValue returnValue)
            {
                return returnValue.Value;
            }
            return obj;
        }

        /// <summary>
        /// 真偽を判定
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsTruthly(IObject obj)
        {
            if (obj == True) return true;
            if (obj == False) return false;
            if (obj == Null) return false;
            return true;
        }

        /// <summary>
        /// エラーオブジェクトかを判定
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsError(IObject obj)
        {
            if (obj != null) return obj.Type() == ObjectType.Error;
            return false;
        }

        /// <summary>
        /// 真偽値オブジェクト：True
        /// 評価時に、新しいオブジェクトを作成せずに True の場合はこの値を返す
        /// </summary>
        private static readonly BooleanObject True = new BooleanObject(true);

        /// <summary>
        /// 真偽値オブジェクト：False
        /// 評価時に、新しいオブジェクトを作成せずに False の場合はこの値を返す
        /// </summary>
        private static readonly BooleanObject False = new BooleanObject(false);

        /// <summary>
        /// 真偽値の値から判定して、作成済みの真偽値オブジェクトを返す
        /// </summary>
        /// <param name="value">真偽値</param>
        /// <returns>作成済みの真偽値オブジェクト</returns>
        private BooleanObject ToBooleanObject(bool value) => value ? True : False;

        /// <summary>
        /// NULL オブジェクト
        /// 評価時に、新しいオブジェクトを作成せずに NULL の場合はこの値を返す
        /// </summary>
        private static readonly NullObject Null = new NullObject();
    }
}
