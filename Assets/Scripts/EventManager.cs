using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EventNames
{
    OnComplete,
    OnStart
}

public class EventManager : MonoBehaviour
{
    // Dictionary to store events by name
    private static Dictionary<EventNames, Action> events = new();

    // Subscribe method
    public static void SubscribeToEvent(EventNames eventName, Action method)
    {
        if (!events.ContainsKey(eventName))
        {
            events.Add(eventName, null);
        }

        // Avoid adding duplicate methods
        if (events[eventName] == null)
        {
            events[eventName] += method;
        }
    }

    // Unsubscribe method
    public static void UnsubscribeFromEvent(EventNames eventName, Action method)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] -= method;
        }
    }

    // Invoke method
    public static void TriggerEvent(EventNames eventName)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName]?.Invoke();
        }
    }
}
