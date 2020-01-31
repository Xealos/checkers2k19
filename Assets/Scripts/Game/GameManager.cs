using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    public GameObject blackCheckerPrefab;
    public GameObject blackKingCheckerPrefab;
    public GameObject whiteCheckerPrefab;
    public GameObject whiteKingCheckerPrefab;
    public GameObject checkersConatiner;
    public InterfaceManager interfaceManager;
    public Camera playerCamera;
    public static bool player1;

    private CheckerColor _checkerColor;
    public static bool MyTurn;
    private static PunTurnManager _turnManager;
    private static bool _instantiated;
    private static GameState _gameState;
    private static GameState _gameStatePrev;


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
    
    public static Dictionary<string, CellState> BoardState = new Dictionary<string, CellState>
    {
        {"A1", CellState.Player1},
        {"A3", CellState.Player1},
        {"A5", CellState.Player1},
        {"A7", CellState.Player1},
        {"B2", CellState.Player1},
        {"B4", CellState.Player1},
        {"B6", CellState.Player1},
        {"B8", CellState.Player1},
        {"C1", CellState.Player1},
        {"C3", CellState.Player1},
        {"C5", CellState.Player1},
        {"C7", CellState.Player1},
        {"D2", CellState.Empty},
        {"D4", CellState.Empty},
        {"D6", CellState.Empty},
        {"D8", CellState.Empty},
        {"E1", CellState.Empty},
        {"E3", CellState.Empty},
        {"E5", CellState.Empty},
        {"E7", CellState.Empty},
        {"F2", CellState.Player2},
        {"F4", CellState.Player2},
        {"F6", CellState.Player2},
        {"F8", CellState.Player2},
        {"G1", CellState.Player2},
        {"G3", CellState.Player2},
        {"G5", CellState.Player2},
        {"G7", CellState.Player2},
        {"H2", CellState.Player2},
        {"H4", CellState.Player2},
        {"H6", CellState.Player2},
        {"H8", CellState.Player2}
    };

    public enum CellState
    {
        Empty,
        Player1,
        Player2
    }
    
    private enum GameState
    {
        WaitingForPlayer,
        PlayingGame,
        PlayerWin,
        OpponentWin,
        OpponentForfeit
    }

    private enum CheckerColor
    {
        Black,
        White
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
                // TODO Brandon: Maybe let the player creating the room pick the color they want. 
                _checkerColor = CheckerColor.Black;
                
                // We'll need to wait for another player to enter before continuing. 
                _gameState = GameState.WaitingForPlayer;
                interfaceManager.waitingText.SetActive(true);
            }
            else
            {
                _checkerColor = CheckerColor.White;
                
                // We know another player is here, so let's start allowing play. 
                _gameState = GameState.PlayingGame;
            }

            SetupCheckers();

            SetupCamera(player1);

            SetupTurns(player1);
            
            _instantiated = true;
        }
        
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            interfaceManager.SetPlayGameText(MyTurn);
        }
        
    }

    void Update()
    {
        if( _gameState != _gameStatePrev)
        {
            switch (_gameState)
            {
                case GameState.PlayingGame:
                    interfaceManager.SetPlayGameText(MyTurn);
                    break;
                case GameState.OpponentForfeit:
                    interfaceManager.SetOpponentForfeitPanel();
                    _gameState = GameState.WaitingForPlayer;
                    break;
                case GameState.PlayerWin:
                    interfaceManager.SetGameOverPanel(true);
                    break;
                case GameState.OpponentWin:
                    interfaceManager.SetGameOverPanel(false);
                    break;
            }

            _gameStatePrev = _gameState;
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
    
    private void SetupCheckers()
    {
        Dictionary<string, Vector3> test;
        string prefabName;

        if (_checkerColor == CheckerColor.Black)
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
            var checker = PhotonNetwork.Instantiate(prefabName, coords.Value,
                Quaternion.Euler(-90, 0, 0));
            
            // TODO Brandon: Can't assign the checkers to this parent as it gets reset when the 2nd player enters the
            // TODO          match. Come up with a way to address this. 
            //checker.transform.parent = checkersConatiner.transform;

            checker.name = prefabName + " " + coords.Key;
            
            // TODO TIM: Is the tag needed for validation?
            checker.tag = coords.Key;

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
            // TODO Brandon: This won't work for double jumps. 
            _turnManager.SendMove(BoardState, false);
            _turnManager.SendMove(BoardState, true);
        }
    }

    public void KingMe(string checkerTag)
    {
        GameObject checkerGo = GameObject.FindWithTag(checkerTag);

        if (checkerGo == null)
        {
            return;
        }
        
        // Make sure this checker isn't already a king. 
        if (checkerGo.GetComponent<MovePiece>().isKing)
        {
            return;
        }

        PhotonNetwork.Destroy(checkerGo);
        Vector3 coords;
        string prefabName;
        
        if (_checkerColor == CheckerColor.Black)
        {
            coords = BlackSpawnPoints[checkerTag];
            prefabName = blackKingCheckerPrefab.name;
        }
        else
        {
            coords = WhiteSpawnPoints[checkerTag];
            prefabName = whiteKingCheckerPrefab.name;
        }

        coords.y = 1.5f;  //We need to set the king checker down from a greater height because it's taller. 
        
        GameObject king = PhotonNetwork.Instantiate(prefabName, coords,
            Quaternion.Euler(-90, 0, 0));
        
        king.transform.parent = checkersConatiner.transform;

        king.name = prefabName + " " + checkerTag; 
        king.tag = checkerTag;
        
        DontDestroyOnLoad(king);
    
    }

    public void OccupySpace(string cell)
    {
        if (player1)
        {
            BoardState[cell] = CellState.Player1;
        }
        else
        {
            BoardState[cell] = CellState.Player2;
        }
    }

    public bool IsOccupied(string cell)
    {
        return BoardState[cell] == CellState.Player1 || BoardState[cell] == CellState.Player2;
    }

    public bool IsOccupiedByOpponent(string cell)
    {
        if (player1)
        {
            return BoardState[cell] == CellState.Player2;
        }
        else
        {
            return BoardState[cell] == CellState.Player1;
        }
    }

    public bool IsOccupiedByPlayer(string cell)
    {
        if (player1)
        {
            return BoardState[cell] == CellState.Player1;
        }
        else
        {
            return BoardState[cell] == CellState.Player2;
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

    private void CheckForGameOver()
    {
        int player1PieceCount = BoardState.Values.Count(k => k==CellState.Player1);
        int player2PieceCount = BoardState.Values.Count(k => k == CellState.Player2);

        // Check to see if either player is out of pieces, then update the game over state based on which player
        // they are. 
        if (player1PieceCount == 0)
        {
            _gameState = player1 ? GameState.OpponentWin : GameState.PlayerWin;
        }
        else if (player2PieceCount == 0)
        {
            _gameState = player1 ? GameState.PlayerWin : GameState.OpponentWin;
        }
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

        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            _gameState = GameState.PlayingGame;
        }

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
            var simpleCast = (Dictionary<string, int>) move;
            
            // For some reason, a direct cast to <string, CellState> does not work, so instead we iterate and cast 
            // each value. 
            foreach (KeyValuePair<string, int> state in simpleCast)
            {
                BoardState[state.Key] = (CellState) state.Value;
            }
            
            MyTurn = true;
        }
        
        CheckForGameOver();
        
        interfaceManager.SetPlayerTurnText(MyTurn);
    }
    
    public void OnTurnCompleted(int turn)
    {
        // Start the next turn.
        _turnManager.BeginTurn();
    }
    
    public void OnTurnBegins(int turn)
    {
        // Intentionally left blank
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {

        string checkerName;

        if (player.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            // Sync the board state after the opponent finishes their movements.
            var simpleCast = (Dictionary<string, int>) move;
            
            // For some reason, a direct cast to <string, CellState> does not work, so instead we iterate and cast 
            // each value. 
            foreach (KeyValuePair<string, int> state in simpleCast)
            {    
                // If our new board state indicates that a space was previously occupied by one of our pieces
                // is no longer there, remove our piece from the board.
                if (IsOccupiedByPlayer(state.Key) && (CellState) state.Value == CellState.Empty)
                {
                    //PhotonNetwork.Destroy(GameObject.FindWithTag(state.Key));
                    if (_checkerColor == CheckerColor.Black)
                    {
                        checkerName = "B Checker " + state.Key;
                    }
                    else
                    {
                        checkerName = "W Checker " + state.Key;
                    }
                    
                    PhotonNetwork.Destroy(GameObject.Find(checkerName));
                }
                
                BoardState[state.Key] = (CellState)state.Value;
            }
        }

        // If the opponent makes a move but hasn't ended their turn, sync the board state based on
        // the opponent's intermediate message. 
        if (player.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            BoardState = (Dictionary<string, CellState>) move;
        }

    }

    public void OnTurnTimeEnds(int turn)
    {
        // Intentionally left blank
    }

    public string GetCheckerStr()
    {
        if (_checkerColor == CheckerColor.Black)
        {
            return "B Checker ";
        }
        else
        {
            return "W Checker ";
        }
    }

    #endregion
    
}

