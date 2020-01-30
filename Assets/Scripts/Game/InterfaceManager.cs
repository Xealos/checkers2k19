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
    public GameObject results; 

    public GameObject gameResult; 

    private static bool _instantiated;
    
    private static GameManager _turnListener;
    
    // Start is called before the first frame update
    public void Start()
    {
        playerName.text = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    public void Update()
    {
        //TODO Brandon: These update calls seem inefficient. Is there a better way to do this?
        // Someone else has entered the room
        if(PhotonNetwork.PlayerList.Count() > 1)
        {
            // Assume we only have two players
            Player[] players = PhotonNetwork.PlayerListOthers;

            opponentName.text = players[0].NickName;
        }

        // Check to see whose turn it is and update the UI accordingly
        if (GameManager.MyTurn)
        {
            playerTurn.text = PhotonNetwork.NickName + "'s Turn";
        }
        else
        {
            playerTurn.text = PhotonNetwork.PlayerListOthers[0].NickName + "'s Turn";
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // TODO Brandon: Add menu panel enablement here. 
        }
        
    }

    public void SetGameOverPanel(bool isWinner)
    {
        if(isWinner)
        {
            results.GetComponent<TextMeshProUGUI>().text = "You Win!";
        }
        else
        {
            results.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }

        gameResult.SetActive(true);
    }

    public void SetOpponentForfeitPanel()
    {
        // Widen the transform to account for longer text
        results.GetComponent<RectTransform>().sizeDelta = new Vector2(425, 50);
        results.GetComponent<TextMeshProUGUI>().text = "Opponent Forfeit!";
        gameResult.SetActive(true);
    }
}
