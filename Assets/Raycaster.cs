using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Raycaster : MonoBehaviour
{
    Camera playerCamera;
    public float raycastLen = 10f;
    public GameObject currentObject;

    public Interactable interactable;
    public bool isOverUI;

    public Avatar avatar;
    public AvatarInfo avatarInfo;

    public bool anyThingSelected;
    public UI_Interactabkle uiInteractable;

    public XRCardboardInputModule inputModule;
    
    void Start()
    {
        if (avatar == null) avatar = transform.GetComponentInParent<Avatar>();

        avatarInfo = avatar.avatarInfo;

        playerCamera = avatarInfo.camera.GetComponent<Camera>();
        inputModule = avatarInfo.cardboardInputModule;
    }

    // Update is called once per frame
    void Update()
    {
        isOverUI = IsPointerOverUIObject();

        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (!isOverUI)
        {
            if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, raycastLen))
            {
                if (hit.collider.gameObject != currentObject && currentObject && interactable)
                {
                    interactable.isHover = false;
                    interactable.SetAvatarInfo(null);
                   
                }

                currentObject = hit.collider.gameObject;

                if (currentObject.GetComponent<Interactable>())
                {
                    interactable = currentObject.GetComponent<Interactable>();
                    interactable.isHover = true;
                    interactable.SetAvatarInfo(avatarInfo);
                }


            }
            else
            {
               
                if (interactable.gameObject)
                {
                    interactable.isHover = false;
                }
                currentObject = null;

            }
        }
        else
        {
           
            if (interactable.gameObject)
            {
                interactable.isHover = false;
            }
            currentObject = null;
        }
    }

    private bool IsPointerOverUIObject()
    {
        if (inputModule.currentTarget)
        {
            if(inputModule.currentTarget.layer == LayerMask.NameToLayer("UI"))
            {
                UI_Interactabkle currentUIinteractable = inputModule.currentTarget.GetComponentInParent<UI_Interactabkle>();

                if (currentUIinteractable != uiInteractable && uiInteractable && currentUIinteractable)
                {
                    uiInteractable.isHover = false;
                    uiInteractable.SetAvatarInfo(null);

                }

                if(currentUIinteractable != null)
                {
                    uiInteractable = currentUIinteractable;
                    uiInteractable.isHover = true;
                    uiInteractable.SetAvatarInfo(avatarInfo);
                }
                

                return true;
            }
            else
            {
                if (uiInteractable)
                {
                    uiInteractable.isHover = false;
                    uiInteractable.SetAvatarInfo(null);
                    uiInteractable = null;
                }
                return false;
            }
        }
        else
        {
            if (uiInteractable)
            {
                uiInteractable.isHover = false;
                uiInteractable.SetAvatarInfo(null);
                uiInteractable = null;
            }
            return false;
        }
    }
}
