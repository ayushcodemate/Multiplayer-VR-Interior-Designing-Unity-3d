using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    [Header("This Will be Assigned Automatically")]
    [SerializeField]
    public AvatarInfo avatarInfo;
    GameInfo gameInfo;
   public PhotonView photonView;

    public bool isMultiplayer;

    private void Awake()
    {
        //FillAvatarInfo();
       
    }

    private void Start()
    {
        FillAvatarInfo();
        gameInfo = GameObject.FindAnyObjectByType<GameInfo>();
        gameInfo.avatarInfo = avatarInfo;
        isMultiplayer = MainMenu.isOnline;
    }

    void FillAvatarInfo()
    {
        avatarInfo.camera = this.GetComponentInChildren<Camera>().gameObject;
        avatarInfo.Raycaster = this.GetComponentInChildren<Raycaster>();
        avatarInfo.cardboardInputModule = this.GetComponentInChildren<XRCardboardInputModule>();
        avatarInfo.itemsDeployer = this.GetComponentInChildren<ItemsDeployer>();
        avatarInfo.characterMovement = this.GetComponentInChildren<CharacterMovement>();
        avatarInfo.anim = this.GetComponent<Animator>();
        avatarInfo.nvrBody = this.GetComponentInChildren<NvrBody>();
        avatarInfo.oneMenu = this.GetComponentInChildren<OneMenuAtTime>();
        if(MainMenu.isOnline)
        {
            if(photonView.IsMine)
            {
                avatarInfo.characterMovement.enabled = true;
                avatarInfo.camera.gameObject.SetActive(true);
                avatarInfo.nvrBody.enabled = true;
                avatarInfo.Raycaster.enabled = true;
                avatarInfo.itemsDeployer.enabled = true;
                avatarInfo.NotificationCanvas.SetActive(true);
                avatarInfo.oneMenu.enabled = true;
            }
            else
            {
                avatarInfo.characterMovement.enabled = false;
                avatarInfo.camera.gameObject.SetActive(false);
                avatarInfo.nvrBody.enabled = false;
                avatarInfo.Raycaster.enabled = false;
                avatarInfo.itemsDeployer.enabled = false;
                avatarInfo.NotificationCanvas.SetActive(false);
                avatarInfo.oneMenu.enabled = false;
            }
        }
    }


}

[System.Serializable]
public class AvatarInfo
{
    public GameObject camera;
    public Raycaster Raycaster;
    public XRCardboardInputModule cardboardInputModule;
    public ItemsDeployer itemsDeployer;
    public CharacterMovement characterMovement;
    public Animator anim;
    public NvrBody nvrBody;
    public GameObject NotificationCanvas;
    public OneMenuAtTime oneMenu;
}
