using System.Collections.Generic;
using System.Reflection;
using Tsumugi.Localize;
using Tsumugi.Script.Objects;

namespace Tsumugi.Script.Evaluating
{
    /// <summary>
    /// 組み込み関数
    /// </summary>
    class Builtins
    {
        public static Dictionary<string, IObject> Functions { get; set; } = new Dictionary<string, IObject>()
        {
            { "length", new BuiltinObject(length) },
            { "str", new BuiltinObject(str) }
        };

        private static IObject length(List<IObject> args)
        {
            if (args.Count != 1)
            {
                return new Error(string.Format(LocalizationTexts.NumberOfArgumentsDoesNotMatch.Localize(), MethodBase.GetCurrentMethod().Name));
            }

            var arg = args[0];

            switch(arg)
            {
                case StringObject stringObject:
                    return new IntegerObject(stringObject.Value.Length);
            }

            return new Error(string.Format(LocalizationTexts.DoesNotSupportArgumentsOfType.Localize(), MethodBase.GetCurrentMethod().Name, arg.Type()));
        }

        private static IObject str(List<IObject> args)
        {
            if (args.Count != 1)
            {
                return new Error(string.Format(LocalizationTexts.NumberOfArgumentsDoesNotMatch.Localize(), MethodBase.GetCurrentMethod().Name));
            }

            var arg = args[0];

            switch (arg)
            {
                case StringObject stringObject:
                    return arg;

                case IntegerObject integerObject:
                    return new StringObject(integerObject.Value.ToString());

                case DoubleObject doubleObject:
                    return new StringObject(doubleObject.Value.ToString());

                case NullObject nullObject:
                    return new StringObject(nullObject.Inspect());

                case BooleanObject booleanObject:
                    return new StringObject(booleanObject.Inspect());

                case Error error:
                    return new StringObject(error.Inspect());
            }

            return new Error(string.Format(LocalizationTexts.DoesNotSupportArgumentsOfType.Localize(), MethodBase.GetCurrentMethod().Name, arg.Type()));
        }
    }
}
