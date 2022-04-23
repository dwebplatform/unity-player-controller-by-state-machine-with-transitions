using UnityEngine;
class IdleState : BaseState 
{
  private Player _player;
  private StateMachine _stateMachine;
  CollisionManager _collisionManager;
  public IdleState(string name, StateMachine stateMachine, Player player, CollisionManager collisionManager):base(name){
      _stateMachine = stateMachine;
      _player = player;
      _collisionManager = collisionManager;
  }
  public override void Enter(){
      base.Enter();
      _player._velocity = Vector2.zero;
  }
    public override void HandleInput(){
        base.HandleInput();
    }
    private bool IsMovementNotZero(){
        return Mathf.Abs(CtxInputManager.getHorizontalInput())>Mathf.Epsilon;
    }
    public override void LogicUpdate(){
        base.LogicUpdate();
    }
    public override void PhysicsUpdate(){
        base.PhysicsUpdate();
        _collisionManager.CheckGround(PlayerController.WALL_OFFSET,
            //* is grounded
            (Vector2 surfacePoint, Vector2 boxSize) =>
            {
              _player.isGrounded = true;
                _player.GetTransform().position = new Vector3(_player.GetTransform().position.x,
                        surfacePoint.y + boxSize.y / 2,
                        _player.GetTransform().position.z);
            //  _player._velocity.y = 0f;
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
    public override void Exit(){
        base.Exit();
    }
}
