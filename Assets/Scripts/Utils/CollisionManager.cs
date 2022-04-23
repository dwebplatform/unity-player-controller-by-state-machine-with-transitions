using UnityEngine;
using System;
using System.Collections.Generic;


public class CollisionExpanded {
  public Vector2 normal;
  public Collider2D collider;
}

public class CollisionManager
{
  private Player _player;
  private LayerMask isWallLayer;
  private ContactFilter2D _filter;
  private Collider2D[] results = new Collider2D[1];
  public CollisionManager(Player player)
  {
    _player = player;
    _filter.SetLayerMask(LayerMask.GetMask("Ground"));
    isWallLayer = LayerMask.GetMask("Wall");
  }
  public CollisionManager CheckGround(float offset, Action<Vector2, Vector2> groundedCallBack, Action notGroundedCallBack)
  {
    Vector2 boxSize = new Vector2(_player.GetTransform().localScale.x, _player.GetTransform().localScale.y);
    Vector2 point = _player.GetTransform().position + Vector3.down * offset;
    if (Physics2D.OverlapBox(point, boxSize, 0, _filter, results) > 0)
    {
      Vector2 surfacePosition = Physics2D.ClosestPoint(_player.GetTransform().position, results[0]);
      groundedCallBack(surfacePosition, boxSize);
    }
    else
    {
      notGroundedCallBack();
    }
    return this;
  }
  public CollisionManager CheckWall(float offset, Action<List<CollisionExpanded>> onHitRightWallCallBack,
   Action<List<CollisionExpanded>> onHitLeftWallCallBack)
  {
    Vector2 topPosition = new Vector2(_player.GetTransform().position.x, 
    _player.GetTransform().position.y + _player.GetBoxCollider().size.y / 2);
    Vector2 middlePosition = _player.GetTransform().position;
    Vector2 bottomPosition = new Vector2(_player.GetTransform().position.x, _player.GetTransform().position.y - _player.GetBoxCollider().size.y / 2);

    //* three point top, middle, bottom
    Vector2[] positions = { bottomPosition, middlePosition, topPosition };

    List<CollisionExpanded> leftColliders = new List<CollisionExpanded>();
    List<CollisionExpanded> rightColliders = new List<CollisionExpanded>();
    foreach (var position in positions)
    {

      RaycastHit2D rightHit = Physics2D.Raycast(position, Vector2.right,
        _player.GetBoxCollider().size.y / 2 + offset,
      isWallLayer);
      if (rightHit.collider != null)
      {
        //* столкнулся с правой стенкой
        // rightColliders.Add(rightHit.collider);
        CollisionExpanded collision = new CollisionExpanded();
        collision.normal = rightHit.normal;
        collision.collider = rightHit.collider;
        rightColliders.Add(collision);
      }
      RaycastHit2D leftHit = Physics2D.Raycast(position, -Vector2.right, _player.GetBoxCollider().size.y / 2,
      isWallLayer);
      if (leftHit.collider != null)
      {
        //* столкнулся с левой стенкой
        CollisionExpanded collision = new CollisionExpanded();
        collision.normal = leftHit.normal;
        collision.collider = leftHit.collider;
        leftColliders.Add(collision);
      }
    }
    onHitLeftWallCallBack(leftColliders);
    onHitRightWallCallBack(rightColliders);

    return this;
  }
}
