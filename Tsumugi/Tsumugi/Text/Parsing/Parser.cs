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
        /// コントロール文字の定義
        /// </summary>
        class ControlCharacter
        {
            /// <summary>
            /// ラベルと見出しの分けるセパレータ
            /// </summary>
            public const char HeadlineSeparator = '|';

            /// <summary>
            /// タグと属性のセパレータ
            /// </summary>
            public const char TagAttributeSeparator = ' ';

            /// <summary>
            /// 割り当て記号
            /// </summary>
            public const char Assignment = '=';
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

            }

            return null;
        }

        /// <summary>
        /// ラベルのパース
        /// </summary>
        /// <returns></returns>
        private Commanding.Commands.LabelCommand ParseLabel()
        {
            var labelName = CurrentToken.Literal;
            var headline = string.Empty;

            var index = labelName.IndexOf(ControlCharacter.HeadlineSeparator);
            if (index >= 0)
            {
                headline = labelName.Substring(index);
                labelName = labelName.Substring(0, index);
            }

            return new Commanding.Commands.LabelCommand(labelName, headline);
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
