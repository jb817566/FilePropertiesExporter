using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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

        private static void CallWithSTA(string file, bool async = false)
        {
            object[] _args = new object[] { file };
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
            int i = 0;
            Parallel.ForEach(_files, new ParallelOptions()
            {
                MaxDegreeOfParallelism = OPTIONS.Threads
            }, fileInfo =>
            {
                Interlocked.Increment(ref i);
                if (i % OPTIONS.SaveInterval == 0)
                {
                    WriteOut();
                }
                try
                {
                    CallWithSTA(fileInfo.FullName);
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
            lock (LOCK)
                File.WriteAllText(OPTIONS.JsonFile, JsonConvert.SerializeObject(_infoList, Formatting.Indented));
        }

        private static void GetExtendedProperties(object fp)
        {
            try
            {
                string filePath = (fp as object[])[0] as string;
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
                            dictionary.Add(header, value);
                        }

                        //Console.WriteLine(header + ": " + value);
                    }

                    Marshal.ReleaseComObject(shell);
                    Marshal.ReleaseComObject(shellFolder);
                    Console.WriteLine("Got " + filePath);
                    _infoList.Add(dictionary);
                }
                else
                {
                    Console.Error.WriteLine("ERROR Directory null");
                    var serializeObject = JsonConvert.SerializeObject(fp);
                    File.AppendAllText($"{OPTIONS.JsonFile}error.txt", serializeObject + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                if (fp != null)
                {
                    var serializeObject = JsonConvert.SerializeObject(fp);
                    Console.Error.WriteLine("ERROR " + serializeObject);
                    File.AppendAllText($"{OPTIONS.JsonFile}error.txt", serializeObject + Environment.NewLine);
                }
            }
        }
    }
}
