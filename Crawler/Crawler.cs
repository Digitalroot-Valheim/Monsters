using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Crawler
{

    [BepInPlugin(PluginId, "Crawler", "0.0.7")]
    public class Crawler : BaseUnityPlugin
    {
        public const string PluginId = "Crawler";
        private Harmony _harmony;
        private static GameObject Nasty;
        private AssetBundle nasty;
        private static GameObject NastySpawner;

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
            zNetScene.m_prefabs.Add(Nasty);
            zNetScene.m_prefabs.Add(NastySpawner);

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

            nasty = GetAssetBundleFromResources("nasty");
            Debug.Log("Loading Nasty Crawler");
            Nasty = nasty.LoadAsset<GameObject>("TheNasty");
            Debug.Log("Loading Nasty Cocoons");
            NastySpawner = nasty.LoadAsset<GameObject>("Nasty_Spawner");
            nasty?.Unload(false);

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