using Photon.Pun;
using Photon.Realtime;
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
                // TODO Tim: Create the whole set of black checkers.
                checker = PhotonNetwork.Instantiate(blackCheckerPrefab.name, new Vector3(0.829f, 0.261f, 2.38423f), 
                    Quaternion.Euler(-90, 0, 0 ));
            }
            else
            {
                // TODO Tim: Create the whole set of white checkers.
                checker= PhotonNetwork.Instantiate(whiteCheckerPrefab.name, new Vector3(0.16f, 0.26f, 2.4f),
                    Quaternion.Euler(-90, 0, 0));
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

}