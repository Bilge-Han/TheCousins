using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterVisualSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int visualId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.ChangePlayerVisual(visualId);
        });
    }
    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        // BURADA ÝMAGE'I KENDÝMÝZ DOLDURACAZ, ELLE...
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().visualId == visualId) {selectedGameObject.SetActive(true); }
        else { selectedGameObject.SetActive(false); }
    }
} 
