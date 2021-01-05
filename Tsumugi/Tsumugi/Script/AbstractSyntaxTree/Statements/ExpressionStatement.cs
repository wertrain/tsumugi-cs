using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Statements
{
    /// <summary>
    /// 式文
    /// </summary>
    public class ExpressionStatement : IStatement
    {
        /// <summary>
        /// 式の最初のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 式
        /// </summary>
        public IExpression Expression { get; set; }

        /// <summary>
        /// トークンのリテラル
        /// </summary>
        /// <returns>トークンのリテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode() => Expression?.ToCode() ?? "";
    }
}
