using System;
using System.Collections.Generic;
using Tsumugi.Localize;
using Tsumugi.Log;
using Tsumugi.Script.AbstractSyntaxTree;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.Parsing
{
    /// <summary>
    /// 前置構文解析関数
    /// </summary>
    using PrefixParseFunction = Func<IExpression>;

    /// <summary>
    /// 中置構文解析関数
    /// </summary>
    using InfixParseFunction = Func<IExpression, IExpression>;

    /// <summary>
    /// 優先順位
    /// </summary>
    public enum Precedence
    {
        Lowest = 1,
        Assign,      /// =
        AndOr,       /// &&, ||
        Equals,      /// ==
        Lessgreater, /// >, <, >=, <=
        Sum,         /// +
        Product,     /// *
        Prefix,      /// -x, !x
        Call,        /// myFunction(x)
    }

    /// <summary>
    /// 構文解析
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// 現在のトークン
        /// </summary>
        public Token CurrentToken { get; set; }

        /// <summary>
        /// 次のトークン
        /// </summary>
        public Token NextToken { get; set; }
        
        /// <summary>
        /// 字句解析
        /// </summary>
        public Lexer Lexer { get; }

        /// <summary>
        /// ログ
        /// </summary>
        public Logger Logger { get; set; }

        /// <summary>
        /// 前置構文解析関数辞書
        /// </summary>
        public Dictionary<TokenType, PrefixParseFunction> PrefixParseFunctions { get; set; }

        /// <summary>
        /// 中置構文解析関数辞書
        /// </summary>
        public Dictionary<TokenType, InfixParseFunction> InfixParseFunctions { get; set; }

        /// <summary>
        /// 優先度の辞書
        /// </summary>
        public Dictionary<TokenType, Precedence> Precedences { get; set; } = new Dictionary<TokenType, Precedence>()
        {
            { TokenType.Equal, Precedence.Equals },
            { TokenType.NotEqual, Precedence.Equals },
            { TokenType.LessThan, Precedence.Lessgreater },
            { TokenType.GreaterThan, Precedence.Lessgreater },
            { TokenType.LessThanOrEqual, Precedence.Lessgreater },
            { TokenType.GreaterThanOrEqual, Precedence.Lessgreater },
            { TokenType.Plus, Precedence.Sum },
            { TokenType.Minus, Precedence.Sum },
            { TokenType.Slash, Precedence.Product },
            { TokenType.Asterisk, Precedence.Product },
            { TokenType.And, Precedence.AndOr },
            { TokenType.Or, Precedence.AndOr },
            { TokenType.LeftParenthesis, Precedence.Call },
            { TokenType.Assign, Precedence.Assign },
        };

        /// <summary>
        /// 現在のトークンの優先度
        /// </summary>
        public Precedence CurrentPrecedence
        {
            get
            {
                if (Precedences.TryGetValue(CurrentToken.Type, out var p))
                {
                    return p;
                }
                return Precedence.Lowest;
            }
        }

        /// <summary>
        /// 次のトークンの優先度
        /// </summary>
        public Precedence NextPrecedence
        {
            get
            {
                if (Precedences.TryGetValue(NextToken.Type, out var p))
                {
                    return p;
                }
                return Precedence.Lowest;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="lexer"></param>
        public Parser(Lexer lexer)
        {
            Lexer = lexer;

            CurrentToken = Lexer.NextToken();
            NextToken = Lexer.NextToken();

            Logger = new Logger();

            RegisterPrefixParseFunctions();
            RegisterInfixParseFunctions();
        }

        /// <summary>
        /// パース処理
        /// </summary>
        /// <returns></returns>
        public Root ParseProgram()
        {
            var root = new Root();
            root.Statements = new List<IStatement>();

            while (CurrentToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    root.Statements.Add(statement);
                }
                ReadToken();
            }
            return root;
        }

        /// <summary>
        /// 文のパース
        /// </summary>
        /// <returns></returns>
        public IStatement ParseStatement()
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Let:
                    return ParseLetStatement();

                case TokenType.Return:
                    return ParseReturnStatement();

                default:
                    return ParseExpressionStatement();
            }
        }

        /// <summary>
        /// let 文のパース
        /// 定義：let <identifier> = <expression>;
        /// </summary>
        /// <returns></returns>
        public LetStatement ParseLetStatement()
        {
            var statement = new LetStatement();
            statement.Token = CurrentToken;

            // let <identifier> = <expression>; という文かチェック

            // 左辺は Identifier である
            if (!ExpectPeek(TokenType.Identifier)) return null;

            statement.Name = new Identifier(CurrentToken, CurrentToken.Literal);

            // 次は等号がある
            if (!ExpectPeek(TokenType.Assign)) return null;

            // = を読み飛ばす
            ReadToken();

            // 式を解析
            statement.Value = ParseExpression(Precedence.Lowest);
            
            // セミコロンは必須ではない
            if (NextToken.Type == TokenType.Semicolon) ReadToken();

            return statement;
        }

        /// <summary>
        /// return 文のパース
        /// 定義：return <expression>;
        /// </summary>
        /// <returns></returns>
        public ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement();
            statement.Token = CurrentToken;
            ReadToken();

            // 式を解析
            statement.ReturnValue = ParseExpression(Precedence.Lowest);

            // セミコロンは必須ではない
            if (NextToken.Type == TokenType.Semicolon) ReadToken();

            return statement;
        }

        /// <summary>
        /// 式のパース
        /// </summary>
        /// <param name="precedence"></param>
        /// <returns></returns>
        public IExpression ParseExpression(Precedence precedence)
        {
            PrefixParseFunctions.TryGetValue(CurrentToken.Type, out var prefix);
            if (prefix == null)
            {
                Error(CurrentToken, string.Format(LocalizationTexts.NoAssociatedWith.Localize(), CurrentToken.Type.ToString(), "Prefix Parse Function"));
                return null;
            }

            var leftExpression = prefix();

            while (NextToken.Type != TokenType.Semicolon && precedence < NextPrecedence)
            {
                InfixParseFunctions.TryGetValue(NextToken.Type, out var infix);
                if (infix == null)
                {
                    return leftExpression;
                }

                ReadToken();
                leftExpression = infix(leftExpression);
            }

            return leftExpression;
        }

        /// <summary>
        /// 式文のパース
        /// </summary>
        /// <returns></returns>
        public ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement();
            statement.Token = CurrentToken;

            statement.Expression = ParseExpression(Precedence.Lowest);

            // セミコロンを読み飛ばす(省略可能)
            if (NextToken.Type == TokenType.Semicolon) ReadToken();

            return statement;
        }

        /// <summary>
        /// 次のトークンが期待するものであれば読み飛ばす
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool ExpectPeek(TokenType type)
        {
            if (NextToken.Type == type)
            {
                ReadToken();
                return true;
            }

            Error(NextToken, string.Format(LocalizationTexts.ThisTokenMustBe.Localize(), type.ToString(), NextToken.Type.ToString()));

            return false;
        }

        /// <summary>
        /// トークンを一つ読み出す
        /// </summary>
        private void ReadToken()
        {
            CurrentToken = NextToken;
            NextToken = Lexer.NextToken();
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <returns></returns>
        public Root ParseRoot()
        {
            return null;
        }

        /// <summary>
        /// 前置構文解析関数を登録
        /// </summary>
        private void RegisterPrefixParseFunctions()
        {
            PrefixParseFunctions = new Dictionary<TokenType, PrefixParseFunction>();
            PrefixParseFunctions.Add(TokenType.Identifier, ParseIdentifier);
            PrefixParseFunctions.Add(TokenType.Integer, ParseIntegerLiteral);
            PrefixParseFunctions.Add(TokenType.Double, ParseDoubleLiteral);
            PrefixParseFunctions.Add(TokenType.Bang, ParsePrefixExpression);
            PrefixParseFunctions.Add(TokenType.Minus, ParsePrefixExpression);
            PrefixParseFunctions.Add(TokenType.True, ParseBooleanLiteral);
            PrefixParseFunctions.Add(TokenType.False, ParseBooleanLiteral);
            PrefixParseFunctions.Add(TokenType.LeftParenthesis, ParseGroupedExpression);
            PrefixParseFunctions.Add(TokenType.If, ParseIfExpression);
            PrefixParseFunctions.Add(TokenType.Function, ParseFunctionLiteral);
            PrefixParseFunctions.Add(TokenType.String, ParseStringLiteral);
        }

        /// <summary>
        /// 中置構文解析関数を登録
        /// </summary>
        private void RegisterInfixParseFunctions()
        {
            InfixParseFunctions = new Dictionary<TokenType, InfixParseFunction>();
            InfixParseFunctions.Add(TokenType.Plus, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.Minus, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.Slash, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.Asterisk, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.Equal, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.NotEqual, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.LessThan, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.GreaterThan, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.LessThanOrEqual, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.GreaterThanOrEqual, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.And, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.Or, ParseInfixExpression);
            InfixParseFunctions.Add(TokenType.LeftParenthesis, ParseCallExpression);
            InfixParseFunctions.Add(TokenType.Assign, ParseAssignExpression);
        }

        /// <summary>
        /// 識別子式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseIdentifier()
        {
            return new Identifier(CurrentToken, CurrentToken.Literal);
        }

        /// <summary>
        /// 整数式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseIntegerLiteral()
        {
            if (int.TryParse(CurrentToken.Literal, out var result))
            {
                return new IntegerLiteral()
                {
                    Token = CurrentToken,
                    Value = result,
                };
            }

            Error(CurrentToken, string.Format(LocalizationTexts.CannotConvertNumber.Localize(), CurrentToken.Literal, "Integer"));

            return null;
        }

        /// <summary>
        /// 倍精度浮動小数点数式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseDoubleLiteral()
        {
            if (double.TryParse(CurrentToken.Literal, out var result))
            {
                return new DoubleLiteral()
                {
                    Token = CurrentToken,
                    Value = result,
                };
            }

            Error(CurrentToken, string.Format(LocalizationTexts.CannotConvertNumber.Localize(), CurrentToken.Literal, "Double"));

            return null;
        }

        /// <summary>
        /// 真偽式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseBooleanLiteral()
        {
            return new BooleanLiteral()
            {
                Token = CurrentToken,
                Value = CurrentToken.Type == TokenType.True,
            };
        }

        /// <summary>
        /// 前置演算子式のパース
        /// 定義：<prefix operator><expression>;
        /// </summary>
        /// <returns></returns>
        public IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression()
            {
                Token = CurrentToken,
                Operator = CurrentToken.Literal
            };

            ReadToken();

            expression.Right = ParseExpression(Precedence.Prefix);

            return expression;
        }

        /// <summary>
        /// 中置演算子式のパース
        /// 定義：<expression> <infix operator> <expression>
        /// </summary>
        /// <returns></returns>
        public IExpression ParseInfixExpression(IExpression left)
        {
            var expression = new InfixExpression()
            {
                Token = CurrentToken,
                Operator = CurrentToken.Literal,
                Left = left,
            };

            var precedence = CurrentPrecedence;
            ReadToken();
            expression.Right = ParseExpression(precedence);

            return expression;
        }

        /// <summary>
        /// If 式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseIfExpression()
        {
            var expression = new IfExpression()
            {
                Token = CurrentToken // If
            };

            // if の後は括弧 ( でなければならない
            if (!ExpectPeek(TokenType.LeftParenthesis)) return null;

            // 括弧 ( を読み飛ばす
            ReadToken();

            // if の条件式を解析する
            expression.Condition = ParseExpression(Precedence.Lowest);

            // 閉じ括弧 ) 中括弧が続く 
            if (!ExpectPeek(TokenType.RightParenthesis)) return null;
            if (!ExpectPeek(TokenType.LeftBraces)) return null;

            // この時点で { が現在のトークン
            // ブロック文の解析関数を呼ぶ
            expression.Consequence = ParseBlockStatement();

            // else 句があれば解析する
            if (NextToken.Type == TokenType.Else)
            {
                // else に読み進める
                ReadToken();
                // else の後に { が続かなければならない
                if (!ExpectPeek(TokenType.LeftBraces)) return null;

                // この時点で { が現在のトークン
                // ブロック文の解析関数を呼ぶ
                expression.Alternative = ParseBlockStatement();
            }

            return expression;
        }

        /// <summary>
        /// ブロックのパース
        /// </summary>
        /// <returns></returns>
        public BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement()
            {
                Token = CurrentToken, // "{"
                Statements = new List<IStatement>(),
            };

            // "{" を読み飛ばす
            ReadToken();

            while (CurrentToken.Type != TokenType.RightBraces && CurrentToken.Type != TokenType.EOF)
            {
                // ブロックの中身を解析する
                var statement = ParseStatement();
                if (statement != null)
                {
                    block.Statements.Add(statement);
                }
                ReadToken();
            }

            return block;
        }

        /// <summary>
        /// 丸括弧でグループされた式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseGroupedExpression()
        {
            // "(" を読み飛ばす
            ReadToken();

            // 括弧内の式を解析する
            var expression = ParseExpression(Precedence.Lowest);

            // 閉じ括弧 ")" がないとエラーになる
            if (!ExpectPeek(TokenType.RightParenthesis)) return null;

            return expression;
        }

        /// <summary>
        /// 関数式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseFunctionLiteral()
        {
            var function = new FunctionLiteral()
            {
                Token = CurrentToken
            };

            if (!ExpectPeek(TokenType.LeftParenthesis)) return null;

            function.Parameters = ParseParameters();

            if (!ExpectPeek(TokenType.LeftBraces)) return null;

            function.Body = ParseBlockStatement();

            return function;
        }

        /// <summary>
        /// 関数の引数のパース
        /// </summary>
        /// <returns>引数のリスト</returns>
        public List<Identifier> ParseParameters()
        {
            var parameters = new List<Identifier>();

            // () となってパラメータがないときは空のリストを返す
            if (NextToken.Type == TokenType.RightParenthesis)
            {
                ReadToken();
                return parameters;
            }

            // ( を読み飛ばす
            ReadToken();

            // 最初のパラメータ
            parameters.Add(new Identifier(CurrentToken, CurrentToken.Literal));

            // 2つ目以降のパラメータをカンマが続く限り処理する
            while (NextToken.Type == TokenType.Comma)
            {
                // すでに処理した識別子とその後ろのカンマを飛ばす
                ReadToken();
                ReadToken();

                // 識別子を処理
                parameters.Add(new Identifier(CurrentToken, CurrentToken.Literal));
            }

            if (!ExpectPeek(TokenType.RightParenthesis)) return null;

            return parameters;
        }

        /// <summary>
        /// 関数呼び出し式のパース
        /// </summary>
        /// <param name="function">関数式</param>
        /// <returns>関数呼び出し式</returns>
        public IExpression ParseCallExpression(IExpression function)
        {
            var expression = new CallExpression()
            {
                Token = CurrentToken,
                Function = function,
                Arguments = ParseCallArguments(),
            };

            return expression;
        }

        /// <summary>
        /// 代入式
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public IExpression ParseAssignExpression(IExpression left)
        {
            var identifier = left as Identifier;

            if (identifier == null)
            {
                Error(CurrentToken, LocalizationTexts.AssigningValuesNotAllowed.Localize());
                return null;
            }

            var expression = new AssignExpression()
            {
                Token = CurrentToken,
                Identifier = identifier,
            };

            var precedence = CurrentPrecedence;
            ReadToken();
            expression.Right = ParseExpression(precedence);

            return expression;
        }

        /// <summary>
        /// 関数呼び出し式の引数のパース
        /// </summary>
        /// <returns></returns>
        public List<IExpression> ParseCallArguments()
        {
            var args = new List<IExpression>();

            // ( を読み飛ばす
            ReadToken();

            // 引数なしの場合
            if (CurrentToken.Type == TokenType.RightParenthesis) return args;

            // 引数ありの場合は1つ目の引数を解析
            args.Add(ParseExpression(Precedence.Lowest));

            // 2つ目以降の引数があればそれを解析
            while (NextToken.Type == TokenType.Comma)
            {
                // カンマ直前のトークンとカンマトークンを読み飛ばす
                ReadToken();
                ReadToken();
                args.Add(ParseExpression(Precedence.Lowest));
            }

            // 閉じかっこがなければエラー
            if (!ExpectPeek(TokenType.RightParenthesis)) return null;

            return args;
        }

        /// <summary>
        /// 文字列式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseStringLiteral()
        {
            return new StringLiteral()
            {
                Token = CurrentToken,
                Value = CurrentToken.Literal
            };
        }

        /// <summary>
        /// エラー発生
        /// </summary>
        /// <param name="token">エラーが発生したトークン</param>
        /// <param name="message">エラーメッセージ</param>
        private void Error(Token token, string message)
        {
            Logger.Logging(Logger.Categories.Error, message);
        }
    }
}
