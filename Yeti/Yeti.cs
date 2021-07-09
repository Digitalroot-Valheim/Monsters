using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace YetiMonsterZ
{

    [BepInPlugin(PluginId, "YetiMonsterZ", "0.0.7")]
    public class YetiMonsterZ : BaseUnityPlugin
    {
        public const string PluginId = "YetiMonsterZ";
        private Harmony _harmony;

        private static GameObject Yeti;
        private static GameObject cape;
        private static GameObject pelt;
        private AssetBundle yetiboy;


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

            zNetScene.m_prefabs.Add(Yeti);
            zNetScene.m_prefabs.Add(cape);
            zNetScene.m_prefabs.Add(pelt);


        }
        public static void RegisterItems()
        {
            if (ObjectDB.instance.m_items.Count == 0 || ObjectDB.instance.GetItemPrefab("Amber") == null)
            {
                Debug.Log("Waiting for game to initialize before adding prefabs.");
                return;
            }
            var itemDrop = cape.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                if (ObjectDB.instance.GetItemPrefab(cape.name.GetStableHashCode()) == null)
                {
                    Debug.Log("Loading ItemDrops For YetiBoy");
                    ObjectDB.instance.m_items.Add(cape);
                    ObjectDB.instance.m_items.Add(pelt);
                   
                }
            }
            if (itemDrop == null)
            {
                Debug.Log("You BEE fuckin up, this kills me");
            }
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

            yetiboy = GetAssetBundleFromResources("yetiboy");
            Debug.Log("Loading Yeti");
            Yeti = yetiboy.LoadAsset<GameObject>("Yeti");
            Debug.Log("Loading Yeti Cape");
            cape = yetiboy.LoadAsset<GameObject>("CapeYeti");
            Debug.Log("Loading Yeti Pelt");
            pelt = yetiboy.LoadAsset<GameObject>("YetiPelt");

            yetiboy?.Unload(false);

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
        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDB_Awake_Patch
        {
            public static void Postfix()
            {
                Debug.Log("Trying to register Items");
                RegisterItems();
            }
        }
        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                Debug.Log("Trying to register Items");
                RegisterItems();
            }
        }
        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}