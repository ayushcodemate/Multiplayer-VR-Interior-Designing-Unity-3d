using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Outline))]
public class KitchenBenchAnim : MonoBehaviour
{
    public Animator mainAnimator;
    public string param;

    public bool isOpen = false;

    public GameObject canvas;

    public AudioClip interactionSound;

    Interactable interactable;
    Outline outline;


    [Header("Auto Assign")]
    public GameObject Player;
    public Camera currentCamera;
    public XRCardboardInputModule inputModule;
    public AvatarInfo currentAvatarInfo;

    PhotonView PV;
    public PhotonView ParentPV;

    void Start()
    {
        interactable = this.GetComponent<Interactable>();
        outline = this.GetComponent<Outline>();
        //if (!Player) Player = GameObject.FindGameObjectWithTag("Player");

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
            if (canvas)
            {

                canvas.SetActive(true);
                canvas.transform.LookAt(Player.transform.position);
                canvas.transform.localEulerAngles = new Vector3(canvas.transform.localEulerAngles.x, canvas.transform.localEulerAngles.y, 0.0f);
            }


            if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX"))
            {
                isOpen = !isOpen;
                if (PV != null) PV.RPC("KitchenDoorMP", RpcTarget.All, isOpen);
                mainAnimator.SetBool(param, isOpen);

                AudioSource.PlayClipAtPoint(interactionSound, this.transform.position);
                if (isOpen)
                    Invoke("CloseItem", 4);
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


    public void CloseItem()
    {
        mainAnimator.SetBool(param, isOpen);
        AudioSource.PlayClipAtPoint(interactionSound, this.transform.position);
        isOpen = false;

    }


    [PunRPC]
    public void KitchenDoorMP(bool value)
    {
        if(ParentPV.IsMine == false)
        ParentPV.TransferOwnership(PhotonNetwork.LocalPlayer);
        if(PV.IsMine == false)
        PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        StartCoroutine("ChangeOwnership",value);
        
    }


    IEnumerator ChangeOwnership(bool value)
    {
        yield return new WaitUntil(() => ParentPV.IsMine == true && PV.IsMine == true);

        isOpen = value;
        mainAnimator.SetBool(param, isOpen);

        if (isOpen)
            Invoke("CloseItem", 4);

    }
}
