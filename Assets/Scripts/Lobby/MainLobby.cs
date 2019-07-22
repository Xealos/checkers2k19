using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MainLobby : MonoBehaviourPunCallbacks
{
    public GameObject mainMenu;
    public GameObject lobbyMenu;
    public GameObject connectingMenu;
    public GameObject playerInfo;
    
    public GameObject playerText;
    public GameObject createRoomText; 
    
    private readonly byte _maxPlayersPerRoom = 2;
    private readonly string _gameVersion = "1";
    private static bool _isConnectedToRoom;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // Check to see whether we are coming back to the Lobby from a match or entering the Lobby 
        // for the very first time. 
        if (_isConnectedToRoom)
        {
            // We are coming back out of a room, so reset the flag.
            _isConnectedToRoom = false;
            
            // Set the nickname in the menu so they remember who they are.
            playerText.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
            
            // Make sure we come back to the lobby menu since we're already logged in. 
            mainMenu.SetActive(false);
            lobbyMenu.SetActive(true);
            playerInfo.SetActive(true);
        }
    }

    public void OnClickConnectButton()
    {
        // TODO Brandon: Hard-coding US West region for now to ensure we are connecting all instances to the 
        // TODO          same servers for testing. 
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
        
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickCreateButton()
    {
        Text roomName = createRoomText.GetComponent<Text>();
        
        if (roomName.text != string.Empty)
        {
            RoomOptions options = new RoomOptions{MaxPlayers = _maxPlayersPerRoom};

            PhotonNetwork.CreateRoom(roomName.text, options);

            PhotonNetwork.JoinRoom(roomName.text);
        }
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

    public override void OnConnectedToMaster()
    {
        if(!_isConnectedToRoom)
        {
            // Once a connection has been established, display the lobby menu.
            connectingMenu.SetActive(false);
            lobbyMenu.SetActive(true);   
            playerInfo.SetActive(true);
            
            // Set the nickname in the menu so they remember who they are.
            playerText.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
        }
    }

    public override void OnJoinedRoom()
    {
        _isConnectedToRoom = true;
        
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