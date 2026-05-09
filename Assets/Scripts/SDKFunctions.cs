
using System;
using System.Collections.Generic;


public class SDKFunctions
{
    private bool Initialized;
    private readonly Queue<Action> pendingEvents = new Queue<Action>();

    public SDKFunctions()
    {
       
    }

    public void GamePlayStartEvent()
    {
        #if UNITY_WEBGL
        //ExecuteOrQueue(() => CrazySDK.Game.GameplayStart());
        #endif
    }

    public void GamePlayStopEvent()
    {
        #if UNITY_WEBGL
        //ExecuteOrQueue(() => CrazySDK.Game.GameplayStop());
        #endif
    }

    private void ExecuteOrQueue(Action sdkCall)
    {
        if (Initialized)
        {
            sdkCall.Invoke();
            return;
        }

        pendingEvents.Enqueue(sdkCall);
    }

    private void FlushPendingEvents()
    {
        while (pendingEvents.Count > 0)
        {
            pendingEvents.Dequeue().Invoke();
        }
    }

}