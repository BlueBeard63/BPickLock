using Rocket.API;
using System.Collections.Generic;

namespace BPickLock
{
    public class Config : IRocketPluginConfiguration
    {
        public List<PickLock> PickLock_Config;
        public string LogoImage;
        public bool AutoCloseDoors;
        public int AutoCloseDoorsDelay;
        public float Cooldown;
        public void LoadDefaults()
        {
            PickLock_Config = new List<PickLock>
            {
                new PickLock
                { Webhook = "Webhook Here!", PickLockBreak = 50,
                    Rules = new List<PickLockInfo>
                    {
                    new PickLockInfo { ID = 281, PickLockID = 1353, Time = 10, Chance = 25},
                    new PickLockInfo { ID = 282, PickLockID = 1353, Time = 10, Chance = 25},
                    new PickLockInfo { ID = 328, PickLockID = 1353, Time = 10, Chance = 25}
                    }
                }
            };
            Cooldown = 300;
            AutoCloseDoors = false;
            AutoCloseDoorsDelay = 2500;
            LogoImage = "https://i.imgur.com/lv4E8TR.jpg";
        }
    }
    public class PickLock
    {
        public string Webhook;
        public string Color;
        public int PickLockBreak;
        public List<PickLockInfo> Rules;
    }
    public class PickLockInfo
    {
        public ushort ID;
        public ushort PickLockID;
        public float Time;
        public int Chance;
    }
}
