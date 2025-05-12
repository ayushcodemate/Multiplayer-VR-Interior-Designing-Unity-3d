using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;
using UnityEngine.Events;

public class UI_Interactabkle : MonoBehaviour
{
    public bool isHover = false;

    public AvatarInfo avatarInfo;

    public bool sofaMenu;
    public bool wallMenu;

    public SofaInteraction sofaInteraction;
    public WallsInteraction wallInteraction;
      

    public XRCardboardInputModule inputModule;

    float currentTime = 0.0f;
    public float TimeToDeselect = 5.0f;

    public UnityEvent eventWhenTimeUp;

   

    public void SetAvatarInfo(AvatarInfo avatarInfoNew)
    {
        avatarInfo = avatarInfoNew;
        if (avatarInfoNew != null) inputModule = avatarInfo.cardboardInputModule;
    }

    public void Update()
    {
        
                if (!isHover)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime > TimeToDeselect)
                    {
                        eventWhenTimeUp.Invoke();
                        currentTime = 0.0f;
                    }
                }
            
            else
            {
                currentTime = 0.0f;
                
            }
        
    }


    public void CloseMenu()
    {
        if (sofaMenu) sofaInteraction.SelectSofa(false);
        if (wallMenu) wallInteraction.SelectWall(false);
    }



}
