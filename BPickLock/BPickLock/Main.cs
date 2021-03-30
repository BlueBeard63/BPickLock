using BPickLock.Modules;
using BPickLock.Storage;
using BPickLock.Utils;
using Rocket.Core;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BPickLock
{

    public class DoorBlackList
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
    public class Main : RocketPlugin<Config>
    {
        public DataStorage<List<DoorBlackList>> DoorBlackList { get; private set; }
        private List<DoorBlackList> doorData;
        public static Main Instance;

        protected override void Load()
        {
            DoorBlackList = new DataStorage<List<DoorBlackList>>(Directory, "PicklockBlacklist.json");
            Instance = this;
            Rocket.Core.Logging.Logger.Log("Cave Networks Staff Plugin >>>>>> Has Been Loaded!", ConsoleColor.DarkMagenta);
            Rocket.Core.Logging.Logger.Log("Cave Networks Staff Plugin >>>>>> Made By: BlueBeard", ConsoleColor.DarkMagenta);
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            ReloadPlayerData();
        }
        #region JsonData For no picklock
        public void ReloadPlayerData()
        {
            doorData = DoorBlackList.Read();
            if (doorData == null)
            {
                doorData = new List<DoorBlackList>();
            }
        }
        public bool GetDoorBlackListed(float px, float py, float pz)
        {
            return doorData.Exists(x => x.x == px && x.y == py && x.z == pz);
        }
        public void AddPlayer(DoorBlackList blacklist)
        {
            doorData.Add(blacklist);
            DoorBlackList.Save(doorData);
        }
        public void RemovePlayer(DoorBlackList blacklist)
        {
            doorData.RemoveAll(x => x.x == blacklist.x && x.y == blacklist.y && x.z == blacklist.z);
            DoorBlackList.Save(doorData);
        }

        #endregion JsonData For no picklock
        #region PlayerConnect / Disconnect
        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            player.Player.equipment.onEquipRequested -= OnEquiped;
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            player.Player.equipment.onEquipRequested += OnEquiped;

        }
        # endregion PlayerConnect / Disconnect
        #region PickLocking

        private void OnEquiped(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            var ePlayer = equipment.player;
            var uPlayer = UnturnedPlayer.FromPlayer(ePlayer);
            var Id = jar.item.id;
            Configuration.Instance.PickLock_Config.ForEach(w => PickLocks(w.Webhook, w.Rules, w.Color, w.PickLockBreak, uPlayer, ePlayer, Id, jar));
        }

        private void PickLocks(string webhook, List<PickLockInfo> rules, string color, int pickLockBreak, UnturnedPlayer uPlayer, Player ePlayer, ushort item_id, ItemJar jar)
        {
            var permissions = R.Instance.GetComponent<RocketPermissionsManager>();
            if (permissions.HasPermission(uPlayer, new List<string>() { "Picklock" }))
            {
                foreach (var rule in rules)
                {
                    var _pickTime = rule.Time;
                    if (rule.PickLockID == item_id)
                    {
                        if (PhysicsUtility.raycast(new Ray(ePlayer.look.aim.position, ePlayer.look.aim.forward), out RaycastHit hit, Mathf.Infinity, RayMasks.BARRICADE_INTERACT))
                        {
                            InteractableDoorHinge hinge = hit.transform.GetComponent<InteractableDoorHinge>();
                            Interactable2SalvageBarricade barri = hit.transform.GetComponent<Interactable2SalvageBarricade>();
                            if (hinge != null)
                            {
                                InteractableDoor door = hinge.door;
                                bool open = !door.isOpen;

                                BarricadeManager.tryGetInfo(door.transform, out byte x, out byte y, out ushort index, out ushort bindex, out BarricadeRegion barricadeR);
                                var BIndex = barricadeR.barricades[bindex];
                                var BId = BIndex.barricade.id;

                                var Bx = BIndex.point.x;
                                var By = BIndex.point.y;
                                var Bz = BIndex.point.z;
                                if (BId == rule.ID)
                                {
                                    if (!GetDoorBlackListed(Bx, By, Bz))
                                    {
                                        if (open)
                                        {
                                            System.Random rand = new System.Random();
                                            int chance = rand.Next(1, 101);

                                            if (chance <= rule.Chance)
                                            {
                                                PickLockingDoorTimer(door, _pickTime, uPlayer);
                                                ChatManager.serverSendMessage("Please wait " + rule.Time + " seconds for the door to open!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                                DiscordMessage(Color.green, uPlayer, BIndex.barricade.asset.itemName, webhook);
                                            }
                                            else
                                            {
                                                ChatManager.serverSendMessage("PickLocking Failed!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                                DiscordMessage(Color.red, uPlayer, BIndex.barricade.asset.itemName, webhook);
                                                if (chance <= pickLockBreak)
                                                {
                                                    List<InventorySearch> list = uPlayer.Inventory.search(rule.PickLockID, true, true);
                                                    uPlayer.Inventory.removeItem(list[0].page, uPlayer.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                                    list.RemoveAt(0);
                                                    ChatManager.serverSendMessage("LockPick has been broken!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                                }
                                            }
                                        }
                                        else // If door is open!
                                        {
                                            ChatManager.serverSendMessage("You cannot picklock a door that is already open!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                        }
                                    }
                                    else // If door is blacklisted
                                    {
                                        ChatManager.serverSendMessage("You cannot picklock this door, as it is blacklisted!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                    }
                                }
                            }
                            else if (barri != null)
                            {
                                BarricadeManager.tryGetInfo(hit.transform, out byte x, out byte y, out ushort index, out ushort bindex, out BarricadeRegion barricadeR);
                                var BIndex = barricadeR.barricades[bindex];
                                var BId = BIndex.barricade.id;

                                var Bx = BIndex.point.x;
                                var By = BIndex.point.y;
                                var Bz = BIndex.point.z;
                                if (BId == rule.ID)
                                {
                                    if (!GetDoorBlackListed(Bx, By, Bz))
                                    {
                                        System.Random rand = new System.Random();
                                        int chance = rand.Next(1, 101);

                                        if (chance <= rule.Chance)
                                        {
                                            PickLockingStorageTimer(_pickTime, uPlayer);
                                            ChatManager.serverSendMessage("Please wait " + rule.Time + " seconds for the stroage to open!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                            DiscordMessage(Color.green, uPlayer, BIndex.barricade.asset.itemName, webhook);
                                        }
                                        else
                                        {
                                            ChatManager.serverSendMessage("PickLocking Failed!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                            DiscordMessage(Color.red, uPlayer, BIndex.barricade.asset.itemName, webhook);
                                            if (chance <= pickLockBreak)
                                            {
                                                List<InventorySearch> list = uPlayer.Inventory.search(rule.PickLockID, true, true);
                                                uPlayer.Inventory.removeItem(list[0].page, uPlayer.Inventory.getIndex(list[0].page, list[0].jar.x, list[0].jar.y));
                                                list.RemoveAt(0);
                                                ChatManager.serverSendMessage("LockPick has been broken!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                            }
                                        }
                                    }
                                    else // If door is blacklisted
                                    {
                                        ChatManager.serverSendMessage("You cannot picklock this stroage, as it is blacklisted!", Color.red, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void PickLockingDoorTimer(InteractableDoor door, float _pickTime, UnturnedPlayer uPlayer) => StartCoroutine(_Timer1(door, _pickTime, uPlayer));
        public void PickLockingStorageTimer(float _pickTime, UnturnedPlayer uPlayer) => StartCoroutine(_Timer(_pickTime, uPlayer));

        private IEnumerator _Timer(float timming, UnturnedPlayer uPlayer)
        {
            yield return new WaitForSeconds(timming);
            var storage = GetInteractableStorage(uPlayer);
            uPlayer.Player.inventory.storage = storage;
            uPlayer.Inventory.updateItems(PlayerInventory.STORAGE, storage.items);
            uPlayer.Player.inventory.sendStorage();
            uPlayer.Player.inventory.isStoring = true;
            ChatManager.serverSendMessage("You have Successfully picklocked the storage!", Color.cyan, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
        }
        private IEnumerator _Timer1(InteractableDoor door, float timming, UnturnedPlayer uPlayer)
        {
            yield return new WaitForSeconds(timming);
            DoorUtil.ToggleDoor(door, true);
            ChatManager.serverSendMessage("You have Successfully picklocked the door!", Color.cyan, null, uPlayer.SteamPlayer(), EChatMode.GLOBAL, Configuration.Instance.LogoImage, true);
        }
        private InteractableStorage GetInteractableStorage(UnturnedPlayer caller)
        {
            var look = caller.Player.look;
            if (PhysicsUtility.raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, Mathf.Infinity, RayMasks.BARRICADE_INTERACT))
            {
                var storage = hit.transform.GetComponent<InteractableStorage>();
                if (storage != null)
                {
                    return storage;
                }
            }

            return null;
        }
        #endregion PickLocking
        public void DiscordMessage(UnityEngine.Color color, UnturnedPlayer uPlayer, string ds, string webhook)
        {
            var n_webmessage = new WebhookMessage()
                 .PassEmbed()
                    .WithColor(color)
                    .WithField("Player: ", uPlayer.DisplayName, true)
                    .WithField("Door/Storage: ", ds, true)
                    .Finalize();

            DiscordSender.PostMessage(webhook, n_webmessage);
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
        }

    }
}
