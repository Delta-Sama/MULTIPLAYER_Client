using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [SerializeField] GameObject createTab, loginTab;

    GameObject submitButton, userNameInput, passwordInput, toggles, loginToggle, createToggle;
    GameObject forgotPasswordButton, createLoginInput, createPasswordInput, createEmailInput;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        createTab.SetActive(true);

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "SubmitButton")
                submitButton = obj;
            else if (obj.name == "ForgotPasswordButton")
                forgotPasswordButton = obj;
            else if (obj.name == "LoginInputField")
                userNameInput = obj;
            else if (obj.name == "PasswordInputField")
                passwordInput = obj;
            else if (obj.name == "CreateLoginInputField")
                createLoginInput = obj;
            else if (obj.name == "CreatePasswordInputField")
                createPasswordInput = obj;
            else if (obj.name == "CreateEmailInputField")
                createEmailInput = obj;
            else if (obj.name == "Toggles")
                toggles = obj;
            else if (obj.name == "LoginToggle")
                loginToggle = obj;
            else if (obj.name == "CreateAccountToggle")
                createToggle = obj;
        }

        createTab.SetActive(false);

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitRequst);
        forgotPasswordButton.GetComponent<Button>().onClick.AddListener(ForgotPasswordRequest);

        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(AdjustUI);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(AdjustUI);

    }

    void ForgotPasswordRequest()
    {
        string login = userNameInput.GetComponent<InputField>().text;
        string password = passwordInput.GetComponent<InputField>().text;

        if (login != "")
            NetworkedClient.Instance.SendMessageToHost(ClientToServerTransferSignifiers.ForgotPassword + "," + login);
    }

    void SubmitRequst()
    {
        if (loginToggle.GetComponent<Toggle>().isOn)
        {
            string login = userNameInput.GetComponent<InputField>().text;
            string password = passwordInput.GetComponent<InputField>().text;

            if (login != "" && password != "")
                NetworkedClient.Instance.SendMessageToHost(ClientToServerTransferSignifiers.Login + "," + login + "," + password);
        }
        else if (createToggle.GetComponent<Toggle>().isOn)
        {
            string login = createLoginInput.GetComponent<InputField>().text;
            string password = createPasswordInput.GetComponent<InputField>().text;
            string email = createEmailInput.GetComponent<InputField>().text;

            if (login != "" && password != "" && email != "")
                NetworkedClient.Instance.SendMessageToHost(ClientToServerTransferSignifiers.CreateAccount + "," + login + "," + password + "," + email);
        }
    }

    void AdjustUI(bool _)
    {
        createTab.SetActive(createToggle.GetComponent<Toggle>().isOn);
        loginTab.SetActive(loginToggle.GetComponent<Toggle>().isOn);
    }
}
