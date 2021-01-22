using System.Collections.Generic;
using System.Linq;
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
            CommandsTokenDictionary = new Dictionary<Commanding.CommandBase, Token>();
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
                    CommandsTokenDictionary.Add(command, CurrentToken);
                    commandQueue.Enqueue(command);
                }
                ReadToken();
            }

            PostProcessCommands(commandQueue);

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
            switch(tag.Name)
            {
                case TagName.NewLine:
                    return new Commanding.Commands.NewLineCommand();

                case TagName.NewPage:
                    return new Commanding.Commands.NewPageCommand();

                case TagName.WaitKey:
                    return new Commanding.Commands.WaitKeyCommand();

                case TagName.WaitTime:
                    {
                        var attr = tag.Attributes.FirstOrDefault(s => s.Name == "time");
                        if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
                            Error(CurrentToken, string.Format(LocalizationTexts.CannotFindAttributeRequiredTag.Localize(), "time", TagName.WaitTime));
                        return new Commanding.Commands.WaitTimeCommand(attr?.Value);
                    }

                case TagName.DefineVariable:
                    {
                        var variables = new List<Commanding.Commands.DefineVariablesCommand.Variable>();
                        foreach (var attr in tag.Attributes)
                            variables.Add(new Commanding.Commands.DefineVariablesCommand.Variable() { Name = attr.Name, Value = attr.Value });
                        return new Commanding.Commands.DefineVariablesCommand(variables);
                    }

                case TagName.Jump:
                    {
                        var attr = tag.Attributes.FirstOrDefault(s => s.Name == "target");
                        if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
                            Error(CurrentToken, string.Format(LocalizationTexts.CannotFindAttributeRequiredTag.Localize(), "target", TagName.Jump));
                        return new Commanding.Commands.JumpCommand(attr?.Value);
                    }

                case TagName.If:
                    {
                        var attr = tag.Attributes.FirstOrDefault(s => s.Name == "exp");
                        if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
                            Error(CurrentToken, string.Format(LocalizationTexts.CannotFindAttributeRequiredTag.Localize(), "exp", TagName.If));
                        return new Commanding.Commands.IfCommand(attr?.Value);
                    }

                case TagName.Else:
                        return new Commanding.Commands.ElseCommand();

                case TagName.Elif:
                    {
                        var attr = tag.Attributes.FirstOrDefault(s => s.Name == "exp");
                        if (attr == null || string.IsNullOrWhiteSpace(attr.Value))
                            Error(CurrentToken, string.Format(LocalizationTexts.CannotFindAttributeRequiredTag.Localize(), "exp", TagName.Elif));
                        return new Commanding.Commands.ElifCommand(attr?.Value);
                    }

                case TagName.Endif:
                    return new Commanding.Commands.EndIfCommand();
            }

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

            while (CurrentToken.Type != TokenType.EOF)
            {
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

                switch (NextToken.Type)
                {
                    case TokenType.TagEnd:
                        return new Tag()
                        {
                            Name = tagName,
                            Attributes = attributes
                        };
                }

                ReadToken();
            }

            return null;

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
        /// コマンドに対する後処理
        /// </summary>
        /// <param name="queue">後処理するコマンド</param>
        /// <returns></returns>
        private bool PostProcessCommands(Commanding.CommandQueue queue)
        {
            Commanding.CommandBase command = null;

            while ((command = queue.Dequeue()) != null)
            {
                switch (command)
                {
                    case Commanding.Commands.IfCommand cmd:
                        if (!Commanding.Commands.IfCommandUtility.InspectSequence(cmd, queue, out var error))
                        {
                            Error(CommandsTokenDictionary[error], LocalizationTexts.ErrorInStructureIfTag.Localize());
                            return false;
                        }
                        if (!Commanding.Commands.IfCommandUtility.InspectRelated(cmd))
                        {
                            Error(CommandsTokenDictionary[cmd], LocalizationTexts.ErrorInStructureIfTag.Localize());
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// エラー発生
        /// </summary>
        /// <param name="token">エラーが発生したトークン</param>
        /// <param name="message">エラーメッセージ</param>
        private void Error(Token token, string message)
        {
            Logger.Logging(Script.Logger.Categories.Error, string.Format("{1} [{0}]",
                string.Format(LocalizationTexts.LexingPosition.Localize(),
                token.Position.Lines, token.Position.Columns, token.Position.Position), message));
        }

        /// <summary>
        /// コマンドとトークンの辞書（エラー処理用）
        /// </summary>
        private Dictionary<Commanding.CommandBase, Token> CommandsTokenDictionary { get; set; }
    }
}
