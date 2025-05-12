using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(Interactable))]

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]

public class SofaInteraction : MonoBehaviour
{
    public GameObject mainObject;
    public GameObject canvasToDisplay;
    UI_Interactabkle canvasInteractable;

    public bool isSelected = false;

   

    public Color[] colors;

    public AudioClip changeSound;

    public GameObject[] objectsToHide;

    public float distance = 2f;

    public Vector3 rotationAxis = new Vector3(0, 0, 0);

    public bool isSelectedToMove;

    public GameObject movingObject,rotationObject;


    [Header("Movement Stuff")]
    public String neededTag = "Floor";
    public bool isColliding = false;
    public GameObject collidingObject;
    public GameObject collisionCanvas;


    [Header("Auto Assign")]
    public GameObject Player;
    public Camera currentCamera;
    public XRCardboardInputModule inputModule;
    public AvatarInfo currentAvatarInfo;
    
    
    //References
    Interactable interactable;
    Outline outline;
    public MeshRenderer meshRenderer;
    public Material mat;
    [HideInInspector]
    public Rigidbody rb;
    
    public BoxCollider[] col;
    public Material selectionMaterial;
    public int materialIndex = 0;
    float currentTime = 0.0f;
    Ray myRay;      // initializing the ray
    RaycastHit hit; // initializing the raycasthit


    public PhotonView PV;

    public PhotonView PVSelf;



    void Start()
    {
        outline = this.GetComponent<Outline>();

        if (!meshRenderer)
            meshRenderer = this.GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        
        interactable = this.GetComponent<Interactable>();
        canvasInteractable = canvasToDisplay.GetComponent<UI_Interactabkle>();
          
        selectionMaterial = Resources.Load<Material>("Selection_MAT");
        rb = this.GetComponent<Rigidbody>();
        if(col == null)
        col = this.GetComponents<BoxCollider>();
        if (!rotationObject) rotationObject = this.gameObject;

        //Player = GameInfo.instance.avatarInfo.camera;
        //inputModule = GameInfo.instance.avatarInfo.cardboardInputModule;

        if (PhotonNetwork.IsConnectedAndReady)
            PVSelf = this.GetComponent<PhotonView>();
        
    }


    void GetCurrentAvatarInfo()
    {
        if(interactable.isHover)
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

        if (canvasInteractable.isHover)
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



        HandleCanvases();

        //If Item Is Selected
        if (isSelectedToMove)
        {
            // For Deselection
            if ((Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX")))
            {
                foreach (BoxCollider collider in col)
                {
                    collider.enabled = true;
                }
                Invoke("CheckCollisions", 0.1f);
                return;


            }

            
           

           // meshRenderer.sharedMaterials[materialIndex] = selectionMaterial;
            
          
            //Checking Raycast For Moving Purposes

            if(CheckRay())
            {
                movingObject.transform.position = hit.point;

                if (neededTag == "Wall")
                {
                    Vector3 perp = Vector3.Cross(hit.normal, Vector3.up);
                    Vector3 targetDir = Vector3.Project(transform.forward, perp).normalized;
                    movingObject.transform.localEulerAngles = targetDir;
                }
            }
            
            return;

            
        }


        // If not Selected Then Thiss Will Happen

        if (interactable.isHover)
        {
            //And If Key Is Pressed
            if ((Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("jsX")))
            {
                if (PhotonNetwork.IsConnectedAndReady)
                {
                    if (isSelected == true) ToastNotification.Instance.SendMessages("Object is Selected by other player, try later");
                }
                if (!isSelected)
                {
                    if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
                    {
                        if (PV == null)
                        {
                            PV = gameObject.GetComponentInParent<PhotonView>();
                        }

                        if (!PV.IsMine)
                            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
                    }


                    SelectSofa(true);
                    //ChangeTexture();

                }
            }

           

           

        }







        if (outline) outline.enabled = interactable.isHover;

        
       
    }



    void HandleCanvases()
    {

        if (canvasToDisplay.activeInHierarchy)
        {
            canvasToDisplay.transform.LookAt(Player.transform.position);
            canvasToDisplay.transform.localEulerAngles = new Vector3(0, canvasToDisplay.transform.localEulerAngles.y, 0.0f);
        }

        if (isSelectedToMove)
        {
            canvasToDisplay.SetActive(false);
            Debug.LogWarning("This is actve");
        } 

        if (collisionCanvas && collisionCanvas.activeInHierarchy && collisionCanvas.activeInHierarchy)
        {
            collisionCanvas.transform.LookAt(Player.transform.position);
            collisionCanvas.transform.localEulerAngles = new Vector3(0, collisionCanvas.transform.localEulerAngles.y, 0.0f);
        }
    }
    

    bool CheckRay()
    {
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);

        // Create a ray from the camera through the center of the screen
        Ray ray = currentCamera.ViewportPointToRay(screenCenter);

        // Create a RaycastHit variable to store information about what the ray hits


        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            if (neededTag == "Wall")
            {
                if (hit.transform.tag == "Wall")
                {

                    Debug.Log("Hit object: " + hit.transform.name);
                    return true;
                }
                else
                {

                    return false;
                }
            }
            else if (neededTag == "Floor")
            {
                if (hit.transform.tag == "Floor")
                {

                    //Debug.Log("Hit object: " + hit.transform.name);
                    return true;
                }
                else
                {

                    return false;
                }
            }
            else
            {
                return false;
            }


        }
        else
        {

            return false;

        }

    }


