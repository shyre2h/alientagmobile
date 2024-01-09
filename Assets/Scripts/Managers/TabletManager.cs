using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using Photon.Voice.Unity;
using Photon.Voice.Unity.Demos;
using Keyboard;
using TMPro;
using QuantumTek.QuantumUI;
using Photon.VR;
using System.Linq;
using Photon.Pun;
using DG.Tweening;
using ModIO;
using ModIOBrowser.Implementation;
using ModIO.Util;

public class TabletManager : MonoBehaviour
{
    //Used to show/hids tablet
    bool isTabletEnabled = false;
    public static TabletManager instance;
    public InputActionAsset inputActionReference;
    public string controllerName;
    public string actionToggleTablet;
    InputActionMap inputActionMap;
    InputAction inputActionPrimaryButton;
    public float value;
    public Recorder recorder;
    public PhotonView photonView;
    public KeyboardManager keyboardManager;
    public List<GameObject> turnStyles;
    public CanvasGroup tabsCanvas;
    public CanvasGroup modioCanvas;


    [Header("Lobby fields")]
    public TMP_InputField lobbyName;
    public Button createLobby;
    public Button joinLobby;
    public Button joinPublicLobby;
    public Button leaveLobby;
    public TextMeshProUGUI currentLobby;
    public QUI_OptionList turnStyleList;


    [Header("Player fields")]
    public TMP_InputField playerName;
    public Button setPlayerNameButton;
    public QUI_OptionList colourList;

    [Header("Mod fields")]
    public Button downloadMods;
    public QUI_OptionList mapsList;

    [Header("Game fields")]
    public Slider bgMusicSlider;
    public Slider sfxMusicSlider;
    public Toggle micToggle;

    [Header("Mod overall")]
    public static SubscribedMod[] subscribedMods = Array.Empty<SubscribedMod>();
    public List<string> availableMaps = new List<string>();

    [Header("Auth panel")]
    public GameObject authPanel;
    public Button authBackButton;
    public TMP_InputField emailInputField;
    public Button emailSendCodeButton;

    [Header("Code panel")]
    public GameObject codePanel;
    public Button codeBackButton;
    public TextMeshProUGUI codeText;
    public TMP_InputField codeInputField;
    public Button submitCodeButton;

    [Header("Home panel")]
    public GameObject homePanel;
    public Button homeBackButton;
    public Button leftArrow;
    public Button rightArrow;
    public List<MapObject> mapObjects;

    [Header("Mod panel")]
    public GameObject modPanel;
    public Button modBackButton;
    public MapObject mapObject;
    public TextMeshProUGUI authorText;
    public TextMeshProUGUI sizeText;
    public TextMeshProUGUI subscribersText;
    public Button subscribeButton;
    public TextMeshProUGUI subscribeButtonText;

    [Header("Error panel")]
    public GameObject errorPanel;
    public Button errorBackButton;
    public TextMeshProUGUI errorText;


    Vector3 targetScale;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;


        if (!recorder) recorder = FindObjectOfType<Recorder>();

        if (PlayerPrefs.GetInt("mic", 1) == 1)
        {
            micToggle.SetValue(true);
            recorder.TransmitEnabled = true;
        }
        else
        {
            micToggle.SetValue(false);
            recorder.TransmitEnabled = false;
        }
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        isTabletEnabled = false;
        inputActionMap = inputActionReference.FindActionMap(controllerName);
        inputActionPrimaryButton = inputActionMap.FindAction(actionToggleTablet);
        inputActionPrimaryButton.performed += ToggleTablet;
        bgMusicSlider.onValueChanged.AddListener(BGMusicVolume);
        sfxMusicSlider.onValueChanged.AddListener(SFXMusicVolume);
        micToggle.onValueChanged.AddListener(MicToggled);
        createLobby.onClick.AddListener(CreateLobbyButtonPressed);
        joinLobby.onClick.AddListener(JoinLobbyButtonPressed);
        joinPublicLobby.onClick.AddListener(JoinPublicLobbyButtonPressed);
        leaveLobby.onClick.AddListener(CreateLobbyButtonPressed);

        downloadMods.onClick.AddListener(DownloadMods);

        setPlayerNameButton.onClick.AddListener(SetName);
        colourList.onChangeOption.AddListener(SetColour);
        turnStyleList.onChangeOption.AddListener(SetTurnStyle);
        lobbyName.onSelect.AddListener(SetLobbyNameOnSelect);
        playerName.onSelect.AddListener(SetPlayerNameOnSelect);

