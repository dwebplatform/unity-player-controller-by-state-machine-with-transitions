using System;
using UnityEngine;


public class ClockController {
  public bool isGravityIgnored;
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

  private bool IsPlayerFacesWall(){
    return _player.hittedWall!=null;
  }
  private void Awake()
  {
    _rigidBody = GetComponent<Rigidbody2D>();
    _boxCollider = GetComponent<BoxCollider2D>();
    
    _stateMachine = new StateMachine();

    _player = new Player(transform, _boxCollider);
    clockController = new ClockController();
    PlayerController.collisionManager = new CollisionManager(_player);

    StateMachine.idleState = new IdleState("IdleState", _stateMachine, _player,PlayerController.collisionManager);
    StateMachine.walkingState = new WalkingState("WalkingState", _stateMachine, _player, PlayerController.collisionManager);
    StateMachine.jumpingState = new JumpingState("JumpingState", _stateMachine, _player, PlayerController.collisionManager,clockController);
    StateMachine.grabWallState = new GrabWallState("GrabWallState", _stateMachine, _player);
    //*TODO add WallCheck Collision for player three dots lazer from left and right!!!!!!!
    When(StateMachine.walkingState, StateMachine.idleState, ()=>IsHorizontalNotPressed() && PlayerIsGrounded());
    When(StateMachine.idleState,    StateMachine.walkingState, isHorizontalPressed);
    When(StateMachine.jumpingState, StateMachine.grabWallState,()=>IsPlayerFacesWall() && !PlayerIsGrounded());
    When(StateMachine.idleState,    StateMachine.jumpingState, ()=> Input.GetKey(KeyCode.Space));
    When(StateMachine.walkingState, StateMachine.jumpingState, ()=> Input.GetKey(KeyCode.Space));
    When(StateMachine.jumpingState, StateMachine.idleState,    ()=> PlayerIsGrounded() && !isGravityIgnored());

    _stateMachine.Initialize();

    void When(BaseState from, BaseState to, Func<bool> predicate)
    {
      _stateMachine.AddTransition(from, to, predicate);
    }
  }
  
  private void Update()
  {
    _stateMachine.HandleInput();
    _stateMachine.LogicUpdate();
  }
  public void FixedUpdate()
  {
    _stateMachine.PhysicsUpdate();
    _rigidBody.velocity = _player.GetVelocity();
  }


}
