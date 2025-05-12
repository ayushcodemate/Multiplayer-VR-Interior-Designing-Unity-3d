using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SofaMultiplayerHandler : MonoBehaviour
{
    public SofaInteraction sofaInteraction;

    PhotonView PV;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]

    void ChangeSofaSelectionState(bool value)
    {
        Debug.Log("Selection State Is Called" + value);
        sofaInteraction.isSelected = value;
    }

    [PunRPC]
    void ChangeSofaMovementState(bool value)
    {
        if (value)
        {
            sofaInteraction.rb.isKinematic = true;
            foreach (BoxCollider collider in sofaInteraction.col)
            {
                collider.enabled = false;
                collider.isTrigger = true;
            }

          // sofaInteraction.isSelectedToMove = true;
        }

        else
        {
            foreach (BoxCollider collider in sofaInteraction.col)
            {
                collider.isTrigger = false;
                collider.enabled = true;
            }

            sofaInteraction.rb.isKinematic = false;
           // sofaInteraction.isSelectedToMove = false;
        }
    }



    [PunRPC]

    void ChangeSofaMatState(bool value)
    {
        if (value)
            sofaInteraction.meshRenderer.material = sofaInteraction.selectionMaterial;
        else
            sofaInteraction.meshRenderer.material = sofaInteraction.mat;
    }

    [PunRPC]

    void ChangeSofaColorState(int id,string name)
    {
        sofaInteraction.mat.color = sofaInteraction.colors[id - 1];

        if (name != PhotonNetwork.LocalPlayer.NickName)
        {
            ToastNotification.Instance.SendMessages(name + " Changed color of " + this.gameObject.name);
        }
    }


    [PunRPC]
    public void DeleteObject(string name)
    {
        Destroy(sofaInteraction.mainObject);

        if(name != PhotonNetwork.LocalPlayer.NickName)
        {
            ToastNotification.Instance.SendMessages(name + " Deleted " + this.gameObject.name);
        }

    }


}
