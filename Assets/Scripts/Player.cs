using System.Collections;
using UnityEngine;

public class Player 
{
    public Vector2 _velocity;
    public static float SPEED_MAGNITUDE = 5f;
    private Transform _transform;
    public bool isGrounded;

    public CollisionExpanded hittedWall;
    public Transform GetTransform(){
        return _transform;
    }
    private BoxCollider2D _boxCollider;
    public BoxCollider2D GetBoxCollider(){
        return _boxCollider;
    }
    public static float WALL_OFFSET = 0.1f;
    public static float GRAVITY = 8f;
    public Player(Transform transform, BoxCollider2D boxCollider){
        _transform = transform;
        _boxCollider = boxCollider;
    }
    public Vector2 GetVelocity(){
        return _velocity;
    }
    public void Stop(){
        _velocity = Vector2.zero;
    }
    
    public void SetVelocity(Vector2 newValue){
        _velocity = new Vector2(newValue.x,newValue.y);
    }
}


