using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawnded;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;



    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int succesfullRecipesAmount;
    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        if (!IsServer) { return; }
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }
    //Server recipeSO türünü tanýmadýðý için parametre olarak gönderemiyoruz, onun yerine index'ini
    // gönderip clienrpc içerisinde recipeSO türünü alýyoruz. Burda client sipariþ alacak o sipariþler host
    // gözükecek.
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawnded?.Invoke(this, EventArgs.Empty);
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        bool ingredientFound;
        bool plateContentsMatchesRecipe;
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //Menü ve tabaktaki ayný sayýda elemente sahip
                plateContentsMatchesRecipe = true;

                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    ingredientFound = false;
                    //menü içindekiler
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //teslim edilen tabak içindekiler
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            //Sipariþ ve teslim ayný
                            //KitchenGameManager.Instance.AddGamePlayingTime();
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //Sipariþ ve teslim edilenler farklý
                        //KitchenGameManager.Instance.SubtractGamePlayingTime();
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                {
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        //sipariþ ve tabaktaki eþleþmedi
        //Teslim yanlýþ
        DeliverIncorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void DeliverIncorrectRecipeServerRpc() { 
        DeliverIncorrectRecipeClientRpc(); }
    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex) {
        succesfullRecipesAmount++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    }
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }
    public int GetSuccesfullRecipesAmount()
    {
        return succesfullRecipesAmount;
    }
}
