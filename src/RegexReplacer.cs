using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataMigrationTools.RegexReplacer
{
    //from data power tools
    internal static class DirectoryScanner
    {
        public static void ScanRecursive(string rootDir, Action<FileInfo> fileAction)
        {
            //recurse dirs too
            var dirs = Directory.GetDirectories(rootDir);
            foreach (var d in dirs)
                ScanRecursive(d, fileAction);

            ScanStandard(rootDir, fileAction);
        }

        public static void ScanStandard(string rootDir, Action<FileInfo> fileAction)
        {
            var files = new DirectoryInfo(rootDir).GetFiles();

            foreach (var file in files)
                fileAction(file);
        }

        public static void ScanRecursive(string rootDir, Action<string> fileAction)
        {
            //recurse dirs too
            var dirs = Directory.GetDirectories(rootDir);
            foreach (var d in dirs)
                ScanRecursive(d, fileAction);

            ScanStandard(rootDir, fileAction);
        }

        public static void ScanStandard(string rootDir, Action<string> fileAction)
        {
            var files = Directory.GetFiles(rootDir);

            foreach (var file in files)
                fileAction(file);
        }
    }

    internal class RegexReplacer
    {
        /// <summary>
        /// Performs regex replacing in files with optional preview
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var rxOptions = RegexOptions.None;
            var preview = false;

            if (args.Length < 3)
            {
                Usage();
                return;
            }

            var rootDir = args[0]; //path is first arg.

            var replacementArgs = args.Skip(1).ToArray(); //replacement args follow first path arg

            var replacementPattern = replacementArgs[1];

            var recursiveSearch = false;

            if (replacementArgs.Length > 2)
            {
                var options = replacementArgs[2].ToLower();
                if (options.StartsWith("/"))
                {
                    if (options.IndexOf("c") == -1)
                    {
                        // c not IgnoreCase (case sensitive)
                        rxOptions = RegexOptions.IgnoreCase;
                    }

                    // w IgnorePatternWhitespace
                    if (options.IndexOf("w") > -1)
                    {
                        rxOptions = rxOptions | RegexOptions.IgnorePatternWhitespace;
                    }

                    // r RightToLeft
                    if (options.IndexOf("r") > -1)
                    {
                        recursiveSearch = true;
                    }
                    //renameDirectories = options.IndexOf("d") > -1;
                    preview = options.IndexOf("p") > -1;
                }
                else
                {
                    Console.WriteLine("BAD OPTIONS");
                    Usage();
                    return;
                }
            }

            var inputPattern = replacementArgs[0];
            var rx = new Regex(inputPattern, rxOptions);
            
            var fileAction = new Action<string> (fileName =>
            {
                var fileContents = File.ReadAllText(fileName);
                if (rx.IsMatch(fileContents))
                {
                    if (preview)
                    {
                        Console.WriteLine("matched: {0}", fileName);
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllText(fileName, rx.Replace(fileContents, replacementPattern));
                            Console.WriteLine("replaced contents: {0}", fileName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("error : {0}=>{1} {2}", fileName, fileName, ex.Message);
                        }
                    }
                }
                else
                {
                    if (preview)
                    {
                        Console.WriteLine("not matched: {0}", fileName);
                    }
                    else
                    {
                        Console.WriteLine("nothing to replace: " + fileName);
                    }
                }
            });
            
            if (recursiveSearch)
            {
                DirectoryScanner.ScanRecursive(rootDir, fileAction);
            }
            else
            {
                DirectoryScanner.ScanStandard(rootDir, fileAction);
            }
        }
        static void Usage()
        {
            Console.WriteLine("RXR.exe - performs regular expression replacing of file contents.");
            Console.WriteLine("Usage: rxr <dir> <pattern> <replacement> [/pcwr]");
            Console.WriteLine("");
            Console.WriteLine(" p   Preview changes");
            Console.WriteLine(" c   Case sensitive");
            Console.WriteLine(" w   Ignore pattern whitespace");
            Console.WriteLine(" r   Recursive search");
            Console.WriteLine("");
        }
    }
}
