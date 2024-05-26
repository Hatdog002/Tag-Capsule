using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private int randomNumber;
    public GameObject[] spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            randomNumber = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        }
        GameManager.instance.CreatePlayer(randomNumber);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
