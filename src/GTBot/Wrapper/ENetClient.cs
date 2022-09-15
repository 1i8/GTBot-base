using System;
using System.Threading;
using ENet.Managed;
using System.Net;

namespace GTProxy.Wrapper;

public class Client
{
    public delegate void Connect(ENetPeer peer);
    public delegate void Disconnect(object sender, uint e);
    public delegate void Receive(object sender, ENetPacket e);

    public event Connect on_connect;
    public event Disconnect on_disconnect;
    public event Receive on_receive;

    public ENetHost client;
    public ENetPeer peer;

    public bool running = false;
    public Thread service_thread;

    /// <summary>
    /// Initialize new enet client with new packet system to work with growtopia server
    /// </summary>
    public Client()
    {
        client = new ENetHost(null, 2, 2);
        client.ChecksumWithCRC32();
        client.CompressWithRangeCoder();
        client.UseNewPacket(1);
    }

    /// <summary>
    /// Connect client to host
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    public void connect(string host, short port)
        => client.Connect(new IPEndPoint(IPAddress.Parse(host), port), 2, 0);

    public void start_service()
    {
        running = true;
        service_thread = new Thread(service);
        service_thread.Start();
    }

    public void destroy_client()
    {
        if (client == null) return;
        foreach (var p in client.PeerList)
            if (!p.IsNull)
                p.DisconnectNow(0);
        running = false;
        service_thread = null;
        client = null;
    }

    private void service()
    {
        while (running)
        {
            if (client.Disposed) return;
            var Event = client.Service(TimeSpan.FromMilliseconds(0));

            switch (Event.Type)
            {
                case ENetEventType.None:
                    break;
                case ENetEventType.Connect:
                    on_connect(Event.Peer);
                    break;
                case ENetEventType.Disconnect:
                    on_disconnect(Event.Peer, 0);
                    Event.Peer.UnsetUserData();
                    break;
                case ENetEventType.Receive:
                    on_receive(Event.Peer, Event.Packet);
                    Event.Packet.Destroy();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Thread.Sleep(1);
        }
    }
}
