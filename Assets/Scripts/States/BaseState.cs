using UnityEngine;

abstract public class BaseState {
    private string _name;
    public BaseState(string name){
        _name = name;
    }
    public virtual void Enter(){
        Debug.Log(_name);
    }
    public virtual void HandleInput(){}
    public virtual void LogicUpdate(){
        Debug.Log(_name+"UPDATE");
    }
    public virtual void PhysicsUpdate(){}
    public virtual void Exit(){}
}