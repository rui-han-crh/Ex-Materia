using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    event Action OnEnded;
    void Interact();

    // Compulsory to flush event handlers at the end of OnEnded(), or they will stay persistent
    void FlushEventHandlers();

}
