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
        print("룸 접속 완료");
        // 두 명의 플레이어가 매칭되면 이곳에서 게임을 시작하거나 다른 처리를 할 수 있습니다.
    }
}