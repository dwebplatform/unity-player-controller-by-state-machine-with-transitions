// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class WalkingAwayFromGrabbingState : BaseState
{
  private Player _player;
  private CollisionManager _collisionManager;
  private ClockController _clockController;
  private float _startTime;
  private HittedParams hittedParams;
  private float _horizontalInput;
  public WalkingAwayFromGrabbingState(string name, Player player, CollisionManager collisionManager, ClockController clockController) : base(name)
  {
    _player = player;
    _collisionManager = collisionManager;
    _clockController = clockController;
  }

  public override void Enter()
  {
    base.Enter();
    _startTime = Time.time;
    _clockController.isGrabWallIgnored = true;
    _player.hittedWall = null;
  }

  private void IgnoreWallCheck()
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
    IgnoreWallCheck();
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
              _player.GetTransform().position = new Vector3(_player.GetTransform().position.x,
                      surfacePoint.y + boxSize.y / 2,
                      _player.GetTransform().position.z);
              _player._velocity.y = 0f;
            },
            //* is not grounded
            () =>
            {
              _player.isGrounded = false;
            }).CheckWall(Player.WALL_OFFSET, (leftColliders) =>
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

            if(!_player.isGrounded){
                _player._velocity.y -= Player.GRAVITY*Time.fixedDeltaTime;
            }

            if(_player.hittedWall != null){
              
            } else {
              _player._velocity.x = _horizontalInput * Player.SPEED_MAGNITUDE;
            }


  }

}
