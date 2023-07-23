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

    public float repeatInterval = 1.0f;
    private bool isRepeating = false;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
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

    public override void OnCreatedRoom() => print("룸 생성 완료: " + PhotonNetwork.CurrentRoom.Name);

    public override void OnJoinedRoom() => print("룸 접속 완료: " + PhotonNetwork.CurrentRoom.Name);

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");

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
