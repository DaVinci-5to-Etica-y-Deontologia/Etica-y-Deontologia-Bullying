using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCallsManager : MonoBehaviour
{
    [SerializeField]
    EventCall prefab; 

    HashSet<EventCall> eventscalls = new HashSet<EventCall>();

    [SerializeField]
    bool select;

    [SerializeField]
    Color selected;

    [SerializeField]
    Color original;

    public EventCall Create(string name, UnityEngine.Events.UnityAction unityAction)
    {
        var aux = Instantiate(prefab, transform);

        aux.Set(name, unityAction);

        Select(aux);

        eventscalls.Add(aux);

        return aux;
    }

    public EventCall Create(Sprite image, UnityEngine.Events.UnityAction unityAction)
    {
        var aux = Instantiate(prefab, transform);

        aux.Set(image, unityAction);

        Select(aux);

        eventscalls.Add(aux);

        return aux;
    }

    void Select(EventCall eventCall)
    {
        if (!select)
            return;

        if(eventscalls.Count>0)
            eventCall.backgroundImage.color = original;
        else
            eventCall.backgroundImage.color = selected;

        eventCall.eventToCall.AddListener(
        (button)=> 
        {
            foreach (var item in eventscalls)
            {
                if (item == button)
                    item.backgroundImage.color = selected;
                else
                    item.backgroundImage.color = original;
            }
        });
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
