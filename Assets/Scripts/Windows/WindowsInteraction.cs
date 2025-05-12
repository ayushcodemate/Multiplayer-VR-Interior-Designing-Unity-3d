using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Interactable))]
public class WindowsInteraction : MonoBehaviour
{
    public GameObject WindowFrameLeft;
    public GameObject WindowFrameRight;
    public AudioClip windowInteractionSound;
    public GameObject canvas;

    public bool isOpen;

    Outline outline;
    Interactable interactable;

    public bool IsAnim = false;

    Animator leftFrameAnim;
    Animator rightFrameAnim;

    [Header("Auto Assign")]
    public GameObject Player;
    public Camera currentCamera;
    public XRCardboardInputModule inputModule;
    public AvatarInfo currentAvatarInfo;

    public PhotonView PV1, PV2;

    void Start()
    {
        outline = this.GetComponent<Outline>();
        interactable = this.GetComponent<Interactable>();
        leftFrameAnim = WindowFrameLeft.GetComponent<Animator>();
        rightFrameAnim = WindowFrameRight.GetComponent<Animator>();
     
       
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
        //Check Which Object is Focusing
        GetCurrentAvatarInfo();
        //If none is Focusing Return


        if (interactable.isHover)
        {
            if (canvas)
            {

                canvas.SetActive(true);
                canvas.transform.LookAt(Player.transform.position);
                canvas.transform.localEulerAngles = new Vector3(canvas.transform.localEulerAngles.x, canvas.transform.localEulerAngles.y, 0.0f);
            }



            if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX"))
            {
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (PV1.IsMine == false)
                        PV1.TransferOwnership(PhotonNetwork.LocalPlayer);

                    if (PV2.IsMine == false)
                        PV2.TransferOwnership(PhotonNetwork.LocalPlayer);


                    StartCoroutine("OpenWindowMultiplayer");
                    return;
                   
                }
                
               

                rightFrameAnim.SetBool("isOpen", !isOpen);
                    leftFrameAnim.SetBool("isOpen", !isOpen);
                    isOpen = !isOpen;
                    AudioSource.PlayClipAtPoint(windowInteractionSound, this.transform.position);
                    if(isOpen)
                    Invoke("CloseDoor", 4);
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

    public void CloseDoor()
    {
        leftFrameAnim.SetBool("isOpen", !isOpen);
        rightFrameAnim.SetBool("isOpen", !isOpen);
        AudioSource.PlayClipAtPoint(windowInteractionSound, this.transform.position);
        isOpen = false;

    }

    IEnumerator OpenWindowMultiplayer()
    {
        yield return new WaitUntil(() => PV1.IsMine && PV2.IsMine);
         rightFrameAnim.SetBool("isOpen", !isOpen);
        leftFrameAnim.SetBool("isOpen", !isOpen);
        isOpen = !isOpen;
        AudioSource.PlayClipAtPoint(windowInteractionSound, this.transform.position);
        if (isOpen)
            Invoke("CloseDoor", 4);
    }


}
