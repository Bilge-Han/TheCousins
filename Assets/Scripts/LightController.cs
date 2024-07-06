using UnityEngine;
using UnityEngine.SceneManagement;

public class LightController : MonoBehaviour
{
    void Start()
    {
        // Iþýk nesnesini sadece belirli bir sahnede kontrol etmek istiyorsanýz,
        // sahne adýný kontrol edebilirsiniz.
        if (SceneManager.GetActiveScene().name == "CharacterSelectScene")
        {
            GetComponent<Light>().enabled = true; // Iþýðý etkinleþtir
        }
        else
        {
            GetComponent<Light>().enabled = false; // Iþýðý devre dýþý býrak
        }
    }
}
