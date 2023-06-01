using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStatechanged;
    [SerializeField] private KitchenObjectSO potatoSlicedSO;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    public enum State
    {
        Idle,
        Start,
        Frying,
        Fried,
        Burned,
    }
    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;
    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            //Containerda obje var
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Start:
                    state = State.Frying;
                    OnStatechanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        //Kýzardý
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = burningRecipeSOArray[0];
                        OnStatechanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        }); ;
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = burningTimer / burningRecipeSO.burningTimerMax });
                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        //Yandý
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;
                        OnStatechanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //Containerda obje yok
            if (player.HasKitchenObject())
            {
                //Oyuncu elinde obje var
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //Oyuncu elinde kýzarabilecek bir þey taþýyor.
                    player.GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(potatoSlicedSO, this);
                    fryingRecipeSO = fryingRecipeSOArray[0];

                    state = State.Start;
                    fryingTimer = 0f;
                    OnStatechanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax });
                }
            }
            else
            {
                //Oyuncunun elinde biþey yok
            }
        }
        else
        {
            //Containerda obje var
            if (player.HasKitchenObject())
            {
                //Oyuncunun elinde obje var
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Oyuncunun elindeki obje -TABAK
                    plateKitchenObject = player.GetKitchenObject() as PlateKitchenObject;
                    if (plateKitchenObject.TryAddIngredient(fryingRecipeSO.output))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStatechanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                    }
                }
            }
            else
            {
                //Oyuncunun elinde biþey yok
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStatechanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            }
        }
    }
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }
    private KitchenObjectSO GetOutpurForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }
    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

}
