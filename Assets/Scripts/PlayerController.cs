using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Private Members

    [SerializeField] private Slider _gasMeter;
    [SerializeField] private int _gasReleaseDelay;
    [SerializeField] private int _gasReleasePower;
    [SerializeField] private int _maxGasAmount;

    [SerializeField] private float _speed;
    [SerializeField] private float _tiltSpeed;

    [SerializeField] private Animator _spriteAnimator;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawn;

    [SerializeField] private float _sufterTilt;

    [SerializeField] private int _boundaryColumnLeft;
    [SerializeField] private int _boundaryColumnRight;

    [SerializeField] private int _invincibilityTime;

    [SerializeField] private int _bulletPoolSize;

    private int _gasAmount = 0;
    private WaitForSeconds _waitForGasRelease;

    private bool _isInvincible;
	private Collider2D _collider;
    private WaitForSeconds _waitForInvincibilityDone;

    private ObjectPool<BulletController> _bulletsPool;

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

	private void Awake()
	{
		_collider = GetComponent<Collider2D> ();
	}

    private void Start()
    {
        _shootHash = Animator.StringToHash("Shooting");
        _waitForGasRelease = new WaitForSeconds(_gasReleaseDelay);
        _waitForInvincibilityDone = new WaitForSeconds(_invincibilityTime);

        _bulletsPool = new ObjectPool<BulletController>(_bulletPoolSize, InstantiateBullet);

        _gasMeter.maxValue = _maxGasAmount;
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

        if (_bulletsPool != null)
        {
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
                    BulletController bulletCtrl = _bulletsPool.GetPooledObject();

                    if (bulletCtrl != null)
                    {
                        bulletCtrl.transform.transform.position = _bulletSpawn.transform.position;
                        bulletCtrl.transform.transform.rotation = _bulletSpawn.transform.rotation;

                        bulletCtrl.gameObject.SetActive(true);
                    }

                    _isShooting = false;
                }
            }
        }
    }

    private BulletController InstantiateBullet()
    {
        GameObject bullet = Instantiate(_bulletPrefab);
        bullet.SetActive(false);

        return bullet.GetComponent<BulletController>();
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
        float tilt = _currentColumn > column ? _sufterTilt : -_sufterTilt;
        Quaternion tiltTo = Quaternion.Euler(0, 0, tilt);
        Quaternion tiltBack = Quaternion.Euler(0, 0, 0);

        while (Mathf.Abs(transform.position.x - finalPositionX) >= float.Epsilon)
        {
            Vector3 finalPos = new Vector3(finalPositionX, transform.position.y, 0.0f);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null && !_isInvincible)
            {
				MoveColumns(-(enemy.size + 1));
                StartCoroutine(HitByEnemy());
            }
        }
        else if (collision.CompareTag("Pickup"))
        {
            Pickup pickup = collision.gameObject.GetComponent<Pickup>();

            if (pickup != null)
            {
                _gasAmount += pickup.value;
                _gasMeter.value = _gasAmount;

                if (_gasAmount >= _maxGasAmount)
                {
                    StartCoroutine(ReleaseFart());
                }
            }
        }
    }

    private IEnumerator HitByEnemy()
    {
        _spriteAnimator.SetBool("IsInvincible", true);
        _isInvincible = true;
		_collider.enabled = false;

        yield return _waitForInvincibilityDone;

		_collider.enabled = true;
        _isInvincible = false;
        _spriteAnimator.SetBool("IsInvincible", false);
    }

    private IEnumerator ReleaseFart()
    {
        _spriteAnimator.SetBool("GasOverload", true);

        yield return _waitForGasRelease;
        
        _spriteAnimator.SetBool("GasOverload", false);

        MoveColumns(_gasReleasePower);

        _gasAmount -= _maxGasAmount;

        if (_gasAmount < 0)
        {
            _gasAmount = 0;
        }

        _gasMeter.value = _gasAmount;
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

    // todo: remove these
    public void TriggerFart()
    {
        StartCoroutine(ReleaseFart());
    }

    public void TriggerInvi()
    {
        StartCoroutine(HitByEnemy());
    }

    public void PickupSomething()
    {
        _gasAmount += 3;
        _gasMeter.value = _gasAmount;

        if (_gasAmount >= _maxGasAmount)
        {
            StartCoroutine(ReleaseFart());
        }
    }

    #endregion
}
