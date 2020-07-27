using CommandLine;

namespace GetAllFileProperties
{
    internal class FileScannerOptions
    {
        [Option('t', "threads", Required = false, HelpText = "# of threads", Default = 4)]
        public int Threads { get; set; }
        [Option('f', "folder", Required = true, HelpText = "folder to scan", Default = null)]
        public string Folder { get; set; }
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