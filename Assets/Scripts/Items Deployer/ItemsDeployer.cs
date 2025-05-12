using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class ItemsDeployer : MonoBehaviour
{
    public string currentArea;
    public GameObject currentSelected;
    public string neededTag;

    public GameObject camera;

    public float distance = 2f;
    [Header("Canvases")]
    public GameObject DrawingRoomDeployerSrc,LivingRoomDeployerSrc;

    public GameObject[] Sofas,Tables,Decorative,DT,DC;

    public GameObject[] SofasMultiplayer, TablesMultiplayer, DecorativeMultiplayer, DTMultiplayer, DCMultiplayer;

    public GameObject DrawingRoomDeployer, LivingRoomDeployer;

    public OneMenuAtTime oneMenu;

    

    Ray myRay;      // initializing the ray
    RaycastHit hit; // initializing the raycasthit

    PhotonView PV;

    void Start()
    {
        DrawingRoomDeployerSrc = Resources.Load<GameObject>("DrawingRoomDeployer");
        LivingRoomDeployerSrc = Resources.Load<GameObject>("LivingRoomDeployer");

         PV = this.GetComponent<PhotonView>();

         oneMenu = this.GetComponentInParent<OneMenuAtTime>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("jsY"))
        {
            if(currentArea == "DrawingRoom")
            {
                if (DrawingRoomDeployer == null)
                {
                    DrawingRoomDeployer = GameObject.Instantiate(DrawingRoomDeployerSrc);
                    Deployer deployer = DrawingRoomDeployer.GetComponentInChildren<Deployer>();
                    deployer.Player = camera.gameObject;
                    deployer.itemDeployer = this;
                }
               
                DrawingRoomDeployer.SetActive(true);
                oneMenu.SetCurrentMenu(null, true);
                myRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                DrawingRoomDeployer.transform.position = myRay.GetPoint(distance);
            }

            if (currentArea == "LivingRoom")
            {
                if (LivingRoomDeployer == null)
                {
                    LivingRoomDeployer = GameObject.Instantiate(LivingRoomDeployerSrc);
                    Deployer deployer = LivingRoomDeployer.GetComponentInChildren<Deployer>();
                    deployer.itemDeployer = this;
                    deployer.Player = camera;
                }
                LivingRoomDeployer.SetActive(true);
                oneMenu.SetCurrentMenu(null, true);
                myRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                LivingRoomDeployer.transform.position = myRay.GetPoint(distance);
            }
        }


        //For Deploying things

        if (Input.GetKeyDown(KeyCode.A)  || Input.GetButtonDown("jsB"))
        {
            if (currentSelected && CheckRay())
            {
                GameObject instantiatedGO;
                if (PhotonNetwork.IsConnectedAndReady)
                   instantiatedGO = PhotonNetwork.Instantiate(currentSelected.name, hit.point, Quaternion.identity);
                else
                instantiatedGO = GameObject.Instantiate(currentSelected, hit.point, Quaternion.identity);

                instantiatedGO.name = currentSelected.name;
                currentSelected = null;
                

                if(PV!= null && PhotonNetwork.IsConnectedAndReady)
                {

                    PV.RPC("CreatedNewObject", RpcTarget.All,instantiatedGO.name, PhotonNetwork.LocalPlayer.NickName);

                }
            }
        }
    }


    bool CheckRay()
    {
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);

        // Create a ray from the camera through the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);

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
            else if(neededTag == "Floor")
            {
                if (hit.transform.tag == "Floor")
                {

                    Debug.Log("Hit object: " + hit.transform.name);
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

    public void ChangeCurrentArea(string area)
    {
        currentArea = area;
    }


    public void SelectSofa(int id)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            currentSelected = SofasMultiplayer[id - 1];
        else
        currentSelected = Sofas[id - 1];
        neededTag = "Floor";
    }

    public void SelectTable(int id)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            currentSelected = TablesMultiplayer[id - 1];
        else
            currentSelected = Tables[id - 1];
        neededTag = "Floor";
    }

    public void SelectDeocrative(int id)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            currentSelected = DecorativeMultiplayer[id - 1];
        else
            currentSelected = Decorative[id - 1];
        neededTag = "Floor";
    }

    public void SelectDT(int id)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            currentSelected = DTMultiplayer[id - 1];
        else
            currentSelected = DT[id - 1];
        neededTag = "Floor";
    }

    public void SelectDC(int id)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            currentSelected = DCMultiplayer[id - 1];
        else
            currentSelected = DC[id - 1];
        neededTag = "Floor";
    }


    [PunRPC]
    public void CreatedNewObject(string objectName, string name)
    {
        if (name != PhotonNetwork.LocalPlayer.NickName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                ToastNotification.Instance.SendMessages(name + " Created new Object: " + objectName);
            }


        }
    }







}
