using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    public TMP_InputField roomNumberInput;
   public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomNumberInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomNumberInput.text);
    }
}
