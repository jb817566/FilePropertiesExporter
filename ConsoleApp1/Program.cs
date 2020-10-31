using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string _fileDict = await File.ReadAllTextAsync(@"C:\Users\Public\output_flac_2.json");
                List<FlacFile> _dict =
                    JsonConvert.DeserializeObject<List<FlacFile>>(_fileDict);
                long _total = _dict.Select(a => Len(a).Seconds).Sum();
                var _s = TimeSpan.FromSeconds(_total).TotalDays;
                Console.WriteLine(_total);
                Console.WriteLine(_s);
                //List<FlacFile> _list = _dict.Where(a => KeySelector(a) > 0).OrderByDescending(KeySelector).Take(20).ToList();
                //foreach (var flacFile in _list)
                //{
                //    Console.WriteLine($"{flacFile.Path} {flacFile.Length}");
                //}
            }
            catch (Exception e)
            {
            }
        }

        private static long KeySelector(FlacFile a)
        {
            string _d = "";
            try
            {
                _d = new String(a.BitRate.Where(Char.IsDigit).ToArray());
                return long.Parse(_d);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        private static TimeSpan Len(FlacFile a)
        {
            
            try
            {
                return a.Length;

            }
            catch (Exception e)
            {
                return TimeSpan.FromMilliseconds(0);
            }
        }
    }
}
