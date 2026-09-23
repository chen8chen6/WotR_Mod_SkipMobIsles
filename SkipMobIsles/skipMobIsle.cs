using HarmonyLib;
using Kingmaker;
using Kingmaker.AreaLogic.Etudes;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items;
using Kingmaker.Dungeon;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipMobIsles
{
    internal class skipMobIsle
    {
        public static Settings Settings => Main.Settings;

        [HarmonyPatch(typeof(DungeonIslandState), nameof(DungeonIslandState.MayBeVisitedNow), MethodType.Getter)]
        public static class DungeonIslandState_MayBeVisitedNow_Patch
        {
            private static void Postfix(DungeonIslandState __instance, ref bool __result)
            {
                //检测当前阶段, 自动完成剩余旅途
                if (IsT3())
                    FastTravel(Etude.T3_Island_1, Etude.T3_Island_6, T3_Reward_Gold, T3_Reward_XP, T3_Reward_Items);
                else if (IsT2())
                    FastTravel(Etude.T2_Island_1, Etude.T2_Island_7, T2_Reward_Gold, T2_Reward_XP, T2_Reward_Items);
                else if (IsT1())
                    FastTravel(Etude.T1_Island_1, Etude.T1_Island_12, T1_Reward_Gold, T1_Reward_XP, T1_Reward_Items);

                __result = __instance.IsLast;   //只可访问boss所在岛
            }
        }

        static bool IsT1()
        {
            var Etude_T1 = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(Etude.DLC3_Int_Tier_1);
            var Etude_T1_End = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(Etude.DLC3_Int_Tier_1_End);

            bool Is_T1_Started = Game.Instance.Player.EtudesSystem.EtudeIsStarted(Etude_T1)
                || Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T1);
            bool Is_T1_Finished = Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T1_End);

            return Is_T1_Started && !Is_T1_Finished;
        }

        static bool IsT2()
        {
            var Etude_T2 = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(Etude.DLC3_Int_Tier_2);
            var Etude_T2_End = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(Etude.DLC3_Int_Tier_2_End);

            bool Is_T2_Started = Game.Instance.Player.EtudesSystem.EtudeIsStarted(Etude_T2)
                || Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T2);
            bool Is_T2_Finished = Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T2_End);

            return Is_T2_Started && !Is_T2_Finished;
        }

        static bool IsT3()
        {
            var Etude_T3 = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(Etude.DLC3_Int_Tier_3);
            bool Is_T3_Started = Game.Instance.Player.EtudesSystem.EtudeIsStarted(Etude_T3)
                || Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T3);
            return Is_T3_Started;
        }

        static void MarkIslandsCompleted(string First)
        {
            var Cur_Island = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(First);
            const int LOOP_CNT_MAX = 12;    //防止意外的无限循环
            int cnt = 0;
            while (null != Cur_Island && cnt++ < LOOP_CNT_MAX)
            {
                Game.Instance.Player.EtudesSystem.MarkEtudeCompleted(Cur_Island);
                Cur_Island = Cur_Island.StartsOnComplete.Empty() ? null : Cur_Island.StartsOnComplete.First();
            }
        }

        static void AddItems(string[] itemList)
        {
            var inventory = Game.Instance.Player.Inventory;
            foreach (string item in itemList)
                inventory.Add(ResourcesLibrary.TryGetBlueprint<BlueprintItem>(item), 1);
        }

        static void FastTravel(string first, string last, int rewardGold, int rewardXp, string[] rewardItems)
        {
            var Last_Island = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(last);
            if (Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Last_Island))
                return; //已经到目的地了

            MarkIslandsCompleted(first);    //将boss以外的岛标记为completed

            //添加奖励
            if (Settings.enableReward_gold)
                Game.Instance.Player.GainMoney(rewardGold);
            
            if (Settings.enableReward_xp)
                Game.Instance.Player.GainPartyExperience(rewardXp);

            AddItems(rewardItems);
        }

        private static int T1_Reward_Gold = 564535;
        private static int T2_Reward_Gold = 1130849;
        private static int T3_Reward_Gold = 1943111;
        private static int T1_Reward_XP = 738000 / 6;
        private static int T2_Reward_XP = 3529482 / 6;
        private static int T3_Reward_XP = 10366164 / 6;
        private static string[] T1_Reward_Items = {
            Item.Evercold,
            Item.Iron_Prayer,
            Item.Boots_of_Elvenkind,
            Item.Lesser_Persistent_Metamagic_Rod,
            Item.Flaming_Seed,
            Item.Apprentice_Robe,
            Item.Kapao,
            Item.Quiver_of_Marksman_s_Aid,
            Item.Guardian_s_Battleaxe,
            Item.Quiver_of_Holy_Ammunition,
            Item.Guardian_s_Shield,
            Item.Robe_of_False_Death,
            Item.Skull_of_the_Thief_of_Names,
            Item.Bracers_of_Bravery,
            Item.Gloves_of_Dueling,
            Item.Gloves_of_Phlebotomy,
            Item.Robe_of_Earth,
            Item.Selective_Metamagic_Rod,
            Item.Quicken_Metamagic_Rod,
            Item.Hat_of_the_Bitter_End,
            Item.Coin_of_Protection,
            Item.Withered_Ruby_Fruit,
            Item.Glasses_of_Undeniable_Truth,
            Item.Wand_of_False_Life_Greater,
            Item.Robe_of_Unspeakable_Truth,
            Item.Bashing_Shield,
            Item.Cloak_of_Carnage,
            Item.Star_Rattle,
            Item.Wind_Breath,
            Item.Bloody_Meat,
            Item.Lesser_Kineticist_Rod,
            Item.Kyonin_Berry_Wine,
            Item.Mulled_Wine
        };

        private static string[] T2_Reward_Items = {
            Item.Greater_Selective_Metamagic_Rod,
            Item.Jab_Stab,
            Item.Padded_Armor_of_Firm_Steps,
            Item.Wand_of_Greater_Magic_Weapon,
            Item.Wand_of_Greater_Magic_Weapon,
            Item.Wand_of_Thundering_Drums,
            Item.Cloak_of_Cinders,
            Item.Quicken_Metamagic_Rod,
            Item.Lucky_Hand_Sandwich,
            Item.Pink_Pearl,
            Item.Maximize_Metamagic_Rod,
            Item.Greater_Persistent_Metamagic_Rod,
            Item.Greater_Quicken_Metamagic_Rod,
            Item.Robe_of_the_Seven_Sins,
            Item.Bloody_Meat,
            Item.Ring_of_Triumphant_Advance,
            Item.Shield_of_Evocation_Resilience,
            Item.Joke_of_Nature_s_Quiver,
            Item.Unholy_Symbol_of_Nahindri,
            Item.Facesmasher,
            Item.Maximize_Metamagic_Rod,
            Item.Greater_Persistent_Metamagic_Rod,
            Item.Cloth_of_Heavy_Fortification,
            Item.Splintershred,
            Item.Priest_s_Circlet,
            Item.Rod_of_Magical_Affinity
        };

        private static string[] T3_Reward_Items = {
            Item.Snakeskin,
            Item.Radiant_Feather,
            Item.Maximize_Metamagic_Rod,
            Item.Maximize_Metamagic_Rod,
            Item.Greater_Persistent_Metamagic_Rod,
            Item.Bonesnatcher,
            Item.Ornate_Key,
            Item.Amulet_of_Combat_Awareness,
            Item.Maximize_Metamagic_Rod,
            Item.Greater_Persistent_Metamagic_Rod,
            Item.Greater_Selective_Metamagic_Rod,
            Item.Broken_Diadem,
            Item.Offroad_Boots,
            Item.Bracers_of_Archery,
            Item.Beacon_of_Innocence,
            Item.Guardian_s_Shield,
            Item.Greater_Maximize_Metamagic_Rod,
            Item.Cloth_of_Heavy_Fortification,
            Item.Cloth_of_Heavy_Fortification,
            Item.Ring_of_Shadow_Stealing,
        };
    }
}