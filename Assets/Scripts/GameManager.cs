using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Checkers
{
    public class GameManager : MonoBehaviourPunCallbacks
    {   
        // public void OnEnable()
        // {
        //     PhotonNetwork.AddCallbackTarget(this);
        // }

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

}
