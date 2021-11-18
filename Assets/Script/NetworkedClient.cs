using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;
   public int tokenInGame;
    public int otherPlayerToken;
    GameObject gameSystemManager;
    public string PlayerUserName;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
		{
            if (go.GetComponent<GameSystemManager>() != null)
			{
                gameSystemManager = go;

            }
        }
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
       

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "10.0.0.173", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }
    
    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);
        if(signifier == ServerToClientSignifier.AccountCreationComplete)
		{
            PlayerUserName = csv[1];
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameState.MainMenu);
		}
        else if (signifier == ServerToClientSignifier.LoginComplete)
        {
            PlayerUserName = csv[1];
            Debug.Log(PlayerUserName);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameState.MainMenu);

        }
        else if (signifier == ServerToClientSignifier.GameStart)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameState.GameRoom);

        
        }
        else if (signifier == ServerToClientSignifier.ObserveGameAccepted)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ShowObserveTextError(false);
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameState.GameRoom);
            gameSystemManager.GetComponent<GameSystemManager>().ShowObserverPanel(true);



        }
        else if (signifier == ServerToClientSignifier.ObserveGameFailed)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ShowObserveTextError(true);


        }
        else if (signifier == ServerToClientSignifier.TextChatMeassage)
        {
               gameSystemManager.GetComponent<GameSystemManager>().AddMessageToChatBox(csv[1]);


        }
        else if (signifier == ServerToClientSignifier.TurnInGame)
        {
            tokenInGame = int.Parse(csv[1]);
            otherPlayerToken = int.Parse(csv[2]);
            gameSystemManager.GetComponent<GameSystemManager>().SetPlayerOpponentTokens(tokenInGame,otherPlayerToken); 
           gameSystemManager.GetComponent<GameSystemManager>().SetPlayersName(csv[3], csv[4]);


        }
        else if (signifier == ServerToClientSignifier.sendChoosenTokenByPlayer)
        {
            
            if (csv[1] == PlayerUserName)
            {
                Debug.Log("You Played id:" + csv[1] + " your id: " + connectionID 
                    + "  your token: "+ tokenInGame+ "  player token: "+ otherPlayerToken );
                gameSystemManager.GetComponent<GameSystemManager>().markGameBoard(int.Parse(csv[2]), tokenInGame);

            }
            else
            {
                Debug.Log("Opponent played id:" + csv[1]+ " your id: "+ connectionID
                    + "  your token: " + tokenInGame + "  player token: " + otherPlayerToken);
                gameSystemManager.GetComponent<GameSystemManager>().markGameBoard(int.Parse(csv[2]), otherPlayerToken);
            }

        }
        else if (signifier == ServerToClientSignifier.sendChoosenTokensToObservers)
        {
            gameSystemManager.GetComponent<GameSystemManager>().markGameBoard(int.Parse(csv[1]), int.Parse(csv[2]));

        }

        else if (signifier == ServerToClientSignifier.sendGameStatus)
        {
            gameSystemManager.GetComponent<GameSystemManager>().SetGameText(csv[1]);

        }

    }

    public bool IsConnected()
    {
        return isConnected;
    }


}

public static class ClientToServerSignifier
{
    public const int createAccount = 1;
    public const int Login = 2;
    public const int JoinQueueForGameRoom = 3;
    public const int GameRoomPlay = 4;
    public const int SendTextMessage = 5;
    public const int SendChoosenToken = 6;
    public const int requestToObserveGame = 7;
}


public static class ServerToClientSignifier
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;
    public const int GameStart = 5;
    public const int TextChatMeassage = 6;
    public const int TurnInGame = 7;
    public const int sendChoosenTokenByPlayer = 8;
    public const int SendwinLoseTie = 9;
    public const int sendGameStatus = 10;
    public const int ObserveGameAccepted = 11;
    public const int ObserveGameFailed = 12;
    public const int sendChoosenTokensToObservers = 13;
}