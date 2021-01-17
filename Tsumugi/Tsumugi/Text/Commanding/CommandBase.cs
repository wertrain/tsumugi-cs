using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Text.Commanding
{
    /// <summary>
    /// 変数を参照を表す型
    /// </summary>
    public class ReferenceVariable<T> where T : struct
    { 
        /// <summary>
        /// 変数名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 値の取得
        /// </summary>
        /// <returns></returns>
        public T? GetValueOrNull()
        {
            if (Referenced)
            {
                return new T?(_value);
            }
            return null;
        }

        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            _value = value;
            Referenced = true;
        }

        /// <summary>
        /// 参照済みかどうか
        /// </summary>
        private bool Referenced { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">変数名</param>
        public ReferenceVariable(string name)
        {
            Name = name;
            Referenced = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        public ReferenceVariable(T value)
        {
            SetValue(value);
        }

        /// <summary>
        /// 変数
        /// </summary>
        private T _value;
    }

    /// <summary>
    /// コマンドのベースクラス
    /// すべてのコマンドはこのクラスを継承する
    /// </summary>
    public class CommandBase
    {
    }
}
