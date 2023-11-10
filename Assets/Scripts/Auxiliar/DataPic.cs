using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Internal;

[System.Serializable]
public class DataPic<T> : IEnumerable<Pictionary<int, T>>
{
    [SerializeField]
    Pictionarys<int, T> _data = new();

    public int lastID { get; private set; }

    public int Count => _data.Count;

    public List<Pictionary<int, T>> GetList() => _data.GetList();

    public (T value, int ID, int index ) GetTByIndex(int index)
    {
        var data = _data.GetPicByIndex(index);

        return (data.Value, data.Key, index);
    }

    public (T value, int ID, int index) GetTByID(int ID)
    {
        if(!_data.ContainsKey(ID, out var index))
        {
            Debug.LogError("no se encontro el ID: " + ID);
            return default;
        }

        return GetTByIndex(index);
    }

    public Pictionary<int, T> Add(T value)
    {
        lastID++;

        return _data.Add(lastID, value);
    }

    public Pictionary<int, T> Add(Internal.Pictionary<int, T> pictionary)
    {
        lastID++;
        return _data.Add(pictionary);
    }

    public void Remove(int ID)
    {
        _data.Remove(ID);
    }

    public void RemoveAt(int index)
    {
        _data.RemoveAt(index);
    }

    public IEnumerator<Pictionary<int, T>> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }
}
