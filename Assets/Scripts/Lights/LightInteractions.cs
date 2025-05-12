using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Outline))]
public class LightInteractions : MonoBehaviour
{
    public GameObject LightObject;
    public bool lightOn = true;
   

    [Header("Canvas Objects")]
    public GameObject canvas;

    Interactable interactable;
    Outline outline;

    [Header("Auto Assign")]
    public GameObject Player;
    public Camera currentCamera;
    public XRCardboardInputModule inputModule;
    public AvatarInfo currentAvatarInfo;

    PhotonView PV;

    void Start()
    {
        interactable = this.GetComponent<Interactable>();
        outline = this.GetComponent<Outline>();

        if (PV == null) PV = this.GetComponent<PhotonView>();
    }

    void GetCurrentAvatarInfo()
    {
        if (interactable.isHover)
        {
            currentAvatarInfo = interactable.avatarInfo;
            Player = currentAvatarInfo.camera;
            inputModule = currentAvatarInfo.cardboardInputModule;
            currentCamera = currentAvatarInfo.camera.GetComponent<Camera>();
            return;
        }
        else
        {
            currentAvatarInfo = null;
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentAvatarInfo();

        if (interactable.isHover)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX"))
            {
                lightOn = !lightOn;
                LightObject.SetActive(lightOn);

                if (PV != null)
                {
                    PV.RPC("LightMultiplayer", RpcTarget.All, lightOn, PhotonNetwork.LocalPlayer.NickName);

                }
            }

            if (canvas)
            {
                canvas.SetActive(true);
                canvas.transform.LookAt(Player.transform.position);
                canvas.transform.localEulerAngles = new Vector3(canvas.transform.localEulerAngles.x, canvas.transform.localEulerAngles.y, 0.0f);
            }
        }
        else
        {
            if(canvas)
            {
                canvas.SetActive(false);
            }
        }

        outline.enabled = interactable.isHover;

    }


    [PunRPC]
    public void LightMultiplayer(bool value,string name)
    {
        lightOn = value;
        if(name != PhotonNetwork.LocalPlayer.NickName)
        {
            LightObject.SetActive(lightOn);
            if(value)
            ToastNotification.Instance.SendMessages(name + " Turned on light");
            else
                ToastNotification.Instance.SendMessages(name + " Turned off light");
        }
    }


}
