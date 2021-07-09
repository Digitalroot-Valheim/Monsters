using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Reaper
{
    [BepInPlugin(PluginId, "ReaperZZ", "0.0.7")]
    public class Reaper : BaseUnityPlugin
    {
        public const string PluginId = "ReaperZZ";
        private Harmony _harmony;
        private static GameObject evlreaper;
        private AssetBundle reaper;

        private void Awake()
        {
            LoadAssets();
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginId);
        }
        public static void TryRegisterFabs(ZNetScene zNetScene)
        {
            if (zNetScene == null || zNetScene.m_prefabs == null || zNetScene.m_prefabs.Count <= 0)
            {
                return;
            }

            zNetScene.m_prefabs.Add(evlreaper);
        }



        private static AssetBundle GetAssetBundleFromResources(string filename)
        {
            var execAssembly = Assembly.GetExecutingAssembly();
            var resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }
        private void LoadAssets()
        {
            reaper = GetAssetBundleFromResources("evlrpr");

            Debug.Log("Loading Reaper");
            evlreaper = reaper.LoadAsset<GameObject>("EvilReaper");

            reaper?.Unload(false);

        }

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        public static class ZNetScene_Awake_Patch
        {
            public static bool Prefix(ZNetScene __instance)
            {

                TryRegisterFabs(__instance);

                Debug.Log("Loading the eggs");
                return true;
            }
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
