using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutDestroyer : MonoBehaviour
{
    public float lifespan;

	private void Start ()
    {
        Destroy(gameObject, lifespan);
	}
}
