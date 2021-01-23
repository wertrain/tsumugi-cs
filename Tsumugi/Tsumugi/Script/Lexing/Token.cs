using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Lexing
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
        public LexingPosition Position { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="literal"></param>
        public Token(TokenType type, string literal)
        {
            Type = type;
            Literal = literal;
            Position = new LexingPosition();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="literal"></param>
        /// <param name="position"></param>
        public Token(TokenType type, string literal, LexingPosition position)
        {
            Type = type;
            Literal = literal;
            Position = position;
        }

        /// <summary>
        /// 識別子かどうか判定する
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static TokenType LookupIdentifier(string identifier)
        {
            if (Keywords.ContainsKey(identifier))
            {
                return Keywords[identifier];
            }
            return TokenType.Identifier;
        }

        /// <summary>
        /// 予約語の辞書
        /// </summary>
        public static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>() {
            { "let", TokenType.Let },
            { "function", TokenType.Function },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "return", TokenType.Return },
            { "true", TokenType.True },
            { "false", TokenType.False },
        };
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
        /// 整数
        /// </summary>
        Integer,

        /// <summary>
        /// 倍精度浮動小数点数
        /// </summary>
        Double,

        /// <summary>
        /// 文字列リテラル
        /// </summary>
        String,

        /// --------- 演算子 --------- 

        /// <summary>
        /// = 演算子
        /// </summary>
        Assign,

        /// <summary>
        /// + 演算子
        /// </summary>
        Plus,

        /// <summary>
        /// - 演算子
        /// </summary>
        Minus,

        /// <summary>
        /// * 演算子
        /// </summary>
        Asterisk,

        /// <summary>
        /// / 演算子
        /// </summary>
        Slash,

        /// <summary>
        /// ! 演算子
        /// </summary>
        Bang,

        /// <summary>
        /// < 演算子
        /// </summary>
        LessThan,

        /// <summary>
        /// > 演算子
        /// </summary>
        GreaterThan,

        /// <summary>
        /// <= 演算子
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// >= 演算子
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// == 演算子
        /// </summary>
        Equal,

        /// <summary>
        /// != 演算子
        /// </summary>
        NotEqual,

        /// <summary>
        /// | 論理和
        /// </summary>
        LogicalDisjunction,

        /// <summary>
        /// & 論理積
        /// </summary>
        LogicalConjunction,

        /// <summary>
        /// && 「かつ」結合
        /// </summary>
        And,

        /// <summary>
        /// || 「または」結合
        /// </summary>
        Or,

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

        /// <summary>
        /// ダブルクォーテーション
        /// </summary>
        StraightQuotes,

        /// --------- キーワード --------- 

        /// <summary>
        /// 関数キーワード
        /// </summary>
        Function,

        /// <summary>
        /// 変数宣言キーワード
        /// </summary>
        Let,

        /// <summary>
        /// if キーワード
        /// </summary>
        If,

        /// <summary>
        /// else キーワード
        /// </summary>
        Else,

        /// <summary>
        /// return キーワード
        /// </summary>
        Return,

        /// <summary>
        /// true キーワード
        /// </summary>
        True,

        /// <summary>
        /// false キーワード
        /// </summary>
        False
    }
}
