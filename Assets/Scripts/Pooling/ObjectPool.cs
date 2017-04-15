using System;
using System.Collections.Generic;
using System.Timers;

public class ObjectPool<T> where T:IPoolableObject
{
    #region Private Properties

    private IList<T> _pooledObjects;
    private Func<T> _createNewObjectDelegate;

    private Timer _objectCheckTimer;

    #endregion

    #region Properties

    public int initialPoolSize { get; set; }
    public bool canGrow { get; set; }

    public int poolSize
    {
        get
        {
            if (_pooledObjects != null)
            {
                return _pooledObjects.Count;
            }

            return 0;
        }
    }

    #endregion

    #region Ctor

    public ObjectPool(int initPoolSize, Func<T> createNewObjectDelegate, bool canPoolGrow = false)
    {
        initialPoolSize = initPoolSize;
        canGrow = canPoolGrow;
        _createNewObjectDelegate = createNewObjectDelegate;

        _objectCheckTimer = new Timer(1000);
        _objectCheckTimer.Elapsed += OnObjectCheckTimerTick;

        CreateAndFillPool();
    }

    #endregion

    #region Private Methods
    
    private void OnObjectCheckTimerTick(object sender, ElapsedEventArgs e)
    {
        if (_pooledObjects != null)
        {
            for (int i = 0; i < _pooledObjects.Count; i++)
            {
                if (_pooledObjects[i].canReturnToPool)
                {
                    _pooledObjects[i].isPooled = true;
                    _pooledObjects[i].DisableObject();
                }
            }
        }
    }

    private void CreateAndFillPool()
    {
        _pooledObjects = new List<T>(initialPoolSize);

        if (_createNewObjectDelegate != null)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                AddNewPooledItem();
            }
        }
    }

    private T AddNewPooledItem()
    {
        T newObject = _createNewObjectDelegate();

        if (newObject != null)
        {
            _pooledObjects.Add(newObject);
        }

        return newObject;
    }

    #endregion

    #region Public Methods

    public T GetPooledObject()
    {
        if (_pooledObjects == null) return default(T);

        for (int i = 0; i < _pooledObjects.Count; i++)
        {
            if (_pooledObjects[i].isPooled)
            {
                _pooledObjects[i].isPooled = true;
                return _pooledObjects[i];
            }
        }

        if (canGrow)
        {
            return AddNewPooledItem();
        }

        return default(T);
    }

    #endregion
}
