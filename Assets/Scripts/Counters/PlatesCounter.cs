using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax=4;
    private void Update() {
        if (!IsServer) { return; }

        spawnPlateTimer += Time.deltaTime;

        if (KitchenGameManager.Instance.IsGamePlaying()&& spawnPlateTimer>spawnPlateTimerMax){
            spawnPlateTimer = 0f;
            //KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, this);
            if (KitchenGameManager.Instance.IsGamePlaying()&&platesSpawnedAmount<platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            }
        }
    }
    [ServerRpc]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player)
    {
       
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount>0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        platesSpawnedAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
