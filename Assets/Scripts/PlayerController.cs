using System;
using UnityEngine;

public class ClockController
{
  public bool isGravityIgnored;
  public bool isGrabWallIgnored;
}

public class PlayerController : MonoBehaviour
{
  private Player _player;
  public Player GetPlayer() => _player;
  private StateMachine _stateMachine;
  public Rigidbody2D _rigidBody { get; private set; }
  public BoxCollider2D _boxCollider { get; private set; }
  public static CollisionManager collisionManager;
  public static float WALL_OFFSET = 0.1f;
  public ClockController clockController;


  private Animator _animator;
  private bool IsHorizontalNotPressed()
  {
    return Mathf.Abs(CtxInputManager.getHorizontalInput()) <= Mathf.Epsilon;
  }
  private bool isHorizontalPressed()
  {
    return Mathf.Abs(CtxInputManager.getHorizontalInput()) > Mathf.Epsilon;
  }
  private bool isGravityIgnored()
  {
    return clockController.isGravityIgnored;
  }
  private bool PlayerIsGrounded()
  {
    return _player.isGrounded;
  }
  private bool IsHorizontalPushedToWall()
  {
    if (_player.hittedWall == null)
    {
      return false;
    }

    Vector2 normal = _player.hittedWall.normal;
    return (CtxInputManager.getHorizontalInput() > 0f && normal.x < 0f) || (CtxInputManager.getHorizontalInput() < 0f && normal.x > 0f);
  }
  private bool IsHorizontalInversedToWall()
  {

    if (_player.hittedWall == null)
    {
      return false;
    }

    Vector2 normal = _player.hittedWall.normal;
    return (CtxInputManager.getHorizontalInput() > 0f && normal.x > 0f) || (CtxInputManager.getHorizontalInput() < 0f && normal.x < 0f);
  }

  private bool IsPreviosInputOver()
  {
    return !_player.isPreviosInputExist;
  }
  private bool IsHorizontalPressedReversedToMovement()
  {
    return (CtxInputManager.getHorizontalInput() > 0f && _player._velocity.x < 0f) || ((CtxInputManager.getHorizontalInput() < 0f && _player._velocity.x > 0f));
  }


  private bool IsWallCheckIgnored()
  {
    return clockController.isGrabWallIgnored;
  }
  private bool IsPressedAnyMovementButton()
  {
    return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
  }
  private bool IsHoldsAnyMovementButton()
  {
    return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
  }

  private bool IsPlayerFacesWall()
  {
    //* player faces wall when direction of normal and input inversed to each other
    if (_player.hittedWall == null)
    {
      return false;
    }

    Vector2 normal = _player.hittedWall.normal;

    return (_player._velocity.x > 0f && normal.x < 0f) || (_player._velocity.x < 0f && normal.x > 0f);
  }

