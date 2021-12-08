using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DEMoreVoiceoverOptions
{
    enum VOType
    {
        None,
        Classic,
        Psychological,
        Full,
        ReversePsychological,
        Default,
    };

    class Character
    {
        public string name;
        public int id;
        public Character(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    };
    class Config
    {
        private readonly BepInEx.Logging.ManualLogSource log;
        private readonly ConfigFile cf;
        private readonly ConfigEntry<String> globalVoiceoverMode;
        private readonly ConfigEntry<bool> logActorId;
        private readonly Dictionary<int, ConfigEntry<string>> characterOverrides;

        private static List<Character> knownCharacters = new List<Character>
        {
            new Character("Garte", 6),
            new Character("Kim", 3),
            new Character("Klaasje", 1),
        };

        private VOType ToVOType(string s)
        {
            switch (s.ToLower())
            {
                case "none":
                    return VOType.None;
                case "classic":
                    return VOType.Classic;
                case "psychological":
                    return VOType.Psychological;
                case "full":
                    return VOType.Full;
                case "reversepsychological":
                    return VOType.ReversePsychological;
                case "default":
                    return VOType.Default;
                default:
                    log.LogWarning($"Unknown VO type \"{s}\", using Default instead.");
                    return VOType.Default;
            }
        }
        public Config(ConfigFile f, BepInEx.Logging.ManualLogSource log)
        {
            this.log = log;
            this.cf = f;
            this.characterOverrides = new Dictionary<int, ConfigEntry<string>>();

            globalVoiceoverMode = f.Bind(
                "General",
                "VoiceoverMode",
                "Default",
                String.Join(
                    Environment.NewLine,
                    "Default voiceover mode. Everything except \"Default\" overrides DE settings. Valid values are:",
                    " * \"None\": disables voiceover entirely.",
                    " * \"Classic\": classic voiceover.",
                    " * \"Psychological\": psychological voiceover.",
                    " * \"ReversePsychological\": reverse psychological voiceover. Classic + (Full - Psychological).",
                    "   Classic character voiceover with skill narration.",
                    " * \"Full\": full voiceover.",
                    " * \"Default\": use the default in DE settings.")
                );
            logActorId = f.Bind(
                "General",
                "LogActorId",
                false,
                "Log actor IDs to console for every conversation you encounter.");
            bool firstEntry = true;
            foreach (var c in knownCharacters)
            {
                var entry = f.Bind(
                    "Overrides",
                    c.name,
                    "Default",
                    firstEntry ? String.Join(
                        Environment.NewLine,
                        "Voiceover overrides for specific characters. You can use the same values as for \"VoiceoverMode\"",
                        "above, with \"Default\" using the main setting.",
                        "If the character you want to override is missing, you can try to add it manually, like so:",
                        " * Enable the \"LogActorId\" option, go ingame and talk to the character.",
                        " * Open the log file located in <Game directory>/BepInEx/LogOutput.log.",
                        " * Find the character's dialogue. Next to it an \"Actor ID\" will be listed.",
                        " * Add an entry \"Actor_<NUMBER>\"=<value> to this section, e.g. Actor_17=Full.",
                        "",
                        "If you think the character you're overriding should be in this mod by default, let me know!",
                        "Contact me either on the mod page or on GitHub and tell me the character name and ID."
                        ) : ""
                    );
                characterOverrides.Add(c.id, entry);
                firstEntry = false;
            }
        }

        // NOTE right now this will be called for almost every line.
        // It shouldn't cause any performance issues since surely BepInEx caches settings.
        private bool checkForActorIdInSettings(int actorId)
        {
            // Just in case
            if (characterOverrides.ContainsKey(actorId))
            {
                return true;
            }

            var exists = cf.TryGetEntry("CharacterOverrides", $"Actor_{actorId}", out ConfigEntry<string> entry);
            if (exists)
            {
                characterOverrides.Add(actorId, entry);
            }
            return exists;
        }
        public VOType getGlobalVO()
        {
            return ToVOType(this.globalVoiceoverMode.Value);
        }

        public VOType getActorVO(int actorId)
        {
            if (characterOverrides.ContainsKey(actorId) ||
                checkForActorIdInSettings(actorId))
            {
                return ToVOType(characterOverrides[actorId].Value);
            }
            return VOType.Default;
        }

        public bool shouldLogActorId()
        {
            return logActorId.Value;
        }
    }
}
