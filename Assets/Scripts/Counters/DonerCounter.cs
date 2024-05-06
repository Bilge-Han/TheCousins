using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DonerCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;

    public event EventHandler OnDonerSpawned;
    public event EventHandler OnDonerRemoved;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private BurningRecipeSO burningRecipeSO;
    private int cuttingProgressFried;
    private int porsiyonDonerFried;

    private int donersSpawnedAmount;
    private int donersSpawnedAmountMax = 1;

    private List<KitchenObject> donerPorsiyonKitchenObjectList;
    private void Start()
    {
        //KitchenObject.SpawnKitchenObject(cuttingRecipeSOArray[0].input, this);
    }
    public override void Interact(Player player)
    {
        #region Doner Alma
        CuttingRecipeSO cuttingRecipeSO = cuttingRecipeSOArray[0];
        if (!player.HasKitchenObject())
        {
            //if (donersSpawnedAmount > 0)
            //{
            //    donersSpawnedAmount--;
            //    porsiyonDonerFried--;
            //    KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, player);
            //    OnDonerRemoved?.Invoke(this, EventArgs.Empty);
            //}
        }
        if (HasKitchenObject())
        {
            //There is a KitchenObject here
            if (player.HasKitchenObject())
            {
                //Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject1))
                {
                    //Player is holding a plate
                    PlateKitchenObject plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;
                    if (plateKitchenObject.TryAddIngredient(cuttingRecipeSO.output))
                    {
                        //KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        InteractLogicPlaceObjectOnCounterServerRpc();
                    }
                }
            }
            #endregion
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc() { InteractLogicPlaceObjectOnCounterClientRpc(); }
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        donersSpawnedAmount--;
        porsiyonDonerFried--;
        //KitchenObject removeKitchenObject = donerPorsiyonKitchenObjectList[donerPorsiyonKitchenObjectList.Count - 1];
        KitchenObject.DestroyKitchenObject(GetKitchenObject());
        //OnDonerRemoved?.Invoke(this, EventArgs.Empty);
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
    }
    public override void InteractAlternate(Player player)
    {
        #region Doner Spawn Control
        if (!player.HasKitchenObject() && porsiyonDonerFried < 3)
        {
            //CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            CutObjectServerRpc();
            TestCuttingProgressServerRpc();
        }
        #endregion
    }
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgressFried++;
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        CuttingRecipeSO cuttingRecipeSO = cuttingRecipeSOArray[0];
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgressFried / cuttingRecipeSO.cuttingProgressMax
        });
    }
    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = cuttingRecipeSOArray[0];
        if (cuttingProgressFried >= cuttingRecipeSO.cuttingProgressMax)
        {
            //KitchenObjectSO outputKitchenObjectSO = GetOutpurForInput(GetKitchenObject().GetKitchenObjectSO());
            if (donersSpawnedAmount < donersSpawnedAmountMax)
            {
                KitchenObjectSO outputKitchenObjectSO = cuttingRecipeSOArray[0].output;
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                cuttingProgressFried = 0;

                donersSpawnedAmount++;
                //donerPorsiyonKitchenObjectList.Add(GetKitchenObject());
                //OnDonerSpawned?.Invoke(this, EventArgs.Empty);
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = (float)cuttingProgressFried / cuttingRecipeSO.cuttingProgressMax
                });
            }
            porsiyonDonerFried++;
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }
    private KitchenObjectSO GetOutpurForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
