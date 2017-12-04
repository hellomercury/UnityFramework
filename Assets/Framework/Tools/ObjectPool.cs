using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Tools
{
    public static class PoolManager
    {
        private static Transform PoolMgrTrans;
        private static readonly  Dictionary<string, ObjectPool> poolsDict = new Dictionary<string, ObjectPool>();

        public static ObjectPool Create(string InPoolName)
        {
            if (null == PoolMgrTrans)
            {
                PoolMgrTrans = (new GameObject("PoolManager")).transform;
                //out of camera view, avoid change active.
                PoolMgrTrans.localPosition = new Vector3(-2000, -2000, -2000);
            }
            if (poolsDict.ContainsKey(InPoolName)) Debug.LogError(InPoolName + " is already exist.");
            else
            {
                GameObject go = new GameObject(InPoolName);
                go.transform.SetParent(PoolMgrTrans);
                ObjectPool objectPool = go.AddComponent<ObjectPool>();
                poolsDict.Add(InPoolName, objectPool);
            }
            
            return poolsDict[InPoolName];
        }

        public static ObjectPool GetObjectPools(string InPoolName)
        {
            ObjectPool pool;
            if (poolsDict.TryGetValue(InPoolName, out pool)) return pool;
            else throw new KeyNotFoundException(InPoolName + " is not exist.");
        }

        public static void Destory(string InPoolName)
        {
            ObjectPool pool;
            if (poolsDict.TryGetValue(InPoolName, out pool))
            {
                poolsDict.Remove(InPoolName);
                Object.DestroyImmediate(pool.GameObj);
            }
        }

        public static void DestoryAll()
        {
            foreach (KeyValuePair<string, ObjectPool> itor in poolsDict)
            {
                Object.DestroyImmediate(itor.Value.GameObj);
            }

            poolsDict.Clear();
        }
    }



    public class ObjectPool : MonoBehaviour
    {
        private Transform trans;
        public GameObject GameObj { get; private set; }
        private string prefabPath;
        private object prefabObj;
        private Queue<PoolItem> pools;
        private Action<PoolItem> InitAction, BackAction;

        public void Init(string InPrefabPath,
            Func<object, string> InLoadAction = null,
            Action<PoolItem> InInitAction = null, Action<PoolItem> InBackAction = null,
            int InLimitAmount = 10)
        {
            prefabPath = InPrefabPath;
            if (null == InLoadAction) prefabObj = Resources.Load(InPrefabPath);
            else prefabObj = InLoadAction(InPrefabPath);

            pools = new Queue<PoolItem>(InLimitAmount);

            InitAction = InInitAction;
            BackAction = InBackAction;
        }

        public void Init(object InPrefabObj,
            Action<PoolItem> InInitAction = null, Action<PoolItem> InBackAction = null,
            int InLimitAmount = 10)
        {
            prefabPath = string.Empty;

            prefabObj = InPrefabObj;

            pools = new Queue<PoolItem>(InLimitAmount);

            InitAction = InInitAction;
            BackAction = InBackAction;
        }

        public PoolItem Get()
        {
            PoolItem item = null;
            if (pools.Count > 0) item = pools.Dequeue();
            if (null == item) item = Instantiate(prefabObj as GameObject).AddComponent<PoolItem>();
            if(null != InitAction) InitAction.Invoke(item);

            return item;
        }

        public void Back(PoolItem InPoolItem)
        {
            InPoolItem.BackToPool();
        }
    }

    public class PoolItem : MonoBehaviour
    {
        private static int index = 0;

        public string poolName { get; private set; }

        public int ID { get; private set; }

        public Transform Trans { get; private set; }
        public GameObject GameObj { get; private set; }

        private PoolItem parentPoolItem;

        private List<PoolItem> childPoolItems;

        public void Init(string InPoolName)
        {
            ID = index++;

            Trans = transform;
            GameObj = gameObject;

            poolName = InPoolName;

            parentPoolItem = GetComponentInParent<PoolItem>();
            if (null != parentPoolItem) parentPoolItem.childPoolItems.Add(this);
        }

        public void AddChildPoolItem(PoolItem InPoolItem)
        {
            if (null != InPoolItem)
            {
                if (null == childPoolItems) childPoolItems = new List<PoolItem>();
                childPoolItems.Add(InPoolItem);
            }
        }

        public void RemoveChildPoolItem(PoolItem InPoolItem)
        {
            if (null != InPoolItem)
            {
                if (null != childPoolItems) childPoolItems.Remove(InPoolItem);
            }
        }

        public void BackToPool()
        {
            if (null != parentPoolItem) parentPoolItem.RemoveChildPoolItem(this);
            if (null != childPoolItems)
                foreach (var itor in childPoolItems)
                {
                    PoolManager.GetObjectPools(itor.poolName).Back(itor);
                }
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public override bool Equals(object InOther)
        {
            if (InOther is PoolItem) return (InOther as PoolItem).ID == ID;
            else return false;
        }


    }
}