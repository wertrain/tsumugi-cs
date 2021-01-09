using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 真偽式
    /// </summary>
    public class BooleanLiteral : IExpression
    {
        /// <summary>
        /// 真偽式のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 真偽式の値
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// リテラル
        /// </summary>
        /// <returns>リテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode() => Token.Literal;
    }
}
