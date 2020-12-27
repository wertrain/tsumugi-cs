using System;
using System.Collections.Generic;
using System.Xml;

namespace Tsumugi.Localize
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class XmlStringLocalizer : StringLocalizer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string Localize(string s, string context)
        {
            List<Tuple<string, string>> translations;
            if (m_translations.TryGetValue(s, out translations))
            {
                if (translations.Count == 1)
                {
                    return translations[0].Item2;
                }

                int i = translations.FindIndex(pair => pair.Item1.Equals(context));
                if (i >= 0)
                {
                    return translations[i].Item2;
                }

                return translations[0].Item2;
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc"></param>
        protected void AddLocalizedStrings(XmlDocument xmlDoc)
        {
            XmlElement root = xmlDoc.DocumentElement;
            if (root == null || root.Name != "StringLocalizationTable")
            {
                throw new InvalidOperationException("invalid localization file: " + xmlDoc.BaseURI);
            }

            var duplicates = new List<string>();
            foreach (XmlElement element in root.GetElementsByTagName("StringItem"))
            {
                string id = element.GetAttribute("id");
                string context = element.GetAttribute("context");
                string translation = element.GetAttribute("translation");

                if (string.IsNullOrEmpty(translation))
                {
                    continue;
                }

                if (translation == "*")
                {
                    translation = id;
                }

                List<Tuple<string, string>> translations;
                if (!m_translations.TryGetValue(id, out translations))
                {
                    m_translations.Add(id, new List<Tuple<string, string>> { new Tuple<string, string>(context, translation) });
                }
                else
                {
                    int i = translations.FindIndex(pair => pair.Item1.Equals(context));
                    if (i < 0)
                    {
                        translations.Add(new Tuple<string, string>(context, translation));
                    }
                    else if (translations[i].Item2 != translation)
                    {
                        duplicates.Add(string.Format("1. \"{0}\", context: \"{1}\" => \"{2}\"", id, context, translations[i].Item2));
                        duplicates.Add(string.Format("2. \"{0}\", context: \"{1}\" => \"{2}\"", id, context, translation));
                    }
                }
            }

            if (duplicates.Count > 0)
            {
                throw new InvalidOperationException("Conflicting translations in a localized XML file: \n\t" + string.Join("\n\t", duplicates));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, List<Tuple<string, string>>> m_translations = new Dictionary<string, List<Tuple<string, string>>>();
    }
}
