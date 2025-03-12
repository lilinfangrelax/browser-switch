using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace browser_switch.Services
{
    public class RegistryService
    {
        private string _exePath;
        public bool CheckIfExistRegistryKey()
        {

            // get current working directory
            string currentDir = System.IO.Directory.GetCurrentDirectory();
            // combine with exe filepath
            _exePath = System.IO.Path.Combine(currentDir, "browser-switch.exe");

            Collection<string[]> registryKeys = new Collection<string[]> { };
            registryKeys.Add(new string[] { "HKEY_CLASSES_ROOT\\BrowserSwitch", null, null });
            registryKeys.Add(new string[] { "HKEY_CLASSES_ROOT\\BrowserSwitch\\DefaultIcon", null, $"{_exePath},0" });
            registryKeys.Add(new string[] { "HKEY_CLASSES_ROOT\\BrowserSwitch\\shell\\open\\command", null, $"\"{_exePath}\" %1" });
            registryKeys.Add(new string[] { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch", null, null });
            registryKeys.Add(new string[] { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities", "ApplicationIcon", $"{_exePath},0" });
            registryKeys.Add(new string[] { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities\\URLAssociations", "http", "BrowserSwitch" });
            registryKeys.Add(new string[] { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities\\URLAssociations", "https", "BrowserSwitch" });
            registryKeys.Add(new string[] { "HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\shell\\open\\command", null, $"\"{_exePath}\"" });
            registryKeys.Add(new string[] { "HKEY_LOCAL_MACHINE\\SOFTWARE\\RegisteredApplications", "Browser Switch", "Software\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities" });


            // check if registry key exist
            foreach (string[] keyItem in registryKeys)
            {
                string key_str = keyItem[0].Split('\\')[0];
                Debug.WriteLine(keyItem[0]);
                RegistryKey key = null;
                if (key_str.Equals("HKEY_CLASSES_ROOT")) key = Registry.ClassesRoot.OpenSubKey(keyItem[0].Replace("HKEY_CLASSES_ROOT\\", ""));
                if (key_str.Equals("HKEY_LOCAL_MACHINE")) key = Registry.LocalMachine.OpenSubKey(keyItem[0].Replace("HKEY_LOCAL_MACHINE\\", ""));
                if (key != null)
                {
                    // check if key-value is correct
                    if (keyItem[2] != null)
                    {
                        string val = key.GetValue(keyItem[1])?.ToString();
                        Debug.WriteLine($"{val.Equals(keyItem[2])}  {keyItem[1]}: {keyItem[2]}");
                        Debug.WriteLine(val);
                        if (!val.Equals(keyItem[2]))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void CreateRegistry()
        {
            Debug.WriteLine("CreateRegistry");
            SaveRegistryFile(_exePath);

            FileInfo fileInfo = new FileInfo(_exePath);
            // get exe file full direcotry
            string exeDir = fileInfo.DirectoryName;

            // manual open reg file dir
            Process.Start("explorer.exe", exeDir);
        }


        // save registry file, it have a specific format!
        public static void SaveRegistryFile(string exePath)
        {
            /**
             * Windows Registry Editor Version 5.00

            [HKEY_CLASSES_ROOT\BrowserSwitch]
            [HKEY_CLASSES_ROOT\BrowserSwitch\DefaultIcon]
            @="D:\\Dev\\CSharp\\browser-switch\\bin\\Release\\browser-switch.exe,0"
            [HKEY_CLASSES_ROOT\BrowserSwitch\shell]
            [HKEY_CLASSES_ROOT\BrowserSwitch\shell\open]
            [HKEY_CLASSES_ROOT\BrowserSwitch\shell\open\command]
            @="\"D:\\Dev\\CSharp\\browser-switch\\bin\\Release\\browser-switch.exe\" %1"
            [HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\Browser Switch]
            @="Browser Switch"
            [HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\Browser Switch\Capabilities]
            "ApplicationIcon"="D:\\Dev\\CSharp\\browser-switch\\bin\\Release\\browser-switch.exe,0"
            [HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\Browser Switch\Capabilities\URLAssociations]
            "http"="BrowserSwitch"
            "https"="BrowserSwitch"
            [HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\Browser Switch\shell]
            [HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\Browser Switch\shell\open]
            [HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet\Browser Switch\shell\open\command]
            @="\"D:\\Dev\\CSharp\\browser-switch\\bin\\Release\\browser-switch.exe\""

            [HKEY_LOCAL_MACHINE\SOFTWARE\RegisteredApplications]
            "Browser Switch"="Software\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities"
            */
            exePath = exePath.Replace("\\", "\\\\");

            // multi line string
            string reg_file_txt = "Windows Registry Editor Version 5.00\n\n";

            reg_file_txt += "[HKEY_CLASSES_ROOT\\BrowserSwitch]\n\n";

            reg_file_txt += "[HKEY_CLASSES_ROOT\\BrowserSwitch\\DefaultIcon]\n";
            reg_file_txt += "@=\"" + exePath + ",0\"\n\n";

            reg_file_txt += "[HKEY_CLASSES_ROOT\\BrowserSwitch\\shell]\n\n";

            reg_file_txt += "[HKEY_CLASSES_ROOT\\BrowserSwitch\\shell\\open]\n\n";

            reg_file_txt += "[HKEY_CLASSES_ROOT\\BrowserSwitch\\shell\\open\\command]\n";
            reg_file_txt += "@=\"\\\"" + exePath + "\\\" %1\"\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch]\n";
            reg_file_txt += "@=\"Browser Switch\"\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities]\n";
            reg_file_txt += "\"ApplicationIcon\"=\"" + exePath + ",0\"\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\Capabilities\\URLAssociations]\n";
            reg_file_txt += "\"http\"=\"BrowserSwitch\"\n";
            reg_file_txt += "\"https\"=\"BrowserSwitch\"\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\shell]\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\shell\\open]\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\Clients\\StartMenuInternet\\Browser Switch\\shell\\open\\command]\n";
            reg_file_txt += "@=\"\\\"" + exePath + "\\\"\"\n\n";

            reg_file_txt += "[HKEY_LOCAL_MACHINE\\SOFTWARE\\RegisteredApplications]\n";
            reg_file_txt += "\"Browser Switch\"=\"Software\\\\Clients\\\\StartMenuInternet\\\\Browser Switch\\\\Capabilities\"\n";

            // save to .reg file
            System.IO.File.WriteAllText("Browser-Switch-Register.reg", reg_file_txt);
        }


        public bool CheckWindowsDefaultBrowser()
        {
            // HKEY_CURRENT_USER\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice"))
            {
                string ProgId = key.GetValue("ProgId")?.ToString();
                if (ProgId.Equals("BrowserSwitch"))
                    return true;
            }
            return false;
        }
    }
}