using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.AllData
{
    /// <summary>
    /// 풀링 클래스
    /// </summary>
    public class PoolManager
    {        
        /// <summary>
        /// 타입별 풀링
        /// </summary>
        private Dictionary<Type, Queue<GameObject>> typePool = new();
        /// <summary>
        /// 각 Type 개수
        /// </summary>
        private Dictionary<Type, int> typeCount = new();
        /// <summary>
        /// Type 오브젝트 Container
        /// </summary>
        public Dictionary<Type, GameObject> typeContainer = new();

        /// <summary>
        /// 오브젝트 풀링 데이터 획득
        /// </summary>
        public T Get<T>(Transform parent)
        {
            var type = typeof(T);
            GameObject obj;
            if (!typePool.ContainsKey(type)) typePool[type] = new();
            if (!typeCount.ContainsKey(type)) typeCount[type] = new();

            if (typePool.ContainsKey(type) && typePool[type].Count > 0)
            {
                obj = typePool[type].Dequeue();
            }
            else
            {
                obj = GetTypeObject(type);
                obj.name = $"{GetTypeName(type)}_{typeCount[type]++}";
            }
            
            obj.transform.SetParent(parent, false);
            obj.SetActive(true);

            return obj.GetComponent<T>();
        }

        public void Return<T>(GameObject obj)
        {
            var type = typeof(T);

            if (!typePool.ContainsKey(type)) typePool[type] = new();
            if (!typeCount.ContainsKey(type)) typeCount[type] = new();

            obj.SetActive(false);
            obj.transform.SetParent(null);

            typePool[type].Enqueue(obj);
        }

        public int Count<T>()
        {
            return typeCount[typeof(T)];
        }

        /// <summary>
        /// 같은 타입 오브젝트 반환
        /// </summary>
        private GameObject GetTypeObject(Type type)
        {
            var game = typeContainer[type];
            return UnityEngine.Object.Instantiate(game);
        }

        private string GetTypeName(Type type)
        {
            string[] names = type.ToString().Split('.');
            string last = string.Empty;
            foreach (var name in names)
            {
                last = name;
            }
            return last;
        }

        // Scripts.Calendar.Date.Interaction.Day
        // Scripts.Calendar.Todo.Interaction.Todo
        // Scripts.Calendar.Todo.TodoItem
        // Scripts.Tasks.TaskItem
    }
}