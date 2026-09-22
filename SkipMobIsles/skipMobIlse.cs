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
    internal class skipMobIlse
    {
        [HarmonyPatch(typeof(DungeonIslandState), nameof(DungeonIslandState.MayBeVisitedNow), MethodType.Getter)]
        public static class DungeonIslandState_MayBeVisitedNow_Patch
        {
            private static void Postfix(DungeonIslandState __instance, ref bool __result)
            {
                //检测当前阶段, 自动完成剩余旅途
                if (IsT3())
                    FastTravel(T3_Island_1, T3_Island_6, T3_Reward_Gold, T3_Reward_XP, T3_Reward_Items);
                else if (IsT2())
                    FastTravel(T2_Island_1, T2_Island_7, T2_Reward_Gold, T2_Reward_XP, T2_Reward_Items);
                else if (IsT1())
                    FastTravel(T1_Island_1, T1_Island_12, T1_Reward_Gold, T1_Reward_XP, T1_Reward_Items);

                __result = __instance.IsLast;   //只可访问boss所在岛
            }
        }

        static bool IsT1()
        {
            var Etude_T1 = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(DLC3_Int_Tier_1);
            var Etude_T1_End = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(DLC3_Int_Tier_1_End);

            bool Is_T1_Started = Game.Instance.Player.EtudesSystem.EtudeIsStarted(Etude_T1)
                || Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T1);
            bool Is_T1_Finished = Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T1_End);

            return Is_T1_Started && !Is_T1_Finished;
        }

        static bool IsT2()
        {
            var Etude_T2 = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(DLC3_Int_Tier_2);
            var Etude_T2_End = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(DLC3_Int_Tier_2_End);

            bool Is_T2_Started = Game.Instance.Player.EtudesSystem.EtudeIsStarted(Etude_T2)
                || Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T2);
            bool Is_T2_Finished = Game.Instance.Player.EtudesSystem.EtudeIsCompleted(Etude_T2_End);

            return Is_T2_Started && !Is_T2_Finished;
        }

        static bool IsT3()
        {
            var Etude_T3 = ResourcesLibrary.TryGetBlueprint<BlueprintEtude>(DLC3_Int_Tier_3);
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
            Game.Instance.Player.GainMoney(rewardGold);
            Game.Instance.Player.GainPartyExperience(rewardXp);
            AddItems(rewardItems);
        }

        private static string DLC3_Int_Tier_1 = "90f94e62903546a6858c1002144a73e4";
        private static string DLC3_Int_Tier_1_End = "0fdfd84107a947b68af1f52a8ef4187d";
        private static string T1_Island_1 = "2d0e66430fd74262b052222abcec5aad";
        private static string T1_Island_12 = "ce4ce2907b774e03a81b5f96836f8683";
        private static string DLC3_Int_Tier_2 = "0912638e3ec84ad58621fc36e5f60556";
        private static string DLC3_Int_Tier_2_End = "9cea64c4096145b9801b042811a83158";
        private static string T2_Island_1 = "e403d06e0947401f84434839e7696cf7";
        private static string T2_Island_7 = "0688e6b6b201449ebe21fab6ff020e4f";
        private static string DLC3_Int_Tier_3 = "833a793a9b4e4fc9a540b0d51bbe40cf";
        private static string T3_Island_1 = "e79a85f5987c4f5881acb357c19b9c89";
        private static string T3_Island_6 = "d6dcb7d0e14949dcab6343d4d89e8860";

        private static int T1_Reward_Gold = 564535;
        private static int T2_Reward_Gold = 1130849;
        private static int T3_Reward_Gold = 1943111;
        private static int T1_Reward_XP = 738000 / 6;
        private static int T2_Reward_XP = 3529482 / 6;
        private static int T3_Reward_XP = 10366164 / 6;
        private static string[] T1_Reward_Items = {
            "551dcb2932443c944a6f120048c7d9f7",
            "e77292f3222c4e97851dd363b916ef32",
            "2d29d6d2674a476fb54c72e8906d7e8e",
            "7fc098d7fbf24964874e81fcb9321a57",
            "9d7e7dbc35ea4298bd51790644caa8c6",
            "defa42ed81cb4bb0ab79acf6b009cd6d",
            "cb7f21a7624e4697827698f0ba629e68",
            "638283d23c388174e9e27c7d02d543a2",
            "5a326f3961ad985448c9c7b72c82b1a7",
            "5cacbec6a8ddfae4389d82fbf75ec2c1",
            "ede78d969190d8844ba0b7b1c3ab5a0e",
            "235d859d7b7c9c240a1cec89eefb1f7a",
            "c256be263e99480da5b7cf2a1da1ef17",
            "4f56360fd24e44049d8899fe17f132d2",
            "9bb667825cf2469a830eeb0e129e1877",
            "ad3a2628bb643c140895c4cec43e6f6f",
            "44ef288ddd2f48f7912ae98fa5e94ae5",
            "289beaa0fe4bf644cb0817a7fa9e9cf2",
            "4476c31e248c4cd8b2613fb18c72e046",
            "75437f52a0f941da85eeb25e79bb8e8d",
            "8f2c8e8443ed4b3d83ad349aab2e9949",
            "2010d68d0aa44615ac3f6b6c41bc4ac3",
            "2051153463a0fde478dc4f6f82ac2d44",
            "1223ceb45ed647b44a04b44a9312328b",
            "fedef0913b7d598478918b81eed9fada",
            "73e7856926fa07c41a0200533a6eaada",
            "e174236c7bc46be44929aa2a79d2e10b",
            "f5423b398d6edd14abc1006993c49b64",
            "afd085bcd15a409d9050661a1fe590b9",
            "13153faee8714b0f8906286a6d1c1399",
            "c091dfe19cd44c6eb8ec6e3ea7e5c67c",
            "2fa3ada8abe04f0189ea2a18c025a946"
        };

        private static string[] T2_Reward_Items = {
            "340cf8ee69c5458c99ecd21daf082704",
            "843ae85d505be8441b9fbb47b04e19e0",
            "551dcb2932443c944a6f120048c7d9f7",
            "6a20e0aef61e4e29990b4c805cc0baa6",
            "9a511d3b04f08944eb3db4462f88c2c0",
            "865d039c14c04344b834818687b2f218",
            "6c29087271234af1819123d31bf9f3c8",
            "55a059b32df920c4abe65b8ee8b56056",
            "51906f1108e6407cb1153cef71d77fe6",
            "b2b41b525b33406596ef4051d6232c1f",
            "a9e22202c101432fa5dc28f3a4769e4f",
            "e8798433c236481d9572cb4908dad988",
            "96f184740ee0af14884a2fde51b159ba",
            "5db66b14642642eaa1c623ed0daa5813",
            "6f7c6bcd1b404f44cbafc08e1eab072b",
            "ede78d969190d8844ba0b7b1c3ab5a0e",
            "b631f6412333463eb620b4fd0571698c",
            "36b4de7055e02ec448daeafc09b9a5f7",
            "6001f3524a3d9384d92725c87710c54c",
            "6001f3524a3d9384d92725c87710c54c", //高等魔化武器法杖确实有2根,这里不是写重复了
            "fb10284bc8a05eb4988f3c98b0d19a60",
            "7789b058cf5aabb4d9ede1f54599b984",
            "4b9a8ce578184679858fd380f3d86a55",
            "32625c7587d24f14ac13482296d37f01",
            "7dc19f7de5bd4424926b57e6fe01e9ac"
        };
        private static string[] T3_Reward_Items = {
            "d0b7d29c9bea99d4bb25f8f6a29261c5",
            "6a20e0aef61e4e29990b4c805cc0baa6",
            "297adaaaaa0d45278e5a2bc1722d89af",
            "9a511d3b04f08944eb3db4462f88c2c0",
            "6c29087271234af1819123d31bf9f3c8",
            "6c29087271234af1819123d31bf9f3c8", //重度护命衣有2件
            "f125e2f29ffd44e7acfb7619586a7e24",
            "178d9ada966a74a4eb96f80badacc01f",
            "77e8e9ceb50447bfbae19f6c9a850d76",
            "4eeecd3170cd4127a84c36e6ce77547b",
            "5db66b14642642eaa1c623ed0daa5813",
            "0e7a0b96f67660c4ca74786c187a02d2",
            "c6b28bf6b42b4650bc3363f141c89bb6",
            "0a3f89bf59cd421fa9b36d301426c1d7",
            "8646192e77ee45f4b2a90f0db752ff1a",
            "6b3443e1a15b4156ad3f194b6bbcde1f"};
    }
}
