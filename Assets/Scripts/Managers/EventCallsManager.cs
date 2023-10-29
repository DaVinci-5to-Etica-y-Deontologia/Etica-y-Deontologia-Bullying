using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCallsManager : MonoBehaviour
{
    [SerializeField]
    EventCall prefab; 

    HashSet<EventCall> eventscalls = new HashSet<EventCall>();

    public void Create(string name, UnityEngine.Events.UnityAction unityAction)
    {
        var aux = Instantiate(prefab, transform);

        aux.Set(name, unityAction);

        eventscalls.Add(aux);
    }

    public void DestroyAll()
    {
        foreach (var item in eventscalls)
        {
            Destroy(item.gameObject);
        }

        eventscalls.Clear();
    }
}
