using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Kzrnm.Convert.Base32768
{
    public static class TestUtil
    {
        public static IReadOnlyDictionary<string, byte[]> TestData = LoadTestData();
        private static Dictionary<string, byte[]> LoadTestData()
        {
            var dic = new Dictionary<string, byte[]>();
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (var path in Directory.EnumerateFiles(dir, "test-data/*", SearchOption.AllDirectories))
            {
                var name = path[dir.Length..].Replace('\\', '/').TrimStart('/');
                dic[name] = File.ReadAllBytes(path);
            }
            return dic;
        }

    }
}
