using System.Collections;
using System.Collections.Generic;
using System;
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

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    public void Host()
    {
        networkManager.StartHost();
    }

    public void Connect()
    {
        networkManager.networkAddress = ipField.text;
        networkManager.StartClient();
    }
}
