using SDG.Unturned;

namespace BPickLock.Utils
{
    public static class DoorUtil
    {
        public static void ToggleDoor(InteractableDoor door, bool open)
        {
            BarricadeManager.tryGetInfo(door.transform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region);
            door.updateToggle(open);

            BarricadeManager.instance.channel.send("tellToggleDoor", ESteamCall.ALL,
                ESteamPacket.UPDATE_RELIABLE_BUFFER, x, y, plant, index, open);
        }
    }
}
