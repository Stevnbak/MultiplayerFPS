using Riptide;
using Riptide.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{ nameof(UIManager)} instance already exists, duplicate destroyed");
                Destroy(value);
            }
        }
    }

    [Header("Connect")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameField;

    [Header("Info")]
    [SerializeField] private string username;

    [Header("Parts")]
    [SerializeField] public GameObject DeathScreen;
    [SerializeField] public GameObject HUDScreen;


    private void Awake()
    {
        Singleton = this;
    }

    public void ConnectClick()
    {
        username = usernameField.text;
        hideUI();
        NetworkManager.Singleton.Connect();       
    }

    public void hideUI()
    {
        //SceneManager.LoadScene(1);
        connectUI.SetActive(false);
    }

    public void BackToMain()
    {
        usernameField.interactable = true;
        connectUI.SetActive(true);
    }

    public void SendName()
    {
        Debug.Log("Sending user info to server");
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerInformation);
        if (usernameField.text == "") usernameField.text = "Guest";
        message.AddString(usernameField.text);
        NetworkManager.Singleton.Client.Send(message);
    }
}
