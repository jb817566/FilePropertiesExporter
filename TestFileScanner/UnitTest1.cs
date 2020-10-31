using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TestFileScanner
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            string _fileDict = await File.ReadAllTextAsync(@"C:\Users\Public\output_flac.json");
            List<Dictionary<string, string>> _dict = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(_fileDict);
            var _br = _dict.OrderByDescending(a => int.Parse(a["Bit rate"].Replace("kbps", ""))).ToList().First();

        }
    }
}
