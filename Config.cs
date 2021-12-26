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

    class Config
    {
        private readonly BepInEx.Logging.ManualLogSource log;
        private readonly ConfigFile cf;

        private VOType ToVOType(string s)
        {
            if (s == null)
                return VOType.Default;

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
        }

        private static string canonId(string Id)
        {
            return Id.ToLower();
        }

        // NOTE right now this will be called for almost every line.
        // It shouldn't cause any performance issues since surely BepInEx caches settings.
        private string GetActorIdFromSettings(string actorId)
        {
            foreach(var pair in cf.overrides)
            {
                var key = canonId(pair.Key);
                if (!actorId.Contains(key))
                    continue;
                return pair.Value;
            }
            return null;
        }
        public VOType getGlobalVO()
        {
            return ToVOType(this.cf.global_mode);
        }

        public VOType getActorVO(string actorId)
        {
            actorId = canonId(actorId);
            return ToVOType(GetActorIdFromSettings(actorId));
        }
    }
}
