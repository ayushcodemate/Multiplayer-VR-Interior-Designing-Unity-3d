using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;


[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Outline))]
public class TVInteraction : MonoBehaviour
{
    public bool isOn = false;
    public VideoPlayer videoPlayer;
    public GameObject canvas;
    public GameObject image;
  

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

        PV = this.GetComponent<PhotonView>();
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
                isOn = !isOn;
                if (isOn)
                {
                    videoPlayer.Play();
                    if (PV != null)
                    {
                        PV.RPC("ToggleTVMultiplayer", RpcTarget.All, true, PhotonNetwork.LocalPlayer.NickName);

                    }
                }
                else
                {
                    videoPlayer.Stop();
                }

                image.SetActive(isOn);
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
            if (canvas)
            {
                canvas.SetActive(false);
            }
        }

        outline.enabled = interactable.isHover;

        
    }

    [PunRPC]
    public void ToggleTVMultiplayer(bool value,string playerName)
    {
        isOn = value;
        if(playerName != PhotonNetwork.LocalPlayer.NickName)
        {
            if(value)
            {
                videoPlayer.Play();
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    ToastNotification.Instance.SendMessage(playerName + " has turned on " + gameObject.name);
                }
            }
            else
            {
                videoPlayer.Stop();
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    ToastNotification.Instance.SendMessage(playerName + " has turned off " + gameObject.name);
                }
            }


            image.SetActive(isOn);
        }


    }
}
