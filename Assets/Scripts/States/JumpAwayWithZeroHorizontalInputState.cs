// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
public class JumpAwayWithZeroHorizontalInputState : BaseState
{
  private Player _player;
  private CollisionManager _collisionManager;
  private float _startTime;
  private ClockController _clockController;
  private HittedParams _hittedParams;
  public JumpAwayWithZeroHorizontalInputState(string name, Player player, CollisionManager collisionManager, ClockController clockController) : base(name)
  {
    _player = player;
    _collisionManager = collisionManager;
    _clockController = clockController;
  }

  public override void Enter()
  {
    base.Enter();
    Vector2 normal = _player.hittedWall.normal;
    _player._velocity = new Vector2(Mathf.Sign(normal.x) * Player.SPEED_MAGNITUDE, Player.SPEED_MAGNITUDE);
    _startTime = Time.time;
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
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    _collisionManager.CheckGround(PlayerController.WALL_OFFSET,
            (Vector2 surfacePoint, Vector2 boxSize) =>
            {
              _player.isGrounded = true;
              _player.GetTransform().position = new Vector3(_player.GetTransform().position.x,
                      surfacePoint.y + boxSize.y / 2,
                      _player.GetTransform().position.z);
              _player._velocity.y = 0f;
            },
            () =>
            {
              _player.isGrounded = false;
            }).CheckWall(Player.WALL_OFFSET, (leftColliders) =>
            {
              if (!_clockController.isGrabWallIgnored)
              {
                if (leftColliders.Count == 0)
                {
                  _hittedParams.isHittedLeft = false;
                }
                else
                {
                  _player.hittedWall = leftColliders[0];
                  _hittedParams.isHittedLeft = true;
                }
              }
            }, (rightColliders) =>
            {
              if (!_clockController.isGrabWallIgnored)
              {
                if (rightColliders.Count == 0)
                {
                  _hittedParams.isHittedRight = false;
                }
                else
                {
                  _player.hittedWall = rightColliders[0];
                  _hittedParams.isHittedRight = true;
                }
              }
            });


    if (!_player.isGrounded)
    {
      _player._velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }

    if (!(_hittedParams.isHittedLeft || _hittedParams.isHittedRight))
    {
      _player.hittedWall = null;
    }
    
  }
}
