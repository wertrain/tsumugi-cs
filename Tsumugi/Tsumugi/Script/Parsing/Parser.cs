using System;
using System.Collections.Generic;
using Tsumugi.Localize;
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
        Equals,      /// ==
        Lessgreater, /// >, <
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
            { TokenType.Plus, Precedence.Sum },
            { TokenType.Minus, Precedence.Sum },
            { TokenType.Slash, Precedence.Product },
            { TokenType.Asterisk, Precedence.Product },
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
                var statement = this.ParseStatement();
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

            // 右辺（TODO: 後で実装）
            while (CurrentToken.Type != TokenType.Semicolon)
            {
                // セミコロンが見つかるまで
                ReadToken();
            }

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

            // TODO: 後で実装。
            while (CurrentToken.Type != TokenType.Semicolon)
            {
                // セミコロンが見つかるまで
                ReadToken();
            }

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
                Logger.Logging(Logger.Categories.Error, string.Format(LocalizationTexts.NoAssociatedWith.Localize(), CurrentToken.Type.ToString(), "Prefix Parse Function"));
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

            Logger.Logging(Logger.Categories.Error, string.Format(LocalizationTexts.ThisTokenMustBe.Localize(), type.ToString(), NextToken.Type.ToString()));

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
            PrefixParseFunctions.Add(TokenType.Integer32, ParseIntegerLiteral);
            PrefixParseFunctions.Add(TokenType.Bang, ParsePrefixExpression);
            PrefixParseFunctions.Add(TokenType.Minus, ParsePrefixExpression);
            PrefixParseFunctions.Add(TokenType.True, ParseBooleanLiteral);
            PrefixParseFunctions.Add(TokenType.False, ParseBooleanLiteral);
            PrefixParseFunctions.Add(TokenType.LeftParenthesis, ParseGroupedExpression);
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
            if (int.TryParse(CurrentToken.Literal, out int result))
            {
                return new IntegerLiteral()
                {
                    Token = this.CurrentToken,
                    Value = result,
                };
            }

            Logger.Logging(Logger.Categories.Error, string.Format(LocalizationTexts.CannotConvertInteger.Localize(), CurrentToken.Literal));

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
        /// 丸括弧でグループされた式のパース
        /// </summary>
        /// <returns></returns>
        public IExpression ParseGroupedExpression()
        {
            // "(" を読み飛ばす
            ReadToken();

            // 括弧内の式を解析する
            var expression = this.ParseExpression(Precedence.Lowest);

            // 閉じ括弧 ")" がないとエラーになる
            if (!ExpectPeek(TokenType.RightParenthesis)) return null;

            return expression;
        }
    }
}
