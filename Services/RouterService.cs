using System;
using System.Collections.Generic;

namespace browser_switch.Services
{
    public class RouterService
    {
        public RouterService()
        {
        }

        // LoadTxtFile reads the contents of a text file and returns it as a string
        public string LoadTxtFile(string filePath)
        {
            // Get current exe directory
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Load route rules from a text file
            string textFilePath = exeDir + "\\" + filePath;
            return System.IO.File.ReadAllText(textFilePath);
        }

        // Load route rules from a text file
        private Dictionary<string, string[]> routeRules;
        public Dictionary<string, string[]> LoadRouteRules(string filePath)
        {
            routeRules = new Dictionary<string, string[]>();
            string[] lines = LoadTxtFile(filePath).Split('\n');

            bool firstLine = true;
            foreach (string line in lines)
            {
                if (firstLine) { firstLine = false; continue; }
                var parts = line.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                string[] rule_items = new string[4];
                // rule name
                rule_items[0] = parts[0].Trim();
                // regex pattern
                rule_items[1] = parts[1].Trim();
                // browser path
                rule_items[2] = parts[2].Trim();
                // params
                if (parts.Length > 3)
                    rule_items[3] = parts[3].Trim();

                routeRules.Add(rule_items[1], rule_items);
            }
            return routeRules;
        }

        // Deal with the routing
        public bool Route(string url)
        {
            LoadRouteRules("router.txt");
            foreach (var rule in routeRules)
            {
                // use regex patter to match the url
                if (System.Text.RegularExpressions.Regex.IsMatch(url, rule.Key))
                {
                    OpenBrowser(rule.Value[2], rule.Value[3], url);
                    return true;
                }
            }
            return false;
        }


        // OpenBrowser opens a browser with the specified path and URL
        public void OpenBrowser(string browserPath, string brwoserParams, string url)
        {
            System.Diagnostics.Process.Start(browserPath, brwoserParams + " " + url);
        }

    }
}
