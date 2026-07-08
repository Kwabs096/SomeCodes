using UnityEngine;
using DG.Tweening;
using System.Collections;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float _maxPlayerDistance;

    [SerializeField] private Transform _player;
    [SerializeField] private int _speed;
    private Vector3 _offset;

    [SerializeField] private float _specialMovementDuration = 0.5f;
    private bool _canMove = true;

    private bool _toSpecialPoint;
    private Transform _specialPoint;

    private Camera _thisCamera;
    private float _sizeCamera;


    public static CameraFollow _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _offset = new Vector3(0f, 0f, -10f);
        _thisCamera = GetComponent<Camera>();
        _sizeCamera = _thisCamera.orthographicSize;
    }
    void LateUpdate()
    {
        if (!_canMove) return;
        
        if (_toSpecialPoint)
        {
            transform.position = _specialPoint.position;
        }

        Vector3 vectorToMove = transform.position - _offset;
        Vector3 currentOffset = _player.position - transform.position;

        if (currentOffset.x > _maxPlayerDistance)
            vectorToMove.x += currentOffset.x - _maxPlayerDistance;
        else if(currentOffset.x < -_maxPlayerDistance)
            vectorToMove.x += currentOffset.x + _maxPlayerDistance;

        if (currentOffset.y > _maxPlayerDistance)
            vectorToMove.y += currentOffset.y - _maxPlayerDistance;
        else if(currentOffset.y < -_maxPlayerDistance)
            vectorToMove.y += currentOffset.y + _maxPlayerDistance;

        transform.position = Vector3.Lerp(transform.position, vectorToMove + _offset, _speed * Time.fixedDeltaTime);
    }
    public void ToPointForTime(Vector3 point, float duration = 1.5f)
    {
        transform.DOMove(point + _offset, _specialMovementDuration);
        StartCoroutine(DisableForTime(duration));
    }
    public void ToTransformForTime(Transform transformPoint, float duration = 1.5f)
    {
        transform.DOMove(transformPoint.position + _offset, _specialMovementDuration);
        _specialPoint = transformPoint;
        StartCoroutine(MoveForTime(duration));
    }
    private IEnumerator DisableForTime(float time)
    {
        _canMove = false;
        yield return new WaitForSeconds(time);
        _canMove = true;
    }
    private IEnumerator MoveForTime(float time)
    {
        yield return StartCoroutine(DisableForTime(_specialMovementDuration));
        _toSpecialPoint = true;
        yield return new WaitForSeconds(time);
        _toSpecialPoint = false;
    }

    public void ToPointEnter(Vector3 point, float sizeCamera)
    {
        transform.DOMove(point + _offset, _specialMovementDuration);
        _thisCamera.DOOrthoSize(sizeCamera, _specialMovementDuration);
        _canMove = false;
    }
    public void ToPointExit()
    {
        _canMove = true;
        _thisCamera.DOOrthoSize(_sizeCamera, _specialMovementDuration);
    }
}
