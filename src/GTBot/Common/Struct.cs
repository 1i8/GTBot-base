namespace GTBot.Common;

public enum ENET_MESSAGE_TYPE
{
    NET_MESSAGE_UNKNOWN = 0,
    NET_MESSAGE_SERVER_HELLO,
    NET_MESSAGE_GENERIC_TEXT,
    NET_MESSAGE_GAME_MESSAGE,
    NET_MESSAGE_GAME_PACKET,
    NET_MESSAGE_ERROR,
    NET_MESSAGE_TRACK,
    NET_MESSAGE_CLIENT_LOG_REQUEST,
    NET_MESSAGE_CLIENT_LOG_RESPONSE,
    NET_MESSAGE_MAX
};

public enum ENET_PACKET_TYPE
{
    PACKET_STATE = 0,
    PACKET_CALL_FUNCTION,
    PACKET_UPDATE_STATUS,
    PACKET_TILE_CHANGE_REQUEST,
    PACKET_SEND_MAP_DATA,
    PACKET_SEND_TILE_UPDATE_DATA,
    PACKET_SEND_TILE_UPDATE_DATA_MULTIPLE,
    PACKET_TILE_ACTIVATE_REQUEST,
    PACKET_TILE_APPLY_DAMAGE,
    PACKET_SEND_INVENTORY_STATE,
    PACKET_ITEM_ACTIVATE_REQUEST,
    PACKET_ITEM_ACTIVATE_OBJECT_REQUEST,
    PACKET_SEND_TILE_TREE_STATE,
    PACKET_MODIFY_ITEM_INVENTORY,
    PACKET_ITEM_CHANGE_OBJECT,
    PACKET_SEND_LOCK,
    PACKET_SEND_ITEM_DATABASE_DATA,
    PACKET_SEND_PARTICLE_EFFECT,
    PACKET_SET_ICON_STATE,
    PACKET_ITEM_EFFECT,
    PACKET_SET_CHARACTER_STATE,
    PACKET_PING_REPLY,
    PACKET_PING_REQUEST,
    PACKET_GOT_PUNCHED,
    PACKET_APP_CHECK_RESPONSE,
    PACKET_APP_INTEGRITY_FAIL,
    PACKET_DISCONNECT,
    PACKET_BATTLE_JOIN,
    PACKET_BATTLE_EVENT,
    PACKET_USE_DOOR,
    PACKET_SEND_PARENTAL,
    PACKET_GONE_FISHIN,
    PACKET_STEAM,
    PACKET_PET_BATTLE,
    PACKET_NPC,
    PACKET_SPECIAL,
    PACKET_SEND_PARTICLE_EFFECT_V2,
    PACKET_ACTIVE_ARROW_TO_ITEM,
    PACKET_SELECT_TILE_INDEX,
    PACKET_SEND_PLAYER_TRIBUTE_DATA,
    PACKET_PVE_UNK1,
    PACKET_PVE_UNK2,
    PACKET_PVE_UNK3,
    PACKET_PVE_UNK4,
    PACKET_PVE_UNK5,
    PACKET_SET_EXTRA_MODS,
    PACKET_ON_STEP_ON_TILE_MOD,
    PACKET_MAX
};



public class GameUpdatePacket
{
    public int type;
    public int obj;
    public int count1;
    public int count2;
    public int netid;
    public int item;
    public int flags_mask;
    public List<byte> flags = new List<byte>();
    public float float_var;
    public int int_data;
    public float vec_x;
    public float vec_y;
    public float vec2_x;
    public float vec2_y;
    public float particle_rotation;
    public int int_x;
    public int int_y;
    public int data_size => flags.Count;

    public byte[] PackForSendingRaw()
    {
        var b = new byte[57 + data_size];
        Array.Copy(BitConverter.GetBytes(type), b, 4);
        Array.Copy(BitConverter.GetBytes(obj), 0, b, 1, 4);
        Array.Copy(BitConverter.GetBytes(count1), 0, b, 2, 4);
        Array.Copy(BitConverter.GetBytes(count2), 0, b, 3, 4);
        Array.Copy(BitConverter.GetBytes(netid), 0, b, 4, 4);
        Array.Copy(BitConverter.GetBytes(item), 0, b, 8, 4);
        Array.Copy(BitConverter.GetBytes(flags_mask), 0, b, 12, 4);
        Array.Copy(BitConverter.GetBytes(float_var), 0, b, 16, 4);
        Array.Copy(BitConverter.GetBytes(int_data), 0, b, 20, 4);
        Array.Copy(BitConverter.GetBytes(vec_x), 0, b, 24, 4);
        Array.Copy(BitConverter.GetBytes(vec_y), 0, b, 28, 4);
        Array.Copy(BitConverter.GetBytes(vec2_x), 0, b, 32, 4);
        Array.Copy(BitConverter.GetBytes(vec2_y), 0, b, 36, 4);
        Array.Copy(BitConverter.GetBytes(particle_rotation), 0, b, 40, 4);
        Array.Copy(BitConverter.GetBytes(int_x), 0, b, 44, 4);
        Array.Copy(BitConverter.GetBytes(int_y), 0, b, 48, 4);
        Array.Copy(BitConverter.GetBytes(data_size), 0, b, 52, 4);
        var dat = flags.ToArray();
        var datLength = dat.Length;
        if (datLength > 0) Buffer.BlockCopy(dat, 0, b, 56, datLength);
        return b;
    }

    public byte[] PackForSendingAsPacket()
    {
        var m = PackForSendingRaw();
        var s = new byte[m.Length + 4];
        Array.Copy(m, 0, s, 4, m.Length);
        return s;
    }

    public static GameUpdatePacket Unpack(byte[] data)
    {
        var dataStruct = new GameUpdatePacket();
        dataStruct.type = BitConverter.ToInt32(data, 0);
        dataStruct.obj = BitConverter.ToInt32(data, 1);
        dataStruct.count1 = BitConverter.ToInt32(data, 2);
        dataStruct.count2 = BitConverter.ToInt32(data, 3);
        dataStruct.netid = BitConverter.ToInt32(data, 4);
        dataStruct.item = BitConverter.ToInt32(data, 8);
        dataStruct.flags_mask = BitConverter.ToInt32(data, 12);
        dataStruct.float_var = BitConverter.ToInt32(data, 16);
        dataStruct.int_data = BitConverter.ToInt32(data, 20);
        dataStruct.vec_x = BitConverter.ToSingle(data, 24);
        dataStruct.vec_y = BitConverter.ToSingle(data, 28);
        dataStruct.vec2_x = BitConverter.ToSingle(data, 32);
        dataStruct.vec2_y = BitConverter.ToSingle(data, 36);
        dataStruct.particle_rotation = BitConverter.ToInt32(data, 40);
        dataStruct.int_x = BitConverter.ToInt32(data, 44);
        dataStruct.int_y = BitConverter.ToInt32(data, 48);
        return dataStruct;
    }

    public static GameUpdatePacket UnpackFromPacket(byte[] p)
    {
        var packet = new GameUpdatePacket();
        if (p.Length >= 48)
        {
            var s = new byte[p.Length - 4];
            Array.Copy(p, 4, s, 0, s.Length);
            packet = Unpack(s);
        }

        return packet;
    }
}