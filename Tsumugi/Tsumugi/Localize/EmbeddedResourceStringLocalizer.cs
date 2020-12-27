using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml;

namespace Tsumugi.Localize
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceStringLocalizer : XmlStringLocalizer
    {
        /// <summary>
        /// 
        /// </summary>
        public EmbeddedResourceStringLocalizer()
        {
            // Get our current language and culture identifiers for the directory names.
            string language = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName; //"en" or "ja"
            string culture = Thread.CurrentThread.CurrentUICulture.Name; //"en-US" or "ja-JP"

            // Embedded resource namespaces can't contain certain characters and get renamed
            //  automatically by the compiler. So, '-' got replaced by '_' while compiling.
            // https://msdn.microsoft.com/en-us/library/ms145952.aspx
            // Note that the Localization.xml files are not "strongly typed", so we can't use
            //  StronglyTypedResourceBuilder.VerifyResourceName(). VerifyResourceName() would
            //  change "as" (American Samoa) to "_as", but that name does not get changed by
            //  the compiler. Since the only bad character we care about is '-', let's just fix it.
            culture = culture.Replace('-', '_');

            // To speed things up, only check the GAC if this assembly has been installed in the GAC.
            // This allows us to skip mscorlib, etc.
            bool searchGAC = Assembly.GetExecutingAssembly().GlobalAssemblyCache;

            // Resources are named like "Some.Random.Namespace.Resources.ja.Another.Random.Name.Localization.xml".
            // Let's search for ".Resources.ja" to make sure we're locating the correct file.
            m_resourceDirectory1 = ".Resources." + language + ".";
            m_resourceDirectory2 = ".Resources." + culture + ".";

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (searchGAC || !assembly.GlobalAssemblyCache)
                {
                    LoadEmeddedResources(assembly);
                }
            }

            // Subscribe to new assemblies
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LoadEmeddedResources(args.LoadedAssembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        private void LoadEmeddedResources(Assembly assembly)
        {
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (IsEmbeddedResource(resourceName, m_resourceDirectory1, m_resourceDirectory2))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(assembly.GetManifestResourceStream(resourceName));
                    AddLocalizedStrings(xmlDoc);
                }
            }
        }

        // Searches a resource name that came from an assembly's manifest to see if it represents an
        // embedded localization file and that it is in within one of the namespaces.
        private bool IsEmbeddedResource(string resourceName, string namespace1, string namespace2)
        {
            return
                resourceName.EndsWith(".Localization.xml") &&
                (
                    resourceName.Contains(namespace1) ||
                    resourceName.Contains(namespace2)
                );
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly string m_resourceDirectory1;

        /// <summary>
        /// 
        /// </summary>
        private readonly string m_resourceDirectory2;
    }
}
