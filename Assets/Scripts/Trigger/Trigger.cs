using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTriggerStay;

    public string areaName;

    private void Awake()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
         if(other.tag == "Player")
        {
            ItemsDeployer itemsDeployer = other.GetComponent<ItemsDeployer>();
            itemsDeployer.ChangeCurrentArea(areaName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke();
    }
}
