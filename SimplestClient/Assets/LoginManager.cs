using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    GameObject submitButton, userNameInput, passwordInput, loginToggle, createToggle;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "SubmitButton")
                submitButton = obj;
            else if (obj.name == "LoginInputField")
                userNameInput = obj;
            else if (obj.name == "PasswordInputField")
                passwordInput = obj;
            else if (obj.name == "LoginToggle")
                loginToggle = obj;
            else if (obj.name == "CreateAccountToggle")
                createToggle = obj;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitRequst);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SubmitRequst()
    {
        Debug.Log("Submit");
        string login = userNameInput.GetComponent<InputField>().text;
        string password = passwordInput.GetComponent<InputField>().text;

        if (loginToggle.GetComponent<Toggle>().isOn)
        {
            NetworkedClient.Instance.SendMessageToHost(ClientToServerTransferSignifiers.Login + "," + login + "," + password);
        }
        else if (createToggle.GetComponent<Toggle>().isOn)
        {
            NetworkedClient.Instance.SendMessageToHost(ClientToServerTransferSignifiers.CreateAccount + "," + login + "," + password);
        }
    }

}
