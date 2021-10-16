using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IodoShibaUtil{

    public abstract class ObjectPool<Type> : MonoBehaviour where Type:class
    {
        [SerializeField] int capacity;

        Type[] pool;
        bool[] isInUse;
        int next = 0;

        public int Capacity => capacity;

        void Awake()
        {
            pool = new Type[capacity];
            isInUse = new bool[capacity];
            for(int i=0; i<isInUse.Length;++i)
            {
                isInUse[i] = false;
            }
        }

        public Container<Type> Rent()
        {
            bool found = false;

            int i=0;
            for(; i<isInUse.Length;++i)
            {
                if(!isInUse[next])
                {
                    found = true;
                    break;
                }

                ++next;
            }

            if(!found)
            {
                throw new System.InvalidOperationException("Pooled object has runned out.");
            }

            return new Container<Type>(this, next);
        }

        void Return(Type item, int index)
        {
            if(index < 0 || index >= pool.Length){throw new System.IndexOutOfRangeException($"index is invalid. given:{index}, Capcacity:{capacity}");}
            if(!isInUse[index]){throw new System.ArgumentException($"Item not in use. given:{index}");}
            if(!Object.ReferenceEquals(item, pool[index])){throw new System.ArgumentException($"given object's Reference is not same with pooled one. given:{index}");}
            isInUse[index] = false;
        }


        public struct Container<Type> where Type : class
        {
            Type content;
            ObjectPool<Type> ownerPool;
            int index;
            bool valid;

            public Type Content 
            {
                get
                {
                    if(!valid){throw new System.InvalidOperationException($"PooledObjectContainer is invalid.");}
                    return content;
                }
            }

            public Container(ObjectPool<Type> ownerPool, int index)
            {
                this.ownerPool = ownerPool;
                this.content = ownerPool.pool[index];
                this.index = index;
                this.valid = true;
            }

            public void Return()
            {
                ownerPool.Return(content, index);
                valid = false;
            }
            
        }

    }

    public class ComponentPool<Type> : ObjectPool<Type> where Type : Component
    {
    }
}