  private void Awake()
  {
    _rigidBody = GetComponent<Rigidbody2D>();
    _boxCollider = GetComponent<BoxCollider2D>();
    _animator = GetComponent<Animator>();
    _stateMachine = new StateMachine();

    _player = new Player(transform, _boxCollider);
    clockController = new ClockController();
    PlayerController.collisionManager = new CollisionManager(_player);

    StateMachine.idleState = new IdleState("IdleState", _player, PlayerController.collisionManager, _animator);
    StateMachine.walkingState = new WalkingState("WalkingState", _player, PlayerController.collisionManager, _animator);
    StateMachine.jumpingState = new JumpingState("JumpingState", _player, PlayerController.collisionManager, clockController);
    StateMachine.grabWallState = new GrabWallState("GrabWallState", _player, PlayerController.collisionManager);
    StateMachine.walkingAwayFromGrabbingState = new WalkingAwayFromGrabbingState("WalkingAwayFromGrabbingState", _player, PlayerController.collisionManager, clockController);
    StateMachine.jumpAwayWallWithInitialHorizontalInput = new JumpAwayWallWithInitialHorizontalInput("JumpAwayWallWithInitialHorizontalInput", _player, collisionManager, clockController);
    StateMachine.jumpAwayWithZeroHorizontalInputState = new JumpAwayWithZeroHorizontalInputState("JumpAwayWithZeroHorizontalInputState", _player, PlayerController.collisionManager, clockController);


    When(StateMachine.jumpAwayWithZeroHorizontalInputState, StateMachine.idleState, () => PlayerIsGrounded());
    When(StateMachine.jumpAwayWithZeroHorizontalInputState, StateMachine.grabWallState, () => !IsWallCheckIgnored() && IsPlayerFacesWall());
    When(StateMachine.jumpAwayWithZeroHorizontalInputState, StateMachine.walkingState, () => IsHorizontalPressedReversedToMovement());

    When(StateMachine.jumpAwayWallWithInitialHorizontalInput, StateMachine.walkingState, () => IsPreviosInputOver() && IsHorizontalPressedReversedToMovement());
    When(StateMachine.jumpAwayWallWithInitialHorizontalInput, StateMachine.idleState, () => PlayerIsGrounded());
    When(StateMachine.jumpAwayWallWithInitialHorizontalInput, StateMachine.grabWallState, () => !IsWallCheckIgnored() && IsPlayerFacesWall());

    When(StateMachine.walkingAwayFromGrabbingState, StateMachine.grabWallState, () => !IsWallCheckIgnored() && IsPlayerFacesWall());
    When(StateMachine.walkingAwayFromGrabbingState, StateMachine.idleState, () => PlayerIsGrounded());
    When(StateMachine.walkingAwayFromGrabbingState, StateMachine.jumpingState, () => Input.GetKey(KeyCode.Space));

    When(StateMachine.grabWallState, StateMachine.jumpAwayWithZeroHorizontalInputState, () => !IsHorizontalPushedToWall() && Input.GetKey(KeyCode.Space));
    When(StateMachine.grabWallState, StateMachine.jumpAwayWallWithInitialHorizontalInput, () => IsHorizontalPushedToWall() && Input.GetKey(KeyCode.Space));
    When(StateMachine.grabWallState, StateMachine.idleState, PlayerIsGrounded);

    When(StateMachine.grabWallState, StateMachine.walkingState, () => _player.hittedWall == null && IsHoldsAnyMovementButton());

    When(StateMachine.walkingState, StateMachine.idleState, () => IsHorizontalNotPressed() && PlayerIsGrounded());
    When(StateMachine.walkingState, StateMachine.jumpingState, () => Input.GetKey(KeyCode.Space) && PlayerIsGrounded());
    When(StateMachine.walkingState, StateMachine.grabWallState, () => IsPlayerFacesWall() && !PlayerIsGrounded());

    When(StateMachine.idleState, StateMachine.walkingState, isHorizontalPressed);
    When(StateMachine.idleState, StateMachine.jumpingState, () => Input.GetKey(KeyCode.Space));

    When(StateMachine.jumpingState, StateMachine.grabWallState, () => IsPlayerFacesWall() && !PlayerIsGrounded());
    When(StateMachine.jumpingState, StateMachine.idleState, () => PlayerIsGrounded() && !isGravityIgnored());

    _stateMachine.Initialize();

    void When(BaseState from, BaseState to, Func<bool> predicate)
    {
      _stateMachine.AddTransition(from, to, predicate);
    }
  }

  private bool _isFacingRight = false;
  private void Update()
  {
    _stateMachine?.HandleInput();
    _stateMachine?.LogicUpdate();
  }
  public void FixedUpdate()
  {
    _stateMachine.PhysicsUpdate();
    _rigidBody.velocity = _player.GetVelocity();
    //* если скорость движения совпадает, с вектор right, то не переворачиваем иначе переворачиваем

    if ((_rigidBody.velocity.x < 0 && _isFacingRight) || (_rigidBody.velocity.x > 0 && !_isFacingRight))
    {
      _isFacingRight = !_isFacingRight;
      transform.Rotate(0,180,0);
    }
    if (_rigidBody.velocity.x > 0f)
    {
      Debug.DrawLine(_player.GetTransform().position, _player.GetTransform().position + Vector3.right);

    }
    else if (_rigidBody.velocity.x < 0f)
    {
      Debug.DrawLine(_player.GetTransform().position, _player.GetTransform().position - Vector3.right);
    }
  }




}
