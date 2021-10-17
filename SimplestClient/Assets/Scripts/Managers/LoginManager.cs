using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [Header("Tabs")]
    [SerializeField] private GameObject createTab;
    [SerializeField] private GameObject loginTab;

    [Header("Buttons")]
    [SerializeField] private GameObject submitButton;
    [SerializeField] private GameObject forgotPasswordButton;

    [Header("Inputs")]
    [SerializeField] private GameObject userNameInput;
    [SerializeField] private GameObject passwordInput;
    [SerializeField] private GameObject createLoginInput;
    [SerializeField] private GameObject createPasswordInput;
    [SerializeField] private GameObject createEmailInput;

    [Header("Toggles")]
    [SerializeField] private GameObject loginToggle;
    [SerializeField] private GameObject createToggle;

    private void Awake()
    {
        Instance = this;

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitRequst);
        forgotPasswordButton.GetComponent<Button>().onClick.AddListener(ForgotPasswordRequest);

        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(AdjustUI);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(AdjustUI);

    }

    void ForgotPasswordRequest()
    {
        string login = userNameInput.GetComponent<InputField>().text;

        if (login != "")
            NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.ForgotPassword + "," + login);
    }

    void SubmitRequst()
    {
        if (loginToggle.GetComponent<Toggle>().isOn)
        {
            string login = userNameInput.GetComponent<InputField>().text;
            string password = passwordInput.GetComponent<InputField>().text;

            if (login != "" && password != "")
                NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.Login + "," + login + "," + password);
        }
        else if (createToggle.GetComponent<Toggle>().isOn)
        {
            string login = createLoginInput.GetComponent<InputField>().text;
            string password = createPasswordInput.GetComponent<InputField>().text;
            string email = createEmailInput.GetComponent<InputField>().text;

            if (login != "" && password != "" && email != "")
                NetworkedClient.Instance.SendServerRequest(ClientToServerTransferSignifiers.CreateAccount + "," + login + "," + password + "," + email);
        }
    }

    void AdjustUI(bool _)
    {
        createTab.SetActive(createToggle.GetComponent<Toggle>().isOn);
        loginTab.SetActive(loginToggle.GetComponent<Toggle>().isOn);
    }
}
