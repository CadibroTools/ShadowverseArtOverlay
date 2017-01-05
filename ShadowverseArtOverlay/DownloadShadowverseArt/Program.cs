using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace DownloadShadowverseArt
{
    class Program
    {
        static void Main(string[] args)
        {
            var language = ConfigurationSettings.AppSettings["language"] ?? "en";
            var totalCardsString = ConfigurationSettings.AppSettings["totalCards"] ?? "680";


            string url = $"https://shadowverse-portal.com/cards?m=index&lang={language}&m=index&include_token=1&card_offset=";




            for (int i = 0; i < int.Parse(totalCardsString); i++)
            {
                try
                {
                    var cardUrl = url + i;
                    var wrGeturl = WebRequest.Create(cardUrl);

                    wrGeturl.Proxy = WebProxy.GetDefaultProxy();

                    var objStream = wrGeturl.GetResponse().GetResponseStream();

                    StreamReader objReader = new StreamReader(objStream);
                    var response = objReader.ReadToEnd();

                    Regex firstCard =
                        new Regex(
                            @"<img class=.*alt=""(.*)"" data.*(https:\/\/shadowverse-portal\.com\/image\/card\/.*\.png)");
                    var match = firstCard.Match(response);

                    string name = match.Groups[1].Value;
                    string link = match.Groups[2].Value;

                    if (!Directory.Exists("Art"))
                        Directory.CreateDirectory("Art");

                    using (WebClient client = new WebClient())
                    {
                        if (!File.Exists("Art/" + name.Replace(" ", "_") + ".png"))
                        {
                            client.DownloadFile(new Uri(link), "Art/" + name.Replace(" ", "_") + ".png");
                            Console.WriteLine($"{i + 1}/{int.Parse(totalCardsString)} - Downloaded {name}");
                        }
                        else
                        {
                            Console.WriteLine($"{i + 1}/{int.Parse(totalCardsString)} - Skipping {name}");
                        }

                        if (!File.Exists("Art/" + name.Replace(" ", "_") + "_Evolved.png"))
                        {
                            client.DownloadFile(new Uri(link.Replace("C_", "E_")),
                                "Art/" + name.Replace(" ", "_") + "_Evolved.png");

                            var fileInfo = new FileInfo("Art/" + name.Replace(" ", "_") + "_Evolved.png");
                            if (fileInfo.Length < 10000)
                                fileInfo.Delete();
                            else
                            {
                                Console.WriteLine($"{i + 1}/{int.Parse(totalCardsString)} - Downloaded {name} evolved");
                            }
                        }
                    }

                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured while trying to download an image, waiting 10 seconds and retrying...");
                    Console.WriteLine($"Error Message was: {ex.Message}");
                    Thread.Sleep(10000);
                    i--;
                }
            }
        }
    }
}
