using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class InputFieldLogic : MonoBehaviour
{
    string playerNamePreKey = null;

    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputText = GetComponent<InputField>();

        if (_inputText != null)
        {
            if (PlayerPrefs.HasKey(playerNamePreKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePreKey);
                _inputText.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Playe Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePreKey, value);
    }
}
