using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region Properties

	public int size { get; private set; }
	public int pickupSize { get; private set; }
	public bool stopped;

    #endregion

	#region Private Members

	private SpriteRenderer _renderer;
	private Rigidbody2D _rigidbody;
	private Collider2D _collider;

	#endregion

    #region Private Methods

	private void Awake()
	{
		size = 0;
		pickupSize = 0;
		_rigidbody = GetComponent<Rigidbody2D> ();
		_renderer = GetComponent<SpriteRenderer> ();
		_collider = GetComponent<Collider2D> ();
		_renderer.sprite = GameManager.enemySprites [0];
	}

	private void Update()
	{
		if (!stopped)
		{
			Move ();
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag ("Bullet"))
		{
			TakeHit ();
			collision.GetComponent<BulletController> ().ReturnBullet ();
		}
    }

	private void Move()
	{
		_rigidbody.MovePosition (transform.position + (Vector3.left * Time.deltaTime * GameManager.speed));
		if (transform.position.x < -GameManager.poolablesXPositionCutoff)
		{
			stopped = true;
		}
	}

	private void TakeHit()
	{
		if (size > 0)
		{
			ModifySize (size - 1, false);
		} 
		else
		{
			Disintegrate ();
		}
	}

	private void Disintegrate()
	{
		//_renderer.enabled = false;
		//_collider.enabled = false;

		Pickup newPickup = Instantiate (GameManager.pickupPrefab, transform.position, Quaternion.identity).GetComponent<Pickup>();
		newPickup.InitialisePickup (pickupSize);

		transform.position = new Vector3(-GameManager.poolablesXPositionCutoff, 0, 0);
	}

    #endregion

	#region Public Methods

	public void Reset(Vector3 position)
	{
		SetPosition (position);
		ModifySize (0, true);
		_renderer.enabled = true;
		_collider.enabled = true;
	}

	public void ModifySize(int value, bool alsoChangePickup)
	{
		size = value;

		if (alsoChangePickup)
		{
			pickupSize = value;
		}

		_renderer.sprite = GameManager.enemySprites [size];
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	#endregion

}