    public void SelectSofa(bool value)
    {
        isSelected = value;

        if (PV != null)
        {
            PV.RPC("ChangeSofaSelectionState", RpcTarget.All, value);
        }

        if(value)currentAvatarInfo.oneMenu.SetCurrentMenu(canvasInteractable, false);
        canvasToDisplay.SetActive(value);
        myRay = currentCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        canvasToDisplay.transform.position = myRay.GetPoint(distance);


        outline.OutlineColor = (value) ? Color.blue : Color.white;

        if(objectsToHide.Length > 0)
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(!value);
        }
    }


    /// <summary>
    /// This function is used to change the colors of wall.
    /// </summary>
    /// <param name="id"></param>
    public void ChangeColor(int id)
    {
        mat.color = colors[id - 1];
        if (changeSound) AudioSource.PlayClipAtPoint(changeSound, this.transform.position);

        if (PV != null)
        {
            PV.RPC("ChangeSofaColorState", RpcTarget.All, id,PhotonNetwork.LocalPlayer.NickName);
        }


    }


    //This Function Will be used to move the Object
    public void MoveObject()
    {
        rb.isKinematic = true;
        foreach (BoxCollider collider in col)
        {
            collider.enabled = false;
            collider.isTrigger = true;
        }

        if (PV != null)
        {
            PV.RPC("ChangeSofaMovementState", RpcTarget.All, true);
        }


        isSelectedToMove = true;
        meshRenderer.material = selectionMaterial;

        if (PV != null)
        {
            PV.RPC("ChangeSofaMatState", RpcTarget.All, true);
        }


        currentAvatarInfo.Raycaster.enabled = false ;
        currentAvatarInfo.Raycaster.anyThingSelected = true;
    }
    // This function will be used to Rotate The Object when holding the button
    public void Rotate()
    {
        //For Multiplayer
        if (PhotonNetwork.IsConnectedAndReady && PVSelf != null)
        {
            if (PVSelf.IsMine == false)
            {
                PVSelf.TransferOwnership(PhotonNetwork.LocalPlayer);
                StartCoroutine("RotationMultiplayer");
              
            }
            else
            {
                Vector3 rotationn = rotationAxis;
                rotationn *= Time.deltaTime;

                rotationObject.transform.Rotate(rotationn);
               
            }
            return;
        }

        //For Single Player
        Vector3 rotation = rotationAxis;
        rotation *= Time.deltaTime;

        rotationObject.transform.Rotate(rotation);
    }


    IEnumerator RotationMultiplayer()
    {
        yield return new WaitUntil(() => PVSelf.IsMine);
        Vector3 rotation = rotationAxis;
        rotation *= Time.deltaTime;

        rotationObject.transform.Rotate(rotation);

    }

    // This functiion will be used to Delete The Object
    public void Delete()
    {

        if (PV != null)
        {
            PV.RPC("DeleteObject", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
        }
        Destroy(mainObject);
    }

   
    public void CheckCollisions()
    {
        if(isColliding)
        {
            collisionCanvas.SetActive(true);
            Debug.LogWarning("Object is Colliding Sorrry");
            Invoke("DisableCollisionCanvas",4f);

            foreach (BoxCollider collider in col)
            {
                collider.enabled = false;
            }
            isColliding = false;
           
        }
        else
        {
            foreach (BoxCollider collider in col)
            {
                collider.isTrigger = false;
                collider.enabled = true;
            }
            
            rb.isKinematic = false;
            isSelectedToMove = false;


            if (PV != null)
            {
                PV.RPC("ChangeSofaMovementState", RpcTarget.All, false);
            }


            // Assign the modified materials array back to the MeshRenderer
            meshRenderer.material = mat;
            if (PV != null)
            {
                PV.RPC("ChangeSofaMatState", RpcTarget.All, false);
            }
            DisableCollisionCanvas();
            currentAvatarInfo.Raycaster.enabled = true;
            currentAvatarInfo.Raycaster.anyThingSelected = false ;
            isSelected = false;

        }
    }


    public void DisableCollisionCanvas()
    {
        collisionCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag != neededTag)
        {
            isColliding = true;
            collidingObject = other.gameObject;
        }
        else
        {
            isColliding = false;
            collidingObject = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != neededTag)
        {
            isColliding = true;
            collidingObject = other.gameObject;
         
        }
        else
        {
            isColliding = false;
            collidingObject = null;
        }
    }


    






}
