using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region Public Variables

    public float speed;

    #endregion

    #region Private Methods

    private void Update()
    {
        transform.position += (Vector3.left * Time.deltaTime * speed);
    }

    #endregion
}
