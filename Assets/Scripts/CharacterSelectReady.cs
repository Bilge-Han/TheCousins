using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    
    private Dictionary<ulong, bool> playerReadyDictionary;
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    public void SetPlayerReady() { SetPlayerReadyServerRpc(); }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clienId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            if (!playerReadyDictionary.ContainsKey(clienId) || !playerReadyDictionary[clienId])
            {
                // Oyuncu Hazýr Deðil.
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady) { Loader.LoadNetwork(Loader.Scene.GameScene);  }
    }
    [ClientRpc]
    private void SetPlayerClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
    

}
