using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainLobby : MonoBehaviourPunCallbacks
{
    public GameObject mainMenu;
    public GameObject lobbyMenu;
    public GameObject connectingMenu;
    public GameObject playerInfo;
    
    public GameObject playerText;
    public GameObject createRoomText;

    // Room objects for 'Join' view
    public GameObject roomPanelPrefab;
    public GameObject roomListPanel;
    private Dictionary<string, RoomInfo> _savedRoomList;
    
    private readonly byte _maxPlayersPerRoom = 2;
    private readonly string _gameVersion = "1";
    private static bool _isConnectedToRoom;
    private static string _selectedRoomToJoin;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        _savedRoomList = new Dictionary<string, RoomInfo>();

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
            RoomOptions options = new RoomOptions{MaxPlayers = _maxPlayersPerRoom, IsVisible = true};

            PhotonNetwork.CreateRoom(roomName.text, options);
        }
    }

    public void OnClickRandomButton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom(); 
        }
    }

    public void OnClickRoomButton()
    {
        _selectedRoomToJoin = EventSystem.current.currentSelectedGameObject.name;
    }

    public void OnClickJoinButton()
    {
        if (_selectedRoomToJoin != null)
        {
            PhotonNetwork.JoinRoom(_selectedRoomToJoin);
        }
        
    }

    public void OnClickLogoutButton()
    {
        PhotonNetwork.Disconnect();
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo room in _savedRoomList.Values)
        {
            // Create the room panel 
            GameObject roomPanel = Instantiate(roomPanelPrefab, roomListPanel.transform, true);
            roomPanel.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // Assign the appropriate listener to the object after we instantiate it
            Button button = roomPanel.GetComponent<Button>();
            button.onClick.AddListener(OnClickRoomButton);

            Transform panelTrans = roomPanel.transform;

            // Also set the game object name to the room name so we can identify it later
            roomPanel.name = room.Name;

            // Set the text properties
            foreach (Transform t in panelTrans)
            {
                if (t.CompareTag("RoomName"))
                {
                    // Set the room name in the appropriate area of the text panel
                    TMP_Text text = t.gameObject.GetComponent<TMP_Text>();
                    text.text = room.Name;
                }
                else if (t.CompareTag("Players"))
                {
                    // TODO Brandon: Try and figure out if it's even possible to get the player names from the room.
                    // TMP_Text text = t.gameObject.GetComponent(<TMP_Text>();
                    // text.text = room.
                }
            }

        }
    }
    
    #region MonoBehaviorPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        
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
        
        Debug.Log("Loading Checkerboard Scene...");
        PhotonNetwork.LoadLevel("Checkerboard Scene");
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = _maxPlayersPerRoom });
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList)
        {
            _savedRoomList.Add(room.Name, room);
        }

        if (_savedRoomList != null)
        {
            UpdateRoomListView();
        }
    }

    #endregion
}