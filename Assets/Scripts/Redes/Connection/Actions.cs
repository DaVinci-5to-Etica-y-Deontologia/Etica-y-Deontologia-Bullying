using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Actions
{
    public static Actions Ban { get; private set; } = new ActBan(UserData.Ban);

    public static Actions Admonition { get; private set; } = new ActAdmonition(UserData.Admonition);

    public static Actions Picantear { get; private set; } = new ActPicantear(UserData.Picantear);

    public static Actions Corromper { get; private set; } = new ActCorromper(UserData.ChangeMoral);

    public static Actions Suspect { get; private set; } = new ActSus(UserData.SuspectChange);

    public static Actions AddComment { get; private set; } = new ActAddComment(UserData.AddComment);

    public static Actions RemoveComment { get; private set; } = new ActRemoveComment(UserData.RemoveComment);


    public static Actions AddUser { get; private set; } = new ActAddUser(StreamerData.AddUser);

    public static Actions RemoveUser { get; private set; } = new ActRemoveUser(StreamerData.RemoveUser);



    public static Actions CreateStream { get; private set; } = new ActCreateStream(StreamerManager.CreateStream);

    public static Actions AddStream { get; private set; } = new ActAddStream(StreamerManager.AddNewStream);

    public static Actions StartUpdateStreamers { get; private set; } = new ActStartUpdateStreamers(StreamerManager.StartUpdateStreamers);

    public static Actions EndUpdateStreamers { get; private set; } = new ActEndUpdateStreamers(StreamerManager.EndUpdateStreamers);

    public static Actions EnableStream { get; private set; } = new ActEnableStream(StreamerData.EnableStream);

    public static Actions UpdateLifeStream { get; private set; } = new ActUpdateLifeStream(StreamerData.LifeUpdate);

    public static Actions FinishStream { get; private set; } = new ActFinishStream(StreamerData.FinishStream);


    public string className;

    public System.Action<string, StreamerManager.SearchResult> action;


    public override bool Equals(object obj)
    {
        return this.GetType() == obj.GetType();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public Actions(System.Action<string, StreamerManager.SearchResult> action)
    {
        className = this.GetType().Name;
        this.action = action;

        StreamerManager.actionsMap.Add(className, action);
    }
}


[System.Serializable]
public class ActionStream : Actions
{
    public ActionStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActionUser : Actions
{
    public ActionUser(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActionComment : Actions
{
    public ActionComment(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}


[System.Serializable]
public class ActCreateStream : ActionStream
{
    public ActCreateStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActAddStream : ActionStream
{
    public ActAddStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActRemoveStream : ActionStream
{
    public ActRemoveStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActStartUpdateStreamers : ActionStream
{
    public ActStartUpdateStreamers(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActEndUpdateStreamers : ActionStream
{
    public ActEndUpdateStreamers(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActUpdateLifeStream : ActionStream
{
    public ActUpdateLifeStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActFinishStream : ActionComment
{
    public ActFinishStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActEnableStream : ActionComment
{
    public ActEnableStream(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}


[System.Serializable]
public class ActAddUser : ActionUser
{
    public ActAddUser(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActRemoveUser : ActionUser
{
    public ActRemoveUser(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActBan : ActionUser
{
    public ActBan(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActAdmonition : ActionUser
{
    public ActAdmonition(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActPicantear : ActionUser
{
    public ActPicantear(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActCorromper : ActionUser
{
    public ActCorromper(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActSus : ActionUser
{
    public ActSus(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}



[System.Serializable]
public class ActAddComment : ActionComment
{
    public ActAddComment(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
[System.Serializable]
public class ActRemoveComment : ActionComment
{
    public ActRemoveComment(System.Action<string, StreamerManager.SearchResult> action) : base(action)
    {
    }
}
