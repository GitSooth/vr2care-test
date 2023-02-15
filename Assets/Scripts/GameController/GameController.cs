using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameController : NetworkBehaviour
{
    [SerializeField] Transform[] positions;
    [SerializeField] public List<Transform> playerPrefabs;

    Transform playerPrefab;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkOnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkOnClientDisconnected;
    }

    private void NetworkOnClientDisconnected(ulong obj)
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (GetComponent<NetworkObject>().NetworkObjectId.Equals(obj))
                {
                    DestroyImmediate(player);
                    break;
                }
            }
        }
    }

    private void NetworkOnClientConnected(ulong obj)
    {
        if (IsServer)
        {
            SpawnPlayerAndPort(obj);
        }
    }

    public override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= NetworkOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkOnClientDisconnected;
        }
        base.OnDestroy();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerData.Instance.payload.mode.Equals("CLIENT"))
        {
            StartingClient();
        }
        if (PlayerData.Instance.payload.mode.Equals("SERVER"))
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

            if (NetworkManager.Singleton.StartServer())
            {
                Debug.Log("Server started...");
            }
            else
            {
                Debug.Log("Server could not be started...");
            }
        }

        if (PlayerData.Instance.payload.mode.Equals("HOST"))
        {
            StartingHost();
        }
    }

    private void SpawnPlayerAndPort(ulong clientId)
    {
        if (IsServer)
        {
            ServerData serverPlayerData = NetworkManager.Singleton.GetComponent<ServerData>();
            string playerName = serverPlayerData.GetPlayerName(clientId);
            playerPrefab = playerPrefabs[serverPlayerData.playerInfos[clientId].playerIndex];
            Transform playerInstance = Instantiate(playerPrefab);

            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            playerInstance.transform.position = positions[clientId].position;
            playerInstance.transform.rotation = positions[clientId].rotation;

            string avatarUrl = serverPlayerData.playerInfos[clientId].avatarUrl;
            playerInstance.GetComponent<GetAvatar>().SetAvatarUrl(avatarUrl);
        }
    }

    private async void StartingClient()
    {
        string playerDataJSON = JsonUtility.ToJson(PlayerData.Instance.payload);
        Encoding enc = Encoding.GetEncoding(Encoding.UTF8.WebName);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = enc.GetBytes(playerDataJSON);
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started...");
        }
        else
        {
            Debug.Log("Client could not be started");
        }
    }

    private async void StartingHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        Encoding enc = Encoding.GetEncoding(Encoding.UTF8.WebName);
        string clientPlayerDataJSON = JsonUtility.ToJson(PlayerData.Instance.payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = enc.GetBytes(clientPlayerDataJSON);
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Starting Host");
            SpawnPlayerAndPort(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.Log("Host could not be started");
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = false;


        Encoding enc = Encoding.GetEncoding(Encoding.UTF8.WebName);
        string playerDataJSON = enc.GetString(connectionData);
        PlayerData.PlayerDataPayload clientPlayerData = JsonUtility.FromJson<PlayerData.PlayerDataPayload>(playerDataJSON);
        ServerData ii = NetworkManager.Singleton.GetComponent<ServerData>();
        ii.AddPlayerInfo(clientId, clientPlayerData.playerName, clientPlayerData.avatarUrl, clientPlayerData.playerIndex);

        // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
}
