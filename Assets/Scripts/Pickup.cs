using UnityEngine;

public class Pickup : MonoBehaviour
{
    #region Properties

    public int value { get; private set; }

    #endregion

    #region Private Methods

    private void Update()
    {
        transform.position += (Vector3.left * Time.deltaTime * GameManager.speed);
    }

    #endregion

    #region Public Methods

    public void InitialisePickup(int pickupValue)
    {
        value = pickupValue;
    }

    #endregion
}
