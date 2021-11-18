/*
 * I used https://www.youtube.com/watch?v=IRAeJgGkjHk&t=894s code to implement chat box and message system
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject SubmitButton, UsernameInput, PasswordInput, createToggle, LoginToggle , JoinGameRoomButton , MainMenu, LoginMenu, WaitingInQueueRoom , GameRoom,
                ChatScroll,MsgInputField, SendTextButton, ReplayButton;
    GameObject HelloButton, GGButton, OOPSButton, NoButton, CurseButton,
               WellPlayedButton, ThatwasFunButton, SickButton, GameText, YouImage,OpponentImage;
    GameObject NetworkClient;

    public bool ReplayGame = false;
    public List<GameObject> tokens;
    public GameObject TextPrefaObject, ChatPanel; 
    List<Message> messageList = new List<Message>();
    public int numOfFrames;
    public int maxNumberOfFrames;
    public int actionsIndex;
    List<Action> actions = new List<Action>();
    public Sprite[] images;
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
            else if (go.name == "ChatScroll")
                ChatScroll = go;
            else if (go.name == "MsgInputField")
                MsgInputField = go;
            else if (go.name == "SendTextButton")
                SendTextButton = go;
            else if (go.name == "HelloButton")
                HelloButton = go;
            else if (go.name == "GGButton")
                GGButton = go;
            else if (go.name == "OOPSButton")
                OOPSButton = go;
            else if (go.name == "NoButton")
                NoButton = go;
            else if (go.name == "CurseButton")
                CurseButton = go;
            else if (go.name == "WellPlayedButton")
                WellPlayedButton = go;
            else if (go.name == "ThatwasFunButton")
                ThatwasFunButton = go;
            else if (go.name == "SickButton")
                SickButton = go;
            else if (go.name == "GameText")
                GameText = go;
            else if (go.name == "YouImage")
                YouImage = go;
            else if (go.name == "OpponentImage")
                OpponentImage = go;
            else if (go.name == "ReplayButton")
                ReplayButton = go;
            else if (go.name == "Token")
                tokens.Add(go);


        }
       
        SubmitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        SendTextButton.GetComponent<Button>().onClick.AddListener(SendTextButtonPressed);
        ReplayButton.GetComponent<Button>().onClick.AddListener(ReplayButtonPressed);

        HelloButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(HelloButton.GetComponentInChildren<Text>().text); });
        GGButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(GGButton.GetComponentInChildren<Text>().text); });
        OOPSButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(OOPSButton.GetComponentInChildren<Text>().text); });
        NoButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(NoButton.GetComponentInChildren<Text>().text); });
        CurseButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(CurseButton.GetComponentInChildren<Text>().text); });
        WellPlayedButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(WellPlayedButton.GetComponentInChildren<Text>().text); });
        ThatwasFunButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(ThatwasFunButton.GetComponentInChildren<Text>().text); });
        SickButton.GetComponent<Button>().onClick.AddListener(delegate { ChatMessageButtonsPressed(SickButton.GetComponentInChildren<Text>().text); });

        LoginToggle.GetComponent<Toggle>().onValueChanged.AddListener(loginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(createToggleChanged);
        JoinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoom);
        // JoinGameRoomButton.SetActive(false);
        ChangeState(GameState.LoginMenu);
    }

    void Update()
    {
        if(ReplayGame)
		{
            numOfFrames++;
            if(numOfFrames>= maxNumberOfFrames)
			{
                ShowGameBoard(actions[actionsIndex].tokenIndex, actions[actionsIndex].tokenImages);
                numOfFrames = 0;
                actionsIndex++;
			}
            if(actionsIndex>=actions.Count)
			{
                numOfFrames = 0;
                actionsIndex = 0;
                ReplayGame = false;
			}
		}
    }

    public void ChatMessageButtonsPressed(string txt)
	{
        string text = txt;
        string msg;
        msg = ClientToServerSignifier.SendTextMessage + "," + NetworkClient.GetComponent<NetworkedClient>().PlayerUserName + "," + text;
        Debug.Log(msg);
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

    }
    public void AddToActions(int index, int img)
	{
        actions.Add(new Action(index, img));
	}
    public void ReplayButtonPressed()
	{
        if (!ReplayGame)
        {
            for (int i = 0; i < 9; i++)
            {
                tokens[i].GetComponent<Image>().sprite = null;
            }
            ReplayGame = true;
            actionsIndex = 0;
        }
	}

    IEnumerator ShowTokens()
	{
        yield return new WaitForSeconds(0.5f);
        
	}
    public void SetPlayerOpponentTokens(int player, int opponent)
	{
        YouImage.GetComponent<Image>().sprite = images[player];
        OpponentImage.GetComponent<Image>().sprite = images[opponent];

    }
    public void AddMessageToChatBox(string txtMesg)
	{
         int MaxNumOfMessages = 25;
         MsgInputField.GetComponent<InputField>().text = "";
        if (messageList.Count >= MaxNumOfMessages)
		{
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
		}
        Message newMsg = new Message(txtMesg);
        GameObject newText = Instantiate(TextPrefaObject, ChatPanel.transform);
        newMsg.textObject = newText.GetComponent<Text>();
        newMsg.textObject.text = "  "+ newMsg.text;
        messageList.Add(newMsg);
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

    public void SendTextButtonPressed()
	{
        string text = MsgInputField.GetComponent<InputField>().text;
        string msg;
        msg = ClientToServerSignifier.SendTextMessage + "," + NetworkClient.GetComponent<NetworkedClient>().PlayerUserName + "," + text;
        Debug.Log(msg);
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

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

    public void SetGameText(string txt)
	{
        GameText.GetComponent<Text>().text = txt;
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

    public void markGameBoard(int indx, int token)
	{
		for (int i = 0; i < 9; i++)
		{
            if(tokens[i].GetComponent<TokenScript>().m_IndexNumber == indx)
			{
                tokens[i].GetComponent<TokenScript>().SetToken(token);
                AddToActions(indx, token);
            }
		}
	}

    public void ShowGameBoard(int indx, int token)
    {
        for (int i = 0; i < 9; i++)
        {
            if (tokens[i].GetComponent<TokenScript>().m_IndexNumber == indx)
            {
                tokens[i].GetComponent<TokenScript>().SetToken(token);
                
            }
        }
    }
    private void OnApplicationQuit()
	{
		
	}
}

public class Action
{
    public int tokenIndex;
    public int tokenImages;

    public Action(int tokenIndex, int tokenImages)
	{
        this.tokenImages = tokenImages;
        this.tokenIndex = tokenIndex;
	}
}
static public class GameState
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int GameRoom = 3;
    public const int WaitingInQueue = 4;
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public Message(string txt)
	{
        this.text = txt;
	}
}