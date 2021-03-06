using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#nullable disable
namespace sauce
{
    internal static class Program
    {
        public static string version = "1.1.0";
        [STAThread]
        public static void Main()
        {
            [DllImport("kernel32")]
            static extern int AllocConsole();
            [DllImport("kernel32")]
            static extern int FreeConsole();
            AllocConsole();
            Console.Title = "Sauce Or Loss";
            Console.WriteLine("BTELNYY's Sauce or Loss Version " + version);
            Console.WriteLine("Enter valid code.");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                switch (input)
                {
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unkown command.");
                        Console.ResetColor();
                        break;
                    case "code":
                        Console.WriteLine("Enter valid code.");
                        Console.Write("> ");
                        string code = Console.ReadLine();
                        DownloadImageFromUrl(code);
                        break;
                    case "file":
                        string path = OpenFileDialog(".\\", "txt files (*.txt)|*.txt|All files (*.*)|*.*");
                        Console.WriteLine($"About to parse codes. Path: {path}");
                        ReadCodes(path);
                        break;
                }
            }
        }
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Console.ResetColor();
        }
        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning: " + message);
            Console.ResetColor();
        }
        public static void DownloadImageFromUrl(string code)
        {
            try
            {
                WebClient wchtml = new WebClient();
                string htmlString = wchtml.DownloadString("https://nhentai.to/g/" + code);
                Directory.CreateDirectory(".\\" + code);
                Thread.Sleep(100);
                string[] things = htmlString.Split('"');
                string lasturl = "";
                foreach (string thing in things)
                {
                    bool result = Regex.IsMatch(thing, "https://t.dogehls.xyz/galleries/" + @"*");
                    if (result)
                    {
                        if (lasturl != thing)
                        {
                            lasturl = thing;
                            Console.WriteLine($"Downloading {thing}");
                            WebClient wc = new WebClient();
                            string[] path = thing.Split('/');
                            string filename = path.Last();
                            string[] uriarray = path.Take(path.Length - 1).ToArray();
                            string uri = "";
                            foreach (string s in uriarray)
                            {
                                uri = uri + s + "/";
                            }
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(thing, $".\\{code}\\" + filename);
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public static string OpenFileDialog(string path, string fileTypes)
        {
            try //try to run this code, on error break and show the error.
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    //sets info about our file dialog.
                    openFileDialog.InitialDirectory = @path;
                    openFileDialog.Filter = fileTypes; //format: "txt files (*.txt)|*.txt|All files (*.*)|*.*"
                    openFileDialog.FilterIndex = 1; //ensure that filter is always this one.
                    openFileDialog.RestoreDirectory = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        string filePath = openFileDialog.FileName;
                        return filePath;
                    }
                    else //needed so that it can return something.
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
                return null;
            }
        }
        public static void ReadCodes(string path)
        {
            try
            {
                string content = File.ReadAllText(path);
                string[] codes = content.Split('\n');
                Console.WriteLine("Split file into " + codes.Length + "code(s)");
                System.Diagnostics.Stopwatch stopwatch = new();
                System.Diagnostics.Stopwatch loopwatch = new();
                stopwatch.Start();
                foreach (string code in codes)
                {
                    string codeproccessed = code.Trim('\n');
                    Console.WriteLine("Parsing " + codeproccessed);
                    loopwatch.Start();
                    DownloadImageFromUrl(codeproccessed);
                    loopwatch.Stop();
                    Console.WriteLine("Parsed " + codeproccessed + ", took " + loopwatch.Elapsed);
                    loopwatch.Reset();
                }
                Console.WriteLine("File read finished, took " + stopwatch.Elapsed);
                stopwatch.Stop();
            }
            catch(Exception ex)
            {
                WriteError(ex.Message);
            }
        }
    }
}