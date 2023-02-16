using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button startBtn;

    public TMP_InputField playerNameInput;
    public TMP_InputField avatarUrlInput;
    public TMP_InputField joinCodeInput;

    int i = -1;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            PlayerData.Instance.payload.mode = "SERVER";
        });
        hostBtn.onClick.AddListener(() =>
        {
            PlayerData.Instance.payload.mode = "HOST";
        });
        clientBtn.onClick.AddListener(() =>
        {
            PlayerData.Instance.payload.mode = "CLIENT";
        });
        startBtn.onClick.AddListener(() =>
        {
            UpdateClientData();
        });
    }

    public void UpdateClientData()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.payload.playerName = playerNameInput.text;
            PlayerData.Instance.payload.avatarUrl = avatarUrlInput.text;
            PlayerData.Instance.payload.joinCode = joinCodeInput.text;
            PlayerData.Instance.payload.playerIndex = ++i;
        }

        SceneManager.LoadScene(0);
    }
}