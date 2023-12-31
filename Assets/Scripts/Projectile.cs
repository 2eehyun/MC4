using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Projectile : MonoBehaviourPunCallbacks
{
    public PhotonView PV; // PhotonView 컴포넌트 추가
    public LayerMask collisionMask;
    public Color trailColour;
    public Image HealthImage;
    public float speed = 10;
    public float damage = 1;

    float lifeTime = 3;
    float skinWidth = .1f;

    private void Start()
    {
        if (PV.IsMine)
        {
            Destroy(gameObject, lifeTime);
            Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
            if (initialCollisions.Length > 0)
            {
                OnHitObject(initialCollisions[0], transform.position);
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        if (PV.IsMine)
        {
            // print("test1");
            float moveDistance = speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            // print("제발");
            OnHitObject(hit.collider, hit.point);
        }
    }

    [PunRPC]
    void ApplyDamageToTarget(int viewID, float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        Debug.Log("맞았어요");
        PhotonView targetPV = PhotonView.Find(viewID);
        if (targetPV && targetPV.gameObject != null)
        {
            IDamageable damageableObject = targetPV.gameObject.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeHit(damage, hitPoint, hitDirection);
            }
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        if (c.gameObject.CompareTag("Enemy"))
        {
            // print("아프겠다e");
            IDamageable enemyObject = c.GetComponent<IDamageable>();
            if (enemyObject != null)
            {
                enemyObject.TakeHit(damage, hitPoint, transform.forward);
            }
            DestroyRPC();
        }
        else if (c.gameObject.CompareTag("Player"))
        {
            PV.RPC("ApplyDamageToTarget", RpcTarget.All, c.gameObject.GetPhotonView().ViewID, damage, hitPoint, transform.forward);
            DestroyRPC();
        }
        else if (c.gameObject.CompareTag("Obstacle"))
        {
            IDamageable obstacleObject = c.GetComponent<IDamageable>();
            if (obstacleObject != null)
            {
                obstacleObject.TakeHit(damage, hitPoint, transform.forward);
            }
            DestroyRPC();
        }
    }


    [PunRPC]
    void DestroyRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
