using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 前置演算子式
    /// </summary>
    public class PrefixExpression : IExpression
    {
        /// <summary>
        /// 識別子のトークン
        /// </summary>
        public Token Token { get; set; }

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
        public string ToCode() => string.Format("({0}{1})", Operator, Right.ToCode());
    }
}
