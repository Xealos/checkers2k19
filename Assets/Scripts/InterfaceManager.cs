using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class InterfaceManager : MonoBehaviour
{
    public Text PlayerName;
    public Text OpponentName; 

    // Start is called before the first frame update
    public void Start()
    {
        PlayerName.text = PhotonNetwork.NickName;
    }

    // Update is called once per frame
    public void Update()
    {
        // Someone else has entered the room
        if(PhotonNetwork.PlayerList.Count() > 1)
        {
            // Assume we only have two players
            Player[] players = PhotonNetwork.PlayerList;

            OpponentName.text = players[1].NickName;
        }
        
    }


}
