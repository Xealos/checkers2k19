using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI opponentName;
    public GameObject redGlowShader;
    public GameObject versus;
    public Text playerTurn;
    public GameObject waitingText;
    public GameObject results;
    public GameObject gameResult;
    public GameObject escMenu;

    private static bool _instantiated;
    private static GameManager _turnListener;

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle menu with escape key.
            escMenu.SetActive(!escMenu.activeSelf);
        }
        
    }

    public void SetPlayGameText(bool myTurn)
    {
        waitingText.SetActive(false);
        playerName.text = PhotonNetwork.NickName;
        opponentName.text = PhotonNetwork.PlayerListOthers[0].NickName;
        versus.SetActive(true);
        SetPlayerTurnText(myTurn);
    }

    public void SetPlayerTurnText(bool myTurn)
    {
        if (myTurn)
        {
            playerTurn.text = playerName.text + "'s Turn";
        }
        else
        {
            playerTurn.text = opponentName.text + "'s Turn";
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
