using UnityEngine;


struct HittedParams {
  public bool isHittedRight;
  public bool isHittedLeft;
}
class WalkingState : BaseState
{
  private Player _player;
  private float _horizontalInput;
  CollisionManager _collisionManager;
  private float _speed = 8f;
  HittedParams hittedParams;
  public WalkingState(string name, Player player,CollisionManager collisionManager) : base(name)
  {
    _player = player;
    _collisionManager = collisionManager;
  }
  public override void Enter()
  {
    base.Enter();
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }

  private bool IsMovementCloseToZero(){
      return Mathf.Abs(CtxInputManager.getHorizontalInput()) < Mathf.Epsilon;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    _horizontalInput = CtxInputManager.getHorizontalInput();
  }

  private bool IsMovingIntoWall(){
    return (_player.hittedWall.normal.x<0f&& CtxInputManager.getHorizontalInput()>0f)||(_player.hittedWall.normal.x>0f
    && CtxInputManager.getHorizontalInput()<0f);
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
            }).CheckWall(Player.WALL_OFFSET,(leftColliders)=>{
              //* если есть хоть одно столкновение то врезался в лево
              if(leftColliders.Count == 0){
                  hittedParams.isHittedLeft  = false;
              } else {
                  _player.hittedWall = leftColliders[0];
                  hittedParams.isHittedLeft  = true;
              }
            },(rightColliders)=>{
              //* если есть хоть одно столкновение то врезался в право
              if(rightColliders.Count == 0){
                  hittedParams.isHittedRight = false;
              } else {
                  _player.hittedWall = rightColliders[0];
                  hittedParams.isHittedRight = true;
              }
            });
            if(!_player.isGrounded){
              _player._velocity.y -= Player.GRAVITY*Time.fixedDeltaTime;
            } 
            if(!(hittedParams.isHittedLeft || hittedParams.isHittedRight)){
              _player.hittedWall = null;
            }
            //* если движение противоположно направлению нормали стенки, с которой он столкнулся, то скорость обнуляем:
            if(_player.hittedWall != null){
                if(!IsMovingIntoWall()){
                  _player._velocity.x = _horizontalInput * Player.SPEED_MAGNITUDE;
                } 
            } else {
              _player._velocity.x = _horizontalInput * Player.SPEED_MAGNITUDE;
            }
  }
  public override void Exit()
  {
    base.Exit();
  }
}