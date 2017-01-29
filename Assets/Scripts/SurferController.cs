using System;
using UnityEngine;

public class SurferController : MonoBehaviour
{
    #region Public Variables

    public Boundaries boundaries;
    public float speed;

    public Animator spriteAnimator;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    #endregion

    #region Private Variables

    private bool _isShooting = false;
    private int _shootHash;

    #endregion

    #region Private Methods

    private void Start()
    {
        _shootHash = Animator.StringToHash("Shooting");
    }

    private void Update()
    {
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 newPos = new Vector3(
            Mathf.Clamp(transform.position.x + horz * Time.deltaTime * speed, boundaries.minX, boundaries.maxX),
            Mathf.Clamp(transform.position.y + vert * Time.deltaTime * speed, boundaries.minY, boundaries.maxY), 0.0f);

        transform.position = newPos;

        if (Input.GetButton("Fire1") && !_isShooting)
        {
            _isShooting = true;
            spriteAnimator.SetTrigger("Shoot");
        }
        else if (_isShooting)
        {
            // Check whether we almost finished with the shooting animation
            if (spriteAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == _shootHash &&
                spriteAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn);
                bullet.transform.localPosition = Vector3.zero;

                _isShooting = false;
            }
        }
    }

    #endregion
}

[Serializable]
public class Boundaries
{
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
}
