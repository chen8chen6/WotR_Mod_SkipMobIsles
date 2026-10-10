using UnityModManagerNet;

namespace SkipMobIsles
{
    public class Settings : UnityModManager.ModSettings
    {
        public bool enableReward_xp = true;
        public bool enableReward_gold = true;
        public bool skipIslesInRoughlike = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
