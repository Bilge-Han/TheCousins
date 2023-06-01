using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DonerCounter;

public class DonerCounterVisual : MonoBehaviour
{

    [SerializeField] private DonerCounter donerCounter;
    [SerializeField] private Transform donerSpawnedPoint;
    [SerializeField] private Transform donerVisualPrefab;

    private const string CUT = "DonerCutting";
    private Animator animator;
    private List<GameObject> donerVisualGameObjectList;
    private void Awake()
    {
        donerVisualGameObjectList = new List<GameObject>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        donerCounter.OnDonerSpawned += DonerCounter_OnDonerSpawned;
        donerCounter.OnDonerRemoved += DonerCounter_OnDonerRemoved;
        donerCounter.OnCut += DonerCounter_OnCut;
    }

    private void DonerCounter_OnCut(object sender, System.EventArgs e)
    {
        animator.SetTrigger(CUT);
    }

    private void DonerCounter_OnDonerRemoved(object sender, System.EventArgs e)
    {
        GameObject donerGameObject = donerVisualGameObjectList[donerVisualGameObjectList.Count - 1];
        donerVisualGameObjectList.Remove(donerGameObject);
        Destroy(donerGameObject);
    }

    private void DonerCounter_OnDonerSpawned(object sender, System.EventArgs e)
    {
      
            Transform donerVisualTransform = Instantiate(donerVisualPrefab, donerSpawnedPoint);
            float donerOffSetY = .1f;
            donerVisualTransform.localPosition = new Vector3(0, 0, donerOffSetY * donerVisualGameObjectList.Count);
            donerVisualGameObjectList.Add(donerVisualTransform.gameObject);
        
    }
}
