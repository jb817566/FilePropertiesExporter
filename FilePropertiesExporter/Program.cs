using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using Shell32;
using Formatting = Newtonsoft.Json.Formatting;

namespace GetAllFileProperties
{
    class Program
    {

        static Object LOCK = new object();
        static ConcurrentBag<Dictionary<string, string>> _infoList = new ConcurrentBag<Dictionary<string, string>>();
        private static HttpClient _xCLIENT = new HttpClient();
        private static FileScannerOptions OPTIONS { get; set; }

        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<FileScannerOptions>(args)
                  .WithParsed<FileScannerOptions>(o =>
                  {
                      Program.OPTIONS = o;
                  });
            AsyncGetProperties(args);

        }

        private static void CallWithSTA(string file, string replacePath = null, bool async = false)
        {
            object[] _args = new object[] { file, replacePath };
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                GetExtendedProperties(_args);
            }
            else
            {
                Thread staThread = null;
                staThread = new Thread(new ParameterizedThreadStart(GetExtendedProperties));
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start(_args);
                staThread.Join();
            }
        }
        private static void AsyncGetProperties(string[] args)
        {
            IEnumerable<FileInfo> _files = new DirectoryInfo(OPTIONS.Folder).EnumerateFiles(OPTIONS.Query ?? "*",
                OPTIONS.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if (OPTIONS.Days != default)
            {
                DateTime afterDate = DateTime.Now.AddDays(OPTIONS.Days);
                IOrderedEnumerable<FileInfo> query = from f in _files
                                                     where f.CreationTime > afterDate
                                                     orderby f.CreationTime descending
                                                     select f;
                _files = query.AsParallel();
            }

            int i = 0;
            Parallel.ForEach(_files, new ParallelOptions()
            {
                MaxDegreeOfParallelism = OPTIONS.Threads
            }, fileInfo =>
            {
                var _fName = fileInfo.FullName;
                if (OPTIONS.FolderAliasPattern != default)
                {
                    string folder = fileInfo.Directory.FullName.Replace(OPTIONS.FolderAliasPattern, OPTIONS.FolderAliasReplace);
                    _fName = Path.Combine(folder, fileInfo.Name);
                }
                Interlocked.Increment(ref i);
                if (i % OPTIONS.SaveInterval == 0)
                {
                    WriteOut();
                }
                try
                {
                    CallWithSTA(_fName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            });
            WriteOut();
        }

        private static void WriteOut()
        {
            return;
            lock (LOCK)
            {
                File.WriteAllText(OPTIONS.JsonFile, JsonConvert.SerializeObject(_infoList, Formatting.Indented));
            }
        }

        private static void GetExtendedProperties(object fp)
        {
            try
            {
                object[] _args = (fp as object[]);
                FileInfo _replaceFileInfo = default;
                string _replacePath = default;
                if (_args.Length == 2)
                {
                    _replacePath = _args[1] as string;
                    _replaceFileInfo = new FileInfo(_replacePath);
                }
                string filePath = _args[0] as string;
                string directory = Path.GetDirectoryName(filePath);
                if (directory != null)
                {

                    Shell shell = new Shell32.Shell();
                    Folder shellFolder = shell.NameSpace(directory);
                    string fileName = Path.GetFileName(filePath);
                    FolderItem folderitem = shellFolder.ParseName(fileName);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    int i = -1;
                    while (++i < 320)
                    {
                        string header = shellFolder.GetDetailsOf(null, i);
                        if (String.IsNullOrEmpty(header))
                        {
                            continue;
                        }

                        string value = shellFolder.GetDetailsOf(folderitem, i);
                        if (!dictionary.ContainsKey(header) && !string.IsNullOrWhiteSpace(value))
                        {
                            if (_replacePath != default)
                            {
                                switch (header)
                                {
                                    case "FolderName":
                                        value = _replaceFileInfo.Directory.FullName;
                                        break;
                                    case "Filename":
                                        value = _replaceFileInfo.Name;
                                        break;
                                    case "Path":
                                        value = _replaceFileInfo.FullName;
                                        break;
                                }
                            }

                            dictionary.Add(header, value);
                        }
                    }

                    Marshal.ReleaseComObject(shell);
                    Marshal.ReleaseComObject(shellFolder);
                    Console.WriteLine("Got " + filePath);
                    _infoList.Add(dictionary);
                    SendToAPI(JsonConvert.SerializeObject(dictionary));
                }
                else
                {
                    Console.Error.WriteLine("ERROR Directory null");
                    string serializeObject = JsonConvert.SerializeObject(fp);
                    File.AppendAllText($"{OPTIONS.JsonFile}error.txt", serializeObject + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                if (fp != null)
                {
                    string serializeObject = JsonConvert.SerializeObject(fp);
                    Console.Error.WriteLine("ERROR " + serializeObject);
                    File.AppendAllText($"{OPTIONS.JsonFile}error.txt", serializeObject + Environment.NewLine);
                }
            }
        }

        private static void SendToAPI(string jsonObj)
        {
            string server = OPTIONS.Server ?? "http://localhost:65044";
            //Console.WriteLine($"\n\n{jsonObj}\n\n");
            int batchSize = 80;
            HttpResponseMessage _response = _xCLIENT
                .PostAsync($"{OPTIONS.Server}/api/flactrack/AddSingle",
                    new StringContent(JsonConvert.SerializeObject(jsonObj), Encoding.UTF8, "application/json")).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            if (!_response.IsSuccessStatusCode)
            {
                Console.WriteLine("error " + _response.StatusCode);
                Environment.Exit(0);
            }
        }

        private static void PostToApi(string value)
        {
            throw new NotImplementedException();
        }
    }
}
