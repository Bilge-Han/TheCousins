using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
public class KitchenGameManager : NetworkBehaviour
{

    public static KitchenGameManager Instance { get; private set; }


    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnPaused;
    public event EventHandler OnLocalPlayerReadyChanged;


    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>();
    private float gamePlayingTimerMax = 300f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePausedState;
    private void Awake()
    {
        Instance = this;
        state.Value = State.WaitingToStart;
    }
    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        autoTestGamePausedState = true;

    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value) { Time.timeScale = 0f; FrutozAudioManager.Instance.PauseFrutozAudio(); OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty); }
        else { Time.timeScale = 1f; FrutozAudioManager.Instance.UnPauseFrutozAudio(); OnMultiplayerGameUnPaused?.Invoke(this, EventArgs.Empty); }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;

            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clienId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            if (!playerReadyDictionary.ContainsKey(clienId) || !playerReadyDictionary[clienId])
            {
                // Oyuncu Hazýr Deðil.
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady) { state.Value = State.CountdownToStart; }
    }
    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer) return;
        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value <= 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;

                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value <= 0f) { state.Value = State.GameOver; }
                break;
            case State.GameOver:
                break;
        }
        Debug.Log(state);
    }
    private void LateUpdate()
    {
        if (autoTestGamePausedState) { autoTestGamePausedState = false; TestGamePausedState(); }
    }
    public void AddGamePlayingTime()
    {
        gamePlayingTimer.Value += 30;
    }
    public void SubtractGamePlayingTime()
    {
        gamePlayingTimer.Value -= 60;
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }
    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }
    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }
    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public bool GetIsPlaying()
    {
        return isLocalGamePaused;
    }
    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            //FrutozAudioManager.Instance.PauseFrutozAudio();
            //Time.timeScale = 0f;
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();
            //FrutozAudioManager.Instance.UnPauseFrutozAudio();
            //Time.timeScale = 1f;
            OnLocalGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }
    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }
    private void TestGamePausedState()
    {
        foreach (ulong clienId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clienId) && playerPausedDictionary[clienId])
            {
                // This player is paused
                isGamePaused.Value = true;
                return;
            }
        }
        //All Players Unpaused
        isGamePaused.Value = false;
    }
}
