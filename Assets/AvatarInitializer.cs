using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarInitializer : MonoBehaviour
{
    public GameObject[] characters;
    public Transform instantiatePoints;


    private void Awake()
    {
        InitializeAvatar();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeAvatar()
    {
        GameObject avatar = GameObject.Instantiate(characters[MainMenu.avatarNum-1], instantiatePoints.position, Quaternion.identity);
    }
}
