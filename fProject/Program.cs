
using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

var rootCommand = new RootCommand("root command for file bundler CLI");
var bundleCommand = new Command("bundle", "bundle files to file");
var create_rsp = new Command("create-rsp", "response file");

var bundleOption = new Option<FileInfo>("--output", "file path and name");
var bundleLanguageOption = new Option<string>("--language", "choose the bundle file language")
{
    IsRequired = true
};
var bundleNoteOption = new Option<bool>("--note", "Writes the code source as a comment in the bundle file");
var bundleSortOption = new Option<string>("--sort", "Copies in the order selected by the user");
var bundleRemoveOption = new Option<bool>("--remove", "remove empty lines");
var bundleAutherOption = new Option<string>("--auther", "registering the name of the creator of the file");

bundleOption.AddAlias("--o");
bundleLanguageOption.AddAlias("--l");
bundleNoteOption.AddAlias("--n");
bundleSortOption.AddAlias("--s");
bundleRemoveOption.AddAlias("--r");
bundleAutherOption.AddAlias("--a");

bundleCommand.AddOption(bundleOption);
bundleCommand.AddOption(bundleLanguageOption);
bundleCommand.AddOption(bundleNoteOption);
bundleCommand.AddOption(bundleSortOption);
bundleCommand.AddOption(bundleRemoveOption);
bundleCommand.AddOption(bundleAutherOption);

create_rsp.SetHandler((FileInfo o, string l, bool n, string s, bool r, string a) =>
{
    try
    {
        using (StreamWriter responFile = new StreamWriter("responFile.rsp"))
        {
            Console.WriteLine("input the name of your new file and him path* (*option)");
            o = new FileInfo(Console.ReadLine());
            responFile.WriteLine("--o " + o);
            Console.WriteLine("input the language you want to copy or choose all");
            l = Console.ReadLine();
            responFile.WriteLine("--l " + l);
            Console.WriteLine("if you want to write a note with the name and the path of the source file press true if not press false");
            n = bool.Parse(Console.ReadLine());
            responFile.WriteLine("--n " + n);
            Console.WriteLine("input the type you want to sort by, if want to sort by the file type press byfiles");
            s = Console.ReadLine();
            responFile.WriteLine("--s " + s);
            Console.WriteLine("if you want to remove empty lines in the file press true else press false");
            r = bool.Parse(Console.ReadLine());
            responFile.WriteLine("--r " + r);
            Console.WriteLine("input the name of the file creater or press null");
            a = Console.ReadLine();
            responFile.WriteLine("--a " + a);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}, bundleOption, bundleLanguageOption, bundleNoteOption, bundleSortOption, bundleRemoveOption, bundleAutherOption);

bundleCommand.SetHandler((FileInfo o, string l, bool n, string s, bool r, string a) =>
{
    try
    {
        string[] language_arr = new string[] { "all", "java", "c#", "c", "linux", "python", "sql", "css", "html", "js" };
        bool flag = false;

        for (int i = 0; i < language_arr.Length; i++)
        {
            if (language_arr[i] == l)
                flag = true;
        }

        if (!flag)
        {
            Console.WriteLine("your language is not valid");
            return;
        }

        string directoryPath = o.FullName;
        string path = Path.GetDirectoryName(directoryPath);
        Console.WriteLine(path);

        string[] files = Directory.GetFiles(".");
        if (s == "byfiles")
        {
            files = files.OrderBy(file => Path.GetExtension(file)).ToArray();
        }

        using (StreamWriter newFileWriter = new StreamWriter(directoryPath))
        {
            if (a != "null")
            {
                newFileWriter.WriteLineAsync("//the auother of this file is " + a);
                Console.WriteLine("\n");
                newFileWriter.WriteLine("-----------------------------------");
            }

            foreach (var file in files)
            {
                try
                {
                    if (l == "all" || Path.GetExtension(file) == "." + l)
                    {
                        string content = File.ReadAllText(file);
                        string file_name = Path.GetFileName(file);

                        if (r)
                        {
                            var lines = File.ReadAllLines(file)
                                .Where(line => !string.IsNullOrWhiteSpace(line))
                                .ToArray();
                            content = string.Join(Environment.NewLine, lines);
                        }

                        if (n)
                        {
                            newFileWriter.WriteLine($"//the content of {file_name} from {path}");
                        }

                        newFileWriter.WriteLine(content);
                        newFileWriter.WriteLine("---------------------------------------------------------------");
                    }
                }
                catch (Exception fileEx)
                {
                    Console.WriteLine($"Failed to process file '{file}': {fileEx.Message}");
                }
            }
        }
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("Error: The specified directory was not found.");
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("Error: Access to a file or directory was denied.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
    }

}, bundleOption, bundleLanguageOption, bundleNoteOption, bundleSortOption, bundleRemoveOption, bundleAutherOption);

rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(create_rsp);
rootCommand.InvokeAsync(args).Wait();


