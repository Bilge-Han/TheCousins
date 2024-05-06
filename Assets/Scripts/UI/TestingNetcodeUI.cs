using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    private void Awake()
    {
        startHostButton.onClick.AddListener(() => {
            Debug.Log("HOST");
            KitchenGameMultiplayer.Instance.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() => {
            Debug.Log("CLÝENT");
            KitchenGameMultiplayer.Instance.StartClient();
            Hide();
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
