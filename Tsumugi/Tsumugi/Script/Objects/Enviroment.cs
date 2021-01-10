using System.Collections.Generic;

namespace Tsumugi.Script.Objects
{
    /// <summary>
    /// 環境
    /// </summary>
    public class Enviroment
    {
        /// <summary>
        /// 変数テーブル
        /// </summary>
        public Dictionary<string, IObject> Store { get; set; } = new Dictionary<string, IObject>();

        /// <summary>
        /// 外部環境
        /// </summary>
        public Enviroment Outer { get; set; }

        /// <summary>
        /// 外部環境を作成
        /// </summary>
        /// <param name="outer"></param>
        /// <returns></returns>
        public static Enviroment CreateNewEnclosedEnviroment(Enviroment outer)
        {
            var enviroment = new Enviroment();
            enviroment.Outer = outer;
            return enviroment;
        }

        /// <summary>
        /// 変数を取得
        /// </summary>
        /// <param name="name">取得する変数名</param>
        /// <returns>評価オブジェクト</returns>
        public (IObject, bool) Get(string name)
        {
            var ok = Store.TryGetValue(name, out var value);

            if (!ok && Outer != null)
            {
                (value, ok) = Outer.Get(name);
            }

            return (value, ok);
        }

        /// <summary>
        /// 変数を設定
        /// </summary>
        /// <param name="name">取得する変数名</param>
        /// <param name="value">評価オブジェクト</param>
        /// <returns>評価オブジェクト</returns>
        public IObject Set(string name, IObject value)
        {
            Store.Add(name, value);
            return value;
        }
    }
}
