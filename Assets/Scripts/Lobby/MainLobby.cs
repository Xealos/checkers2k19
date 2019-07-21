using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainLobby : MonoBehaviourPunCallbacks
{
    private readonly byte _maxPlayersPerRoom = 2;
    private readonly string _gameVersion = "1";
    private bool _isConnected;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnClickConnectButton()
    {
        // TODO Brandon: Hard-coding US West region for now to ensure we are connecting all instances to the 
        // TODO          same servers for testing. 
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
        
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickRandomButton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom(); 
        }
    }

    public void OnClickLogoutButton()
    {
        PhotonNetwork.Disconnect();
    }
    
    #region MonoBehaviorPunCallbacks Callbacks
    
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Loading Checkerboard Scene...");
            PhotonNetwork.LoadLevel("CheckerboardScene");
        }
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = _maxPlayersPerRoom });
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    #endregion
}