using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI ConnectionStatus;
    public TMP_InputField IDtext;
    public Button connetBtn;

    void Start()
    {
        // Photon PUN 초기화
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        ConnectionStatus.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void ConnectAndJoinOrCreateRoom()
    {
        // 서버에 플레이어 이름 설정
        PhotonNetwork.LocalPlayer.NickName = IDtext.text;

        // 동일한 룸 이름으로 룸에 접속하거나 생성
        PhotonNetwork.JoinOrCreateRoom("MyRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        print("서버 접속 완료");
        
        // 서버에 연결되면 Connect 버튼을 누를 때 매칭을 시도
        connetBtn.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        print("룸 접속 완료: " + PhotonNetwork.CurrentRoom.Name);

        // 두 명의 플레이어가 매칭되면 이곳에서 게임을 시작하거나 다른 처리를 할 수 있습니다.

        // 정보를 주고받을 함수 호출
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트는 SendInfoToClient 함수를 호출하여 정보를 전달
            SendInfoToClient();
        }
        else
        {
            // 마스터 클라이언트가 아닌 경우, 마스터 클라이언트로부터 정보를 받아올 준비를 함
            photonView.RPC("RequestInfoFromMasterClient", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        }
    }

    // 정보를 주고받을 함수들
    [PunRPC]
    void SendInfoToClient()
    {
        // 마스터 클라이언트에서 다른 플레이어에게 정보를 전달하는 로직
        string message = "Hello, other client!";
        photonView.RPC("ReceiveInfoFromMasterClient", RpcTarget.Others, message);
    }

    [PunRPC]
    void ReceiveInfoFromMasterClient(string message)
    {
        // 마스터 클라이언트로부터 정보를 받아오는 로직
        print("Received message: " + message);
    }
}
