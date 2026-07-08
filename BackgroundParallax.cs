using UnityEngine;
using System.Collections.Generic;
// Delete backslashes just in case

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private List<float> _relativeXSpeed;
    //[SerializeField] private List<float> _relativeYSpeed;
    private List<Transform> _backgroundLayers = new List<Transform>();
    private Transform _cameraTransform;
    private Vector2 _currentVector;
    private List<float> _originalDistancesX = new List<float>();
    //private List<float> _originalDistancesY = new List<float>();
    void Awake()
    {
        _currentVector = transform.GetChild(0).position;
        _cameraTransform = Camera.main.transform;
        foreach (Transform layer in transform)
        {
            _backgroundLayers.Add(layer);
            _originalDistancesX.Add(layer.position.x - _cameraTransform.position.x);
            //_originalDistancesY.Add(layer.position.y - _cameraTransform.position.y);
        }
    }
    void Update()
    {
        for (int i = 0; i < _backgroundLayers.Count; i++)
        {
            _currentVector.x = _cameraTransform.position.x * _relativeXSpeed[i] + _originalDistancesX[i];
            //_currentVector.y = _cameraTransform.position.y * _relativeYSpeed[i] + _originalDistancesY[i];
            transform.GetChild(i).position = _currentVector;
        }
    }
}
