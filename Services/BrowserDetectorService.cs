using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace browser_switch.Services
{
    class BrowserDetectorService
    {
        public class BrowserInfo
        {
            public string Name { get; set; }
            public bool IsInstalled { get; set; }
            public string InstallPath { get; set; }
            public List<string> Profiles { get; set; } = new List<string>();
        }

        public List<BrowserInfo> DetectBrowsers()
        {
            return new List<BrowserInfo>
        {
            DetectChrome(),
            DetectFirefox(),
            DetectEdge()
        };
        }

        private BrowserInfo DetectChrome()
        {
            var info = new BrowserInfo { Name = "Google Chrome" };

            // 检测安装路径
            info.InstallPath = GetRegistryValue(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                "");
            info.IsInstalled = !string.IsNullOrEmpty(info.InstallPath);

            // 获取Profile
            if (info.IsInstalled)
            {
                string userData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Google\Chrome\User Data");
                GetChromiumProfiles(userData, info);
            }

            return info;
        }

        private BrowserInfo DetectFirefox()
        {
            var info = new BrowserInfo { Name = "Mozilla Firefox" };

            // 检测安装路径
            info.InstallPath = GetRegistryValue(
                @"SOFTWARE\Mozilla\Mozilla Firefox",
                "CurrentVersion");
            if (!string.IsNullOrEmpty(info.InstallPath))
            {
                info.InstallPath = GetRegistryValue(
                    $@"SOFTWARE\Mozilla\Mozilla Firefox\{info.InstallPath}\Main",
                    "PathToExe");
            }
            info.IsInstalled = !string.IsNullOrEmpty(info.InstallPath);

            // 获取Profile
            if (info.IsInstalled)
            {
                string profilesIni = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Mozilla\Firefox\profiles.ini");
                GetFirefoxProfiles(profilesIni, info);
            }

            return info;
        }

        private BrowserInfo DetectEdge()
        {
            var info = new BrowserInfo { Name = "Microsoft Edge" };

            info.InstallPath = GetRegistryValue(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe",
                "");
            info.IsInstalled = !string.IsNullOrEmpty(info.InstallPath);

            if (info.IsInstalled)
            {
                string userData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Microsoft\Edge\User Data");
                GetChromiumProfiles(userData, info);
            }

            return info;
        }

        private void GetChromiumProfiles(string userDataPath, BrowserInfo info)
        {
            try
            {
                if (Directory.Exists(userDataPath))
                {
                    foreach (var dir in Directory.EnumerateDirectories(userDataPath))
                    {
                        string dirName = Path.GetFileName(dir);
                        if (dirName.StartsWith("Profile") || dirName == "Default")
                        {
                            info.Profiles.Add(dir);
                        }
                    }
                }
            }
            catch { /*  */ }
        }

        private void GetFirefoxProfiles(string profilesIniPath, BrowserInfo info)
        {
            try
            {
                if (!File.Exists(profilesIniPath)) return;

                var profiles = new List<string>();
                string currentSection = "";
                var iniData = new Dictionary<string, Dictionary<string, string>>();

                foreach (var line in File.ReadAllLines(profilesIniPath))
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        currentSection = trimmed.Trim('[', ']');
                        iniData[currentSection] = new Dictionary<string, string>();
                    }
                    else if (currentSection != "" && trimmed.Contains("="))
                    {
                        var parts = trimmed.Split(new[] { '=' }, 2);
                        iniData[currentSection][parts[0]] = parts[1];
                    }
                }

                foreach (var section in iniData)
                {
                    if (section.Key.StartsWith("Profile"))
                    {
                        if (section.Value.TryGetValue("Path", out string path))
                        {
                            bool isRelative;
                            if (section.Value.ContainsKey("IsRelative"))
                            {
                                isRelative = section.Value["IsRelative"] == "1";
                            }
                            else
                            {
                                isRelative = true; // 默认值 "1" 对应 true
                            }
                            string fullPath = isRelative ?
                                Path.Combine(Path.GetDirectoryName(profilesIniPath), path) :
                                path;
                            info.Profiles.Add(fullPath);
                        }
                    }
                }
            }
            catch { /* 处理解析错误 */ }
        }

        private string GetRegistryValue(string path, string valueName)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(path))
                {
                    if (key == null)
                    {
                        using (var key2 = Registry.CurrentUser.OpenSubKey(path))
                        {
                            return key2?.GetValue(valueName)?.ToString() ?? "";
                        }
                    }
                    return key?.GetValue(valueName)?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

    }
}
