using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

[RequireComponent(typeof(InputField))]

public class PlayerNameInputField : MonoBehaviour
{
    //Store the PlayerPref Key to avoid typos
    const string PlayerNamePrefKey = "PlayerName";
    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();

        if (PlayerPrefs.HasKey(PlayerNamePrefKey))
        {
            defaultName = PlayerPrefs.GetString(PlayerNamePrefKey);
            _inputField.text = defaultName;
        }

        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player name is null or empty!");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(PlayerNamePrefKey, value);
    }
}
