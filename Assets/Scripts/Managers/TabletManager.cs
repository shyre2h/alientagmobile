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
        setPlayerNameButton.onClick.AddListener(SetName);
        colourList.onChangeOption.AddListener(SetColour);
        lobbyName.onSelect.AddListener(SetLobbyNameOnSelect);
        playerName.onSelect.AddListener(SetPlayerNameOnSelect);
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




    void CreateLobbyButtonPressed()
    {
        if (!photonView.IsMine) return;
        Blink.instance.CloseEyes(true, () => PhotonVRManager.LeaveCurrentRoomToCreatePrivateRoom());
    }


    void SetLobbyNameOnSelect(string wut)
    {
        keyboardManager.SetOutputField = lobbyName;
    }
    void JoinLobbyButtonPressed()
    {
        if (!photonView.IsMine) return;
        if (lobbyName.text.Length == 0) return;

        Blink.instance.CloseEyes(true, () => PhotonVRManager.LeaveCurrentRoomToJoinPrivateRoom(lobbyName.text));

    }


    public void SetCurrentLobbyName(string newLobbyName)
    {
        currentLobby.text = "Current lobby: " + newLobbyName;
    }



    void SetPlayerNameOnSelect(string wut)
    {
        Debug.Log(wut);
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
