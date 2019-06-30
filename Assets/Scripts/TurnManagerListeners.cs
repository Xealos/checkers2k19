using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class TurnManagerListeners : MonoBehaviour, IPunTurnManagerCallbacks
{
    public bool myTurn;

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        // If the local player drove the callback, then it's the end of their turn. 
        if (player.UserId == PhotonNetwork.LocalPlayer.UserId)
        {
            myTurn = false;
        }
        // If the other player receives the callback, it's the beginning of their turn.
        else
        {
            //TODO BRP: Send the move update back to the game manager for processing.
            myTurn = true;
        }
    }

    public void OnTurnBegins(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void OnTurnCompleted(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        throw new System.NotImplementedException();
    }

    public void OnTurnTimeEnds(int turn)
    {
        throw new System.NotImplementedException();
    }
}