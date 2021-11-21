using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;

namespace DEReversePsychVO
{
    public class Voiceover
	{
        public static BepInEx.Logging.ManualLogSource log;

        public static bool CheckIfCanPlayBasedOnVOMode(PixelCrushers.DialogueSystem.DialogueEntry entry, ref bool __result)
        {
            foreach (var field in entry.fields)
            {
                if (field.title == "AlwaysPlayVoice" && field.value == "True")
                {
                    __result = true;
                    return false;
                }
                //Reverse psychological mode
                if (field.title == "PlayVoiceInPsychologicalMode" && field.value == "False") 
                {
                    __result = true;
                    return false;
                }
            }
            __result = false;
            return false;
        }

        public static void ApplyPatches(BepInEx.Logging.ManualLogSource log)
        {
            Voiceover.log = log;

            var harmony = new Harmony("Wesmania.DiscoElysium.il2cpp");

            var originalVO = AccessTools.Method(typeof(VOTool.VoiceOverSystem), "CheckIfCanPlayBasedOnVOMode");
            var postVO = AccessTools.Method(typeof(Voiceover), "CheckIfCanPlayBasedOnVOMode");
            harmony.Patch(originalVO, prefix: new HarmonyMethod(postVO));

            log.LogMessage("[DEReversePsychVO] Voiceover patch applied!");
        }
    }

    [BepInPlugin("com.wesmania.DEReversePsychVO", "DEVOControl", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            // Plugin startup logic
            Log.LogInfo($"Reverse psychological mode is loaded!");
            Voiceover.ApplyPatches(Log);
        }
    }
}
