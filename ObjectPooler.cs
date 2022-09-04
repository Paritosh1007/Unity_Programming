using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool 
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionay;

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
        poolDictionay = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objPool = new Queue<GameObject>();

            for (int i = 1; i <= pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);

                objPool.Enqueue(obj);
            }

            poolDictionay.Add(pool.tag, objPool);
        }
    }
    #endregion


    private void Start()
    {
   
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // if key not present return null
        if (!poolDictionay.ContainsKey(tag))
        {
            Debug.Log("Pool with tag " + tag + " doesn't exist!!!");
            return null;
        }

        // remove the top most element
        GameObject obj = poolDictionay[tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        IPooledObject pooledObject = obj.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }
        // place the element at the back of the queue
        poolDictionay[tag].Enqueue(obj);

        return obj;
    }
}
