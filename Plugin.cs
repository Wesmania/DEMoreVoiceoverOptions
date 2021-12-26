using BepInEx;
using BepInEx.IL2CPP;
using PixelCrushers.DialogueSystem;
using HarmonyLib;
using System;
using System.Text.RegularExpressions;

namespace DEMoreVoiceoverOptions
{
    public class Voiceover
	{
        private static BepInEx.Logging.ManualLogSource log;
        private static Config config;
        private static Regex actorRegex = new Regex("^([^:]*):.*$");

        private static string GetActorName(DialogueEntry entry)
        {
            var db = DialogueBridgePixelCrushers.DialogueSystem?.masterDatabase;
            var actor = db?.GetActor(entry.ActorID);
            return actor?.LookupValue("Name");
        }
        public static bool CheckIfCanPlayBasedOnVOMode(DialogueEntry entry, ref bool __result)
        {
            var actorName = GetActorName(entry);

            var isMandatory = false;
            var isEnabledInPsychMode = false;
            var isDisabledInPsychMode = false;
            foreach (var field in entry.fields)
            {
                if (field.title == "AlwaysPlayVoice" && field.value == "True")
                {
                    isMandatory = true;
                }
                //Reverse psychological mode
                if (field.title == "PlayVoiceInPsychologicalMode")
                {
                    if (field.value == "True")
                        isEnabledInPsychMode = true;
                    if (field.value == "False")
                        isDisabledInPsychMode = true;
                }
            }

            var mode = config.getActorVO(actorName);
            if (mode == VOType.Default)
            {
                mode = config.getGlobalVO();
            }

            switch (mode)
            {
                case VOType.None:
                    __result = false;
                    return false;
                case VOType.Classic:
                    __result = isMandatory;
                    return false;
                case VOType.Psychological:
                    __result = isMandatory || isEnabledInPsychMode;
                    return false;
                case VOType.Full:
                    __result = true;
                    return false;
                case VOType.ReversePsychological:
                    __result = isMandatory || isDisabledInPsychMode;
                    return false;
                case VOType.Default:
                default:
                    return true;
            }
        }

        public static void ApplyPatches(BepInEx.Logging.ManualLogSource log)
        {
            Voiceover.log = log;
            Voiceover.config = new Config(new ConfigFile(), Voiceover.log);

            var harmony = new Harmony("Wesmania.DiscoElysium.il2cpp");

            var originalVO = AccessTools.Method(typeof(VOTool.VoiceOverSystem), "CheckIfCanPlayBasedOnVOMode");
            var postVO = AccessTools.Method(typeof(Voiceover), "CheckIfCanPlayBasedOnVOMode");
            harmony.Patch(originalVO, prefix: new HarmonyMethod(postVO));

            log.LogMessage("[DEMoreVoiceoverOptions] Voiceover patch applied!");
        }
    }

    [BepInPlugin("DEMoreVoiceoverOptions", "DEMoreVoiceoverOptions", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            // Plugin startup logic
            Voiceover.ApplyPatches(Log);
            Log.LogInfo($"More voiceover options are loaded!");
        }
    }
}
