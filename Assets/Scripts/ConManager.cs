using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI ConnectionStatus;
    public TMP_InputField IDtext;
    public Button connectBtn;

    public Transform[] spawnPoints;
    public GameObject canvasParent;

    public GameObject spawner;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        spawnPoints = new Transform[2]; // respawn
        spawnPoints[0] = new GameObject().transform;
        spawnPoints[0].position = new Vector3(10, 3, 10);
        spawnPoints[1] = new GameObject().transform;
        spawnPoints[1].position = new Vector3(-10, 3, -10);
    }

    void Update()
    {
        ConnectionStatus.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void JoinOrCreateRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = IDtext.text;
        PhotonNetwork.JoinOrCreateRoom("MyRoom", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnConnectedToMaster()
    {
        print("hshs conncect server");
        connectBtn.interactable = true;
    }

    public override void OnCreatedRoom() => print("hshs create room");

    public override void OnJoinedRoom()
    {
        print("hshs enter room");
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonNetwork.Instantiate("Player", spawnPoints[0].position, Quaternion.identity);
            Renderer playerRenderer = player.GetComponent<Renderer>();
            playerRenderer.material.color = Color.blue;
        }
        else 
        {
            GameObject player = PhotonNetwork.Instantiate("Player", spawnPoints[1].position, Quaternion.identity);
            Renderer playerRenderer = player.GetComponent<Renderer>();
            playerRenderer.material.color = Color.blue;

        }
        canvasParent.SetActive(false);
        spawner.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("hshs f1");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("hshs f2");

    // public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    // {
    //     print("hs player : " + newPlayer.NickName);
    //     if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
    //     {
    //         if (PhotonNetwork.IsMasterClient) // only master
    //         {
    //             GameObject player1 = PhotonNetwork.Instantiate("Player", spawnPoints[0].position, Quaternion.identity);
    //             GameObject player2 = PhotonNetwork.Instantiate("Player", spawnPoints[1].position, Quaternion.identity);
    //         }
    //     }
    // }

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("room name : " + PhotonNetwork.CurrentRoom.Name);
            print("cur player # : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("max player # : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "who are you???";
            for (int i=0; i<PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
    }
}
