using BepInEx;

using HarmonyLib;

namespace StudioEnhancementSuite;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BaseUnityPlugin {
    public void Awake() {
        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
    }
}