using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerCrouchController : MonoBehaviour
{
    public const string ActionName = "Crouch";
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FirstPersonController movement;
    [SerializeField] private StarterAssetsInputs inputs;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform cameraHeightRoot;
    [SerializeField] private TMP_Text crouchStatus;
    [SerializeField, Range(.5f,.75f)] private float crouchHeightRatio=.6f;
    [SerializeField, Range(.1f,.5f)] private float transitionSeconds=.22f;
    [SerializeField, Range(.4f,.9f)] private float speedMultiplier=.65f;
    [SerializeField] private LayerMask overheadMask=~(1<<2);
    private float standingHeight, crouchingHeight, standingMoveSpeed, standingSprintSpeed, standingJumpHeight;
    private Vector3 standingCenter, crouchingCenter, standingCameraPosition, crouchingCameraPosition;
    private bool wantsCrouch, inputEnabled=true, blockedLogged;
    public bool IsCrouching => wantsCrouch;
    public bool CanStand { get; private set; }=true;
    public float CurrentHeight => characterController==null?0f:characterController.height;
    public float StandingHeight => standingHeight;
    public float CrouchingHeight => crouchingHeight;
    public Collider OverheadObstructionCollider { get; private set; }
    public event Action<bool> CrouchChanged;

    private void Awake()
    {
        if(characterController==null)characterController=GetComponent<CharacterController>();
        if(movement==null)movement=GetComponent<FirstPersonController>();
        if(inputs==null)inputs=GetComponent<StarterAssetsInputs>();
        if(playerInput==null)playerInput=GetComponent<PlayerInput>();
        standingHeight=characterController.height;standingCenter=characterController.center;
        crouchingHeight=CalculateCrouchHeight(standingHeight,characterController.radius,crouchHeightRatio);
        crouchingCenter=standingCenter-Vector3.up*(standingHeight-crouchingHeight)*.5f;
        if(cameraHeightRoot!=null){standingCameraPosition=cameraHeightRoot.localPosition;crouchingCameraPosition=standingCameraPosition-Vector3.up*(standingHeight-crouchingHeight)*.82f;}
        if(movement!=null){standingMoveSpeed=movement.MoveSpeed;standingSprintSpeed=movement.SprintSpeed;standingJumpHeight=movement.JumpHeight;}
        SetStatus(false);
    }
    private void Update()
    {
        bool held=inputEnabled&&playerInput!=null&&playerInput.actions!=null&&playerInput.actions.FindAction(ActionName,false)?.IsPressed()==true;
        if(held&&!wantsCrouch)BeginCrouch();
        else if(!held&&wantsCrouch&&CheckCanStand())EndCrouch();
        else if(!held&&wantsCrouch&&!CanStand&&!blockedLogged){blockedLogged=true;Debug.Log("[Crouch] Stand blocked by "+(OverheadObstructionCollider?.name??"overhead geometry"),this);}
        SmoothPosture();
        if(wantsCrouch&&inputs!=null)inputs.JumpInput(false);
    }
    public void SetInputEnabled(bool enabled){inputEnabled=enabled;if(!enabled&&inputs!=null)inputs.JumpInput(false);}
    public void BeginCrouch()
    {
        if(wantsCrouch)return;wantsCrouch=true;blockedLogged=false;
        if(movement!=null){movement.MoveSpeed=standingMoveSpeed*speedMultiplier;movement.SprintSpeed=standingSprintSpeed*speedMultiplier;movement.JumpHeight=0f;}
        SetStatus(true);CrouchChanged?.Invoke(true);Debug.Log("[Crouch] Crouch started.",this);
    }
    public bool TryStand()
    {
        if(!wantsCrouch)return true;if(!CheckCanStand())return false;EndCrouch();return true;
    }
    private void EndCrouch()
    {
        wantsCrouch=false;blockedLogged=false;
        if(movement!=null){movement.MoveSpeed=standingMoveSpeed;movement.SprintSpeed=standingSprintSpeed;movement.JumpHeight=standingJumpHeight;}
        SetStatus(false);CrouchChanged?.Invoke(false);Debug.Log("[Crouch] Crouch ended.",this);
    }
    public bool CheckCanStand()
    {
        OverheadObstructionCollider=null;
        float radius=Mathf.Max(.05f,characterController.radius-.02f);
        Vector3 center=transform.TransformPoint(standingCenter);
        Vector3 up=transform.up;
        Vector3 p1=center-up*(standingHeight*.5f-radius);
        Vector3 p2=center+up*(standingHeight*.5f-radius);
        foreach(Collider hit in Physics.OverlapCapsule(p1,p2,radius,overheadMask,QueryTriggerInteraction.Ignore))
        {
            if(hit==characterController||hit.transform.IsChildOf(transform))continue;
            OverheadObstructionCollider=hit;CanStand=false;return false;
        }
        CanStand=true;return true;
    }
    private void SmoothPosture()
    {
        float targetHeight=wantsCrouch?crouchingHeight:standingHeight;
        Vector3 targetCenter=wantsCrouch?crouchingCenter:standingCenter;
        float step=Time.unscaledDeltaTime*Mathf.Abs(standingHeight-crouchingHeight)/Mathf.Max(.01f,transitionSeconds);
        characterController.height=Mathf.MoveTowards(characterController.height,targetHeight,step);
        characterController.center=Vector3.MoveTowards(characterController.center,targetCenter,step);
        if(cameraHeightRoot!=null)cameraHeightRoot.localPosition=Vector3.MoveTowards(cameraHeightRoot.localPosition,wantsCrouch?crouchingCameraPosition:standingCameraPosition,step);
    }
    private void SetStatus(bool visible){if(crouchStatus==null)return;crouchStatus.gameObject.SetActive(visible);crouchStatus.text=visible?"AGACHADO":string.Empty;crouchStatus.raycastTarget=false;}
    public static float CalculateCrouchHeight(float standing,float radius,float ratio)=>Mathf.Max(radius*2f+.08f,standing*Mathf.Clamp(ratio,.5f,.75f));
    public static bool CanJump(bool crouching)=>!crouching;
}
