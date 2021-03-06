using UnityEngine;
class GrabWallState : BaseState
{
  private CollisionManager _collisionManager;
  private Player _player;
  HittedParams _hittedParams;
  public GrabWallState(string name, Player player, CollisionManager collisionManager) : base(name)
  {
    _player = player;
    _collisionManager = collisionManager;
  }
  
  public override void Enter()
  {
    base.Enter();
    _player._velocity = Vector2.zero;
    Vector3 playerPosition = _player.GetTransform().position;
    BoxCollider2D playerBoxCollider = _player.GetBoxCollider();
    _player.GetTransform().position = CollisionManager.AdjustPosition(_player.hittedWall, playerPosition, playerBoxCollider);
  }

  public override void LogicUpdate()
  {
    base.LogicUpdate();
  }
  public override void Exit()
  {
    base.Exit();
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
                if (leftColliders.Count == 0)
                {
                  _hittedParams.isHittedLeft = false;
                }
                else
                {
                  _player.hittedWall = leftColliders[0];
                  _hittedParams.isHittedLeft = true;
                }
            }, (rightColliders) =>
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
            });
    if(!_hittedParams.isHittedLeft && !_hittedParams.isHittedRight){
      _player.hittedWall = null;
      
    }
    if (!_player.isGrounded)
    {
      _player._velocity.y -= Player.FRICTION_GRAVITY * Time.fixedDeltaTime;
    }
  }
}
