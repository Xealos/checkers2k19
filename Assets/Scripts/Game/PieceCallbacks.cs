using Photon.Pun;
using UnityEngine;

public class PieceCallbacks : MonoBehaviourPunCallbacks
{
    public string tempTag; 
    
    void Start()
    {
        this.GetComponent<PhotonView>().RPC("SyncGameTag", RpcTarget.All, "Hello!");
    }
    [PunRPC]
    void SyncGameTag(string str)
    {
        this.tag = str;
    }
}
