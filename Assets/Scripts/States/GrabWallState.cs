// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

class GrabWallState : BaseState
{

    private  StateMachine _stateMachine;
    private Player _player;
    public GrabWallState(string name, StateMachine stateMachine, Player player):base(name){
        _player = player;
    }
  public override void Enter()
  {
    base.Enter();
    _player._velocity = Vector2.zero;
  }
}
