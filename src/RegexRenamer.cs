using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataMigrationTools.RegexRenamer
{
    internal class Program
    {
        /// <summary>
        /// Performs regex renaming of files with optional preview
        /// </summary>
        /// <param name="replacementArgs"></param>
        private static void Main(string[] args)
        {
            var rxOptions = RegexOptions.None;
            var preview = false;
            
            if (args.Length < 3)
            {
                Usage();
                return;
            }

            var replaceDir = args[0]; //path is first arg.
            
            var replacementArgs = args.Skip(1).ToArray(); //replacement args follow first path arg
            
            var replacement = replacementArgs[1];
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
                        rxOptions = rxOptions | RegexOptions.RightToLeft;
                    }

                    //recurseDirectories = options.IndexOf("s") > -1;
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

            var pattern = replacementArgs[0];
            var rx = new Regex(pattern, rxOptions);

            var files = Directory.GetFiles(replaceDir);
            foreach (string file in files)
            {
                if (rx.IsMatch(file))
                {
                    string newName = rx.Replace(file, replacement);
                    if (preview)
                    {
                        Console.WriteLine("preview: {0}", newName);
                    }
                    else
                    {
                        try
                        {
                            File.Move(file, newName);
                            Console.WriteLine(newName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("error : {0}=>{1} {2}", file, newName, ex.Message);
                        }
                    }
                }
                else
                {
                    if (preview)
                    {
                        Console.WriteLine("preview: {0}", file);
                    }
                    else
                    {
                        Console.WriteLine(file);
                    }
                }
            }
        }

        static void Usage()
        {
            Console.WriteLine("RXN.exe - performs regular expression renaming of files.");
            Console.WriteLine("Usage: rxn <dir> <pattern> <replacement> [/pcwr]");
            Console.WriteLine("");
            Console.WriteLine(" p   Preview changes");
            Console.WriteLine(" c   Case sensitive");
            Console.WriteLine(" w   Ignore pattern whitespace");
            Console.WriteLine(" r   Right-to-left");
            Console.WriteLine("");
        }
    }
}
