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

    public KeyboardManager keyboardManager;


    [Header("Lobby fields")]
    public TMP_InputField lobbyName;
    public Button createLobby;
    public Button joinLobby;
    public TextMeshProUGUI currentLobby;

    [Header("Player fields")]
    public TMP_InputField playerName;
    public Button setPlayerNameButton;
    public QUI_OptionList colourList;


    [Header("Game fields")]
    public Slider bgMusicSlider;
    public Slider sfxMusicSlider;
    public Toggle micToggle;




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
        setPlayerNameButton.onClick.AddListener(SetName);
        colourList.onChangeOption.AddListener(SetColour);
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
    }

    public void ToggleTablet(CallbackContext context)
    {
        if (isTabletEnabled)
        {
            transform.localScale = Vector3.zero;
            isTabletEnabled = false;
        }
        else
        {
            transform.localScale = Vector3.one;
            isTabletEnabled = true;
        }
    }




    void CreateLobbyButtonPressed()
    {
        PhotonVRManager.JoinPrivateRoom(PhotonVRManager.Manager.CreateRoomCode());
    }

    void JoinLobbyButtonPressed()
    {
        if (lobbyName.text.Length == 0) return;
        PhotonVRManager.JoinPrivateRoom(lobbyName.text);
    }


    public void SetCurrentLobbyName(string newLobbyName)
    {
        currentLobby.text = "Current lobby: " + newLobbyName;
    }


    void SetName()
    {
        if (playerName.text.Length == 0) return;
        PhotonVRManager.SetUsername(playerName.text);

    }


    void SetColour()
    {
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
