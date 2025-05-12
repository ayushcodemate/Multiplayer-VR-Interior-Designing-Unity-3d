using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public AvatarInfo avatarInfo;

    public static GameInfo instance;

    public GameObject[] charactersOffline,charactersOnline;
    public Transform[] instantiatePoints;

    PhotonView PV;

    private void Awake()
    {

        if (instance == null)
            instance = this;

        PV = this.GetComponent<PhotonView>();

        InitializeAvatar();

        
    }




    public void InitializeAvatar()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            InstantiateOnlinePlayer();
        }
        else
        {
            InstantiateOfflinePlayer();
        }
      
    }

    public void InstantiateOfflinePlayer()
    {
        GameObject avatar = GameObject.Instantiate(charactersOffline[MainMenu.avatarNum - 1], instantiatePoints[0].position, Quaternion.identity);
      
    }

    public void InstantiateOnlinePlayer()
    {
        int randomPoint = Random.Range(0, instantiatePoints.Length);
        GameObject character = PhotonNetwork.Instantiate(charactersOnline[MainMenu.avatarNum - 1].name, instantiatePoints[randomPoint].position, Quaternion.identity);
        ToastNotification.Instance.SendMessages("You Joined The Room!"); 

        if (PV != null)
        {
            PV.RPC("ShowJoinedNotification", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
        }
    }


    [PunRPC]

    void ShowJoinedNotification(string name)
    {
        Debug.Log(name);
       if(name != PhotonNetwork.LocalPlayer.NickName)
        {
            Debug.Log("2222");
            ToastNotification.Instance.SendMessages(name + " Joined The Room!");

        }
    }
}
