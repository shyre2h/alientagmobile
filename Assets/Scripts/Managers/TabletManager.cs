using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using Photon.Voice.Unity;
using Photon.Voice.Unity.Demos;
public class TabletManager : MonoBehaviour
{
    //Used to show/hids tablet
    bool isTabletEnabled = false;

    public InputActionAsset inputActionReference;
    public string controllerName;
    public string actionToggleTablet;
    InputActionMap inputActionMap;
    InputAction inputActionPrimaryButton;
    public float value;
    public Recorder recorder;

    [Header("Player fields")]
    public string playerName;
    [Header("Game fields")]
    public Slider bgMusicSlider;
    public Slider sfxMusicSlider;
    public Toggle micToggle;

    [Header("lobby fields")]
    public string lobbyName;


    private void Awake()
    {
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
