using UnityEngine;
using System;

public class PlayerData : PlayerSingleton<PlayerData>
{
    [Serializable]
    public class PlayerDataPayload
    {
        public string mode = "";
        public string playerName = "";
        public string avatarUrl = "";
        public int playerIndex = -1;
        public string joinCode = "";
    }

    public PlayerDataPayload payload = new PlayerDataPayload();

    private void Start()
    {
        DontDestroyOnLoad(transform);
    }
}
