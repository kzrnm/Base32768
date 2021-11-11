using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Xunit;

namespace Kzrnm.Convert.Base32768
{
    public static class TestUtil
    {
        public static IReadOnlyDictionary<string, byte[]> TestData { get; } = LoadTestData();
        private static Dictionary<string, byte[]> LoadTestData()
        {
            var root = TestProjectPath();
            var dic = new Dictionary<string, byte[]>();
            foreach (var path in Directory.EnumerateFiles(root, "test-data/*", SearchOption.AllDirectories))
            {
                var name = path.Substring(root.Length + 1).Replace('\\', '/').TrimStart('/');
                dic[name] = File.ReadAllBytes(path);
            }
            return dic;
        }

        public static string TestProjectPath()
        {
            return Path.GetDirectoryName(GetCallerPath());
            static string GetCallerPath([CallerFilePath] string path = "") => path;
        }

        public static TheoryData<T1> ToTheoryData<T1>(this IEnumerable<T1> collection)
        {
            var theoryData = new TheoryData<T1>();
            foreach (var item in collection)
                theoryData.Add(item);
            return theoryData;
        }
        public static TheoryData<T1, T2> ToTheoryData<T1, T2>(this IEnumerable<(T1, T2)> collection)
        {
            var theoryData = new TheoryData<T1, T2>();
            foreach (var (t1, t2) in collection)
                theoryData.Add(t1, t2);
            return theoryData;
        }
        public static TheoryData<T1, T2, T3> ToTheoryData<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> collection)
        {
            var theoryData = new TheoryData<T1, T2, T3>();
            foreach (var (t1, t2, t3) in collection)
                theoryData.Add(t1, t2, t3);
            return theoryData;
        }
        public static TheoryData<T1, T2, T3, T4> ToTheoryData<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> collection)
        {
            var theoryData = new TheoryData<T1, T2, T3, T4>();
            foreach (var (t1, t2, t3, t4) in collection)
                theoryData.Add(t1, t2, t3, t4);
            return theoryData;
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
