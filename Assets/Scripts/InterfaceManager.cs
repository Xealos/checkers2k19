using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    public Text playerName;
    public Text opponentName;
    public Text playerTurn;

    private static bool _instantiated;
    
    private static GameManager _turnListener;
    
    // Start is called before the first frame update
    public void Start()
    {
//        if (_instantiated == false)
//        {
//            _turnListener = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().turnListeners;
//            _instantiated = true;
//        }

        playerName.text = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    public void Update()
    {
        //TODO Brandon: These update calls seem inefficient. Is there a better way to do this?
        if (_instantiated == false)
        {
            if (_turnListener == null)
            {
                _turnListener = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
                _instantiated = true;
            }

        }

        // Someone else has entered the room
        if(PhotonNetwork.PlayerList.Count() > 1)
        {
            // Assume we only have two players
            Player[] players = PhotonNetwork.PlayerListOthers;

            opponentName.text = players[0].NickName;
        }

        // Check to see whose turn it is and update the UI accordingly
        if (GameManager.myTurn)
        {
            playerTurn.text = PhotonNetwork.NickName + "'s Turn";
        }
        else
        {
            playerTurn.text = PhotonNetwork.PlayerListOthers[0].NickName + "'s Turn";
        }
        
    }


}
