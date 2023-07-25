using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Projectile : MonoBehaviourPunCallbacks
{
    public PhotonView PV; // PhotonView 컴포넌트 추가
    public LayerMask collisionMask;
    float speed = 10;
    float damage = 1;

    float lifeTime = 3;
    float skinWidth = .1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        if (PV.IsMine) // PhotonView가 자신의 것일 때에만 움직임 처리
        {
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
            OnHitObject(hit.collider, hit.point);
        }
    }

    [PunRPC]
    void ApplyDamageToTarget(int viewID, float damage, Vector3 hitPoint, Vector3 hitDirection)
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
        PV.RPC("ApplyDamageToTarget", RpcTarget.All, c.gameObject.GetPhotonView().ViewID, damage, hitPoint, transform.forward);
        GameObject.Destroy(gameObject);
    }
}
