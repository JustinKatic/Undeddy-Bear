using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;


public class CharacterLocoMotion : MonoBehaviour
{
    Animator animator;
    Vector2 input;
    public float moveSpeed;
    CharacterMovement playerInput;
    PhotonView PV;
    private ShootProjectile shootProjectile;
    private DevourEnemy devourEnemy;

    private void Awake()
    {
        //Player Input
        playerInput = new CharacterMovement();
        playerInput.Movement.Shoot.performed += OnFire;
        playerInput.Movement.Devour.performed += OnDevour;


        //Get Components off Player
        shootProjectile = GetComponent<ShootProjectile>();
        devourEnemy = GetComponent<DevourEnemy>();

        //Player Animation
        animator = GetComponent<Animator>();
        animator.SetFloat("MoveSpeed", moveSpeed);

        //Networking
        PV = GetComponent<PhotonView>();


    }
    private void Start()
    {
        //Makes Sure the player cant control others while networking
        if (!PV.IsMine)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
    }



    private void Update()
    {
        if (!PV.IsMine)
            return;


        input = playerInput.Movement.Move.ReadValue<Vector2>();

        animator.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
        animator.SetFloat("InputY", input.y, 0.1f, Time.deltaTime);
    }

    private void OnEnable()
    {
        if (!PV.IsMine)
            return;
        playerInput.Movement.Enable();
    }

    private void OnDisable()
    {
        if (!PV.IsMine)
            return;
        playerInput.Movement.Disable();
    }

    void OnFire(InputAction.CallbackContext callback)
    {
        shootProjectile.Shoot();
    }

    void OnDevour(InputAction.CallbackContext callback)
    {
        //If able to devour Disable this controls
        devourEnemy.CheckIfEnemyIsInRange();
        if (devourEnemy.isDevouringEnemy)
        {
            DisableControls();
        }

    }

    //Used to Disable controls if stunned
    public void DisableControls()
    {
        playerInput.Movement.Disable();

    }
    //Used to Enable controls if stunned
    public void EnableControls()
    {
        if (!devourEnemy.isDevouringEnemy)
            playerInput.Movement.Enable();
    }
}