        authBackButton.onClick.AddListener(AuthBackButton);
        emailInputField.onSelect.AddListener((string txt) => { SetInputFieldOnSelect(emailInputField); });
        emailSendCodeButton.onClick.AddListener(SendEmailCode);

        codeInputField.onSelect.AddListener((string txt) => { SetInputFieldOnSelect(codeInputField); });
        submitCodeButton.onClick.AddListener(SubmitEmailCode);
    }


    private void OnEnable()
    {
        inputActionPrimaryButton.Enable();
    }

    private void OnDisble()
    {
        inputActionPrimaryButton.Disable();


    }

    private void Start()
    {
        bgMusicSlider.value = AudioManager.instance.bgVolume;
        sfxMusicSlider.value = AudioManager.instance.sfxVolume;

        Initialize();
    }

    static async void Initialize()
    {
        // wait and check if we're authenticated, we need to know if our access token is still valid
        var isAuthed = await ModIOUnityAsync.IsAuthenticated();
        if (isAuthed.Succeeded())
        {
            Authentication.Instance.IsAuthenticated = true;
            ModIOUnity.EnableModManagement(ModManagementDelegate);
            ModIOUnity.FetchUpdates((Result r) => { CacheLocalSubscribedModStatuses(); instance.GetAllAvailableMaps(); });
        }
        else
        {
            Authentication.Instance.IsAuthenticated = false;
        }
    }

    public void ToggleTablet(CallbackContext context)
    {
        if (isTabletEnabled)
        {
            DOTween.Kill(transform);
            transform.DOScale(Vector3.zero, 0.3f);
            isTabletEnabled = false;
        }
        else
        {
            DOTween.Kill(transform);
            transform.DOScale(targetScale, 0.3f);
            isTabletEnabled = true;
        }
    }

    void AuthBackButton()
    {
        tabsCanvas.interactable = true;
        tabsCanvas.blocksRaycasts = true;
        tabsCanvas.alpha = 1f;
        modioCanvas.interactable = false;
        modioCanvas.blocksRaycasts = false;
        modioCanvas.alpha = 0f;

        authPanel.SetActive(false);
    }

    void DownloadMods()
    {
        //ModIOBrowser.Browser.Open(null);
        tabsCanvas.interactable = false;
        tabsCanvas.blocksRaycasts = false;
        tabsCanvas.alpha = 0f;
        modioCanvas.interactable = true;
        modioCanvas.blocksRaycasts = true;
        modioCanvas.alpha = 1f;

        if (!Authentication.Instance.IsAuthenticated)
        {
            authPanel.SetActive(true);
        }
        else
        {
            ShowHomePanel();
        }
    }

    void SetInputFieldOnSelect(TMP_InputField inpField)
    {
        keyboardManager.SetOutputField = inpField;
    }
    void SendEmailCode()
    {
        // if (!photonView.IsMine) return;
        if (emailInputField.text.Length == 0) return;

        ModIOUnity.RequestAuthenticationEmail(emailInputField.text, EmailSent);
        authPanel.SetActive(false);
    }

    void EmailSent(Result result)
    {
        if (result.Succeeded())
        {
            ShowCodePanel(emailInputField.text);
        }
        else
        {
            if (result.IsInvalidEmailAddress())
            {
                ShowErrorPanel("Invalid email address", authPanel);
            }
            else
            {
                ShowErrorPanel("Something went wrong", authPanel);
            }
        }
    }

    void ShowCodePanel(string email)
    {
        codePanel.SetActive(true);

        codeInputField.text = "";

        codeText.text = "Please check your email <b>" + email + "</b> for your 5 digit code to verify it below.";

        codeBackButton.onClick.RemoveAllListeners();
        codeBackButton.onClick.AddListener(() =>
        {
            authPanel.SetActive(true);
            codePanel.SetActive(false);
        });
    }

    void SubmitEmailCode()
    {
        if (codeInputField.text.Length == 0) return;
        print(codeInputField.text);

        ModIOUnity.SubmitEmailSecurityCode(codeInputField.text.ToUpper(), CodeSubmitted);
        codePanel.SetActive(false);
    }

    void CodeSubmitted(Result result)
    {
        if (result.Succeeded())
        {
            Authentication.Instance.IsAuthenticated = true;
            ModIOUnity.EnableModManagement(ModManagementDelegate);
            ModIOUnity.FetchUpdates((Result r) => { CacheLocalSubscribedModStatuses(); });
            ShowHomePanel();
        }
        else
        {
            if (result.IsInvalidSecurityCode())
            {
                ShowErrorPanel("Invalid code", codePanel);
            }
            else
            {
                ShowErrorPanel("Something went wrong", codePanel);
            }
        }
    }

    static void ModManagementDelegate(ModManagementEventType eventType, ModId modId, Result result)
    {
        if(eventType.Equals(ModManagementEventType.Installed) || eventType.Equals(ModManagementEventType.Uninstalled))
        {
           instance.GetAllAvailableMaps();
        }

        Debug.Log("a mod management event of type " + eventType.ToString() + " has been invoked");
    }

    void ShowHomePanel(bool updateMods = true)
    {
        homePanel.SetActive(true);

        homeBackButton.onClick.RemoveAllListeners();
        homeBackButton.onClick.AddListener(() =>
        {
            homePanel.SetActive(false);
            AuthBackButton();
        });

        if (updateMods)
        {
            SearchFilter filter = new SearchFilter();
            filter = new SearchFilter();
            filter.SetPageIndex(0);
            filter.SetPageSize(100);
            filter.SortBy(SortModsBy.Popular);
            filter.SetToAscending(false);

            ModIOUnity.GetMods(filter, GetModsResponse);
        }
    }

    void GetModsResponse(ResultAnd<ModPage> response)
    {

        ModProfile[] modProfiles = response.value.modProfiles;


        if (response.result.Succeeded())
        {
            ShowModdedMaps(0, modProfiles);
        }
        else
        {
            ShowErrorPanel("Couldn't get mods", homePanel);
        }
    }

    void ShowModdedMaps(int startingIndex, ModProfile[] modProfiles)
    {
        leftArrow.onClick.RemoveAllListeners();
        rightArrow.onClick.RemoveAllListeners();
        if (startingIndex > 0)
        {
            leftArrow.onClick.AddListener(() => { ShowModdedMaps(startingIndex - 3, modProfiles); });
        }
        if (modProfiles.Length > startingIndex + 3)
        {
            rightArrow.onClick.AddListener(() => { ShowModdedMaps(startingIndex + 3, modProfiles); });
        }

        for (int i = startingIndex; i < startingIndex + 3; i++)
        {
            mapObjects[i - startingIndex].Button.onClick.RemoveAllListeners();
            if (modProfiles.Length > i)
            {
                ModProfile currentMod = modProfiles[i];
                mapObjects[i - startingIndex].SetMap(currentMod);
                mapObjects[i - startingIndex].Button.onClick.AddListener(() => { homePanel.SetActive(false); ShowModPanel(currentMod); });
            }
            else
            {
                mapObjects[i - startingIndex].SetMapLoading();
            }
        }
    }

    void ShowModPanel(ModProfile map)
    {
        modPanel.SetActive(true);

        modBackButton.onClick.RemoveAllListeners();
        modBackButton.onClick.AddListener(() =>
        {
            modPanel.SetActive(false);
            ShowHomePanel(false);
        });

        mapObject.SetMap(map);
        authorText.text = "Author: " + map.creator.username;
        sizeText.text = "Size: " + Utility.GenerateHumanReadableStringForBytes(map.archiveFileSize);
        subscribersText.text = "Subscribers: " + map.stats.subscriberTotal;

        subscribeButton.onClick.RemoveAllListeners();
        subscribeButton.interactable = true;
        if (IsSubscribed(map))
        {
            subscribeButtonText.text = "Unsubscribe";
            subscribeButton.onClick.AddListener(() =>
            {
                subscribeButton.interactable = false;
                ModIOUnity.UnsubscribeFromMod(map.id,
                    delegate (Result result)
                    {
                        if (result.Succeeded())
                        {
                            CacheLocalSubscribedModStatuses();
                            ShowModPanel(map);
                        }

                    });
            });
        }
        else
        {
            subscribeButtonText.text = "Subscribe";
            subscribeButton.onClick.AddListener(() =>
            {
                subscribeButton.interactable = false;
                ModIOUnity.SubscribeToMod(map.id,
                delegate (Result result)
                {
                    if (result.Succeeded())
                    {
                        CacheLocalSubscribedModStatuses();
                        ShowModPanel(map);
                    }
                    else
                    {
                        ShowErrorPanel("Failed to subscribe", modPanel);
                    }
                });
            });
        }
    }

    static void CacheLocalSubscribedModStatuses()
    {
        SubscribedMod[] subs = ModIOUnity.GetSubscribedMods(out Result result);
        if (subs == null)
        {
            subs = new SubscribedMod[0];
        }
        subscribedMods = subs;
    }

    void GetAllAvailableMaps()
    {
        CacheLocalSubscribedModStatuses();

        availableMaps.Clear();

        availableMaps.Add("Default");
        foreach (SubscribedMod mod in subscribedMods)
        {
            if (mod.status == SubscribedModStatus.Installed)
            {
                availableMaps.Add(mod.modProfile.name);
            }
        }

        mapsList.options = availableMaps;
        mapsList.SetOption(0);
    }

    string GetCurrentSelectedMapId()
    {
        string mapName = mapsList.GetOption();
        foreach (SubscribedMod mod in subscribedMods)
        {
            if (mod.modProfile.name == mapName)
            {
                return mod.modProfile.id.id.ToString();
            }
        }
        return "Default";
    }

    string GetCurrentSelectedMapDirectory()
    {
        string mapName = mapsList.GetOption();
        foreach (SubscribedMod mod in subscribedMods)
        {
            if (mod.modProfile.name == mapName)
            {
                return mod.directory.ToString();
            }
        }
        return "Default";
    }

    bool IsSubscribed(ModProfile mod)
    {
        foreach (var m in subscribedMods)
        {
            if (m.modProfile.id == mod.id)
            {
                return true;
            }
        }

        return false;
    }

    void ShowErrorPanel(string errorString, GameObject previuousPanel)
    {
        previuousPanel.SetActive(false);
        errorPanel.SetActive(true);

        errorText.text = errorString;

        errorBackButton.onClick.RemoveAllListeners();
        errorBackButton.onClick.AddListener(() =>
        {
            previuousPanel.SetActive(true);
            errorPanel.SetActive(false);
        });
    }

    void CreateLobbyButtonPressed()
    {
        if (!photonView.IsMine) return;
        Blink.instance.CloseEyes(true, () => PhotonVRManager.LeaveCurrentRoomToCreatePrivateRoom(GetCurrentSelectedMapId(), GetCurrentSelectedMapDirectory()));
    }


    void SetLobbyNameOnSelect(string wut)
    {
        keyboardManager.SetOutputField = lobbyName;
    }
    void JoinLobbyButtonPressed()
    {
        if (!photonView.IsMine) return;
        if (lobbyName.text.Length == 0) return;

        Blink.instance.CloseEyes(true, () => PhotonVRManager.LeaveCurrentRoomToJoinPrivateRoom(lobbyName.text, GetCurrentSelectedMapId(), GetCurrentSelectedMapDirectory()));

    }


    void JoinPublicLobbyButtonPressed()
    {
        if (!photonView.IsMine) return;

        Blink.instance.CloseEyes(true, () => PhotonVRManager.LeaveCurrentRoomToJoinPublicRoom(GetCurrentSelectedMapId(), GetCurrentSelectedMapDirectory()));

    }

    public void SetCurrentLobbyName(string newLobbyName)
    {
        currentLobby.text = "Current lobby: " + newLobbyName;
    }



    void SetPlayerNameOnSelect(string wut)
    {
        keyboardManager.SetOutputField = playerName;
    }
    void SetName()
    {
        // if (!photonView.IsMine) return;
        if (playerName.text.Length == 0) return;
        PhotonVRManager.SetUsername(playerName.text);

    }


    void SetColour()
    {

        if (!photonView.IsMine) return;


        // Debug.LogWarning("Set colour: " + photonView.IsMine);
        string colour = colourList.option;
        Color newColour;

        switch (colour)
        {
            case "Red": newColour = Color.red; break;
            case "Blue": newColour = Color.blue; break;
            case "Cyan": newColour = Color.cyan; break;
            case "Yellow": newColour = Color.yellow; break;
            case "Magenta": newColour = Color.magenta; break;
            case "Grey": newColour = Color.grey; break;
            default: newColour = Color.green; break;
        }

        PhotonVRManager.SetColour(newColour);
    }


    void SetTurnStyle()
    {

        string turnStyleText = turnStyleList.option;
        int selectedIndex = 0;
        switch (turnStyleText)
        {
            case "None": selectedIndex = 0; break;
            case "Smooth": selectedIndex = 1; break;
            case "Snap": selectedIndex = 2; break;
            default: selectedIndex = 0; break;
        }

        if (selectedIndex >= turnStyles.Count) return;

        for (int i = 0; i < turnStyles.Count; i++)
        {
            if (i == selectedIndex)
                turnStyles[i].SetActive(true);
            else
                turnStyles[i].SetActive(false);

        }


    }



    void BGMusicVolume(float volume)
    {
        AudioManager.instance.SetBGVolume(volume);
    }


    void SFXMusicVolume(float volume)
    {
        AudioManager.instance.SetSFXVolume(volume);

    }


    void MicToggled(bool value)
    {
        PlayerPrefs.SetInt("mic", value ? 1 : 0);
        recorder.TransmitEnabled = value;
    }

}
