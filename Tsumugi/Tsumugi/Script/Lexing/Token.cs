using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Lexing
{
    /// <summary>
    /// 構文解析トークン
    /// </summary>
    public class Token
    {
        public Token(TokenType type, string literal)
        {
            this.Type = type;
            this.Literal = literal;
        }
        public TokenType Type { get; set; }
        public string Literal { get; set; }
    }

    /// <summary>
    /// トークン種別
    /// </summary>
    public enum TokenType
    {
        /// --------- 特殊 --------- 

        /// <summary>
        /// 不正なトークン
        /// </summary>
        Illegal,

        /// <summary>
        /// 終端
        /// </summary>
        EOF,

        /// --------- 識別子 --------- 

        /// <summary>
        /// 識別子
        /// </summary>
        Identifier,

        /// <summary>
        /// 変数リテラル
        /// </summary>
        Variable,

        /// --------- 演算子 --------- 

        /// <summary>
        /// = 演算子
        /// </summary>
        Assign,

        /// <summary>
        /// + 演算子
        /// </summary>
        Plus,

        /// --------- デリミタ --------- 
        /// 
        /// <summary>
        /// カンマデリミタ
        /// </summary>
        Comma,

        /// <summary>
        /// セミコロンデリミタ
        /// </summary>
        Semicolon,

        /// --------- 括弧 --------- 

        /// <summary>
        /// 左丸括弧
        /// </summary>
        LeftParenthesis,

        /// <summary>
        /// 右丸括弧
        /// </summary>
        RightParenthesis,

        /// <summary>
        /// 左波括弧
        /// </summary>
        LeftBraces,

        /// <summary>
        /// 右波括弧
        /// </summary>
        RightBraces,

        /// <summary>
        /// 左角括弧
        /// </summary>
        LeftBrackets,

        /// <summary>
        /// 右角括弧
        /// </summary>
        RightBrackets,

        /// --------- キーワード --------- 

        /// <summary>
        /// 関数キーワード
        /// </summary>
        Function,

        /// <summary>
        /// 変数宣言キーワード
        /// </summary>
        Let,
    }
}
