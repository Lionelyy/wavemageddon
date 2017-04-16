using UnityEngine;

public class Pickup : MonoBehaviour
{
    #region Properties

    public int value { get; private set; }

    #endregion

    #region Private Methods

    private void Update()
    {
		if (transform.position.x > -GameManager.poolablesXPositionCutoff)
		{
			transform.position += (Vector3.left * Time.deltaTime * GameManager.speed);
		} 
		else
		{
			Destroy (gameObject);
		}
    }

    #endregion

    #region Public Methods

    public void InitialisePickup(int pickupValue)
    {
        value = pickupValue;
		GetComponent<SpriteRenderer> ().sprite = GameManager.pickupSprites[value];
    }

    #endregion
}