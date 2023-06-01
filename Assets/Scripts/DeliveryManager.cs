using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawnded;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    


    public static DeliveryManager Instance { get;private set; }
    [SerializeField] private RecipeListSO recipeListSO;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax=4;
    private int succesfullRecipesAmount;
    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }
    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer<=0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (waitingRecipeSOList.Count<waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                Debug.Log(waitingRecipeSO);
                waitingRecipeSOList.Add(waitingRecipeSO);
                OnRecipeSpawnded?.Invoke(this,EventArgs.Empty);

            }
         
        }
    }
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        bool ingredientFound;
        bool plateContentsMatchesRecipe;
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            if (waitingRecipeSO.kitchenObjectSOList.Count==plateKitchenObject.GetKitchenObjectSOList().Count)
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
                        if (plateKitchenObjectSO==recipeKitchenObjectSO)
                        {
                            //Sipariþ ve teslim ayný
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //Sipariþ ve teslim edilenler farklý
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                {
                    succesfullRecipesAmount++;
                    waitingRecipeSOList.RemoveAt(i);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        //sipariþ ve tabaktaki eþleþmedi
        //Teslim yanlýþ
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
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
