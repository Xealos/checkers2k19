using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject blackCheckerPrefab;

    public GameObject whiteCheckerPrefab;

    public Camera playerCamera;
    
    private PunTurnManager _turnManager;

    private TurnManagerListeners _turnListeners;

    private static bool _instantiated;

    public bool _player1;

    void Start()
    {
        _turnManager = GetComponent<PunTurnManager>();
        _turnListeners = GetComponent<TurnManagerListeners>();

        _turnManager.TurnManagerListener = _turnListeners;
        
        // Only execute if we haven't instantiated our checkers yet. This prevents the player 1 entering the room
        // from instantiating another set of objects when player 2 enters. 
        if (_instantiated == false)
        {
            // If we're the first player to join, set us to player 1. 
            _player1 = PhotonNetwork.CurrentRoom.PlayerCount == 1;

            SetupCheckers(_player1);

            SetupCamera(_player1);

            SetupTurns(_player1);
            
            _instantiated = true;

        }
        
    }

    public bool IsMoveValid()
    {
        if (_turnListeners.myTurn)
        {
            //TODO Tim: Add move validity logic here maybe? 
        
            //TODO Tim: Update the game manager with the new board state
        
            //If we've determined the move to be valid, send it to the other player and finish our turn
            //TODO Brandon: What data should we send? The piece and new coordinates?
            _turnManager.SendMove(null, true);
        }

        else
        {
            // It's not the players turn, they can't move anything right now.
            return false;
        }


        return true;
    }

    #region Photon Callbacks
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not seen if you're the player connecting
        Debug.LogFormat("OnPlayerEnteredRoom () {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        // seen when other disconnects
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }

    #endregion

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void LoadArena()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError("PhotonNetwork : Trying to load a level be we are not the master client.");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("CheckerboardScene");
    }

    private void SetupCheckers(bool player1)
    {
        GameObject checker = null;
        Dictionary<string, Vector3> test;
        string prefabName;

        // TODO Brandon: Maybe let the player creating the room pick the color they want. 
        // TODO Tim: This logic is repetitive. Recommend assigning local variables and consolidating to one foreach loop.
        if (player1)
        {
            test = blackSpawnPoints;
            prefabName = blackCheckerPrefab.name;
        }
        else
        {

            test = whiteSpawnPoints;
            prefabName = whiteCheckerPrefab.name;
        }

        // Create the whole set of checkers.
        foreach (KeyValuePair<string, Vector3> coords in test)
        {
            checker = PhotonNetwork.Instantiate(prefabName, coords.Value,
                Quaternion.Euler(-90, 0, 0));

            checker.name = prefabName + " " + coords.Key;
            
            // TODO TIM: Is the tag needed for validation?
            checker.tag = coords.Key;

            // Update the board state. The space is occupied.
            boardState[coords.Key] = true;

            //Make sure that we don't destroy already instantiated objects when another player enters the room and the 
            //scene reloads.
            DontDestroyOnLoad(checker);
        }
    }

    private void SetupCamera(bool player1)
    {
        var camTransform = playerCamera.transform;
        
        if (player1)
        {
            
            camTransform.position = new Vector3(0.303f, 3.185f, 4.822f);
            camTransform.rotation = Quaternion.Euler(45, -180, 0);
        }
        else
        {
            camTransform.position = new Vector3(0.303f, 3.185f, -4.822f);
            camTransform.rotation = Quaternion.Euler(45, 0, 0);
        }
    }

    private void SetupTurns(bool player1)
    {
        if (player1)
        {
            _turnListeners.myTurn = true;
        }
        else
        {
            _turnListeners.myTurn = false;
        }
    }

    private Dictionary<string, Vector3> blackSpawnPoints = new Dictionary<string, Vector3>(){
        { "A1", new Vector3(2.765f, 0.261f, 2.422f)},
        { "A3", new Vector3(1.459f, 0.261f, 2.422f)},
        { "A5", new Vector3(0.186f, 0.261f, 2.422f)},
        { "A7", new Vector3(-1.102f, 0.261f, 2.422f)},
        { "B2", new Vector3(2.102f, 0.261f, 1.759f)},
        { "B4", new Vector3(0.819f, 0.261f, 1.759f)},
        { "B6", new Vector3(-0.457f, 0.261f, 1.759f)},
        { "B8", new Vector3(-1.759f, 0.261f, 1.759f)},
        { "C1", new Vector3(2.765f, 0.261f, 1.136f)},
        { "C3", new Vector3(1.459f, 0.261f, 1.136f)},
        { "C5", new Vector3(0.186f, 0.261f, 1.136f)},
        { "C7", new Vector3(-1.102f, 0.261f, 1.136f)}
    };

    private Dictionary<string, Vector3> whiteSpawnPoints = new Dictionary<string, Vector3>(){
        { "F2", new Vector3(2.102f, 0.261f, -0.78f)},
        { "F4", new Vector3(0.819f, 0.261f, -0.78f)},
        { "F6", new Vector3(-0.457f, 0.261f, -0.78f)},
        { "F8", new Vector3(-1.759f, 0.261f, -0.78f)},
        { "G1", new Vector3(2.765f, 0.261f, -1.44f)},
        { "G3", new Vector3(1.459f, 0.261f, -1.44f)},
        { "G5", new Vector3(0.186f, 0.261f, -1.44f)},
        { "G7", new Vector3(-1.102f, 0.261f, -1.44f)},
        { "H2", new Vector3(2.102f, 0.261f, -2.07f)},
        { "H4", new Vector3(0.819f, 0.261f, -2.07f)},
        { "H6", new Vector3(-0.457f, 0.261f, -2.07f)},
        { "H8", new Vector3(-1.759f, 0.261f, -2.07f)}
    };

    public Dictionary<string, bool> boardState = new Dictionary<string, bool>()
    {
        {"A1", false},
        {"A3", false},
        {"A5", false},
        {"A7", false},
        {"B2", false},
        {"B4", false},
        {"B6", false},
        {"B8", false},
        {"C1", false},
        {"C3", false},
        {"C5", false},
        {"C7", false},
        {"D2", false},
        {"D4", false},
        {"D6", false},
        {"D8", false},
        {"E1", false},
        {"E3", false},
        {"E5", false},
        {"E7", false},
        {"F2", false},
        {"F4", false},
        {"F6", false},
        {"F8", false},
        {"G1", false},
        {"G3", false},
        {"G5", false},
        {"G7", false},
        {"H2", false},
        {"H4", false},
        {"H6", false},
        {"H8", false}
    };
}

