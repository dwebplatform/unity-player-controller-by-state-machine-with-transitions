// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;


class JumpingState : BaseState
{
  private StateMachine _stateMachine;
  private Player _player;
  private CollisionManager _collisionManager;
  private float _jumpForce = 5f;
  private float _startTime;
  private float _horizontalInput;
  private ClockController _clockController;

  private HittedParams hittedParams;
  public JumpingState(string name, StateMachine stateMachine, Player player, CollisionManager collisionManager, ClockController clockController) : base(name)
  {
    _stateMachine = stateMachine;
    _player = player;
    _collisionManager = collisionManager;
    _clockController = clockController;
  }


  private bool IsMovingIntoWall()
  {
    return (_player.hittedWall.normal.x < 0f && CtxInputManager.getHorizontalInput() > 0f) || (_player.hittedWall.normal.x > 0f
    && CtxInputManager.getHorizontalInput() < 0f);
  }
  private void IgnoreGravityCheck()
  {
    float deltaTime = Time.time - _startTime;
    if (deltaTime < 0.2f)
    {
      _clockController.isGravityIgnored = true;
    }
    else
    {
      _clockController.isGravityIgnored = false;
    }
  }
  public override void Enter()
  {
    base.Enter();
    Vector2 targetVelocity = _player.GetVelocity();
    _player._velocity = new Vector2(targetVelocity.x, _jumpForce);
    _startTime = Time.time;
    _clockController.isGravityIgnored = true;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    IgnoreGravityCheck();
    _horizontalInput = CtxInputManager.getHorizontalInput();
  }

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();

    _collisionManager.CheckGround(PlayerController.WALL_OFFSET,
            //* is grounded
            (Vector2 surfacePoint, Vector2 boxSize) =>
            {
              _player.isGrounded = true;
              if (!_clockController.isGravityIgnored)
              {
                _player.GetTransform().position = new Vector3(_player.GetTransform().position.x,
                        surfacePoint.y + boxSize.y / 2,
                        _player.GetTransform().position.z);
                _player._velocity.y = 0f;
              }
            },
            //* is not grounded
            () =>
            {
              _player.isGrounded = false;
            }).CheckWall(Player.WALL_OFFSET, (leftColliders) =>
            {
              //* если есть хоть одно столкновение то врезался в лево
              if (leftColliders.Count == 0)
              {
                hittedParams.isHittedLeft = false;
              }
              else
              {
                _player.hittedWall = leftColliders[0];
                //TODO Adjust collision position
                hittedParams.isHittedLeft = true;
              }
            }, (rightColliders) =>
            {
              //* если есть хоть одно столкновение то врезался в право
              if (rightColliders.Count == 0)
              {
                hittedParams.isHittedRight = false;
              }
              else
              {
                //TODO Adjust collision position
                _player.hittedWall = rightColliders[0];
                hittedParams.isHittedRight = true;
              }
            });

    if (!(hittedParams.isHittedLeft || hittedParams.isHittedRight))
    {
      _player.hittedWall = null;
    }

    if (_player.hittedWall != null)
    {
      if (!IsMovingIntoWall())
      {
        _player._velocity.x = _horizontalInput * Player.SPEED_MAGNITUDE;
      }
      else
      {
        _player._velocity.x = 0f;
      }
    }
    else
    {
      _player._velocity.x = _horizontalInput * Player.SPEED_MAGNITUDE;
    }

     if (!_player.isGrounded)
    {
      _player._velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }
  }
  public override void Exit()
  {
    base.Exit();
  }
}
