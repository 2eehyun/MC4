using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Gun : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;

    public Transform muzzle;
    public GameObject projectilePrefab; // GameObject 타입으로 변경
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;

    public Transform shell;
    public GameObject shellPrefab; // GameObject 타입으로 변경
    public Transform shellEjection;
    MuzzleFlash muzzleflash;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;

    private void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
    }

    float nextShotTime;

    // [PunRPC]
    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            // print("shooooooot!");

            nextShotTime = Time.time + msBetweenShots / 1000;
            GameObject newProjectile = PhotonNetwork.Instantiate(projectilePrefab.name, muzzle.position, muzzle.rotation);
            newProjectile.GetComponent<Projectile>().SetSpeed(muzzleVelocity);
            PhotonNetwork.Instantiate(shellPrefab.name, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }

    // [PunRPC]
    public void OnTriggerHold()
    {
        // PV.RPC("Shoot", RpcTarget.All);
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}


