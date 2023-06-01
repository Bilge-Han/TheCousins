using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultRecipesDeliveredText;
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += Instance_OnStateChanged;
        Hide();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();
            resultRecipesDeliveredText.text = DeliveryManager.Instance.GetSuccesfullRecipesAmount().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
