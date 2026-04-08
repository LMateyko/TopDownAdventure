using UnityEngine;

public interface IRoomObject
{
    /// <summary> Is this object currently enabled or disabled. </summary>
    bool IsEnabled { get; }
    /// <summary> Determine if the object should always spawn when entering the room. </summary>
    bool PersistantRespawn { get; }

    /// <summary> Enable the object when entering the room. </summary>
    void EnableObject();
    /// <summary> Disable the object when exitign the room. </summary>
    void DisableObject();
}
