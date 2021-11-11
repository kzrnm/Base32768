using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kzrnm.Convert.Base32768
{
    public record PairTestData(string Name, string String, byte[] Bytes)
    {
        public static PairTestData FromStringFileName(string name)
            => new(Path.GetFileNameWithoutExtension(name), Encoding.UTF8.GetString(TestUtil.TestData[name]), TestUtil.TestData[Path.ChangeExtension(name, "bin")]);

        public override string ToString() => Name;
    }
}
