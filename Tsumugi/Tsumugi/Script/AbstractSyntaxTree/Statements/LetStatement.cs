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
    }
}
