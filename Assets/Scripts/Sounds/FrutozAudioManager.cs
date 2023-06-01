using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrutozAudioManager : MonoBehaviour
{
    [SerializeField] private PotatoCounter potatoCounter;
    [SerializeField] private GameObject frutozFrySound;
    [SerializeField] private GameObject frutozEndSound;

    private void Start()
    {
        potatoCounter.OnStatechanged += PotatoCounter_OnStatechanged;
    }
    private void PotatoCounter_OnStatechanged(object sender, PotatoCounter.OnStateChangedEventArgs e)
    {
        bool friedSound = e.state == PotatoCounter.State.Fried;
        bool friedOrFryingSound = e.state == PotatoCounter.State.Frying || e.state == PotatoCounter.State.Fried;
        if (friedOrFryingSound)
        {
            if (frutozFrySound.GetComponent<AudioSource>().isPlaying==false)
            {
                frutozFrySound.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            frutozFrySound.GetComponent<AudioSource>().Pause();
        }
    }
}
