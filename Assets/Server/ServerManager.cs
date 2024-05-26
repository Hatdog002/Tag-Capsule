using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("You are connected");

        PhotonNetwork.JoinLobby(); // connects to lobby 
        base.OnConnectedToMaster();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        base.OnJoinedLobby();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created " + PhotonNetwork.CurrentRoom.Name.ToString());
        base.OnCreatedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed Joined Room");
        base.OnJoinRandomFailed(returnCode, message);
    }


    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
        base.OnJoinedRoom();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
