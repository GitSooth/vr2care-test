using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerData : MonoBehaviour
{
    public class PlayerInfo
    {
        public ulong clientId;
        public string playerName;
        public string avatarUrl;
        public int playerIndex;

        public PlayerInfo() { }

        public PlayerInfo(ulong _clientId, string _playerName, string _avatarUrl, int _playerIndex)
        {
            clientId = _clientId;
            playerName = _playerName;
            avatarUrl = _avatarUrl;
            playerIndex = _playerIndex;
        }
    }

    public Dictionary<ulong, PlayerInfo> playerInfos = new Dictionary<ulong, PlayerInfo>();

    public void AddPlayerInfo(ulong _clientId, string _playername, string _avatarUrl, int _playerIndex)
    {
        playerInfos.Add(_clientId, new PlayerInfo(_clientId, _playername, _avatarUrl, _playerIndex));
    }

    public string GetPlayerName(ulong _clientId)
    {
        PlayerInfo info;
        if (playerInfos.TryGetValue(_clientId, out info))
        {
            return info.playerName;
        }
        return "PlayerNotFound";
    }
}