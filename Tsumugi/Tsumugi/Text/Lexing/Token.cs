using System.Collections.Generic;

namespace Tsumugi.Text.Lexing
{
    /// <summary>
    /// 字句解析トークン
    /// </summary>
    public class Token
    {
        /// <summary>
        /// トークン種別
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// リテラル
        /// </summary>
        public string Literal { get; set; }

        /// <summary>
        /// 解析位置
        /// </summary>
        public Script.Lexing.LexingPosition Position { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="literal"></param>
        public Token(TokenType type, string literal)
        {
            Type = type;
            Literal = literal;
            Position = new Script.Lexing.LexingPosition();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="literal"></param>
        /// <param name="position"></param>
        public Token(TokenType type, string literal, Script.Lexing.LexingPosition position)
        {
            Type = type;
            Literal = literal;
            Position = position;
        }

        /// <summary>
        /// 予約語の辞書
        /// </summary>
        public static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>() {
 
        };
    }

    /// <summary>
    /// トークン種別
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// 不正なトークン
        /// </summary>
        Illegal,

        /// <summary>
        /// 終端
        /// </summary>
        EOF,

        /// <summary>
        /// テキスト
        /// </summary>
        Text,

        /// <summary>
        /// タグ
        /// </summary>
        Tag,

        /// <summary>
        /// タグ終了
        /// </summary>
        TagEnd,

        /// <summary>
        /// タグの属性名
        /// </summary>
        TagAttributeName,

        /// <summary>
        /// タグの属性値
        /// </summary>
        TagAttributeValue,

        /// <summary>
        /// ラベル
        /// </summary>
        Label,

        /// <summary>
        /// ラベルの見出し
        /// </summary>
        LabelHeadline,

        /// <summary>
        /// 代入文字
        /// </summary>
        Assign
    }
}
