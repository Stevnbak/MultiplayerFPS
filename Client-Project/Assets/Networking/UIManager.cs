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
    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField serverIdField;

    [Header("Info")]
    [SerializeField] private string username;

    [Header("Parts")]
    [SerializeField] public GameObject DeathScreen;
    [SerializeField] public GameObject HUDScreen;


    private void Awake()
    {
        Singleton = this;
        BackToMain();
        ipField.text = "127.0.0.1";
        serverIdField.text = "0";
    }

    public void ConnectClick()
    {
        username = usernameField.text;

        hideUI();

        //Ask server to join a game
        Message message = Message.Create(MessageSendMode.Reliable, MessageIds.joinGame);
        ushort serverId = (ushort) int.Parse(serverIdField.text);
        message.AddUShort(serverId);
        NetworkManager.Singleton.Client.Send(message);

        ///NetworkManager.Singleton.Connect(ipField.text, serverIdField.text);       
    }

    public void hideUI()
    {
        //SceneManager.LoadScene(1);
        connectUI.SetActive(false);
        HUDScreen.SetActive(true);
    }

    public void BackToMain()
    {
        usernameField.interactable = true;
        ipField.interactable = true;
        serverIdField.interactable = true;
        connectUI.SetActive(true);
        HUDScreen.SetActive(false);
    }

    public void SendName()
    {
        hideUI();
        Debug.Log("Sending user info to server");
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerInformation);
        message.AddUShort(NetworkManager.Singleton.ConnectedServerId);
        if (usernameField.text == "") usernameField.text = "Guest";
        message.AddString(usernameField.text);
        NetworkManager.Singleton.Client.Send(message);
    }
}
