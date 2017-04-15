using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region Properties

	public int size = 0;

    #endregion

	#region Private Members

	[SerializeField] private Sprite[] _levelSprites;
	private SpriteRenderer myRenderer;

	#endregion

    #region Private Methods

	private void Awake()
	{
		myRenderer = GetComponent<SpriteRenderer> ();
		myRenderer.sprite = _levelSprites [0];
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    #endregion

	#region Public Methods

	public void Reset(Vector3 position)
	{
		transform.position = position;
		ModifySize (0);
	}

	public void ModifySize(int value)
	{
		size += value;
		myRenderer.sprite = _levelSprites [size];
	}

	#endregion

}
