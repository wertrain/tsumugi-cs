using System.Collections.Generic;
using Tsumugi.Localize;
using Tsumugi.Script.AbstractSyntaxTree;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.Parsing
{
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
        /// コンストラクタ
        /// </summary>
        /// <param name="lexer"></param>
        public Parser(Lexer lexer)
        {
            Lexer = lexer;

            CurrentToken = Lexer.NextToken();
            NextToken = Lexer.NextToken();

            Logger = new Logger();
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
                default:
                    return null;
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
    }
}
