using UnityEngine;
public class CameraFollow : MonoBehaviour
{

  private Transform _playerTransform;
  private Camera _cameraRefference;

  private Vector3 _offset = new Vector3(0,0.1f,-10f);
  private float _smoothTimeOffset = 2f;
  private Vector3 velocity = Vector3.zero;
  private float xMin = -2.58f;
  private float xMax = 37.1f;
  private float yMin = -0.8f;
  private float yMax = 19.03f;
  private void Start()
  {
    PlayerController playerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
    Player player = playerController.GetPlayer();
    _playerTransform = player.GetTransform();
    _cameraRefference = Camera.main;
  }
  private void Update()
  {
    Vector3 targetPosition = new Vector3(_playerTransform.position.x + _offset.x, _playerTransform.position.y + _offset.y, _playerTransform.position.z + _offset.z);

    Vector3 newPosition = Vector3.SmoothDamp(_cameraRefference.transform.position, targetPosition, ref velocity, _smoothTimeOffset * Time.deltaTime);

    newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
    newPosition.y = Mathf.Clamp(newPosition.y, yMin, yMax);
    transform.position = newPosition;

  }
}
