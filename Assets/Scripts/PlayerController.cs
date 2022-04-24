using System;
using UnityEngine;

public class ClockController {
  public bool isGravityIgnored;
  public bool isGrabWallIgnored;
}
public class PlayerController : MonoBehaviour
{
  private Player _player;
  private StateMachine _stateMachine;
  public Rigidbody2D _rigidBody { get; private set; }
  public BoxCollider2D _boxCollider { get; private set; }
  public static CollisionManager collisionManager;
  public static float WALL_OFFSET = 0.1f;
  public ClockController clockController;

  private bool IsHorizontalNotPressed()
  {
    return Mathf.Abs(CtxInputManager.getHorizontalInput()) <= Mathf.Epsilon;
  }
  private bool isHorizontalPressed()
  {
    return Mathf.Abs(CtxInputManager.getHorizontalInput()) > Mathf.Epsilon;
  }
  private bool isGravityIgnored(){
    return clockController.isGravityIgnored;
  }
  private bool PlayerIsGrounded(){
    return _player.isGrounded;
  }


  private bool IsHorizontalPushedToWall(){
    if(_player.hittedWall == null){
      return false;
    }

    Vector2 normal = _player.hittedWall.normal;
    return (CtxInputManager.getHorizontalInput()>0f&& normal.x<0f)||(CtxInputManager.getHorizontalInput()<0f&& normal.x>0f);
  }
  private bool IsHorizontalInversedToWall(){

    if(_player.hittedWall == null){
      return false;
    }

    Vector2 normal = _player.hittedWall.normal;
    return (CtxInputManager.getHorizontalInput() > 0f && normal.x > 0f) || (CtxInputManager.getHorizontalInput() < 0f && normal.x < 0f);
  }  
  
  private bool IsPreviosInputOver(){
    return !_player.isPreviosInputExist;
  }
  private bool IsHorizontalPressedReversedToMovement(){

    return (CtxInputManager.getHorizontalInput()>0f && _player._velocity.x<0f)||((CtxInputManager.getHorizontalInput()<0f && _player._velocity.x>0f));
  }


  private bool IsWallCheckIgnored(){
    return clockController.isGrabWallIgnored;
  }
  private bool IsPlayerFacesWall(){
    //* player faces wall when direction of normal and input inversed to each other
    if(_player.hittedWall == null){
      return false;
    }

    Vector2 normal = _player.hittedWall.normal;

    return  (_player._velocity.x > 0f && normal.x < 0f) || (_player._velocity.x < 0f && normal.x > 0f);
  }

  private void Awake()
  {
    _rigidBody = GetComponent<Rigidbody2D>();
    _boxCollider = GetComponent<BoxCollider2D>();
    
    _stateMachine = new StateMachine();

    _player = new Player(transform, _boxCollider);
    clockController = new ClockController();
    PlayerController.collisionManager = new CollisionManager(_player);

    StateMachine.idleState = new IdleState("IdleState", _player,PlayerController.collisionManager);
    StateMachine.walkingState = new WalkingState("WalkingState", _player, PlayerController.collisionManager);
    StateMachine.jumpingState = new JumpingState("JumpingState",  _player, PlayerController.collisionManager,clockController);
    StateMachine.grabWallState = new GrabWallState("GrabWallState", _player, PlayerController.collisionManager);
    StateMachine.walkingAwayFromGrabbingState = new WalkingAwayFromGrabbingState("WalkingAwayFromGrabbingState", _player,PlayerController.collisionManager, clockController);
    StateMachine.jumpAwayWallWithInitialHorizontalInput = new JumpAwayWallWithInitialHorizontalInput("JumpAwayWallWithInitialHorizontalInput",_player,collisionManager, clockController);


    When(StateMachine.jumpAwayWallWithInitialHorizontalInput,StateMachine.walkingState,()=>IsPreviosInputOver() && IsHorizontalPressedReversedToMovement());
    When(StateMachine.jumpAwayWallWithInitialHorizontalInput, StateMachine.idleState,()=>PlayerIsGrounded());

    When(StateMachine.walkingAwayFromGrabbingState, StateMachine.grabWallState,()=>!IsWallCheckIgnored() && IsPlayerFacesWall());
    When(StateMachine.walkingAwayFromGrabbingState,StateMachine.idleState,()=>PlayerIsGrounded());
    
    When(StateMachine.grabWallState, StateMachine.jumpAwayWallWithInitialHorizontalInput,()=>IsHorizontalPushedToWall() && Input.GetKey(KeyCode.Space));
    When(StateMachine.grabWallState, StateMachine.idleState, PlayerIsGrounded);
    When(StateMachine.grabWallState, StateMachine.walkingAwayFromGrabbingState,()=>IsHorizontalInversedToWall());

    When(StateMachine.walkingState, StateMachine.idleState,    ()=>IsHorizontalNotPressed() && PlayerIsGrounded());
    When(StateMachine.walkingState, StateMachine.jumpingState, ()=> Input.GetKey(KeyCode.Space) && PlayerIsGrounded());
    When(StateMachine.walkingState, StateMachine.grabWallState, ()=>IsPlayerFacesWall() && !PlayerIsGrounded());

    When(StateMachine.idleState,    StateMachine.walkingState, isHorizontalPressed);
    When(StateMachine.idleState,    StateMachine.jumpingState, ()=> Input.GetKey(KeyCode.Space));
    
    When(StateMachine.jumpingState, StateMachine.grabWallState,()=>IsPlayerFacesWall() && !PlayerIsGrounded());
    When(StateMachine.jumpingState, StateMachine.idleState,    ()=> PlayerIsGrounded() && !isGravityIgnored());

    _stateMachine.Initialize();

    void When(BaseState from, BaseState to, Func<bool> predicate)
    {
      _stateMachine.AddTransition(from, to, predicate);
    }
  }
  
  private void Update()
  {
    _stateMachine?.HandleInput();
    _stateMachine?.LogicUpdate();
  }
  public void FixedUpdate()
  {
    _stateMachine.PhysicsUpdate();
    _rigidBody.velocity = _player.GetVelocity();
  }


}
