using System.Text;
using Tsumugi.Script.Lexing;

namespace Tsumugi.Script.AbstractSyntaxTree.Expressions
{
    /// <summary>
    /// 識別子式
    /// </summary>
    public class Identifier : IExpression
    {
        /// <summary>
        /// 識別子のトークン
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 識別子の値
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        public Identifier(Token token, string value)
        {
            Token = token;
            Value = value;
        }

        /// <summary>
        /// リテラル
        /// </summary>
        /// <returns>リテラル</returns>
        public string TokenLiteral() => Token.Literal;

        /// <summary>
        /// コードに変換
        /// </summary>
        /// <returns>コード</returns>
        public string ToCode() => Value;
    }
}
