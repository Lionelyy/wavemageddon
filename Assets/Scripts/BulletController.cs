using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IPoolableObject
{
    #region Private Variables

    [SerializeField] private float _speed;

    #endregion

    #region Properties

    public bool isPooled { get; set; }
    public bool canReturnToPool { get; private set; }

    #endregion

    #region Private Methods

    private void Update()
    {
        transform.position += (Vector3.right * Time.deltaTime * _speed);

        if (transform.position.x > GameManager.bulletXPositionCutoff)
        {
            canReturnToPool = true;
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    public void DisableObject()
    {
        canReturnToPool = false;
    }

    #endregion
}
