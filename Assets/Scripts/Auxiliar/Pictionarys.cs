using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Internal;


/*
 Implementar sort
 */
[Serializable]
public class Pictionarys<K, V> : IEnumerable<Pictionary<K, V>>
{
    [SerializeField]
    protected List<Pictionary<K, V>> pictionaries;

    Stack<Pictionary<K, V>> auxiliarObjects;

    public int Count
    {
        get => pictionaries.Count;
    }

    public K[] keys
    {
        get
        {
            K[] keys = new K[Count];
            for (int i = 0; i < Count; i++)
            {
                keys[i] = pictionaries[i].Key;
            }

            return keys;
        }
    }

    public V[] values
    {
        get
        {
            V[] values = new V[Count];

            for (int i = 0; i < Count; i++)
            {
                values[i] = pictionaries[i].Value;
            }

            return values;
        }
    }

    /// <summary>
    /// busca por el orden de la lista
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public V this[int k]
    {
        get
        {
            return pictionaries[k].Value;
        }

        set
        {
            pictionaries[k].Value = value;
        }
    }

    /// <summary>
    /// busca por el nombre del enum, si coincide con el nombre del key
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public V this[Enum e]
    {
        get
        {
            return pictionaries[StringIndex(e)].Value;
        }
        set
        {
            pictionaries[StringIndex(e)].Value = value;
        }
    }

    /// <summary>
    /// Busca por el key ingresado
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public V this[K k]
    {
        get
        {
            var index = SearchIndex(k);
            if (index < 0)
                Debug.Log("se busco " + k.ToString() + " y no se encontro");
            return pictionaries[index].Value;
        }
        set
        {
            pictionaries[SearchIndex(k)].Value = value;
        }
    }

    public static Pictionarys<K, V> operator +(Pictionarys<K, V> original, Pictionarys<K, V> sumado)
    {
        original.AddRange(sumado);

        return original;
    }

    public static Pictionarys<K, V> operator +(Pictionarys<K, V> original, Dictionary<K, V> sumado)
    {
        foreach (var item in sumado)
        {
            original.Add(item.Key, item.Value);
        }
        return original;
    }

    public static Dictionary<K, V> operator +(Dictionary<K, V> original, Pictionarys<K, V> sumado)
    {
        foreach (var item in sumado)
        {
            original.Add(item.Key, item.Value);
        }
        return original;
    }

    public override string ToString()
    {
        return ToString("=");
    }

    public string ToString(string glue, string entreKeys = "\n\n")
    {
        string salida = "";

        foreach (var item in pictionaries)
        {
            if (!item.Value.Equals(default(V)) && ((item.Value as string) != ""))
                salida += item.Key.ToString().RichText("u") + glue + item.Value + entreKeys;
        }

        /*
        foreach (var item in pictionaries)
        {
            salida += item.key + glue + item.value + entreKeys;
        }
        */

        return salida;
    }


    /// <summary>
    /// devuelve el index en caso de encontrar similitud con el nombre del key
    /// </summary>
    /// <param name="s"></param>
    /// <returns>devuelve la poscion en un entero</returns>
    public int StringIndex(Enum s)
    {
        return StringIndex(s.ToString());
    }

    /// <summary>
    /// devuelve el index en caso de encontrar similitud con el nombre del key
    /// </summary>
    /// <param name="s"></param>
    /// <returns>devuelve la poscion en un entero</returns>
    public int StringIndex(string s)
    {
        for (int i = 0; i < pictionaries.Count; i++)
        {
            if (pictionaries[i].Key.ToString() == s)
            {
                return i;
            }
        }
        return -1;
    }

    public void Sort(IComparer<Pictionary<K, V>> comparer)
    {
        pictionaries.Sort(comparer);
    }

    public Pictionary<K, V> GetPic(K key)
    {
        if (ContainsKey(key, out int index))
            return pictionaries[index];

        else
            return default;
    }

    public Pictionary<K, V> GetPicByIndex(int index)
    {
        return pictionaries[index];
    }

    public bool TryGetPic(K key, out Pictionary<K,V> pic)
    {
        bool ret = ContainsKey(key, out int index);

        if (ret)
            pic = pictionaries[index];
        else
            pic = default;

        return ret;
    }

    public bool TryGetValue(K key, out V value)
    {
        bool ret = ContainsKey(key, out int index);

        if (ret)
            value = pictionaries[index].Value;
        else
            value = default;

        return ret;
    }

    public bool ContainsKey(K key, out int index)
    {
        if ((index = SearchIndex(key)) > -1)
        {
            return true;
        }
        return false;
    }

    public bool ContainsKey(K key)
    {
        if (SearchIndex(key) > -1)
            return true;
        return false;
    }

    public void AddRange(IEnumerable<Pictionary<K, V>> pic)
    {
        pictionaries.AddRange(pic);
    }

    public Pictionary<K, V> Add(Pictionary<K, V> pictionary)
    {
        if (ContainsKey(pictionary.Key))
        {
            return default;
        }
            

        pictionaries.Add(pictionary);

        return pictionary;
    }

    public Pictionary<K, V> Add(K key, V value)
    {
        if (ContainsKey(key))
            return default;

        Pictionary<K, V> aux;

        if (auxiliarObjects.Count>0)
        {
            aux = auxiliarObjects.Pop();

            aux.Key = key;
            aux.Value = value;
        }
        else
        {
            aux = new Pictionary<K, V>(key, value);
        }        

        pictionaries.Add(aux);

        return aux;
    }

