// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class JumpAwayWallWithInitialHorizontalInput : BaseState
{
  private Player _player;
  private ClockController _clockController;
  private CollisionManager _collisionManager;
  private float _startTime;
  HittedParams hittedParams;
  public JumpAwayWallWithInitialHorizontalInput(string name, Player player, CollisionManager collisionManager, ClockController clockController) : base(name)
  {
    _player = player;
    _clockController = clockController;
    _collisionManager = collisionManager;
  }
  private bool IsPressedAnyMovementButton()
  {
    return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
  }
  public override void Enter()
  {
    base.Enter();
    _startTime = Time.time;
    _player.isPreviosInputExist = true;
    _clockController.isGrabWallIgnored = true;
    _player._velocity = new Vector2(Mathf.Sign(_player.hittedWall.normal.x)* Player.SPEED_MAGNITUDE, Player.SPEED_MAGNITUDE);
  }
  private void IsWallCheckIgnored()
  {
    float deltaTime = Time.time - _startTime;
    if (deltaTime < 0.2f)
    {
      _clockController.isGrabWallIgnored = true;
    }
    else
    {
      _clockController.isGrabWallIgnored = false;
    }
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    IsWallCheckIgnored();
    //*нажал на кнопку
    if (_player.isPreviosInputExist)
    {
      //*если что-то нажал
      if (IsPressedAnyMovementButton())
      {
        _player.isPreviosInputExist = false;
      }
      else
      {
        if (Mathf.Abs(CtxInputManager.getHorizontalInput()) < Mathf.Epsilon)
        {
          _player.isPreviosInputExist = false;
        }
      }
    }
    else
    {
      //* если нажали на кнопку в том же направлении, то не реагируем, 

    }
    //* проверим, если нажал на кнопку, horizontal Input если 
  }

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _collisionManager
    .CheckGround(PlayerController.WALL_OFFSET,
            //* is grounded
            (Vector2 surfacePoint, Vector2 boxSize) =>
            {
              _player.isGrounded = true;
              _player.GetTransform().position = new Vector3(_player.GetTransform().position.x,
                      surfacePoint.y + boxSize.y / 2,
                      _player.GetTransform().position.z);
              _player._velocity.y = 0f;
            },
            //* is not grounded
            () =>
            {
              _player.isGrounded = false;
            })
            .CheckWall(Player.WALL_OFFSET, (leftColliders) =>
            {
              //* если есть хоть одно столкновение то врезался в лево
              if (!_clockController.isGrabWallIgnored)
              {

                if (leftColliders.Count == 0)
                {
                  hittedParams.isHittedLeft = false;
                }
                else
                {
                  _player.hittedWall = leftColliders[0];
                  hittedParams.isHittedLeft = true;
                }
              }
            }, (rightColliders) =>
            {
              //* если есть хоть одно столкновение то врезался в право
              if (!_clockController.isGrabWallIgnored)
              {
                if (rightColliders.Count == 0)
                {
                  hittedParams.isHittedRight = false;
                }
                else
                {
                  _player.hittedWall = rightColliders[0];
                  hittedParams.isHittedRight = true;
                }
              }
            });

    if (!(hittedParams.isHittedLeft || hittedParams.isHittedRight))
    {
      _player.hittedWall = null;
    }
    if (!_player.isGrounded)
    {
      _player._velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }

  }

}
