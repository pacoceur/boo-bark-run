using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform rowsParent;
    public TMP_InputField nameInput;
    public GameObject registerUI;
    public GameObject menuUI;

    private void Start()
    {
        if (PlayerPrefs.GetString("User Email", "") == "" || PlayerPrefs.GetString("User Password", "") == "")
        {
            registerUI.SetActive(true);
        }
        else
        {
            var request = new LoginWithEmailAddressRequest
            {
                Email = PlayerPrefs.GetString("User Email").ToLower(),
                Password = PlayerPrefs.GetString("User Password"),
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnSuccess, OnError);
        }

        if (PlayFab.PlayFabClientAPI.IsClientLoggedIn())
        {
            registerUI.SetActive(false);
            menuUI.SetActive(true);
        }

        /* if (PlayerPrefs.GetInt("Music Muted", 0) == 0)
            music.mute = false;
        else
            music.mute = true;

        if (PlayerPrefs.GetInt("Sound Muted", 0) == 0)
        {
            foreach (AudioSource sound in soundFx)
                sound.mute = false;
        }
        else
        {
            foreach (AudioSource sound in soundFx)
                sound.mute = true;
        } */
    }

    public void RegisterButton()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = nameInput.text.ToLower() + "010102@hotmail.com",
            DisplayName = nameInput.text,
            Password = "12345abcd",
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registered and logged in!");
        PlayerPrefs.SetString("User Email", nameInput.text.ToLower() + "010102@hotmail.com");
        PlayerPrefs.SetString("User Password", "12345abcd");
        registerUI.SetActive(false);
        menuUI.SetActive(true);
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = nameInput.text.ToLower() + "010102@hotmail.com",
            Password = "12345abcd",
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successful login!");
        PlayerPrefs.SetString("User Email", nameInput.text.ToLower() + "010102@hotmail.com");
        PlayerPrefs.SetString("User Password", "12345abcd");
        registerUI.SetActive(false);
        menuUI.SetActive(true);
    }

    public void PlayAsGuest()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login!");
        registerUI.SetActive(false);
        menuUI.SetActive(true);
    }

    void OnLoginError(PlayFabError error)
    {
        RegisterButton();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account!");
        Debug.Log(PlayerPrefs.GetString("User Email"));
        Debug.Log(error.GenerateErrorReport());
        registerUI.SetActive(true);
        menuUI.SetActive(false);
    }

    public void SendLeaderboard(float score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "Highscore",
                    Value = (int)Mathf.Round(score)
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Success leaderboard sent");
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Highscore",
            StartPosition = 0,
            MaxResultsCount = 8
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            TextMeshProUGUI[] texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString() + "m";
        }
    }
}
