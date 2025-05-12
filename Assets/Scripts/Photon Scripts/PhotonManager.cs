using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using Photon.Pun.Demo.Cockpit;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField userNameText;
    public TMP_InputField roomNameText;
    public int maxPlayer;

    [Header("Panles")]
    //References of panels
    public GameObject UserNamePanel;
    public GameObject ConnectingPanel;
    public GameObject LobbyPanel;
    public GameObject CreateRoomPanel;
    public GameObject RoomListPanel;


    private Dictionary<string, RoomInfo> roomListData;

    public GameObject RoomListPrefab,RoomListParent;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;
    public GameObject PlayerListItemPrefab;
    public GameObject PlayerListItemParent;
    public Button PlayButton;

    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> playerListGameObject;


    public bool isInLobby;
    public bool inRoom;

    public MainMenu menuScript;
    
    #region UnityMethods

    private void Start()
    {
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        isInLobby = PhotonNetwork.InLobby;
        inRoom = PhotonNetwork.InRoom;
    }

    #endregion

    #region UI Methods

    public void OnLoginClick()
    {
        string name = userNameText.text;

        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActivateMyPanel("Connecting Panel");
        }
        else
        {
            Debug.Log("Empty Name");
        }

       
    }

  

    public void OnClickRoomCreate()
    {
        string roomName = roomNameText.text;

        if(string.IsNullOrEmpty(roomName))
        {
            roomName = roomName + Random.Range(0, 1000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayer;

        PhotonNetwork.CreateRoom(roomName, roomOptions,TypedLobby.Default);

    }

    public void OnBackClick()
    {
        ActivateMyPanel("Lobby Panel");
    }


    public void OnRoomListClick()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivateMyPanel(RoomListPanel.name);
    }

    public void BackFromRoomList()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivateMyPanel(LobbyPanel.name);
        Debug.Log("Back Button Clicked");
    }

    public void BackFromPlayerList()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            playerListGameObject.Clear();
        }

        ActivateMyPanel(LobbyPanel.name);
        Debug.Log("Back Button Clicked");
            

      
    }

    public void OnPlayButtonClick()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    #endregion

    #region PhotonCallbacks

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Is connected to photon");
        ActivateMyPanel("Lobby Panel");
        menuScript.SetOnline(true);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Joined Room");
      
        if(playerListGameObject == null)
        {
            playerListGameObject = new Dictionary<int, GameObject>();
        }


        if (PhotonNetwork.IsMasterClient)
        {
            PlayButton.interactable = true;
        }
        else
        {
            PlayButton.interactable = false;
        }

        ActivateMyPanel(InsideRoomPanel.name);

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerListItemObject = Instantiate(PlayerListItemPrefab);
            playerListItemObject.transform.SetParent(PlayerListItemParent.transform);
            playerListItemObject.transform.localScale = Vector3.one;
            playerListItemObject.transform.localPosition = Vector3.zero;
            playerListItemObject.transform.localEulerAngles = Vector3.zero;

            playerListItemObject.transform.GetChild(0).GetComponent<TMP_Text>().text = p.NickName;

            if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItemObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListItemObject.transform.GetChild(1).gameObject.SetActive(false);
            }

            playerListGameObject.Add(p.ActorNumber, playerListItemObject);

        }

     
    }


    public override void OnPlayerEnteredRoom(Player p)
    {
        GameObject playerListItemObject = Instantiate(PlayerListItemPrefab);
        playerListItemObject.transform.SetParent(PlayerListItemParent.transform);
        playerListItemObject.transform.localScale = Vector3.one;
        playerListItemObject.transform.localPosition = Vector3.zero;
        playerListItemObject.transform.localEulerAngles = Vector3.zero;

        playerListItemObject.transform.GetChild(0).GetComponent<TMP_Text>().text = p.NickName;

        if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListItemObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerListItemObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        playerListGameObject.Add(p.ActorNumber, playerListItemObject);
    }

   

    //This is for other Players
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameObject[otherPlayer.ActorNumber]);
        playerListGameObject.Remove(otherPlayer.ActorNumber);
    }

    //This is for main Player Who Created the room
    public override void OnLeftRoom()
    {
        ActivateMyPanel(LobbyPanel.name);

        foreach(GameObject obj in playerListGameObject.Values)
        {
            Destroy(obj);
        }

        if(PhotonNetwork.IsMasterClient)
        {
            PlayButton.interactable = true;
        }
        else
        {
            PlayButton.interactable = false;
        }

        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("List is upadated");
        //Clear List 
        ClearRoomList();

        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if(roomListData.ContainsKey(room.Name))
                {
                    // Removing room from list if it is destroyed
                    Debug.Log(1);
                    roomListData.Remove(room.Name);
                }
            }
            else
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    //Update already existed Room
                    Debug.Log(2);
                    roomListData[room.Name] = room;
                }
                else
                {
                    //Adding New Room 
                    Debug.Log(3);
                    roomListData.Add(room.Name, room);
                }
               
            }
        }

        // Generate List Item

        foreach (RoomInfo roomItem in roomListData.Values)
        {
            Debug.Log(4);
            GameObject roomListItemObject = Instantiate(RoomListPrefab);
            roomListItemObject.transform.SetParent(RoomListParent.transform);
            roomListItemObject.transform.localScale = Vector3.one;
            roomListItemObject.transform.localPosition = Vector3.zero;
            roomListItemObject.transform.localEulerAngles = Vector3.zero;

            roomListItemObject.transform.GetChild(0).transform.GetComponent<TMP_Text>().text = roomItem.Name;
            roomListItemObject.transform.GetChild(1).transform.GetComponent<TMP_Text>().text = roomItem.PlayerCount + "/"+ roomItem.MaxPlayers;
            roomListItemObject.transform.GetChild(2).transform.GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomItem.Name));

            roomListGameObject.Add(roomItem.Name, roomListItemObject);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomListData.Clear();
    }

    #endregion


    #region Public Methods

    public void RoomJoinFromList(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void ActivateMyPanel(string panelName)
    {
        UserNamePanel.SetActive(panelName.Equals(UserNamePanel.name));
        ConnectingPanel.SetActive(panelName.Equals(ConnectingPanel.name));
        LobbyPanel.SetActive(panelName.Equals(LobbyPanel.name));
        CreateRoomPanel.SetActive(panelName.Equals(CreateRoomPanel.name));
        RoomListPanel.SetActive(panelName.Equals(RoomListPanel.name));
        InsideRoomPanel.SetActive(panelName.Equals(InsideRoomPanel.name));
    }

    public void ClearRoomList()
    {
        if (roomListGameObject.Count > 0)
        {
            foreach (var item in roomListGameObject.Values)
            {
                Destroy(item);
            }
        }

        roomListGameObject.Clear();
    }

    #endregion
}
