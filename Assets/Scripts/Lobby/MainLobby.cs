using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainLobby : MonoBehaviourPunCallbacks
{
    private byte _maxPlayersPerRoom = 2;
    private bool _isConnected;
    private string _gameVersion = "1";
    
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

    #endregion
}