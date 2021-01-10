using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 文字列オブジェクト
    /// </summary>
    public class StringObject : IObject
    {
        /// <summary>
        /// 文字列
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">整数値</param>
        public StringObject(string value) => Value = value;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.String;

        /// <summary>
        /// 文字列の評価結果
        /// </summary>
        /// <returns>評価結果</returns>
        public string Inspect() => Value.ToString();
    }
}
