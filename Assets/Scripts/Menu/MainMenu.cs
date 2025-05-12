using FWC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Outline))]
public class MainMenu : MonoBehaviour
{
    public XRCardboardInputModule cardboardInput;
    public XRCardboardController controller;
    public StandaloneInputModule unityInput;
    public GameObject PlayButtonUI;
    public GameObject FirstAvatarUI;
    public GameObject SinglePlayerUI;
    public GameObject Menu4FirstSelect;
    public EventSystem eventSystem;

    public GameObject Menu1, Menu2,Menu3,Menu4;
    [SerializeField]
    public static int avatarNum;

    public static bool isOnline;
    void Start()
    {
        controller.EnableVR();
        eventSystem.SetSelectedGameObject(PlayButtonUI);
    }

    // Update is called once per frame
    void Update()
    {
        cardboardInput.enabled = false;
        unityInput.enabled = true;
    }

    public void PlayButton()
    {
        Menu1.SetActive(false);
        Menu2.SetActive(true);
        eventSystem.UpdateModules();
        eventSystem.firstSelectedGameObject = FirstAvatarUI;
        eventSystem.SetSelectedGameObject(FirstAvatarUI);
        Debug.Log(eventSystem.currentSelectedGameObject);
        eventSystem.UpdateModules();
        

       
    }


    public void SelectAvatar(int num)
    {
        avatarNum = num;
        Menu2.SetActive(false);
        Menu3.SetActive(true);
        eventSystem.UpdateModules();
        eventSystem.firstSelectedGameObject = SinglePlayerUI;
        eventSystem.SetSelectedGameObject(SinglePlayerUI);
        Debug.Log(eventSystem.currentSelectedGameObject);
        eventSystem.UpdateModules();

    }

    public void ShowMenu4()
    {
        Menu1.SetActive(false);
        Menu2.SetActive(false);
        Menu3.SetActive(false);
        Menu4.SetActive(true);

        eventSystem.UpdateModules();
        eventSystem.firstSelectedGameObject = Menu4FirstSelect;
        eventSystem.SetSelectedGameObject(Menu4FirstSelect);
        Debug.Log(eventSystem.currentSelectedGameObject);
        eventSystem.UpdateModules();
    }

    public void SetOnline(bool value)
    {
        isOnline = value;
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void LoadSinglePlayer()
    {
        SceneManager.LoadScene(1);
    }
}
