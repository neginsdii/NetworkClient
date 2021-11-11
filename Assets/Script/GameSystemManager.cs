using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject SubmitButton, UsernameInput, PasswordInput, createToggle, LoginToggle;
    GameObject NetworkClient;
    void Start()
    {
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

        }
        SubmitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        LoginToggle.GetComponent<Toggle>().onValueChanged.AddListener(loginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(createToggleChanged);

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
}
