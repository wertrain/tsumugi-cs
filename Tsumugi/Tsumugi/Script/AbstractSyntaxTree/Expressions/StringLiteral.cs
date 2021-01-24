using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 文字列式
    /// </summary>
    public class StringLiteral : IExpression
    {
        /// <summary>
        /// 文字列式のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        ///文字列式の値
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// リテラル
        /// </summary>
        /// <returns>リテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode() => string.Format("\"{0}\"", Value);
    }
}
