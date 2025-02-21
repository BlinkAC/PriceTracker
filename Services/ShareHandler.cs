using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products3.Services
{
    public interface IShareHandler
    {
        bool HandleShare(object intent);
        //Tuple<string, string> ExtractProductInfo(object intent);
    }

    public static class ShareHandler
    {
        private static IShareHandler _platformShareHandler;

        public static void Initialize(IShareHandler platformShareHandler)
        {
            _platformShareHandler = platformShareHandler;
        }

        public static bool HandleShare(object intent)
        {
            return _platformShareHandler.HandleShare(intent);

        }

        //public static Tuple<string, string> ExtractProductInfo(object intent)
        //{

        //        var patterns = new string[]
        //        {
        //    @"\/([^\/]+)\/p\/([^\/]+)", // Pattern for URLs with /p/
        //    @"\/([^\/]+?)-([^\/_]+)"   // Pattern for URLs with -
        //        };

        //        foreach (var pattern in patterns)
        //        {
        //            var match = Regex.Match(url, pattern);
        //            if (match.Success)
        //            {
        //                string part1 = match.Groups[1].Value;
        //                string part2 = match.Groups[2].Value;

        //                // Reorder parts if necessary (name-id or id-name)
        //                if (pattern.Contains("-"))
        //                {
        //                    return Tuple.Create(part2, part1);
        //                }
        //                return Tuple.Create(part1, part2);
        //            }
        //        }
        //        return null;

        //    }
        }
}
