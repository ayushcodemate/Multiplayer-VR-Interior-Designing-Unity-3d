using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deployer : MonoBehaviour
{
    public ItemsDeployer itemDeployer;
    public GameObject Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
          
            this.transform.LookAt(Player.transform.position);
            this.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
       
    }

    public void SelectSofa(int id)
    {
        itemDeployer.SelectSofa(id);
    }

    public void SelectDC(int id)
    {
        itemDeployer.SelectDC(id);
    }

    public void SelectDT(int id)
    {
        itemDeployer.SelectDT(id);
    }

    public void SelectDecorative(int id)
    {
        itemDeployer.SelectDeocrative(id);
    }

    public void SelectTable(int id)
    {
        itemDeployer.SelectTable(id);
    }


}
