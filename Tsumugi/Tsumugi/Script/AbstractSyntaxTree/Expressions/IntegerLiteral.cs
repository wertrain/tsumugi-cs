using System;
using System.Collections.Generic;
using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 整数リテラル式
    /// </summary>
    public class IntegerLiteral : IExpression
    {
        /// <summary>
        /// 整数のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 整数の値
        /// </summary>
        public int Value { get; set; }

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
