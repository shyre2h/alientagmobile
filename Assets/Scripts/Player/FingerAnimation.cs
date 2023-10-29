using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class FingerAnimation : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionReference;
    [SerializeField] Animator animator;
    InputActionMap inputActionMapLeft, inputActionMapRight;
    InputAction inputActionTriggerLeft, inputActionTriggerRight, inputActionGripLeft, inputActionGripRight;
    InputAction inputActionA, inputActionB, inputActionX, inputActionY;
    [SerializeField] string controllerNameLeft, controllerNameRight;
    [SerializeField] string triggerButton;
    [SerializeField] string gripButton;
    [SerializeField] string aButton;
    [SerializeField] string bButton;
    [SerializeField] string xButton;
    [SerializeField] string yButton;
    [SerializeField] PhotonView photonView;
    //grip, trigger, b/y, a/x

    public float a, b, x, y;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!photonView) photonView = GetComponent<PhotonView>();
        inputActionMapLeft = inputActionReference.FindActionMap(controllerNameLeft);
        inputActionMapRight = inputActionReference.FindActionMap(controllerNameRight);

        inputActionTriggerLeft = inputActionMapLeft.FindAction(triggerButton);
        inputActionTriggerRight = inputActionMapRight.FindAction(triggerButton);

        inputActionGripLeft = inputActionMapLeft.FindAction(gripButton);
        inputActionGripRight = inputActionMapRight.FindAction(gripButton);


        inputActionA = inputActionMapRight.FindAction(aButton);
        inputActionB = inputActionMapRight.FindAction(bButton);

        inputActionX = inputActionMapLeft.FindAction(xButton);
        inputActionY = inputActionMapLeft.FindAction(yButton);

        // Debug.Log(inputActionMapLeft.name);
        // Debug.Log(inputActionMapRight.name);

    }


    private void OnEnable()
    {
        inputActionTriggerLeft.Enable();
        inputActionTriggerRight.Enable();
        inputActionGripLeft.Enable();
        inputActionGripRight.Enable();
        inputActionA.Enable();
        inputActionB.Enable();
        inputActionX.Enable();
        inputActionY.Enable();
    }

    private void OnDisble()
    {
        inputActionTriggerLeft.Disable();
        inputActionTriggerRight.Disable();
        inputActionGripLeft.Disable();
        inputActionGripRight.Disable();
        inputActionA.Disable();
        inputActionB.Disable();
        inputActionX.Disable();
        inputActionY.Disable();
    }



    private void Update()
    {
        if (!photonView.IsMine) return;

        animator.SetFloat("l_thumb", inputActionTriggerLeft.ReadValue<float>());
        animator.SetFloat("r_thumb", inputActionTriggerRight.ReadValue<float>());

        animator.SetFloat("l_index", inputActionGripLeft.ReadValue<float>());
        animator.SetFloat("r_index", inputActionGripRight.ReadValue<float>());


        animator.SetFloat("l_middle", inputActionY.ReadValue<float>());
        animator.SetFloat("r_middle", inputActionB.ReadValue<float>());

        animator.SetFloat("l_ring", inputActionX.ReadValue<float>());
        animator.SetFloat("r_ring", inputActionA.ReadValue<float>());


        //Testing
        // a = inputActionA.ReadValue<float>();
        // x = inputActionX.ReadValue<float>();
        // b = inputActionB.ReadValue<float>();
        // y = inputActionY.ReadValue<float>();
    }







}
