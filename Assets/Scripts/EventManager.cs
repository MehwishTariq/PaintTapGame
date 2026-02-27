using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventNames
{
    OnComplete,
    OnPlay,
    OnMainMenu,
    OnCameraReset,
    OnPauseLevel,
    OnResumeLevel,
    OnRestartLevel,
    OnVolumeTrigger,
    OnOpenLevel,
    OnCompleteUI,
    OnColorFill,
    OnNextLevel,
    OnCheckLevel,
    OnSaveLevel,
    OnColorSelect,
    OnColored,
    OnChangeParticlePos,
    RotateStateChange,
    OnObjectSet,
    OnChangeParticleSize,
    OnResetGame,
    OnGameComplete,
    OnTimeUpdate,
}

public class EventManager : MonoBehaviour
{
    // Store any kind of delegate (Action, Action<T>, etc.)
    private static Dictionary<EventNames, Delegate> events = new();

    // Subscribe method
    public static void SubscribeToEvent(EventNames eventName, Action method)
    {
        if (!events.ContainsKey(eventName))
            events[eventName] = null;

        events[eventName] = (Action)events[eventName] + method;
    }

    // Subscribe with parameter
    public static void SubscribeToEvent<T>(EventNames eventName, Action<T> method)
    {
        if (!events.ContainsKey(eventName))
            events[eventName] = null;

        events[eventName] = (Action<T>)events[eventName] + method;
    }

    // Unsubscribe
    public static void UnsubscribeFromEvent(EventNames eventName, Action method)
    {
        if (events.ContainsKey(eventName))
            events[eventName] = (Action)events[eventName] - method;
    }

    public static void UnsubscribeFromEvent<T>(EventNames eventName, Action<T> method)
    {
        if (events.ContainsKey(eventName))
            events[eventName] = (Action<T>)events[eventName] - method;
    }

    // Trigger (no parameter)
    public static void TriggerEvent(EventNames eventName)
    {
        if (events.ContainsKey(eventName))
            (events[eventName] as Action)?.Invoke();
    }

    // Trigger (with parameter)
    public static void TriggerEvent<T>(EventNames eventName, T param)
    {
        if (events.ContainsKey(eventName))
            (events[eventName] as Action<T>)?.Invoke(param);
    }
}
