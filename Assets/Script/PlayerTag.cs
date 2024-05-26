using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerTag : MonoBehaviourPunCallbacks
{
    public PlayerType playerType;
    public Material newMaterial;
    public Material newMaterial2;
    //public Color[] color;
    public Renderer characterRenderer;

    void Start()
    {
        if (playerType == PlayerType.IT)
        {
            GameManager.instance.itPlayerCount += 1;
        }

        if (playerType == PlayerType.Normal)
        {
            GameManager.instance.normalPlayerCount+=1;
            //GameManager.instance.scoreTeam.color =Color.green;
        }

        PlayerTag player = GetComponent<PlayerTag>();
        if (player.playerType == PlayerType.IT)
        {
            GameManager.instance.scoreTeam.color = Color.green;     
            GameManager.instance.scoreIt.color = Color.white;
        }
        
        if (player.playerType == PlayerType.Normal) 
        {
            GameManager.instance.scoreIt.color = Color.red;
            GameManager.instance.scoreTeam.color = Color.white;
        }
    }



    public void Update()
    {
        //characterRenderer = GetComponent<Renderer>();
    }
    public void OnTriggerStay(Collider other)
    {
        PlayerTag otherPlayer = other.GetComponent<PlayerTag>();

       
        if (Input.GetKeyDown(KeyCode.E) && photonView.IsMine && otherPlayer != null)
        {
            
            if (otherPlayer.playerType == PlayerType.IT && playerType != PlayerType.IT)
            {
                if (GameManager.instance.teamGoal >= 0 && GameManager.instance.teamGoal ==1)
                {
                    ChangePlayerCounts();
                    GameManager.instance.GameOver(PlayerType.Normal);
                }
                else
                {
                    ChangePlayerCounts();
                }
                otherPlayer.photonView.RPC("ChangePlayerType", RpcTarget.AllBuffered, otherPlayer.photonView.ViewID);
            }
         
            else if (otherPlayer.playerType == PlayerType.Normal && playerType != PlayerType.Normal)
            {
                if (GameManager.instance.itGoal >= 0 && GameManager.instance.itGoal == 1)
                {
                    ChangePlayerCounts1();
                    GameManager.instance.GameOver(PlayerType.IT);
                }
                else
                {
                    ChangePlayerCounts1();
                }
                // Call the RPC to change the player type
                otherPlayer.photonView.RPC("ChangePlayerType1", RpcTarget.AllBuffered, otherPlayer.photonView.ViewID);
            }
        }
    }

    public void ChangePlayerCounts()
    {
        // Decrement itPlayerCount and increment normalPlayerCount
        GameManager.instance.itPlayerCount-=1;
        GameManager.instance.normalPlayerCount+=1;

        // Call an RPC to synchronize the changes across all players
        photonView.RPC("SyncPlayerCounts", RpcTarget.AllBuffered, GameManager.instance.itPlayerCount, GameManager.instance.normalPlayerCount);
    }
    public void ChangePlayerCounts1()
    {
        // Decrement itPlayerCount and increment normalPlayerCount
        GameManager.instance.itPlayerCount += 1;
        GameManager.instance.normalPlayerCount -= 1;

        // Call an RPC to synchronize the changes across all players
        photonView.RPC("SyncPlayerCounts", RpcTarget.AllBuffered, GameManager.instance.itPlayerCount, GameManager.instance.normalPlayerCount);
    }
    [PunRPC]
    private void SyncPlayerCounts(int itCount, int normalCount)
    {
        // Update itPlayerCount and normalPlayerCount on all clients
        GameManager.instance.itPlayerCount = itCount;
        GameManager.instance.normalPlayerCount = normalCount;
    }

    [PunRPC]

    void ChangePlayerType(int viewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(viewID);
            GameObject targetObject1 = PhotonView.Find(viewID).gameObject;


        if (targetPhotonView != null)
            {
                GameObject targetObject = targetPhotonView.gameObject;
                if (playerType == PlayerType.IT)
                {
                    playerType = PlayerType.Normal;
                Debug.Log("Normal");
            }
        }
        if (targetObject1 != null)
        {
            Renderer renderer = targetObject1.GetComponent<Renderer>();


            if (renderer != null)
            {
                if (playerType == PlayerType.Normal)
                {
                    renderer.material = newMaterial;
                }
            }
        }

    }
    [PunRPC]
    void ChangePlayerType1(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        GameObject targetObject1 = PhotonView.Find(viewID).gameObject;

        if (targetPhotonView != null)
        {
            GameObject targetObject = targetPhotonView.gameObject;

            if (playerType == PlayerType.Normal)
            {
                playerType = PlayerType.IT;
                Debug.Log("IT");
            }
        }
        if (targetObject1 != null)
        {
            Renderer renderer = targetObject1.GetComponent<Renderer>();

            // Check if the Renderer component exists
            if (renderer != null)
            {
                if (playerType == PlayerType.IT)
                {
                    renderer.material = newMaterial2;
                }
            }
        }
    }

}
    
public enum PlayerType { IT, Normal }
