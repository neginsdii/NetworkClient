using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject SubmitButton, UsernameInput, PasswordInput, createToggle, LoginToggle , JoinGameRoomButton , MainMenu, LoginMenu, WaitingInQueueRoom , GameRoom;
    GameObject NetworkClient;
 //   static GameObject instace;
    void Start()
    {
      //  instace = this.gameObject;
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach(GameObject go in allObjects)
		{
            if (go.name == "UsernameInputField")
                UsernameInput = go;
            else if (go.name == "PasswordInputField")
                PasswordInput = go;
            else if (go.name == "SubmitButton")
                SubmitButton = go;
            else if (go.name == "CreateToggle")
                createToggle = go;
            else if (go.name == "LoginToggle")
                LoginToggle = go;
            else if (go.name == "NetworkedClient")
                NetworkClient = go;
            else if (go.name == "JoinGameRoomButton")
                JoinGameRoomButton = go;
            else if (go.name == "MainMenu")
                MainMenu = go;
            else if (go.name == "LoginMenu")
                LoginMenu = go;
            else if (go.name == "WaitingInQueue")
                  WaitingInQueueRoom = go;
            else if (go.name == "GameRoom")
                GameRoom = go;

        }
        SubmitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        LoginToggle.GetComponent<Toggle>().onValueChanged.AddListener(loginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(createToggleChanged);
        JoinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoom);
        // JoinGameRoomButton.SetActive(false);
        ChangeState(GameState.LoginMenu);
    }

    void Update()
    {
        
    }

    public void SubmitButtonPressed()
	{
        string p = PasswordInput.GetComponent<InputField>().text;
        string n = UsernameInput.GetComponent<InputField>().text;
        string msg;
        if(createToggle.GetComponent<Toggle>().isOn)
        msg = ClientToServerSignifier.createAccount + "," + n + "," + p;
        else
         msg = ClientToServerSignifier.Login + "," + n + "," + p;
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log(msg);
    }
    public void JoinGameRoom()
	{
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifier.JoinQueueForGameRoom + "");
        ChangeState(GameState.WaitingInQueue);
    }

    public void loginToggleChanged(bool val)
	{
        // Debug.Log("loginToggle");
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!val);
	}
    public void createToggleChanged(bool val)
    {
        // Debug.Log("createToggle");
        LoginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!val);

    }

    public void ChangeState( int  newState)
	{
        if(newState== GameState.LoginMenu)
		{
            GameRoom.SetActive(false);
            WaitingInQueueRoom.SetActive(false);
            MainMenu.SetActive(false);
            LoginMenu.SetActive(true);
		}
        else if (newState == GameState.WaitingInQueue)
        {
            GameRoom.SetActive(false);
            MainMenu.SetActive(false);
            LoginMenu.SetActive(false);
           WaitingInQueueRoom.SetActive(true);
        }
        else if (newState == GameState.GameRoom)
        {

            MainMenu.SetActive(false);
            LoginMenu.SetActive(false);
            WaitingInQueueRoom.SetActive(false);
            GameRoom.SetActive(true);
        }
        else if (newState == GameState.MainMenu)
        {
            GameRoom.SetActive(false);
            LoginMenu.SetActive(false);
            WaitingInQueueRoom.SetActive(false);
            MainMenu.SetActive(true);
        }

    }
}

static public class GameState
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int GameRoom = 3;
    public const int WaitingInQueue = 4;
}
