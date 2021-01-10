using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Statements
{
    /// <summary>
    /// If 文
    /// 定義：if (<condition>) <consequence> else <alternative>
    /// </summary>
    public class IfExpression : IExpression
    {
        /// <summary>
        /// If 文の最初のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 条件式
        /// </summary>
        public IExpression Condition { get; set; }

        /// <summary>
        /// if ブロック文
        /// </summary>
        public BlockStatement Consequence { get; set; }

        /// <summary>
        /// else ブロック文
        /// </summary>
        public BlockStatement Alternative { get; set; }

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
            builder.Append("if");
            builder.Append(Condition.ToCode());
            builder.Append(" ");
            builder.Append(Consequence.ToCode());

            if (Alternative != null)
            {
                builder.Append("else ");
                builder.Append(Alternative.ToCode());
            }

            return builder.ToString();
        }
    }
}