    public void Remove(K key)
    {
        for (int i = 0; i < pictionaries.Count; i++)
        {
            if (pictionaries[i].Key.Equals(key))
            {
                RemoveAt(i);

                return;
            }
        }
    }

    public void RemoveAt(int i)
    {
        var aux = pictionaries[i];

        pictionaries.RemoveAt(i);

        auxiliarObjects.Push(aux);
    }

    public void Clear()
    {
        pictionaries.Clear();
    }

    public List<Pictionary<K, V>> GetList()
    {
        return pictionaries;
    }

    public void SetList(List<Pictionary<K, V>> pictionaries)
    {
        this.pictionaries = pictionaries;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Pictionary<K, V>> GetEnumerator()
    {
        return pictionaries.GetEnumerator();
    }


    public T SearchOrCreate<T>(K key) where T : V, new()
    {
        if (!ContainsKey(key, out int index) || pictionaries[index].Value == null)
        {
            var newAux = new T();

            if (index < 0)
                Add(key, newAux);
            else
                pictionaries[index].Value = newAux;

            return newAux;
        }

        return (T)pictionaries[index].Value;
    }

    public void CreateOrSave(K key, V value)
    {
        if (ContainsKey(key, out int index))
        {
            pictionaries[index].Value = value;
            return;
        }

        Add(key, value);
    }

    public V SearchOrDefault(K key, V defoult)
    {
        if (ContainsKey(key, out int index))
        {
            return pictionaries[index].Value;
        }

        return defoult;
    }
    
    int SearchIndex(K key)
    {
        //string pantalla = "busqueda de: " + key;
        for (int i = 0; i < pictionaries.Count; i++)
        {
            //pantalla += $"key {pictionaries[i].Key} value {pictionaries[i].Value} indice {i}";
            if (pictionaries[i].Key.Equals(key))
            {
                //Debug.Log(pantalla);
                return i;
            }
        }
        //Debug.Log(pantalla);
        return -1;
    }

    public Pictionarys()
    {
        pictionaries = new List<Pictionary<K, V>>();

        auxiliarObjects = new Stack<Pictionary<K, V>>();
    }
}

namespace Internal
{

    [System.Serializable]
    public class Pictionary<K, V> : IComparable<Pictionary<K, V>>
    {
        public K Key;

        public V Value;

        public int CompareTo(Pictionary<K, V> other)
        {
            return string.Compare(this.Key.ToString(), other.Key.ToString());
        }

        public Pictionary(K k, V v)
        {
            Key = k;
            Value = v;
        }
    }
}

/*
namespace Internal
{

    [System.Serializable]
    public class Pictionary<K, V> : IComparable<Pictionary<K, V>>, ISerializationCallbackReceiver
    {
        public K Key
        {
            get => getKey();
            set
            {
                setKey(value);
            }
        }

        public V Value
        {
            get => getValue();
            set
            {
                setValue(value);
            }
        }

        [SerializeField]
        bool isKeyReference;

        [SerializeReference]
        K _keyReference;

        [SerializeField]
        K _keyField;

        [SerializeField]
        bool isValueReference;

        [SerializeReference]
        V _valueReference;

        [SerializeField]
        V _valueField;

        System.Action<V> setValue;

        System.Func<V> getValue;

        System.Action<K> setKey;

        System.Func<K> getKey;

        public void OnBeforeSerialize()
        {
            Init();
        }

        public void OnAfterDeserialize()
        {
            Init();
        }

        void Init()
        {
            isKeyReference = !typeof(K).IsValueType;
            isValueReference = !typeof(V).IsValueType;
            if (isKeyReference)
            {
                setKey = (_key) => _keyReference = _key;
                getKey = () => _keyReference;
            }
            else
            {
                setKey = (_key) => _keyField = _key;
                getKey = () => _keyField;
            }


            if (isValueReference)
            {
                setValue = (_value) => _valueReference = _value;
                getValue = () => _valueReference;
            }
            else
            {
                setValue = (_value) => _valueField = _value;
                getValue = () => _valueField;
            }
        }


        public int CompareTo(Pictionary<K, V> other)
        {
            return string.Compare(this.Key.ToString(), other.Key.ToString());
        }

        public Pictionary(K k, V v)
        {
            Init();
            Key = k;
            Value = v;
        }
    }
}
*/


/*
namespace Internal
{

    [System.Serializable]
    public class Pictionary<K, V> : IComparable<Pictionary<K, V>>
    {
        public K Key
        {
            get => isKeyReference ?  _keyReference : _keyField;
            set
            {
                if (isKeyReference)
                    _keyReference = value;
                else
                    _keyField = value;
            }
        }

        public V Value
        {
            get => isValueReference ? _valueReference : _valueField;
            set
            {
                if (isValueReference)
                    _valueReference = value;
                else
                    _valueField = value;
            }
        }

        [SerializeField]
        bool isKeyReference;

        [SerializeReference]
        K _keyReference;

        [SerializeField]
        K _keyField;


        [SerializeField]
        bool isValueReference;

        [SerializeReference]
        V _valueReference;

        [SerializeField]
        V _valueField;

        public int CompareTo(Pictionary<K, V> other)
        {
            return string.Compare(this.Key.ToString(), other.Key.ToString());
        }

        public Pictionary(K k, V v)
        {
            isKeyReference = !typeof(K).IsValueType;

            isValueReference = !typeof(V).IsValueType;

            Key = k;
            Value = v;
        }
    }
}
*/

