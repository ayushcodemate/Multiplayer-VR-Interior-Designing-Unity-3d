using JetBrains.Annotations;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.StandaloneInputModule;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Interactable))]
public class WallsInteraction : MonoBehaviour 
{

    public bool focused;
    public bool isSelected = false;

    [System.Serializable]
    public class DifferentTextures
    {
        public Texture tex;
        public Vector2 tiling;
        public Vector2 offSet;
    }
    [SerializeField] public DifferentTextures[] textures;
    [SerializeField] public Color[] colors;

    public int currentTex = 0;
    public int currentColor = 0;

    public AudioClip changeSound;
   
    public GameObject canvasToDisplay;
    UI_Interactabkle canvasInteractable;



    [Header("Enable/Disable Items")]
    public GameObject[] objectsToHide;

    //References
    Outline outline;
    MeshRenderer meshRenderer;
    Material mat;

    public float distance = 2f;


    public float TimeToDeselect = 5f;
    float currentTime = 0.0f;
   

    Ray myRay;      // initializing the ray
    RaycastHit hit; // initializing the raycasthit
    Interactable interactable;


    [Header("Auto Assign")]
    public GameObject Player;
    public Camera currentCamera;
    public XRCardboardInputModule inputModule;
    public AvatarInfo currentAvatarInfo;

    PhotonView PV;

    void Start()
    {
        outline = this.GetComponent<Outline>();
        meshRenderer = this.GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        interactable = this.GetComponent<Interactable>();

        PV = this.GetComponent<PhotonView>();

        canvasInteractable = canvasToDisplay.GetComponent<UI_Interactabkle>();

        //objectsToHide = this.transform.GetComponentsInChildren<GameObject>();
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
            if ((Input.GetKeyDown(KeyCode.X)|| Input.GetButtonDown("jsX")) )
            {
                if (isSelected == true) ToastNotification.Instance.SendMessages("Wall is being edited by other player");
                if (!isSelected)
                {
                    SelectWall(true);
                }
                //ChangeTexture();
            }
        }

        if (canvasToDisplay.activeInHierarchy)
        {
            canvasToDisplay.transform.LookAt(Player.transform.position);
            canvasToDisplay.transform.localEulerAngles = new Vector3(0, canvasToDisplay.transform.localEulerAngles.y, 0.0f);
        }
            
        

        if (outline) outline.enabled = interactable.isHover;

        if (!interactable.isHover && canvasToDisplay.activeInHierarchy)
        {
            currentTime+= Time.deltaTime;
            if(currentTime > TimeToDeselect)
            {
                SelectWall(false);
                currentTime = 0.0f;
            }
        } 
    }


    /// <summary>
    /// When Wall is selected canvas will start displaying.
    /// Not important objects will be hided to give clear focus on selected object.
    /// </summary>
    /// <param name="value"></param>
    public void SelectWall(bool value)
    {
        isSelected = value;

        currentTime = 0.0f;
        
            canvasToDisplay.SetActive(value);
        if (value) currentAvatarInfo.oneMenu.SetCurrentMenu(canvasInteractable, false);
            myRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            canvasToDisplay.transform.position = myRay.GetPoint(distance);
        

        outline.OutlineColor = (value) ?Color.blue : Color.white;

        if(objectsToHide.Length > 0)
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(!value);
        }
    }


    //This Function will change the texture of the walls according to the given id and play sound when texture is changed
    public void ChangeTexture(int id)
    {
        currentTex = id - 1;
        if (currentTex > textures.Length - 1) currentTex = 0;
        mat.mainTexture = textures[currentTex].tex;
        mat.mainTextureScale = textures[currentTex].tiling;
        mat.color = Color.white;

        if (changeSound) AudioSource.PlayClipAtPoint(changeSound, this.transform.position);

        PV.RPC("RPC_ChangedWallTexture", RpcTarget.All, currentTex,PhotonNetwork.LocalPlayer.NickName);
    }

    /// <summary>
    /// This fuction will remove the texture 
    /// </summary>
    public void NoTexture()
    {
        mat.mainTexture = null;
    }

    /// <summary>
    /// This function is used to change the colors of wall.
    /// </summary>
    /// <param name="id"></param>
    public void ChangeColor(int id)
    {
        mat.color = colors[id - 1];
        if (changeSound) AudioSource.PlayClipAtPoint(changeSound, this.transform.position);

        PV.RPC("RPC_ChangedWallTexture", RpcTarget.All, id,PhotonNetwork.LocalPlayer.NickName);

    }


    [PunRPC]
    void RPC_ChangedWallTexture(int currentTexx,string name)
    {
        if (!PV.IsMine)
        {
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        //PhotonView targetPV = PhotonView.Find(targetPropID);



        // if (targetPV.gameObject == null)
        // return;

        // gameObject.GetComponent<MeshFilter>().mesh = targetPV.gameObject.GetComponent<MeshFilter>().mesh;
        // gameObject.GetComponent<MeshRenderer>().material = targetPV.gameObject.GetComponent<MeshRenderer>().material;

        mat.mainTexture = textures[currentTexx].tex;
        mat.mainTextureScale = textures[currentTexx].tiling;
        mat.color = Color.white;

        if(name != PhotonNetwork.LocalPlayer.NickName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                ToastNotification.Instance.SendMessages(name + " Changed the texture of wall!");
            }
        }

        Debug.Log(currentTexx);
    }


    [PunRPC]
    void RPC_ChangedWallColor(int id,string name)
    {
        if (!PV.IsMine)
        {
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        mat.color = colors[id - 1];

        if (name != PhotonNetwork.LocalPlayer.NickName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                ToastNotification.Instance.SendMessages(name + " Changed the Color of " + this.gameObject.name);
            }
        }

    }







}
