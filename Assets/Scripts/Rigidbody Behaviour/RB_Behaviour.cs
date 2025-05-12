using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Behaviour : MonoBehaviour
{
    public bool isKinematic;

    Rigidbody rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isKinematic) rb.isKinematic = true;
    }
}
