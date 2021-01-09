using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 関数呼び出し式
    /// </summary>
    public class CallExpression : IExpression
    {
        /// <summary>
        /// 関数呼び出し式のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 関数
        /// </summary>
        public IExpression Function { get; set; }

        /// <summary>
        /// 関数の引数
        /// </summary>
        public List<IExpression> Arguments { get; set; }

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
            var args = Arguments.Select(a => a.ToCode());
            var builder = new StringBuilder();
            builder.Append(Function.ToCode());
            builder.Append("(");
            builder.Append(string.Join(", ", args));
            builder.Append(")");
            return builder.ToString();
        }
    }
}
