using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;
using UnityEngine;
using UnityEngine.UI;


namespace SkipMobIsles
{
    static class Main
    {
        public static bool Enabled;
        public static Settings Settings;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = Settings.Load<Settings>(modEntry);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            //enable reward options
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Enable Rewards: ", GUILayout.ExpandWidth(false));

            GUILayout.Space(80);
            Settings.enableReward_xp = GUILayout.Toggle(Settings.enableReward_xp, " XP", GUILayout.ExpandWidth(false));
            GUILayout.Space(50);
            Settings.enableReward_gold = GUILayout.Toggle(Settings.enableReward_gold, " Gold", GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();

            //enable "skip isles in Roughlike" option
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("Skip isles in roughlike campaign: ", GUILayout.ExpandWidth(false));

            GUILayout.Space(22);
            Settings.skipIslesInRoughlike = GUILayout.Toggle(Settings.skipIslesInRoughlike, " ", GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
        }
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

    }
}