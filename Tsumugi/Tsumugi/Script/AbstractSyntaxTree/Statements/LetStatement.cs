using System.Text;
using Tsumugi.Script.AbstractSyntaxTree.Expressions;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Statements
{
    /// <summary>
    /// Let 文
    /// 定義：let <identifier> = <expression>;
    /// </summary>
    public class LetStatement : IStatement
    {
        /// <summary>
        /// Let 文のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 左辺の識別子名
        /// </summary>
        public Identifier Name { get; set; }

        /// <summary>
        /// 右辺の式
        /// </summary>
        public IExpression Value { get; set; }

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
            builder.Append(Token?.Literal ?? "");
            builder.Append(" ");
            builder.Append(Name?.ToCode() ?? "");
            builder.Append(" = ");
            builder.Append(Value?.ToCode() ?? "");
            builder.Append(";");
            return builder.ToString();
        }
    }
}
