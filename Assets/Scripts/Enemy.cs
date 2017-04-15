using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region Properties

	public int size { get; private set; }
	public bool isPooled { get; set; }

    #endregion

	#region Private Members

	[SerializeField] private Sprite[] _levelSprites;
	private SpriteRenderer myRenderer;
	private Rigidbody2D myRB;

	#endregion

    #region Private Methods

	private void Awake()
	{
		isPooled = true;
		size = 0;
		myRB = GetComponent<Rigidbody2D> ();
		myRenderer = GetComponent<SpriteRenderer> ();
		myRenderer.sprite = _levelSprites [0];
	}

	private void Update()
	{
		if (!isPooled)
		{
			Move ();
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

	private void Move()
	{
		myRB.MovePosition (transform.position - new Vector3(Time.deltaTime, 0, 0));
	}

    #endregion

	#region Public Methods

	public void Reset(Vector3 position)
	{
		SetPosition (position);
		ModifySize (0);
	}

	public void ModifySize(int value)
	{
		size += value;
		myRenderer.sprite = _levelSprites [size];
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	#endregion

}
