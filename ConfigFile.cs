using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.IO.Path;

namespace DEMoreVoiceoverOptions
{
    // Our own config file. Quick and dirty. BepInEx's doesn't have arbitrary keyss.


    public class ConfigFile
    {
        private static string NAME = GetFullPath(Combine(BepInEx.Paths.ConfigPath, "DEMoreVoiceoverOptions.cfg"));

        public string global_mode;
        public Dictionary<string, string> overrides;

        public ConfigFile()
        {
            overrides = new Dictionary<string, string>();
            global_mode = "Default";

            if (!File.Exists(NAME))
            {
                CatFile();
            }
            string section = null;
            foreach (var l in File.ReadAllLines(NAME))
            {
                var line = l.Trim();
                if (line.StartsWith("#"))
                    continue;
                if (line == "[General]")
                {
                    section = "general";
                    continue;
                }
                if (line == "[Overrides]")
                {
                    section = "overrides";
                    continue;
                }
                if (line.Contains("="))
                {
                    var items = line.Split("=");
                    var key = items[0].Trim();
                    var value = items[1].Trim();
                    if (section == "general")
                    {
                        global_mode = value;
                    }
                    if (section == "overrides")
                    {
                        overrides[key] = value;
                    }
                }
            }
        }

        private static void CatFile()
        {
            var output = String.Join(
                    Environment.NewLine,
                    "[General]",
                    "# Default voiceover mode. Everything except \"Default\" overrides DE settings. Valid values are:",
                    "#  * \"None\": disables voiceover entirely.",
                    "#  * \"Classic\": classic voiceover.",
                    "#  * \"Psychological\": psychological voiceover.",
                    "#  * \"ReversePsychological\": reverse psychological voiceover. Classic + (Full - Psychological).",
                    "#    Classic character voiceover with skill narration.",
                    "#  * \"Full\": full voiceover.",
                    "#  * \"Default\": use the default in DE settings.",
                    "VoiceoverMode = Default",
                    "",
                    "[Overrides]",
                    "# Voiceover overrides for specific characters. You can use the same values as for \"VoiceoverMode\"",
                    "# above, with \"Default\" using the main setting.",
                    "# To add more characters, add their name here.",
                    "# You don't have to add the entire name: for example you can use \"Klaasje\" rather than the",
                    "# entire \"Klaasje (Miss Oranje Disco Dancer)\".",
                    "Kim Kitsuragi = Default",
                    "");

            var directoryName = Path.GetDirectoryName(NAME);
            if (directoryName != null) Directory.CreateDirectory(directoryName);

            using (var writer = new StreamWriter(NAME, false, BepInEx.Utility.UTF8NoBom))
            {
                writer.Write(output);
            }
        }
    }
}
