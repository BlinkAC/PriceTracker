using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Content;
using Java.Net;
using Products3.Services;

namespace Products3.Platforms.Android
{
    public class AndroidShareHandler : IShareHandler
    {
        public bool HandleShare(object intent)
        {
            var androidIntent = intent as Intent;

            if (androidIntent?.Action == Intent.ActionSend && androidIntent.Type == "text/plain")
            {
                    var data = androidIntent?.ClipData?.GetItemAt(0);
                    var urlReceived = data.Text;
                    var retrievedItem = ExtractProductInfo(urlReceived);
                if (retrievedItem != null)
                {
                    MessagingService.SendUrlReceivedMessage(string.Join(",", [retrievedItem.Item2, retrievedItem.Item1, urlReceived]));
                    System.Diagnostics.Debug.WriteLine("URL compartida: " + urlReceived);
                    return true;
                }

                    System.Diagnostics.Debug.WriteLine("El intent no contiene datos de texto.");
                    return false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("El intent no contiene datos de texto.");
                return false;
            }
        }
        static Tuple<string, string> ExtractProductInfo(string url)
        {
            var patterns = new string[]
            {
            @"\/([^\/]+)\/p\/([^\/]+)", // Pattern for URLs with /p/
            @"\/(MLM-\d+)-([^\/_]+)"   // Pattern for URLs with -\/([^\/]+?)-([^\/]+)
            };
            if (!string.IsNullOrEmpty(url))
                foreach (var pattern in patterns)
                {
                    var match = Regex.Match(url, pattern);
                    if (match.Success)
                    {
                        string part1 = match.Groups[1].Value;
                        string part2 = match.Groups[2].Value;

                        //Mercado libre specific - depending on the url format the values came swapped
                        if (part1.Contains("MLM"))
                        {
                            return Tuple.Create(part1.Replace("-", ""), part2.Replace("-", " "));
                        }
                        else
                        {
                            return Tuple.Create(part2, part1.Replace("-", " "));
                        }
                    }
                }
            return null;
        }
    }
}
