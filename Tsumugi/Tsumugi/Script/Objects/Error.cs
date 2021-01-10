using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// エラー評価オブジェクト
    /// </summary>
    public class Error : IObject
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public Error(string message) => Message = message;

        /// <summary>
        /// オブジェクトのタイプ
        /// </summary>
        /// <returns>タイプ</returns>
        public ObjectType Type() => ObjectType.Error;

        /// <summary>
        /// 評価式
        /// </summary>
        /// <returns>評価式</returns>
        public string Inspect() => Message;
    }
}
