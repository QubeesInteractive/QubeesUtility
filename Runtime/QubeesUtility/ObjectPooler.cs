using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QubeesUtility.Runtime.QubeesUtility
{
    public class ObjectPooler : Singleton<ObjectPooler>
    {
        [SerializeField] private List<Pool> pools;
        private Dictionary<string,Queue<GameObject>> _poolDictionary;
        
        private void Awake()
        {
            Init();
        }
        
        private void Init()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            
            foreach (var pool in pools)
            {
                CreatePoolObject(pool.type, pool.size);
            }
        }
        
        private void CreatePoolObject(string type, int count = 1)
        {
            var pool = pools.First(x => x.type == type);
            for (var i = 0; i < count; i++)
            {
                var go = Instantiate(pool.prefab);
                if (pool.parent)
                    go.transform.SetParent(pool.parent, false);
                go.SetActive(false);
                if (!_poolDictionary.ContainsKey(type))
                {
                    _poolDictionary.Add(type,new Queue<GameObject>());
                }
                _poolDictionary[pool.type] ??= new Queue<GameObject>();
                _poolDictionary[pool.type].Enqueue(go);   
            }
        }
        
        public GameObject GetPoolObject(string type, bool isGetAsActive = true)
        {
            if (!_poolDictionary.ContainsKey(type))
            {
                return null;
            }
            if (_poolDictionary[type].Count == 0)
            {
                CreatePoolObject(type);
            }
            var spawned = _poolDictionary[type].Dequeue();
            spawned.SetActive(isGetAsActive);
            spawned.GetComponent<IPoolable>().OnGet();
            return spawned;
        }

        public void ReleasePoolObject(GameObject go)
        {
            var poolable = go.GetComponent<IPoolable>();
            poolable.OnRelease();
            go.SetActive(false);
            _poolDictionary[poolable.PoolObjectType].Enqueue(go);
        }
    }

    [Serializable]
    public class Pool
    {
        public string name;
        public string type;
        public Transform parent;
        public GameObject prefab;
        public IPoolable Poolable;
        public int size;
    }

    public interface IPoolable
    {
        public string PoolObjectType { get; }
        void OnGet();
        public void OnRelease();
    }
}
