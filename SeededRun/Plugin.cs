using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeededRun;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("White Knuckle.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static string seed = "";

    private static bool IsSeeded() => bool.Parse(SettingsManager.GetSetting("g_seeded"));

    private void Awake()
    {
        Logger = base.Logger;

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        {
            var origin = AccessTools.Method(typeof(WorldLoader), "Awake");
            var patch = AccessTools.Method(typeof(MyPatches), "Awake_MyPatch");
            harmony.Patch(origin, new HarmonyMethod(patch));
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name.Equals("Main-Menu") && IsSeeded())
            seed = GUI.TextField(new Rect(10, 10, 200, 20), seed, 25);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("Main-Menu")) AddSeededRunToggle();
    }

    private void AddSeededRunToggle()
    {
        var gMan = GameObject.Find("GameManager");
        var mutators = gMan.transform.Find("Canvas/Play Menu/Play Pane/Mutators");
        var compToggle = mutators.Find("Competitive Toggle");

        var seededToggle = Instantiate(compToggle.gameObject, mutators);
        seededToggle.transform.SetSiblingIndex(1);
        seededToggle.name = "Seeded Run Toggle";
        seededToggle.GetComponent<ToggleSettingsBinder>().settingName = "g_seeded";

        var seededToggleLabel = seededToggle.transform.Find("Background/Label (1)");
        seededToggleLabel.GetComponent<TextMeshProUGUI>().SetText("Seeded Run\n(Top Left Input)");
    }

    public class MyPatches
    {
        public static void Awake_MyPatch()
        {
            if (IsSeeded()) Random.InitState(int.Parse(seed));
        }
    }
}
