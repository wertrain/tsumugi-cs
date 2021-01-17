using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// オブジェクトの基底クラス
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        ObjectType Type();

        /// <summary>
        /// 評価結果
        /// </summary>
        /// <returns>評価結果</returns>
        string Inspect();
    }

    /// <summary>
    /// タイプ
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// 整数値
        /// </summary>
        Integer,

        /// <summary>
        /// 倍精度浮動小数点数値
        /// </summary>
        Double,

        /// <summary>
        /// 真偽値
        /// </summary>
        Boolean,

        /// <summary>
        /// NULL
        /// </summary>
        Null,

        /// <summary>
        /// Return オブジェクト
        /// </summary>
        Return,

        /// <summary>
        /// エラーオブジェクト
        /// </summary>
        Error,

        /// <summary>
        /// 関数オブジェクト
        /// </summary>
        Function,

        /// <summary>
        /// 文字列オブジェクト
        /// </summary>
        String,
        
        /// <summary>
        /// 組み込み関数オブジェクト
        /// </summary>
        Builtin
    }
}
