using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Private Members
    
    [SerializeField] private float _speed;
    [SerializeField] private float _tiltSpeed;

    [SerializeField] private Animator _spriteAnimator;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawn;

    [SerializeField] private float _sufterTilt;

    [SerializeField] private int _boundaryColumnLeft;
    [SerializeField] private int _boundaryColumnRight;

    private bool _isShooting = false;
    private int _shootHash;

    private bool _bounadryInitialised = false;
    private float _lowerYBoundary;
    private float _upperYBoundary;
    private float _leftXBoundary;
    private float _rightXBoundary;

    private int _currentColumn;

    #endregion

    #region Private Methods

    private void Start()
    {
        _shootHash = Animator.StringToHash("Shooting");
    }

    private void Update()
    {
        if (!_bounadryInitialised)
        {
            Camera cam = Camera.main;
            Vector3 camPosition = cam.transform.position;

            _lowerYBoundary = camPosition.y - cam.orthographicSize + 0.5f;
            _upperYBoundary = camPosition.y + cam.orthographicSize - 0.5f;

            _leftXBoundary = camPosition.x - cam.orthographicSize * cam.aspect;
            _rightXBoundary = camPosition.x + cam.orthographicSize * cam.aspect;

            _currentColumn = GetCurrentColumn(cam.transform.position.x);

            _bounadryInitialised = true;
        }

        float vert = Input.GetAxis("Vertical");

        float newYPos = transform.position.y + (vert * Time.deltaTime * _speed);
        Vector3 newPos = new Vector3(transform.position.x, Mathf.Clamp(newYPos, _lowerYBoundary, _upperYBoundary), 0.0f);

        transform.position = newPos;

        if (Input.GetButton("Fire1") && !_isShooting)
        {
            _isShooting = true;
            _spriteAnimator.SetTrigger("Shoot");
        }
        else if (_isShooting)
        {
            // Check whether we almost finished with the shooting animation
            if (_spriteAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == _shootHash &&
                _spriteAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
            {
                GameObject bullet = Instantiate(_bulletPrefab, _bulletSpawn);
                bullet.transform.localPosition = Vector3.zero;

                _isShooting = false;
            }
        }
    }

    private int GetCurrentColumn(float camX)
    {
        float minXPlayerPosition = _leftXBoundary + 0.5f;
        float maxXPlayerPosition = _rightXBoundary - 0.5f;

        // substract 1 because we count the columns from 0
        int numOfCols = (int)(maxXPlayerPosition - minXPlayerPosition);
        
        return (int)(Mathf.InverseLerp(minXPlayerPosition, maxXPlayerPosition, transform.position.x) * numOfCols);
    }

    private IEnumerator MoveToColumn(int column)
    {
        float finalPositionX = _leftXBoundary + 0.5f + column;
        float startingPositionX = transform.position.x;
        Vector3 finalPos = new Vector3(finalPositionX, transform.position.y, 0.0f);
        float tilt = _currentColumn > column ? _sufterTilt : -_sufterTilt;
        Quaternion tiltTo = Quaternion.Euler(0, 0, tilt);
        Quaternion tiltBack = Quaternion.Euler(0, 0, 0);

        while (Mathf.Abs(transform.position.x - finalPositionX) >= float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, _speed * Time.deltaTime);
            float rotationLerp = Mathf.InverseLerp(startingPositionX, finalPositionX, transform.position.x);
            
            transform.rotation = Quaternion.Lerp(transform.rotation, tiltTo, rotationLerp);

            yield return null;
        }

        _currentColumn = column;
        
        while (Mathf.Abs(transform.rotation.z) >= float.Epsilon)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, tiltBack, _tiltSpeed * Time.deltaTime);

            yield return null;
        }
    }

    #endregion

    #region Public Methods

    public void MoveColumns(int value)
    {
        int colToMoveTo = _currentColumn + value;

        if (colToMoveTo >= _boundaryColumnLeft && colToMoveTo <= _boundaryColumnRight)
        {
            StartCoroutine(MoveToColumn(_currentColumn + value));
        }
    }

    #endregion
}
