using CommandLine;
using System;

namespace GetAllFileProperties
{
    internal class FileScannerOptions
    {

        [Option("folderAliasPattern", Required = false, HelpText = "Part of folder to alias (for temp inbox purposes)", Default = null)]
        public string FolderAliasPattern { get; set; }
        [Option("folderAliasReplace", Required = false, HelpText = "Alias for folder to be replaced to (for temp inbox purposes)", Default = null)]
        public string FolderAliasReplace{ get; set; }
        [Option('u', "urlserver", Required = false, HelpText = "url for song server", Default = null)]
        public string Server { get; set; }
        [Option('t', "threads", Required = false, HelpText = "# of threads", Default = 4)]
        public int Threads { get; set; }
        [Option('f', "folder", Required = true, HelpText = "folder to scan", Default = null)]
        public string Folder { get; set; }
        [Option('d', "pastdays", Required = false, HelpText = "Past N Days of Files", Default = 0)]
        public int Days { get; set; }
        [Option('r', "recursive", Required = false, HelpText = "Is Recursive", Default = false)]
        public bool Recursive { get; set; }
        [Option('q', "query", Required = false, HelpText = "Query filter", Default = null)]
        public string Query { get; set; }
        [Option('s', "saveinterval", Required = false, HelpText = "File Interval to save at", Default = 25)]
        public int SaveInterval { get; set; }
        [Option('o', "outputfile", Required = true, HelpText = "JSON file to output", Default = 25)]
        public string JsonFile { get; set; }


    }
}