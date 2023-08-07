using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObject : MonoBehaviour
{
    public GameObject prefab;
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj =  PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(parent.transform);
        obj.transform.localPosition = new Vector3(0f, -0.05f, 0.3f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
