using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class ObstacleItem : LivingEntity
{
    public PhotonView PV;

    public Gun machineGun;

    protected override void Start()
    {
        base.Start();
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        PV.RPC("TakeHitRPC", RpcTarget.All, damage, hitPoint, hitDirection);
    }

    [PunRPC]
    void TakeHitRPC(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // ������ ���� ���ظ� �Դ� ������ �����մϴ�.
        if (damage >= health)
        {
            Destroy(Instantiate(machineGun), 20f);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }
}
