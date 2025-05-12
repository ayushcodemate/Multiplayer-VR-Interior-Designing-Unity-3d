using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstSelectionObject : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject FirstSelectedObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        eventSystem.UpdateModules();
        eventSystem.firstSelectedGameObject = FirstSelectedObject;
        eventSystem.SetSelectedGameObject(FirstSelectedObject);
        Debug.Log(eventSystem.currentSelectedGameObject);
        eventSystem.UpdateModules();
    }

    private void OnDisable()
    {
        
    }
}
