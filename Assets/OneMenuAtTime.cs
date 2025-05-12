using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneMenuAtTime : MonoBehaviour
{
    public UI_Interactabkle CurrentMenu;

    ItemsDeployer deployer;
    void Start()
    {
        deployer = this.GetComponent<ItemsDeployer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetCurrentMenu(UI_Interactabkle menu,bool isDeployer)
    {
        if (menu == null && isDeployer)
        {
            if(CurrentMenu!=null) CurrentMenu.CloseMenu();

        }
        else
        {
            if(isDeployer == false)
            {
                if(deployer.DrawingRoomDeployer)
                deployer.DrawingRoomDeployer.SetActive(false);
                if(deployer.LivingRoomDeployer)
                deployer.LivingRoomDeployer.SetActive(false);
            }
            if (CurrentMenu == null)
            {
                CurrentMenu = menu;
            }
            else
            {
                CurrentMenu.CloseMenu();

                CurrentMenu = menu;
            }
        }
    }


    
}
