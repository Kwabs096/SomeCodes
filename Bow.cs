// This code was written by Kwabs
// Implementation of bow in game Magaharu

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    private PoolManager _poolManager;
    private AudioManager _audioManager;

    [SerializeField] private Slider _bowSlider;
    [SerializeField] private GameObject _arrow;
    [SerializeField] Vector3 _offset;

    [SerializeField] private float _chargeTime;
    private float _startTime;
    public bool _isChargeNow { get; private set; }

    [SerializeField] private float _targetMaxDistance;
    public Transform _target;

    [SerializeField] private InputActionReference _bowKey;
    [SerializeField] private Animator _animator;
    [SerializeField] private Movement _playerMovement;
    void Start() 
    {
        //SET PROPERTIES
        _poolManager = FindAnyObjectByType<PoolManager>();
        _audioManager = FindAnyObjectByType<AudioManager>();


        _bowKey.action.started += StartCharge;
        _bowKey.action.canceled += ReleaseArrow;
        _bowSlider.maxValue = _chargeTime;
    }
    void Update()
    {
        if (_isChargeNow)
        {
            //timer
            _bowSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + _offset);
            float currentTime = Time.time - _startTime;
            _bowSlider.value = Mathf.Clamp(currentTime, 0f, _chargeTime);
            if (currentTime > _chargeTime)
            {
                //indicate full charge
            }
            if (_target != null) // turn player to the target
            {
                if (transform.position.x > _target.position.x)
                    _playerMovement.transform.localScale = new Vector3(_playerMovement._originalScale, _playerMovement.transform.localScale.y);
                else
                    _playerMovement.transform.localScale = new Vector3(-_playerMovement._originalScale, _playerMovement.transform.localScale.y);
            }
        }
    }
    void StartCharge(InputAction.CallbackContext context)
    {
        _isChargeNow = true;
        _startTime = Time.time;
        _bowSlider.gameObject.SetActive(true);
        _bowSlider.value = 0f;
        _audioManager.PlaySFX("Tension");
        _playerMovement.SetCanRotate(false);
        _animator.SetBool("ChargingBow", true);
    }
    void ReleaseArrow(InputAction.CallbackContext context)
    {
        _isChargeNow = false;
        _playerMovement.SetCanRotate(true);
        _animator.SetBool("ChargingBow", false);
        if (Time.time - _startTime > _chargeTime) // check if you actually ready to shoot
        {
            _target = SetTarget();
            if (_target != null)
            {
                Vector2 direction = _target.position - transform.position;

                GameObject _currentArrow = _poolManager.GetObject(_arrow.name);
                _currentArrow.transform.position = transform.position;
                _currentArrow.transform.eulerAngles = new Vector3 (0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg); //set arrow initial rotation

                _audioManager.PlaySFX("Shot");
            }
        }
        _bowSlider.gameObject.SetActive(false);
    }

    

    private Transform SetTarget()
    {
        float minDistance = _targetMaxDistance + 1f;
        Transform _currentTarget = null;
        List<RaycastHit2D> collidingObjects = Physics2D.CircleCastAll(transform.position, _targetMaxDistance, Vector2.zero).ToList();
        foreach (RaycastHit2D hit in collidingObjects)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Transform currentHit = hit.collider.transform;
                if (minDistance > Vector2.Distance(transform.position, currentHit.position))
                {
                    minDistance = Vector2.Distance(transform.position, currentHit.position);
                    _currentTarget = currentHit;
                }
            }
        }
        return _currentTarget;
    }

    #region Unity functions
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _targetMaxDistance);
    }
    void OnDisable()
    {
        _isChargeNow = false;
        _playerMovement.SetCanRotate(true);
        _animator.SetBool("ChargingBow", false);
        _bowSlider.gameObject.SetActive(false);
        _bowSlider.value = 0f;
        _bowKey.action.started -= StartCharge;
        _bowKey.action.canceled -= ReleaseArrow;
    }

    void OnEnable()
    {
        _bowKey.action.started += StartCharge;
        _bowKey.action.canceled += ReleaseArrow;
    }
    void OnDestroy()
    {
        _bowKey.action.started -= StartCharge;
        _bowKey.action.canceled -= ReleaseArrow;
    }
    #endregion
}
