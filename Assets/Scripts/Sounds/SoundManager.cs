using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    [SerializeField] private PotatoCounter potatoCounter;
    public static SoundManager Instance { get; private set; }
    
    
    [SerializeField] private SoundsSO soundsSO;



    private float volume=1f;

    private void Awake()
    {
        Instance = this;
        volume= PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        DonerCounter.OnAnyCut += DonerCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
        //potatoCounter.OnStatechanged += PotatoCounter_OnStatechanged;
        
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier*volume);
    }
    private void PotatoCounter_OnStatechanged(object sender, PotatoCounter.OnStateChangedEventArgs e)
    {
        bool startSound = e.state == PotatoCounter.State.Start;
        bool fryingSound = e.state == PotatoCounter.State.Frying;
        bool friedSound = e.state == PotatoCounter.State.Frying || e.state == PotatoCounter.State.Fried;
        bool endSound = e.state == PotatoCounter.State.Fried;
        if (startSound)
        {
            PlaySound(soundsSO.frutozBasla, potatoCounter.transform.position);
        }
        if (fryingSound)
        {
            PlaySound(soundsSO.kizarmaSesi, potatoCounter.transform.position);
        }
        if (friedSound)
        {
            PlaySound(soundsSO.frutozCalisma, potatoCounter.transform.position);
        }
        if (endSound)
        {
            PlaySound(soundsSO.frutozBitis, potatoCounter.transform.position);
        }
        else
        {
        }
    }
    private void DonerCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        DonerCounter donerCounter = sender as DonerCounter;
        //PlaySound(soundsSO.ekmekKesme, donerCounter.transform.position);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(soundsSO.copAtma, trashCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, System.EventArgs e)
    {
        PlaySound(soundsSO.pickUp, Player.Instance.transform.position);
    }
    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(soundsSO.drop, baseCounter.transform.position);
    }
    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(soundsSO.deliveryFail,deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(soundsSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(soundsSO.soganKesme, cuttingCounter.transform.position);
    }
    public void ChangeVolume()
    {
        volume += .1f;
        if (volume>1f)
        {
            volume = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}
