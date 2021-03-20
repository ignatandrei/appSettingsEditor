using System;
using System.IO;
using System.Reflection;

namespace RoslynSettingEditor
{
    public static class EmbeddedResource
    {
        public static string GetContent(string relativePath, Assembly assembly = null)
        {
            if(assembly == null)
                assembly= Assembly.GetExecutingAssembly();
            var baseName = assembly.GetName().Name;
            var resourceName = relativePath
                .TrimStart('.')
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');

            using (var stream = assembly.GetManifestResourceStream($"{baseName}.{resourceName}")) {

                if (stream == null)
                {
                    throw new NotSupportedException("Unable to get embedded resource content, because the stream was null");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
