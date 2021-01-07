using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 中置演算子式
    /// </summary>
    public class InfixExpression : IExpression
    {
        /// <summary>
        /// 識別子のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 左辺の式
        /// </summary>
        public IExpression Left { get; set; }

        /// <summary>
        /// 演算子
        /// </summary>
        public string Operator { get; set; }

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
            builder.Append(this.Left.ToCode());
            builder.Append(" ");
            builder.Append(this.Operator);
            builder.Append(" ");
            builder.Append(this.Right.ToCode());
            builder.Append(")");
            return builder.ToString();
        }
    }
}
