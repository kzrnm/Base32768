using System.Collections.Generic;
using System.IO;

namespace Kzrnm.Convert.Base32768
{
    public static class TestUtil
    {
        public static readonly IReadOnlyDictionary<string, byte[]> TestData = LoadTestData();
        private static Dictionary<string, byte[]> LoadTestData()
        {
            var dic = new Dictionary<string, byte[]>();
            foreach (var path in Directory.EnumerateFiles(".", "test-data/*", SearchOption.AllDirectories))
            {
                var name = path.Substring(1).Replace('\\', '/').TrimStart('/');
                dic[name] = File.ReadAllBytes(path);
            }
            return dic;
        }

        public static Stream ToStream(this byte[] bytes) => new MemoryStream(bytes);

#if NETFRAMEWORK
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
#endif
    }
}
