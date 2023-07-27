using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class GunController : MonoBehaviourPun
{
    public Transform weaponHold;
    public Gun[] allGuns;
    Gun equippedGun;

    public void Start()
    {
        
    }
    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate (gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGuns[weaponIndex]);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gun0"))
        {
            EquipGun(0);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Gun1"))
        {
            EquipGun(1);
            Destroy(other);
        }
        if (other.gameObject.CompareTag("Gun2"))
        {
            EquipGun(2);
            Destroy(other);
        }
        if (other.gameObject.CompareTag("Gun3"))
        {
            EquipGun(3);
            Destroy(other);
        }
    }

    public void OnTriggerHold()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }
}
