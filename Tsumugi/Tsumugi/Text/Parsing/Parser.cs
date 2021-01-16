using System;
using System.Collections.Generic;
using System.Text;
using Tsumugi.Localize;
using Tsumugi.Text.Lexing;

namespace Tsumugi.Text.Parsing
{
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
        public Script.Logger Logger { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="lexer"></param>
        public Parser(Lexer lexer)
        {
            Lexer = lexer;

            CurrentToken = Lexer.NextToken();
            NextToken = Lexer.NextToken();

            Logger = new Script.Logger();
        }

        /// <summary>
        /// パース処理
        /// </summary>
        /// <returns></returns>
        public Commanding.CommandQueue ParseProgram()
        {
            var commandQueue = new Commanding.CommandQueue();

            while (CurrentToken.Type != TokenType.EOF)
            {
                var command = ParseCommand();
                if (command != null)
                {
                    commandQueue.Enqueue(command);
                }
                ReadToken();
            }
            return commandQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Commanding.CommandBase ParseCommand()
        {
            switch (CurrentToken.Type)
            {
                case TokenType.Text:
                    return new Commanding.Commands.PrintTextCommand(CurrentToken.Literal);

                case TokenType.Label:
                    string headline = string.Empty;
                    if (NextToken.Type == TokenType.LabelHeadline)
                        headline = NextToken.Literal;
                    return new Commanding.Commands.LabelCommand(CurrentToken.Literal, headline);

                case TokenType.Tag:
                    return CreateTagCommand(ParseTag(CurrentToken.Literal));

            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Commanding.CommandBase CreateTagCommand(Tag tag)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tag ParseTag(string tagName)
        {
            var attributes = new List<Tag.Attribute>();

            if (NextToken.Type == TokenType.TagEnd)
            {
                return new Tag() { Name = tagName, Attributes = attributes };
            }

            ReadToken();

            while (NextToken.Type != TokenType.EOF)
            {
                switch (NextToken.Type)
                {
                    case TokenType.TagEnd:
                        return new Tag()
                        {
                            Name = tagName,
                            Attributes = attributes
                        };
                }

                switch (CurrentToken.Type)
                {
                    case TokenType.TagAttributeName:
                        var value = string.Empty;
                        if (NextToken.Type == TokenType.TagAttributeValue)
                        {
                            value = NextToken.Literal;
                        }

                        attributes.Add(new Tag.Attribute()
                        {
                            Name = CurrentToken.Literal,
                            Value = value
                        });
                        break;
                }

                ReadToken();
            }

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
        /// エラー発生
        /// </summary>
        /// <param name="token">エラーが発生したトークン</param>
        /// <param name="message">エラーメッセージ</param>
        private void Error(Token token, string message)
        {
            Logger.Logging(Script.Logger.Categories.Error, message);
        }
    }
}
