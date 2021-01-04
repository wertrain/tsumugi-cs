using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Statements
{
    /// <summary>
    /// Return 文
    /// 定義：return <expression>
    /// </summary>
    public class ReturnStatement : IStatement
    {
        /// <summary>
        /// Return 文のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 右辺の式
        /// </summary>
        public IExpression ReturnValue { get; set; }

        /// <summary>
        /// トークンのリテラル
        /// </summary>
        /// <returns>トークンのリテラル</returns>
        public string TokenLiteral() => Token.Literal;
    }
}
