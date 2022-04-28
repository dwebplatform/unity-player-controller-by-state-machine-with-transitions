using UnityEngine;
class IdleState : BaseState
{
  private Player _player;
  CollisionManager _collisionManager;
  private Animator _animator;
  public IdleState(string name, Player player, CollisionManager collisionManager, Animator animator) : base(name)
  {
    _player = player;
    _collisionManager = collisionManager;
    _animator = animator;
  }
  public override void Enter()
  {
    base.Enter();
    _player._velocity = Vector2.zero;
    _animator.SetBool("IsIdle", true);
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }
  private bool IsMovementNotZero()
  {
    return Mathf.Abs(CtxInputManager.getHorizontalInput()) > Mathf.Epsilon;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
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
        },
        //* is not grounded
        () =>
        {
          _player.isGrounded = false;
        });
    if (!_player.isGrounded)
    {
      _player._velocity.y -= Player.GRAVITY * Time.fixedDeltaTime;
    }
    
  }
  public override void Exit()
  {
    base.Exit();
    _animator.SetBool("IsIdle", false);
  }
}
