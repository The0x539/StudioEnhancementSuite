using BepInEx;

using HarmonyLib;

using StudioEnhancementSuite.Patches;

namespace StudioEnhancementSuite;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BaseUnityPlugin {

    public void Awake() {
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        harmony.PatchAll(typeof(Scratchpad));
        UncapFramerate.Register(harmony, this.Config);
        HighContrastOutlines.Register(harmony, this.Config);
        ShorthandSearch.Register(harmony, this.Config);
        Blendsight.Register(harmony, this.Config);
        ShortPartNames.Register(harmony, this.Config);
    }
}