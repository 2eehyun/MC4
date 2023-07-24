// // Player.cs part

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;
// using UnityEngine.UI;
// using Cinemachine;

// [RequireComponent (typeof (PlayerController))]
// [RequireComponent(typeof(GunController))]
// public class PlayerScript : LivingEntity, IPunObservable
// {
//     public float moveSpeed = 5;
//     public PhotonView PV;
//     public Text NickNameText;
//     public Image HealthImage;

//     Camera viewCamera;
//     PlayerController controller;
//     GunController gunController;
//     protected override void Start()
//     {
//         base.Start();
//         controller = GetComponent<PlayerController>();
//         gunController = GetComponent<GunController>();
//         viewCamera = Camera.main;
//     }

//     void Awake()
//     {
//         // 닉네임
//         NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
//         NickNameText.color = PV.IsMine ? Color.green : Color.red;

//         if (PV.IsMine)
//         {
//             // 2D 카메라
//             // var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
//             // CM.Follow = transform;
//             // CM.LookAt = transform;

//             // 위의 것 대신 3D 카메라 위치 변경해야함
//         }
//     }

//     void Update()
//     {
//         if (PV.IsMine)
//         {
//             // Movement input
//             Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
//             Vector3 moveVelocity = moveInput.normalized * moveSpeed;
//             controller.Move(moveVelocity);

//             // Look input
//             Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
//             Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
//             float rayDistance;

//             if (groundPlane.Raycast(ray, out rayDistance))
//             {
//                 Vector3 point = ray.GetPoint(rayDistance);
//                 // Debug.DrawLine(ray.origin, point, Color.red);
//                 controller.LookAt(point);
//             }

//             // Weapon input
//             if (Input.GetMouseButton(0))
//             {
//                 gunController.Shoot();
//             }


//             // 여기는 어떻게해야함 ? 그냥 rotation 정보 주기 ?
//             if (axis != 0)
//             {
//                 AN.SetBool("walk", true);
//                 PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis); // 재접속시 filpX를 동기화해주기 위해서 AllBuffered
//             }
//             else AN.SetBool("walk", false);

//             // 스페이스 총알 발사
//             if (Input.GetKeyDown(KeyCode.Space))
//             {
//                 PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(SR.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
//                     .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
//                 AN.SetTrigger("shot");
//             }


//         }

//         // IsMine이 아닌 것들은 부드럽게 위치 동기화
//         else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
//         else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
//     }

//     public void Hit()
//     {
//         HealthImage.fillAmount -= 0.1f;
//         if (HealthImage.fillAmount <= 0)
//         {
//             GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
//             PV.RPC("DestroyRPC", RpcTarget.AllBuffered); // AllBuffered로 해야 제대로 사라져 복제버그가 안 생긴다
//         }
//     }

//     [PunRPC]
//     void DestroyRPC() => Destroy(gameObject);
    
//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         if (stream.IsWriting)
//         {
//             stream.SendNext(transform.position);
//             stream.SendNext(HealthImage.fillAmount);
//             // 아마 방향도 필요함
//         }
//         else
//         {
//             curPos = (Vector3)stream.ReceiveNext();
//             HealthImage.fillAmount = (float)stream.ReceiveNext();
//             // 아마 방향도 필요함
//         }
//     }
// }
