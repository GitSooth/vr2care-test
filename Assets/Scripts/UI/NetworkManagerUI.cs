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

    int i;

    private void Awake()
    {
        i = 0;
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

    // void Start()
    // {
    //     if (PlayerData.Instance == null)
    //     {
    //         playerNameInput.GetComponent<Image>().color = Color.gray;
    //         playerNameInput.enabled = false;
    //         playerNameInput.text = PlayerData.Instance.payload.playerName;

    //         avatarUrlInput.GetComponent<Image>().color = Color.gray;
    //         avatarUrlInput.enabled = false;
    //         avatarUrlInput.text = PlayerData.Instance.payload.avatarUrl;

    //         joinCodeInput.GetComponent<Image>().color = Color.gray;
    //         joinCodeInput.enabled = false;
    //         joinCodeInput.text = PlayerData.Instance.payload.joinCode;
    //     }
    // }

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
            PlayerData.Instance.payload.playerIndex = i;

            i++;
        }

        SceneManager.LoadScene(0);
    }
}


/*

 if (NetworkManager.Singleton.IsServer)
 {
     GameObject player = Instantiate(playerPrefab);
     player.transform.position = positions[NetworkManager.LocalClientId].position;
     player.transform.rotation = positions[NetworkManager.LocalClientId].rotation;
     player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, false);
 }
*/
