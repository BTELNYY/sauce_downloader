using System;
using System.Linq;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace sauce
{
    internal class Program
    {
        public static string version = "1.0.0";
        public static void Main()
        {
            Console.Title = "Sauce Or Loss";
            Console.WriteLine("BTELNYY's Sauce or Loss Version " + version);
            Console.WriteLine("Enter valid code.");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                DownloadImageFromUrl(input);
            }
        }
        public static void DownloadImageFromUrl(string code)
        {
            try
            {
                WebClient wchtml = new WebClient();
                string htmlString = wchtml.DownloadString("https://nhentai.to/g/" + code);
                Directory.CreateDirectory($".\\{code}");
                string[] things = htmlString.Split('"');
                string lasturl = "";
                foreach (string thing in things)
                {
                    bool result = Regex.IsMatch(thing, "https://t.dogehls.xyz/galleries/" + @"*");
                    if (result)
                    {
                        if(lasturl != thing)
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
    }
}