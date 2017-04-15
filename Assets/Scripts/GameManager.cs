using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Members

    [SerializeField] private EnemyWaveManager _enemyWaveManager;

    [SerializeField] private int _columns;

    [SerializeField] private int _enemyWaveDelay;

    private WaitForSeconds _enemyWaveDelayWFS;

    #endregion

    #region Properties

    public int columns { get { return _columns; } }

    public int currentWave { get; private set; }

    public static GameManager instance { get; private set; }

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
        currentWave = 0;
    }

    private void Update()
    {
        if (_enemyWaveManager != null && _enemyWaveManager.enemiesLeft <= 0)
        {
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
    }

    #endregion
}
