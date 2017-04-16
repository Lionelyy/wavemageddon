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
	[SerializeField] private float _maxY = 2;
	[SerializeField] private float _minY = -2;
	[SerializeField] private float _minVertGap = 1;
	[SerializeField] private WaitForSeconds _groupDelay;

	private List<Enemy> _enemiesInWave; //this is the pool of the waves haha get it
	private int _currentEnemyCount = 0; //this is the number of waves in the pool haha get it
	private int _currentEnemyUpgradeCount = 0;

	#endregion

	#region Private Methods

	private void Awake()
	{
		_groupDelay = new WaitForSeconds (1.7f);
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

		//reset enemy positions and levels
		for (int i = 0; i < _enemiesInWave.Count; i++)
		{
			_enemiesInWave [i].isPooled = true;
			_enemiesInWave [i].Reset(_enemyRootPosition);
		}
	}

	private IEnumerator InitializeWave(int waveNumber)
	{
		//Initialize the new enemy count and enemy upgrade count
		_currentEnemyCount = _baseEnemyCount + (int)(waveNumber * _enemyCountModifier);
		_currentEnemyUpgradeCount = _baseUpgradeCount + (int)(waveNumber * _enemyUpgradeCountModifier);

		enemiesLeft = _currentEnemyCount;

		InitializePool ();

		//upgrade the correct amount of enemies
		int lastRandom = 0;
		for (int i = 0; i < _currentEnemyUpgradeCount; i++)
		{
			int random = Random.Range (0, _enemiesInWave.Count);
			while (random == lastRandom || _enemiesInWave[random].size >= _maxEnemyLevel)
			{
				random = Random.Range (0, _enemiesInWave.Count);
				yield return null;
			}
			_enemiesInWave [random].ModifySize (1);
		}

		//determine number of groups (group == up to 3 enemies sharing one column)
		int minNumOfGroups = _enemiesInWave.Count / 3;
		if (minNumOfGroups <= 0)
		{
			minNumOfGroups = 1;
		}
		int numOfGroups = Random.Range (minNumOfGroups, _enemiesInWave.Count);

		//determine number of enemies per group
		int[] groupSizes = new int[numOfGroups];
		int enemiesToGroup = _enemiesInWave.Count;

		while (enemiesToGroup > 0)
		{
			int random = Random.Range (0, groupSizes.Length);

			if (groupSizes [random] < 3)
			{
				groupSizes [random]++;
				enemiesToGroup--;
			}

			yield return null;
		}

		//iterate through groups to set enemy positions
		int currentIndex = 0;
		int targetIndex = 0;

		for (int i = 0; i < numOfGroups; i++)
		{
			Vector3 newPos = Vector3.zero;
			List<Vector3> currentGroup = new List<Vector3> ();

			targetIndex = currentIndex + groupSizes[i];
			for (int j = currentIndex; j < targetIndex; j++)
			{
				newPos = new Vector3 (0, Random.Range (_minY, _maxY), 0);

				while (!IsValidGap(newPos, currentGroup))
				{
					newPos = new Vector3 (0, Random.Range (_minY, _maxY), 0);
					yield return null;
				}
				_enemiesInWave [j].SetPosition(_enemyRootPosition + newPos);
				currentGroup.Add (newPos);
			}

			for (int j = currentIndex; j < targetIndex; j++) 
			{
				_enemiesInWave [j].isPooled = false;

				currentIndex = j+1;
			}

			yield return _groupDelay;
		}
	}

	private bool IsValidGap(Vector3 myPosition, List<Vector3> groupPositions)
	{
		if (groupPositions.Count == 0)
		{
			return true;
		}

		bool value = true;
		for (int i = 0; i < groupPositions.Count; i++)
		{
			if (Vector3.Distance (myPosition, groupPositions [i]) < _minVertGap)
			{
				value = false;
			}
		}
		return value;
	}

	#endregion

    #region Public Methods

    public void StartWave(int waveNumber)
    {
		StartCoroutine (InitializeWave (waveNumber));
    }

	public void UpdateEnemiesLeft(int value)
	{
		enemiesLeft += value;
	}

    #endregion

}
