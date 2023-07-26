using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Shell : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Rigidbody myRigidbody;
    public float forceMin;
    public float forceMax;

    float lifetime = 3;
    float fadetime = 1.5f;
    Vector3 curPos;

    void Start()
    {
        if (PV.IsMine)
        {
            float force = Random.Range(forceMax, forceMin);
            myRigidbody.AddForce(transform.right * force);
            myRigidbody.AddTorque(Random.insideUnitSphere * force);

            StartCoroutine(Fade());
        }
        
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColour = mat.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColour, Color.clear, percent);
            yield return null;
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
