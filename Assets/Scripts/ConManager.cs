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

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        spawnPoints[0] = new GameObject().transform;
        spawnPoints[0].position = new Vector3(0, 1, 2);
        spawnPoints[1] = new GameObject().transform;
        spawnPoints[1].position = new Vector3(0, 1, -2);
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
        print("서버 접속 완료");
        connectBtn.interactable = true;
    }

    public override void OnCreatedRoom() => print("룸 생성 완료");

    public override void OnJoinedRoom() => print("룸 접속 완료");

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        print("플레이어 입장: " + newPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient) // 방장만 처리하도록 변경
            {
                // 첫 번째 플레이어 생성
                GameObject player1 = PhotonNetwork.Instantiate("Player", spawnPoints[0].position, Quaternion.identity);
                player1.GetComponent<PlayerScript>().PV.RPC("SetPlayerPosition", RpcTarget.AllBuffered, spawnPoints[0].position);

                // 두 번째 플레이어 생성
                GameObject player2 = PhotonNetwork.Instantiate("Player", spawnPoints[1].position, Quaternion.identity);
                player2.GetComponent<PlayerScript>().PV.RPC("SetPlayerPosition", RpcTarget.AllBuffered, spawnPoints[1].position);

                PhotonNetwork.LoadLevel("player2");
            }
        }
    }

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원 수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대 인원 수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i=0; i<PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
    }
}
