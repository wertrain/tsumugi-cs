using System.Collections.Generic;
using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Statements
{
    /// <summary>
    /// ブロック文
    /// </summary>
    public class BlockStatement : IStatement
    {
        /// <summary>
        /// ブロック文の最初のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// ブロック文
        /// </summary>
        public List<IStatement> Statements { get; set; }

        /// <summary>
        /// トークンのリテラル
        /// </summary>
        /// <returns>トークンのリテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("{");
            foreach (var statement in Statements)
            {
                builder.Append(statement.ToCode());

                if (Statements.LastIndexOf(statement) > 0)
                    builder.Append(" ");
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
