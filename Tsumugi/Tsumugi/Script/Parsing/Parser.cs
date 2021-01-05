﻿using System;
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
            while (this.CurrentToken.Type != TokenType.Semicolon)
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
            if (prefix == null) return null;

            var leftExpression = prefix();
            return leftExpression;
        }

        /// <summary>
        /// 式文のパース
        /// </summary>
        /// <returns></returns>
        public ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement();
            statement.Token = this.CurrentToken;

            statement.Expression = this.ParseExpression(Precedence.Lowest);

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
    }
}