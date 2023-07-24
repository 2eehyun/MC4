// // Player.cs part

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class PlayerScript : LivingEntity, IPunObservable
{
    public float moveSpeed = 5;
    public PhotonView PV;
    public TextMeshProUGUI NickNameText;
    public Image HealthImage;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;
    Vector3 curPos;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Awake()
    {
        // 닉네임
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        if (PV.IsMine)
        {
            // 위의 것 대신 3D 카메라 위치 변경해야함
        }
    }

    void Update()
    {
        if (PV.IsMine)
        {
            // Movement input
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);

            // Look input
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                // Debug.DrawLine(ray.origin, point, Color.red);
                controller.LookAt(point);
            }

            // Weapon input
            if (Input.GetMouseButton(0))
            {
                gunController.Shoot();
            }
            // 여기는 어떻게해야함 ? 그냥 rotation 정보 주기 ?
            PV.RPC("SyncRotation", RpcTarget.Others, transform.rotation);
        }

        // IsMine이 아닌 것들은 부드럽게 위치 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    [PunRPC]
    void SetPlayerPosition(Vector3 position)
    {
        // 플레이어 위치 설정 함수 (PlayerScript에서 구현)
        transform.position = position;
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
            stream.SendNext(transform.rotation); // 회전 정보 동기화
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext(); // 회전 정보 수신 및 적용
        }
    }
}
