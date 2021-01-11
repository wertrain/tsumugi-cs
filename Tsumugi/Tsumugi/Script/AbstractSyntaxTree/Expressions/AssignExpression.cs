using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 代入式
    /// </summary>
    public class AssignExpression : IExpression
    {
        /// <summary>
        /// 代入式のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 左辺の識別子
        /// </summary>
        public Identifier Identifier { get; set; }

        /// <summary>
        /// 右辺の式
        /// </summary>
        public IExpression Right { get; set; }

        /// <summary>
        /// リテラル
        /// </summary>
        /// <returns>リテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(Identifier.ToCode());
            builder.Append(" = ");
            builder.Append(Right.ToCode());
            builder.Append(")");
            return builder.ToString();
        }
    }
}
