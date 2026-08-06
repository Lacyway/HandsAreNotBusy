using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace HandsAreNotBusy;

[BepInPlugin("com.lacyway.hanb", "HandsAreNotBusy", "1.7.0")]
internal sealed class HANB_Plugin : BaseUnityPlugin
{
    internal static ConfigEntry<KeyboardShortcut> ResetKey;
    internal static ManualLogSource HANB_Logger;

    private void Awake()
    {
        ResetKey = Config.Bind("Keybind", "Reset Key", new KeyboardShortcut(KeyCode.End), new ConfigDescription("The key to reset the broken controller with."));
        HANB_Logger = Logger;

        HANB_Logger.LogInfo($"{nameof(HANB_Plugin)} has been loaded.");
        new HANB_Patch().Enable();
    }
}
