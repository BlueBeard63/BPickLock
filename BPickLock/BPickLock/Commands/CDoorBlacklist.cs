using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace BPickLock.Commands
{
    class CDoorBlacklist : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "rdoor";

        public string Help => string.Empty;

        public string Syntax => "do /rdoor <add/remove> while looking at it to add/remove picklock access!";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "Picklock.doorblacklist" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var uPlayer = caller as UnturnedPlayer;
            if (command.Length != 1)
            {
                ChatManager.serverSendMessage(Syntax, Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Main.Instance.Configuration.Instance.LogoImage, true);
            }
            else
            {
                if (command[0] == "add")
                {
                    if (PhysicsUtility.raycast(new Ray(uPlayer.Player.look.aim.position, uPlayer.Player.look.aim.forward), out RaycastHit ahit, Mathf.Infinity, RayMasks.BARRICADE_INTERACT))
                    {
                        BarricadeManager.tryGetInfo(ahit.transform, out byte x, out byte y, out ushort index, out ushort bindex, out BarricadeRegion barricadeR);

                        var Bindex = barricadeR.barricades[bindex];
                        var Bx = Bindex.point.x;
                        var By = Bindex.point.y;
                        var Bz = Bindex.point.z;
                        var n_blacklist = new DoorBlackList
                        {
                            x = Bx,
                            y = By,
                            z = Bz
                        };
                        Main.Instance.AddPlayer(n_blacklist);
                        ChatManager.serverSendMessage("You have successfully added this door/storage to the blacklist!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Main.Instance.Configuration.Instance.LogoImage, true);
                    }
                    else if (command[0] == "remove")
                    {
                        if (PhysicsUtility.raycast(new Ray(uPlayer.Player.look.aim.position, uPlayer.Player.look.aim.forward), out RaycastHit rhit, Mathf.Infinity, RayMasks.BARRICADE_INTERACT))
                        {
                            BarricadeManager.tryGetInfo(rhit.transform, out byte x, out byte y, out ushort index, out ushort bindex, out BarricadeRegion barricadeR);

                            var Bindex = barricadeR.barricades[bindex];
                            var Bx = Bindex.point.x;
                            var By = Bindex.point.y;
                            var Bz = Bindex.point.z;
                            var n_blacklist = new DoorBlackList
                            {
                                x = Bx,
                                y = By,
                                z = Bz
                            };
                            Main.Instance.RemovePlayer(n_blacklist);
                            ChatManager.serverSendMessage("You have successfully removed this door/storage from the blacklist!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Main.Instance.Configuration.Instance.LogoImage, true);
                        }
                        else
                        {
                            ChatManager.serverSendMessage("Please look at a barricade to remove it to the blacklist for picklocking!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Main.Instance.Configuration.Instance.LogoImage, true);
                        }
                    }
                }
            }
        }
    }
}
