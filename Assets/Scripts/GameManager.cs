using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Members

    [SerializeField] private EnemyWaveManager _enemyWaveManager;
    [SerializeField] private int _columns;
    [SerializeField] private int _enemyWaveDelay;

    [SerializeField] private int _bulletXPositionCutoff;

    private WaitForSeconds _enemyWaveDelayWFS;
	private bool _callingWave = false;

    #endregion

    #region Properties

    public int columns { get { return _columns; } }
    public int currentWave { get; private set; }
    public static GameManager instance { get; private set; }

    #region Static Properties

    public static float speed { get; private set; }
    public static float bulletXPositionCutoff { get; private set; }

    #endregion

    #endregion

    #region Private Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("A new GameManager has been created and destroyed");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _enemyWaveDelayWFS = new WaitForSeconds(_enemyWaveDelay);
        speed = 5;
        bulletXPositionCutoff = _bulletXPositionCutoff;

        currentWave = 0;
    }

    private void Update()
    {
		if (_enemyWaveManager != null && _enemyWaveManager.enemiesLeft <= 0 && !_callingWave)
        {
			_callingWave = true;
            currentWave++;
            StartCoroutine(CallStartWave());
        }
    }

    private IEnumerator CallStartWave()
    {
        yield return _enemyWaveDelayWFS;
        if (_enemyWaveManager != null)
        {
            _enemyWaveManager.StartWave(currentWave);
        }
		_callingWave = false;
    }

    #endregion
}
