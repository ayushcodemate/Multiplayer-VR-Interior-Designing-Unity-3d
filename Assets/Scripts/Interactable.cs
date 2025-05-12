using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool isHover = false;

    public AvatarInfo avatarInfo;
  

    public void SetAvatarInfo(AvatarInfo avatarInfoNew)
    {
        avatarInfo = avatarInfoNew;
    }
    
}
