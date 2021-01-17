using System;
using System.Collections.Generic;
using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 倍精度浮動小数点数リテラル式
    /// </summary>
    public class DoubleLiteral : IExpression
    {
        /// <summary>
        /// 倍精度浮動小数点数のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 倍精度浮動小数点数の値
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// トークンのリテラル
        /// </summary>
        /// <returns>トークンのリテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode() => Token.Literal;
    }
}
