using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Internal;

[System.Serializable]
public class DataPic<T> : Pictionarys<int, T>
{
    public int lastID { get; private set; }

    public (T value, int ID, int index ) GetTByIndex(int index)
    {
        var data = GetPicByIndex(index);

        return (data.Value, data.Key, index);
    }

    public (T value, int ID, int index) GetTByID(int ID)
    {
        if(!ContainsKey(ID, out var index))
        {
            Debug.LogError("no se encontro el ID: " + ID);
            return default;
        }

        return GetTByIndex(index);
    }

    public Pictionary<int, T> Add(T value)
    {
        lastID++;

        return Add(lastID, value);
    }

    public int Prepare()
    {
        return ++lastID;
    }

    new public Pictionary<int, T> Add(Internal.Pictionary<int, T> pictionary)
    {
        if (pictionary.Key > lastID)
            lastID = pictionary.Key;
        return base.Add(pictionary);
    }
}
