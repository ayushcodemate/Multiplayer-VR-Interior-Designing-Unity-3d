using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[RequireComponent(typeof(Interactable))]
public class DoorInteraction : MonoBehaviour
{
    public bool focused = false;
    public bool isOpen = false;
    public bool canOpened = false;

    [Header("Animation Stuff")]
    public Animator animator;

    [Header("UI Stuff")]
    public GameObject UI_Text1;
    public GameObject UI_Text2;

    Outline outline;

    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    Interactable interactable;

    PhotonView PV;

    void Start()
    {
        outline = this.GetComponent<Outline>();
        animator = this.GetComponent<Animator>();

        if (outline) outline.enabled = false;

        interactable = this.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canOpened && interactable.isHover)
        {
            if(Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX"))
            {

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (this.GetComponent<PhotonView>().Owner != PhotonNetwork.LocalPlayer)
                    {
                        PV = this.GetComponent<PhotonView>();

                        PV.TransferOwnership(PhotonNetwork.LocalPlayer);

                        StartCoroutine("DoLaterThings");
                        return;
                    }
                  
                }

                
                animator.SetBool("OpenDoor", !isOpen);
                isOpen = true;
                AudioSource.PlayClipAtPoint(doorOpenSound, this.transform.position);
                Invoke("CloseDoor", 4);
            }
        }

        if (outline) outline.enabled = interactable.isHover;
    }


    IEnumerator DoLaterThings()
    {
        yield return new WaitUntil(() => PV.IsMine);
        animator.SetBool("OpenDoor", !isOpen);
        isOpen = true;
        AudioSource.PlayClipAtPoint(doorOpenSound, this.transform.position);
        Invoke("CloseDoor", 4);
    }


    public void CloseDoor()
    {
        animator.SetBool("OpenDoor", !isOpen);
        AudioSource.PlayClipAtPoint(doorCloseSound, this.transform.position);
        isOpen = false;
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !isOpen && interactable.isHover)
        {
            canOpened = true;
            UI_Text1.SetActive(true);
            UI_Text2.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            canOpened = false;
            UI_Text1.SetActive(false);
            UI_Text2.SetActive(false);
        }
    }

    
}
