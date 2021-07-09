using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WhereWolves
{

    [BepInPlugin(PluginId, "WhereWolves", "0.0.7")]
    public class WhereWolves : BaseUnityPlugin
    {
        public const string PluginId = "WhereWolves";
        private Harmony _harmony;
        private static GameObject WereWolf1;
        private static GameObject WereWolf2;
        private static GameObject WereWolf3;
        private AssetBundle werewolf;
        private static GameObject WereWolf4;
        private static GameObject WereWolf5;
        private static GameObject Cape1;


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
            zNetScene.m_prefabs.Add(WereWolf1);
            zNetScene.m_prefabs.Add(WereWolf2);
            zNetScene.m_prefabs.Add(WereWolf3);
            zNetScene.m_prefabs.Add(WereWolf4);
            zNetScene.m_prefabs.Add(WereWolf5);
            zNetScene.m_prefabs.Add(Cape1);
        }

        public static void RegisterItems()
        {
            if (ObjectDB.instance.m_items.Count == 0 || ObjectDB.instance.GetItemPrefab("Amber") == null)
            {
                Debug.Log("Waiting for game to initialize before adding prefabs.");
                return;
            }
            var itemDrop = Cape1.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                if (ObjectDB.instance.GetItemPrefab(Cape1.name.GetStableHashCode()) == null)
                {
                    Debug.Log("Loading ItemDrops For YetiBoy");
                    ObjectDB.instance.m_items.Add(Cape1);

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
            werewolf = GetAssetBundleFromResources("werewolf");
            Debug.Log("Loading Wolf1");
            WereWolf1 = werewolf.LoadAsset<GameObject>("WereWolfBlackArmored");
            Debug.Log("Loading Wolf2");
            WereWolf2 = werewolf.LoadAsset<GameObject>("WereWolfDarkGrey");
            Debug.Log("Loading Wolf3");
            WereWolf3 = werewolf.LoadAsset<GameObject>("WereWolfBlack");
            Debug.Log("Loading Wolf4");
            WereWolf4 = werewolf.LoadAsset<GameObject>("WereWolfBrown");
            Debug.Log("Loading Wolf5");
            WereWolf5 = werewolf.LoadAsset<GameObject>("WereWolfWhite");
            Debug.Log("Loading Cape 1");
            Cape1 = werewolf.LoadAsset<GameObject>("WerewolfCape");
            
            werewolf?.Unload(false);
            
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