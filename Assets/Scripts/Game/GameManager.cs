using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    public GameObject blackCheckerPrefab;

    public GameObject whiteCheckerPrefab;

    public Camera playerCamera;
    
    public bool player1;

    public static bool MyTurn;

    private static PunTurnManager _turnManager;

    private static bool _instantiated;

    private static GameState _gameState;

    private static readonly Dictionary<string, Vector3> BlackSpawnPoints = new Dictionary<string, Vector3>
    {
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

    private static readonly Dictionary<string, Vector3> WhiteSpawnPoints = new Dictionary<string, Vector3>
    {
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
    
    public static Dictionary<string, bool> BoardState = new Dictionary<string, bool>
    {
        {"A1", true},
        {"A3", true},
        {"A5", true},
        {"A7", true},
        {"B2", true},
        {"B4", true},
        {"B6", true},
        {"B8", true},
        {"C1", true},
        {"C3", true},
        {"C5", true},
        {"C7", true},
        {"D2", false},
        {"D4", false},
        {"D6", false},
        {"D8", false},
        {"E1", false},
        {"E3", false},
        {"E5", false},
        {"E7", false},
        {"F2", true},
        {"F4", true},
        {"F6", true},
        {"F8", true},
        {"G1", true},
        {"G3", true},
        {"G5", true},
        {"G7", true},
        {"H2", true},
        {"H4", true},
        {"H6", true},
        {"H8", true}
    };
    
    private enum GameState
    {
        WaitingForPlayer,
        PlayingGame,
        PlayerWin,
        OpponentWin,
        OpponentForfeit
    }

    void Start()
    {
        _turnManager = gameObject.AddComponent<PunTurnManager>();
        _turnManager.TurnManagerListener = this;
        
        // Only execute if we haven't instantiated our checkers yet. This prevents the player 1 entering the room
        // from instantiating another set of objects when player 2 enters. 
        if (_instantiated == false)
        {
            _turnManager.BeginTurn();
            
            // If we're the first player to join, set us to player 1. 
            player1 = PhotonNetwork.CurrentRoom.PlayerCount == 1;
            
            if (player1)
            {
                name = "GameManager Player 1";
                
                // We'll need to wait for another player to enter before continuing. 
                _gameState = GameState.WaitingForPlayer;
            }
            else
            {
                name = "Game Manager PLayer 2";
                
                // We know another player is here, so let's start allowing play. 
                _gameState = GameState.PlayingGame;
            }

            SetupCheckers(player1);

            SetupCamera(player1);

            SetupTurns(player1);
            
            _instantiated = true;
        }
    }

    void Update()
    {
        switch (_gameState)
        {
            case GameState.WaitingForPlayer when PhotonNetwork.CurrentRoom.PlayerCount == 2:
                _gameState = GameState.PlayingGame;
                break;
            case GameState.OpponentForfeit:
                //Wait for another player to enter the room 
                //TODO Brandon: Do we want to wait for another player or just end the game? 
                _gameState = GameState.WaitingForPlayer;
                break;
            case GameState.PlayerWin:
                GameOver();
                break;
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
            MyTurn = true;
        }
        else
        {
            MyTurn = false;
        }
    }
    
    private void SetupCheckers(bool player1)
    {
        GameObject checker = null;
        Dictionary<string, Vector3> test;
        string prefabName;

        // TODO Brandon: Maybe let the player creating the room pick the color they want. 
        if (player1)
        {
            test = BlackSpawnPoints;
            prefabName = blackCheckerPrefab.name;
        }
        else
        {

            test = WhiteSpawnPoints;
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
            BoardState[coords.Key] = true;

            //Make sure that we don't destroy already instantiated objects when another player enters the room and the 
            //scene reloads.
            DontDestroyOnLoad(checker);
        }
    }

    public bool GamePlayAllowed()
    {
        if(PhotonNetwork.IsConnected && MyTurn && _gameState == GameState.PlayingGame)
        {
            return true; 
        }

        return false;
    }

    public void UpdateGameState()
    {
        if (MyTurn)
        {
            //TODO Brandon: Maybe the setting of the boardState can go here (pass the key and value as args to this
            //TODO          function instead of setting it directly in the MovePiece class. 

            // If the move is valid, send the updated board state and indicate the player has finished their turn. 
            _turnManager.SendMove(BoardState, true);
        }
    }
    
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
    
    private void GameOver()
    {
        
    }

    #region Photon Callbacks
    public override void OnLeftRoom()
    {
        _instantiated = false;
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not seen if you're the player connecting
        Debug.LogFormat("{0} has entered the match!", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        // seen when other disconnects
        Debug.LogFormat("{0} has left the match!", other.NickName);

        // Opponent has left the game, we're no longer allowing play at this point.
        _gameState = GameState.OpponentForfeit;
    }

    #endregion

    #region Turn Manager Callbacks

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        // If the local player drove the callback, then it's the end of their turn. 
        if (player.UserId == PhotonNetwork.LocalPlayer.UserId)
        {
            MyTurn = false;
        }
        // If the other player receives the callback, it's the beginning of their turn.
        else
        {
            // Sync the board state after the opponent finishes their movements.
            BoardState = (Dictionary<string, bool>) move;
            MyTurn = true;
        }
    }
    
    public void OnTurnCompleted(int turn)
    {
        // Start the next turn.
        _turnManager.BeginTurn();
    }
    
    public void OnTurnBegins(int turn)
    {
        // Intentionally left blank
//        Debug.LogFormat("A1 {0}, A2 {1}, A3 {2}, A4 {3}, A5 {4}, A6 {5}, A7 {6}, A8 {7}", boardState["A1"],
//            boardState["A2"], boardState["A3"], boardState["A4"], boardState["A5"], boardState["A6"], boardState["A7"],
//            boardState["A8"]);
//        
//        Debug.LogFormat("B1 {0}, B2 {1}, B3 {2}, B4 {3}, B5 {4}, B6 {5}, B7 {6}, B8 {7}", boardState["B1"],
//            boardState["B2"], boardState["B3"], boardState["B4"], boardState["B5"], boardState["B6"], boardState["B7"],
//            boardState["B8"]);
//        
//        Debug.LogFormat("C1 {0}, C2 {1}, C3 {2}, C4 {3}, C5 {4}, C6 {5}, C7 {6}, C8 {7}", boardState["C1"],
//            boardState["C2"], boardState["C3"], boardState["C4"], boardState["C5"], boardState["C6"], boardState["C7"],
//            boardState["C8"]);
//        
//        Debug.LogFormat("D1 {0}, D2 {1}, D3 {2}, D4 {3}, D5 {4}, D6 {5}, D7 {6}, D8 {7}", boardState["D1"],
//            boardState["D2"], boardState["D3"], boardState["D4"], boardState["D5"], boardState["D6"], boardState["D7"],
//            boardState["D8"]);
//        
//        Debug.LogFormat("E1 {0}, E2 {1}, E3 {2}, E4 {3}, E5 {4}, E6 {5}, E7 {6}, E8 {7}", boardState["E1"],
//            boardState["E2"], boardState["E3"], boardState["E4"], boardState["E5"], boardState["E6"], boardState["E7"],
//            boardState["E8"]);
//        
//        Debug.LogFormat("F1 {0}, F2 {1}, F3 {2}, F4 {3}, F5 {4}, F6 {5}, F7 {6}, F8 {7}", boardState["F1"],
//            boardState["F2"], boardState["F3"], boardState["F4"], boardState["F5"], boardState["F6"], boardState["F7"],
//            boardState["F8"]);
//        
//        Debug.LogFormat("G1 {0}, G2 {1}, G3 {2}, G4 {3}, G5 {4}, G6 {5}, G7 {6}, G8 {7}", boardState["G1"],
//            boardState["G2"], boardState["G3"], boardState["G4"], boardState["G5"], boardState["G6"], boardState["G7"],
//            boardState["G8"]);
//        
//        Debug.LogFormat("H1 {0}, H2 {1}, H3 {2}, H4 {3}, H5 {4}, H6 {5}, H7 {6}, H8 {7}", boardState["H1"],
//            boardState["H2"], boardState["H3"], boardState["H4"], boardState["H5"], boardState["H6"], boardState["H7"],
//            boardState["H8"]);
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        // If the opponent makes a move but hasn't ended their turn, sync the board state based on
        // the opponent's intermediate message. 
        if (player.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            BoardState = (Dictionary<string, bool>) move;
        }

    }

    public void OnTurnTimeEnds(int turn)
    {
        // Intentionally left blank
    }

    #endregion
    
}

