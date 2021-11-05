using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(NetworkManager))]
public class NetworkUI : MonoBehaviour
{
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField usernameField;
    [SerializeField] TMP_InputField passwordField;

    NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    public void Host()
    {
        networkManager.StartHost();
        Debug.Log("Yay, I be alib");
    }

    public void Connect()
    {
        networkManager.networkAddress = ipField.text;
        networkManager.StartClient();
    }
}
