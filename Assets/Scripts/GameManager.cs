using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject blackCheckerPrefab;

    public GameObject whiteCheckerPrefab;

    private static bool _instantiated;

    private bool _player1;

    void Start()
    {
        GameObject checker = null;

        // Only execute if we haven't instantiated our checkers yet. This prevents the player 1 entering the room
        // from instantiating another set of objects when player 2 enters. 
        if (_instantiated == false)
        {
            // If we're the first player to join, set us to player 1. 
            _player1 = PhotonNetwork.CurrentRoom.PlayerCount == 1;

            // TODO Brandon: Maybe let the player creating the room pick the color they want. 
            if (_player1)
            {
                // Create the whole set of black checkers.
                foreach (KeyValuePair<string, Vector3> coords in blackSpawnPoints)
                {
                    checker = PhotonNetwork.Instantiate(blackCheckerPrefab.name, coords.Value,
                    Quaternion.Euler(-90, 0, 0));
                }
            }
            else
            {
                // Create the whole set of white checkers.
                foreach (KeyValuePair<string, Vector3> coords in whiteSpawnPoints)
                {
                    checker = PhotonNetwork.Instantiate(blackCheckerPrefab.name, coords.Value,
                    Quaternion.Euler(-90, 0, 0));
                }
            }

            _instantiated = true;

        }

        if (checker != null)
        {
            //Make sure that we don't destroy already instantiated objects when another player enters the room and the 
            //scene reloads.
            DontDestroyOnLoad(checker);
        }
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
}