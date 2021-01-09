using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tsumugi.Script.AbstractSyntaxTree.Statements;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 関数式
    /// </summary>
    public class FunctionLiteral : IExpression
    {
        /// <summary>
        /// 関数式のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 関数の引数
        /// </summary>
        public List<Identifier> Parameters { get; set; }

        /// <summary>
        /// 関数ブロック
        /// </summary>
        public BlockStatement Body { get; set; }

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
            var parameters = Parameters.Select(p => p.ToCode());
            var builder = new StringBuilder();
            builder.Append("function");
            builder.Append("(");
            builder.Append(string.Join(", ", parameters));
            builder.Append(")");
            builder.Append(Body.ToCode());
            return builder.ToString();
        }
    }
}
