using System.IO;
using System.Text;

namespace Kzrnm.Convert.Base32768.Models
{
    public record PairTestData(string Name, string String, byte[] Bytes)
    {
        public static PairTestData FromStringFileName(string name)
            => new(Path.GetFileNameWithoutExtension(name), Encoding.UTF8.GetString(TestUtil.TestData[name]), TestUtil.TestData[Path.ChangeExtension(name, "bin")]);

        public override string ToString() => Name;
    }
}
