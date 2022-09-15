using ENet.Managed;
using GTProxy.Wrapper;
using System.Text;

namespace GTBot.Common;

public class Bot
{
    public void JoinWorld(string name) => SendPacket(3, "action|join_request\nname|" + name + "\ninvitedWorld|0");

    public Client client { get; private set; }

    public ENetPeer peer { get; private set; }

    public string tankIDName { get; private set; }
    public string tankIDPass { get; private set; }

    private string meta = "";

    private string? ip;
    private short port;

    public Bot(string tin, string tip)
    {
        tankIDName = tin;
        tankIDPass = tip;

        if (ENetWrapper.enet_initialize(AppDomain.CurrentDomain.BaseDirectory + "enet.dll") != 0)
            Console.WriteLine("Failed to initialize enet!");

        hash2 = 19999999 + rand.Next() % 65535;
        hash = 39999999 + rand.Next() % 65535;
        mac = Utils.CreateMAC();
        rid = Utils.CreateRID();
        wk = Utils.CreateWinKey();

        //Parse server ip and port
        var parse = new Parse(Utils.request_server_data().Result);
        this.ip = parse.get("server").Trim();
        this.port = short.Parse(parse.get("port").Trim());
        
    }

    public void connect() => connect(ip, port);

    public void connect(string host, short port)
    {
        Console.WriteLine($"Connecting to {host}:{port}");

        client = new Client();

        client.on_connect += Client_on_connect;
        client.on_receive += Client_on_receive;
        client.on_disconnect += Client_on_disconnect;

        client.start_service();
        client.connect(host, port);
    }

    private void Client_on_connect(ENetPeer peer)
    {
        Console.WriteLine("ENet connected to server");
        peer.Timeout(1000, 4000, 6000);
        this.peer = peer;
    }

    private void Client_on_receive(object sender, ENetPacket e)
    {
        var packet = e.Data.ToArray();
        var packetType = (ENET_MESSAGE_TYPE)packet[0];
#if DEBUG
        Console.WriteLine(packetType.ToString());
#endif
        switch(packetType)
        {
            case ENET_MESSAGE_TYPE.NET_MESSAGE_SERVER_HELLO:
                {
                    SendPacket(2, CreateLogon());
                    break;
                }
            case ENET_MESSAGE_TYPE.NET_MESSAGE_GAME_PACKET:
                {
                    byte[] tankPacket = Variant.get_struct_data(packet);
                    if (tankPacket == null) break;
                    byte tankPacketType = tankPacket[0];
                    ENET_PACKET_TYPE varType = (ENET_PACKET_TYPE)tankPacketType;
                    switch (varType)
                    {
                        case ENET_PACKET_TYPE.PACKET_CALL_FUNCTION:
                            {
                                Variant.variant_t VarListFetched = Variant.GetCall(Variant.get_extended_data(tankPacket));
                                VarListFetched.netid = BitConverter.ToInt32(tankPacket, 4);
                                VarListFetched.delay = BitConverter.ToUInt32(tankPacket, 20);
                                switch (VarListFetched.FunctionName)
                                {
                                    case "OnSuperMainStartAcceptLogonHrdxs47254722215a":
                                        {
                                            SendPacket(2, "action|enter_game");
                                            break;
                                        }
                                    case "OnSendToServer":
                                        {
                                            Console.WriteLine("Bot changing servers");
                                            string ip = (string)VarListFetched.functionArgs[4];
                                            string[] arr = ip.Split('|');
                                            ip = arr[0];
                                            doorID = arr[1];
                                            uuid_token = arr[2];
                                            int port = (int)VarListFetched.functionArgs[1];
                                            int token = (int)VarListFetched.functionArgs[2];
                                            int user = (int)VarListFetched.functionArgs[3];
                                            userID = user;
                                            this.token = token;
                                            connect(ip.Trim(), (short)port);
                                            break;
                                        }
                                    case "OnRequestWorldSelectMenu":
                                        {
                                            Console.WriteLine("Bot joining world anarchy1love");
                                            JoinWorld("anarchy1love");
                                            break;
                                        }
                                    case "onShowCaptcha":
                                        {
                                            Console.Write("CAPTHA");
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                    break;
                }
            case ENET_MESSAGE_TYPE.NET_MESSAGE_TRACK:
                {
                    string track_message = Encoding.ASCII.GetString(packet.Skip(4).ToArray());
                    var parse = new Parse(track_message);
                    string err = parse.get("Authentication_error");
                    if (!string.IsNullOrEmpty(err) && err != "0") connect();
                    break;
                }
        }
    }

    private string CreateLogon()
    {
        var x = new Parse(Utils.request_server_data().Result);
        meta = x.get("meta");
        Random rand = new Random();
        var parse = new Parse();
        parse.append("tankIDName", tankIDName);
        parse.append("tankIDPass", tankIDPass);
        parse.append("requestedName", "CrazyFrog");
        parse.append("f", "0");
        parse.append("protocol", "164");
        parse.append("game_version", "3.99");
        parse.append("fz", "16137536");
        parse.append("lmode", "0");
        parse.append("cbits", "0");
        parse.append("player_age", "22");
        parse.append("GDPR", "1");
        parse.append("category", "_0");
        parse.append("totalPlaytime", "0");
        parse.append("hash2", hash2.ToString());
        parse.append("meta", meta);
        parse.append("fhash", "-716928004");
        parse.append("rid", rid);
        parse.append("platformID", "0,1,1");
        parse.append("deviceVersion", "0");
        parse.append("country", "fi");
        parse.append("hash", hash.ToString());
        parse.append("mac", mac);
        if (userID != 0)
        {
            parse.append("user", userID.ToString());
            parse.append("token", token.ToString());
            parse.append("UUIDToken", uuid_token);
            parse.append("doorID", doorID);
            parse.set("lmode", "1");
        }
        parse.append("wk", wk);
        parse.append("zf", (-19999999 - rand.Next() % 65535).ToString());
        return parse.raw();
    }


    public string? rid { get; set; }

    public string? mac { get; set; }

    public int userID = 0;
    public int platformID = 0, token = 0;
    public string? doorID = "", uuid_token;

    public int hash, hash2;
    public string? wk;

    private Random rand = new Random();

    //Disconnect is ignored for now.
    private void Client_on_disconnect(object sender, uint e) { }

    public void SendData(byte[] data, ENetPacketFlags flag = ENetPacketFlags.Reliable)
        => peer.Send((byte)rand.Next(0, 1), data, flag);

    public void SendPacketRaw(int type, byte[] data, ENetPacketFlags flag = ENetPacketFlags.Reliable)
    {
        var packetData = new byte[data.Length + 5];
        Array.Copy(BitConverter.GetBytes(type), packetData, 4);
        Array.Copy(data, 0, packetData, 4, data.Length);
        SendData(packetData);
    }

    public void SendPacket(int type, string str, ENetPacketFlags flag = ENetPacketFlags.Reliable)
        => SendPacketRaw(type, Encoding.ASCII.GetBytes(str.ToCharArray()));
}
