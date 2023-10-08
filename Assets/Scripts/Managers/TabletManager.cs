using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
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


    private void Awake()
    {
        transform.localScale = Vector3.zero;
        isTabletEnabled = false;
        inputActionMap = inputActionReference.FindActionMap(controllerName);
        inputActionPrimaryButton = inputActionMap.FindAction(actionToggleTablet);
        inputActionPrimaryButton.performed += ToggleTablet;
    }


    private void OnEnable()
    {
        inputActionPrimaryButton.Enable();
    }

    private void OnDisble()
    {
        inputActionPrimaryButton.Disable();
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

    // private void Update()
    // {
    //     var primaryButtonValue = inputActionPrimaryButton.ReadValue<float>();
    //     value = inputActionPrimaryButton.ReadValue<float>();
    // }

}
