using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObjectVisual;

    private void Awake()
    {
        // Do�ru ba�lang�� de�erini ayarla
        playerGameObjectVisual = transform.GetChild(0).gameObject;
    }

    public void SetPlayerVisual(GameObject gameObject)
    {
        // Eski oyun objesini yok et
        Destroy(playerGameObjectVisual);
        // Yeni oyun objesini instantiate et ve PlayerVisual alt�na yerle�tir
        playerGameObjectVisual = Instantiate(gameObject, transform);
    }
}
