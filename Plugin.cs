using BepInEx;
using BepInEx.IL2CPP;
using PixelCrushers.DialogueSystem;
using HarmonyLib;
using System;

namespace DEMoreVoiceoverOptions
{
    public class Voiceover
	{
        private static BepInEx.Logging.ManualLogSource log;
        private static Config config;

        public static bool CheckIfCanPlayBasedOnVOMode(DialogueEntry entry, ref bool __result)
        {
            if (config.shouldLogActorId()) {
                log.LogInfo($"Actor ID: {entry.ActorID}, Line: {entry.Title}");
            }
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

            var actorId = entry.ActorID;
            var mode = config.getActorVO(actorId);
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

        public static void ApplyPatches(BepInEx.Logging.ManualLogSource log, BepInEx.Configuration.ConfigFile config)
        {
            Voiceover.log = log;
            Voiceover.config = new Config(config, Voiceover.log);

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
            Voiceover.ApplyPatches(Log, Config);
            Log.LogInfo($"More voiceover options are loaded!");
        }
    }
}
