using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    #region Properties

    public int enemiesLeft { get; private set; }

    #endregion

	#region Private Members

	[SerializeField] private GameObject _enemyPrefab;
	[SerializeField] private int _baseEnemyCount = 3;
	[SerializeField] private int _baseUpgradeCount = 0;
	[SerializeField] private int _maxEnemyCount = 80;
	[SerializeField] private int _maxEnemyLevel = 2;
	[SerializeField] private float _enemyCountModifier = 1.4f;
	[SerializeField] private float _enemyUpgradeCountModifier = 0.50f;
	[SerializeField] private Vector3 _enemyRootPosition;

	private List<Enemy> _enemiesInWave; //this is the pool of the waves haha get it
	private int _currentEnemyCount = 0; //this is the number of waves in the pool haha get it
	private int _currentEnemyUpgradeCount = 0;

	#endregion

	#region Private Methods

	private void Awake()
	{
		_enemiesInWave = new List<Enemy> ();
	}

	private void Start()
	{
		
	}

	private void InitializePool()
	{

		//Add as many enemies to the pool as are required by the current wave
		int difference = 0;
		if (_currentEnemyCount <= _maxEnemyCount && (difference = _currentEnemyCount - _enemiesInWave.Count) > 0)
		{
			for (int i = 0; i < difference; i++) 
			{
				_enemiesInWave.Add (Instantiate (_enemyPrefab).GetComponent<Enemy>());
			}
		}

		//reset the enemy positions and levels
		for (int i = 0; i < _enemiesInWave.Count; i++)
		{
			_enemiesInWave [i].Reset(_enemyRootPosition);
		}

		//upgrade the correct amount of enemies
		int lastRandom = 0;
		for (int i = 0; i < _currentEnemyUpgradeCount; i++)
		{
			int random = Random.Range (0, _enemiesInWave.Count);
			while (random == lastRandom || _enemiesInWave[random].size >= _maxEnemyLevel)
			{
				random = Random.Range (0, _enemiesInWave.Count);
			}
			_enemiesInWave [random].ModifySize (1);
		}
	}

	#endregion

    #region Public Methods

    public void StartWave(int waveNumber)
    {
		_currentEnemyCount = _baseEnemyCount + (int)(waveNumber * _enemyCountModifier);
		_currentEnemyUpgradeCount = _baseUpgradeCount + (int)(waveNumber * _enemyUpgradeCountModifier);

		InitializePool ();

		enemiesLeft = _enemiesInWave.Count;
    }

	public void UpdateEnemiesLeft(int value)
	{
		enemiesLeft += value;
	}

    #endregion

}
