using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public static GameManager instance = null;

    [Header("OBJECTIVES")]
    //public int teamScore;
    public int teamGoal;
    public int itGoal;
    //public int itScore;
    public int itPlayerCount;
    public int normalPlayerCount;
    public Material newMaterial;
    [Header("ShowScore")]
    public Color color;
    public TextMeshProUGUI scoreTeam;
    public TextMeshProUGUI scoreIt;
    PlayerType playerType;
    public GameObject NormalplayerWin;
    public GameObject ITplayerWin;
    public GameObject playerLose;
    public SpawnManager sPoint;
    //public PlayerTag playerTag;
    public bool ItWin;
    public bool NormalWin;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
      
    }


    // Update is called once per frame
    void Update()
    {
        teamGoal = itPlayerCount;
        itGoal = normalPlayerCount;

        // Update UI text with synchronized player counts
        scoreTeam.text = "Normal Goal: " + teamGoal;
        scoreIt.text = "It Goal: " + itGoal;

        if (NormalWin == true)
        {
            GameOver(PlayerType.Normal);
        }
        
        if (ItWin == true)
        {
            GameOver(PlayerType.IT);
        }
    }
    public void UpdatePlayerCounts()
    {
        // Call an RPC to synchronize the player counts across all players
        photonView.RPC("SyncPlayerCounts", RpcTarget.AllBuffered, itPlayerCount, normalPlayerCount, teamGoal, itGoal);
    }
    [PunRPC]
    private void SyncPlayerCounts(int itCount, int normalCount, int tGoal, int iGoal)
    {
        itPlayerCount = itCount;
        normalPlayerCount = normalCount;
        teamGoal = tGoal;
        itGoal = iGoal;

        // Update UI text with synchronized player counts
        scoreTeam.text = "Normal Goal: " + teamGoal;
        scoreIt.text = "It Goal: " + itGoal;
    }

    public void CreatePlayer(int randomNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("InstantiatePlayer", RpcTarget.AllBufferedViaServer, randomNumber);
        }
    }


    [PunRPC]
    void InstantiatePlayer(int randomNumber)
    {
        int idNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        // Calculate the number of IT players
        int numITPlayers = totalPlayers / 2;

        if (totalPlayers % 2 != 0)
        {
            // If the total number of players is odd, decrease the number of IT players by 1
            numITPlayers--;
        }

        // Calculate the spawn number for each player
        int spawnNumber = (idNumber - 1) % totalPlayers;

        // Check if the current player should be IT
        bool isTag = (idNumber <= numITPlayers);

        if (isTag)
        {
            playerType = PlayerType.IT;
            PhotonNetwork.Instantiate("CharacterEnemy", sPoint.spawnPoint[spawnNumber].transform.position, Quaternion.identity);
        }
        else
        {
            playerType = PlayerType.Normal;
            PhotonNetwork.Instantiate("CharacterPlayer", sPoint.spawnPoint[spawnNumber].transform.position, Quaternion.identity);
        }
    }
                
    [PunRPC]
    public void UpdatePlayerCountsRPC(int itCount, int normalCount)
    {
        // Update player counts on all clients
        itPlayerCount = itCount;
        normalPlayerCount = normalCount;

        // Do any additional processing or UI updates as needed
        Debug.Log("Updated Player Counts - IT Players: " + itPlayerCount + ", Normal Players: " + normalPlayerCount);
    }


    [PunRPC]
    public void SendGameOver(byte playerWon)
    {
        PlayerType winningPlayerType = (PlayerType)playerWon;

        // Determine if the local player has won
        bool isWinner = (winningPlayerType == playerType);

        // Call the appropriate method to display win/lose UI for the local player
        if (isWinner)
        {
            ShowWinUI();
        }
        else
        {
            ShowLoseUI();
        }

        // Wait for a brief period before restarting the game
        StartCoroutine(RestartGame());
    }

    private void ShowWinUI()
    {
        // Display win UI specific to the local player
        if (playerType == PlayerType.IT)
        {
            ITplayerWin.SetActive(true);
        }
        else if (playerType == PlayerType.Normal)
        {
            // Show win UI for Normal player
            NormalplayerWin.SetActive(true);
        }
    }

    private void ShowLoseUI()
    {
        // Display lose UI specific to the local player
        playerLose.SetActive(true);
    }

    private IEnumerator RestartGame()
    {
        // Wait for a brief period before restarting the game
        yield return new WaitForSeconds(2f);
        ItWin = false;
        NormalWin = false;
        // Restart the game by reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver(PlayerType winningPlayer)
    {
        // Call an RPC to inform all players about the game over event
        photonView.RPC("SendGameOver", RpcTarget.AllBufferedViaServer, (byte)winningPlayer);
        Debug.Log("Game Over: " + winningPlayer + " wins!");
    }

}
