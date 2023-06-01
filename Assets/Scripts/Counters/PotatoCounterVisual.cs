using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoCounterVisual : MonoBehaviour
{
    [SerializeField] private PotatoCounter potatoCounter;
    //[SerializeField] private GameObject stoveOnGameObject;
    //[SerializeField] private GameObject particleGameObject;
    private void Start()
    {
        potatoCounter.OnStatechanged += PotatoCounter_OnStatechanged;
    }

    private void PotatoCounter_OnStatechanged(object sender, PotatoCounter.OnStateChangedEventArgs e)
    {
        //bool showVisual = e.state == PotatoCounter.State.Frying || e.state == PotatoCounter.State.Fried;
        //stoveOnGameObject.SetActive(showVisual);
        //particleGameObject.SetActive(showVisual);
    }
}
