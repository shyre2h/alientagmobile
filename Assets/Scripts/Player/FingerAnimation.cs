using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FingerAnimation : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionReference;
    [SerializeField] Animator animator;
    InputActionMap inputActionMapLeft, inputActionMapRight;
    InputAction inputActionTriggerLeft, inputActionTriggerRight, inputActionGripLeft, inputActionGripRight;
    [SerializeField] string controllerNameLeft, controllerNameRight;
    [SerializeField] string triggerButton;
    [SerializeField] string gripButton;





    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        inputActionMapLeft = inputActionReference.FindActionMap(controllerNameLeft);
        inputActionMapRight = inputActionReference.FindActionMap(controllerNameRight);

        inputActionTriggerLeft = inputActionMapLeft.FindAction(triggerButton);
        inputActionTriggerRight = inputActionMapRight.FindAction(triggerButton);

        inputActionGripLeft = inputActionMapLeft.FindAction(gripButton);
        inputActionGripRight = inputActionMapRight.FindAction(gripButton);

        // Debug.Log(inputActionMapLeft.name);
        // Debug.Log(inputActionMapRight.name);

    }


    private void OnEnable()
    {
        inputActionTriggerLeft.Enable();
        inputActionTriggerRight.Enable();
        inputActionGripLeft.Enable();
        inputActionGripRight.Enable();
    }

    private void OnDisble()
    {
        inputActionTriggerLeft.Disable();
        inputActionTriggerRight.Disable();
        inputActionGripLeft.Disable();
        inputActionGripRight.Disable();
    }



    private void Update()
    {
        animator.SetFloat("l_thumb", inputActionTriggerLeft.ReadValue<float>());
        animator.SetFloat("l_fingers", inputActionGripLeft.ReadValue<float>());
        animator.SetFloat("r_thumb", inputActionTriggerRight.ReadValue<float>());
        animator.SetFloat("r_fingers", inputActionGripRight.ReadValue<float>());
    }







}
