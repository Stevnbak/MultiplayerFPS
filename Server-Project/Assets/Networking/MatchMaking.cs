using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Utils;
using Unity.VisualScripting;

public static class MatchMaking
{
    [MessageHandler((ushort) MessageIds.joinGame)]
    public static void StartMatchmaking(ushort fromClientId, Message message)
    {
        NetworkManager server = ServerManager.Singleton.GetServer(message.GetUShort());
        if(server == null)
        {
            ServerManager.Singleton.StartNewServer(fromClientId);
        } else
        {
            Message newMessage = Message.Create(MessageSendMode.Reliable, MessageIds.connectToServer);
            newMessage.AddUShort(server.Port);
            newMessage.AddUShort(server.Id);
            ServerManager.Singleton.MainServer.Send(newMessage, fromClientId);
            Debug.Log("User connected to already running match server " + server.Id);
        }
    }
}
