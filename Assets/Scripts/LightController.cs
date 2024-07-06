using UnityEngine;
using UnityEngine.SceneManagement;

public class LightController : MonoBehaviour
{
    void Start()
    {
        // I��k nesnesini sadece belirli bir sahnede kontrol etmek istiyorsan�z,
        // sahne ad�n� kontrol edebilirsiniz.
        if (SceneManager.GetActiveScene().name == "CharacterSelectScene")
        {
            GetComponent<Light>().enabled = true; // I���� etkinle�tir
        }
        else
        {
            GetComponent<Light>().enabled = false; // I���� devre d��� b�rak
        }
    }
}
