using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimedActions
{
    public static IEnumerator StartTimedAction(uint seconds, Action action)
    {
        //Wait for the specified amount of seconds
        yield return new WaitForSeconds(seconds);
        //Run action
        action();
    }
}
