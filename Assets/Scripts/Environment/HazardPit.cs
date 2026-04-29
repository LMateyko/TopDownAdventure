using System.Collections.Generic;
using UnityEngine;

public class HazardPit : MonoBehaviour
{
    List<BaseCharacterController> m_trappedCharacters;

    // Update is called once per frame
    protected virtual void Update()
    {
        // Pull Character towards center while in range

        // If close enough to the center, trigger Fall. 
    }

    protected void PullCharacterToCenter(BaseCharacterController character)
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: After entering the area, slowly pull the Character towards the center
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        // TODO: Character leaves pit and returns to normal
    }
}
