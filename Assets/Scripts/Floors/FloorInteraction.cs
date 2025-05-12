using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Interactable))]
public class FloorInteraction : MonoBehaviour 
{
    [System.Serializable]
    public class DifferentTextures
    {
        public Texture tex;
        public Vector2 tiling;
        public Vector2 offSet;
    }
    public bool focused;

    [SerializeField]
    public DifferentTextures[] textures;
    public int currentTex = 0;



    public AudioClip changeSound;
    //References
    Outline outline;
    MeshRenderer meshRenderer;
    Material mat;
    Interactable interactable;

    PhotonView PV;


    void Start()
    {
        outline = this.GetComponent<Outline>();
        meshRenderer = this.GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        interactable = this.GetComponent<Interactable>();
        PV = this.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if(interactable.isHover)
        {
            if(Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX"))
            {
                ChangeTexture();
            }
        }

        if (outline) outline.enabled = interactable.isHover;
    }


    public void ChangeTexture()
    {
        currentTex++;
        if (currentTex > textures.Length-1) currentTex = 0;
        mat.mainTexture = textures[currentTex].tex;
        mat.mainTextureScale = textures[currentTex].tiling;
        mat.color = Color.white;
        if (changeSound) AudioSource.PlayClipAtPoint(changeSound, this.transform.position);

        if (PV != null)
        {
            PV.RPC("RPC_PropChangeModel", RpcTarget.All, currentTex,PhotonNetwork.LocalPlayer.NickName);
        }
    }


    [PunRPC]
    void RPC_PropChangeModel(int currentTexx,string name)
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

        Debug.Log(currentTexx);

        if (name != PhotonNetwork.LocalPlayer.NickName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                ToastNotification.Instance.SendMessages(name + " Changed the texture of " + this.gameObject.name);
            }
        }

    }



}
