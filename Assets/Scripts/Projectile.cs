using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Projectile : MonoBehaviourPunCallbacks
{
    public PhotonView PV; // PhotonView 컴포넌트 추가
    public LayerMask collisionMask;
    public Color trailColour;
    float speed = 10;
    float damage = 1;

    float lifeTime = 3;
    float skinWidth = .1f;

    Vector3 curPos;

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
            float moveDistance = speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    [PunRPC]
    void ApplyDamageToTarget(int viewID, float damage, Vector3 hitPoint, Vector3 hitDirection, Collider c)
    {
        
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
            IDamageable enemyObject = c.GetComponent<IDamageable>();
            if (enemyObject != null)
            {
                enemyObject.TakeHit(damage, hitPoint, transform.forward);
            }
            GameObject.Destroy(gameObject);
        }
        else if (c.gameObject.CompareTag("Player"))
        {
            PV.RPC("ApplyDamageToTarget", RpcTarget.All, c.gameObject.GetPhotonView().ViewID, damage, hitPoint, transform.forward, c);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        } else
        {
            GameObject.Destroy(gameObject);
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
}
