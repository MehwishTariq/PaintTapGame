using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

namespace MLabsSdk
{

    public class AnalyticsImp : MonoBehaviour
    {
        private Firebase.FirebaseApp app;
        bool isInitialized;

        public static AnalyticsImp instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(this);
        }

        private void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    app = Firebase.FirebaseApp.DefaultInstance;
                    isInitialized = true;
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
        public void LogEvent(string eventName)
        {
            if (isInitialized)
            {
                MLogs(eventName);
                FirebaseAnalytics.LogEvent(eventName);
            }else
            {
                MLogs("Firebase not initialized");
            }
        }

        public void LogEvent(string eventName, string paramName, int paramValue)
        {
            if (isInitialized)
            {
                MLogs(eventName + " : " + paramName + " : " + paramValue);
                Parameter myparams = new Parameter(paramName, paramValue);
                FirebaseAnalytics.LogEvent(eventName, myparams);
            }
            else
            {
                MLogs("Firebase not initialized");
            }
            
        }

        public void LogLevelStartedEvent(int level)
        {
            if (isInitialized)
            {
                MLogs("EventLevelStart" + " : " + "ParameterLevel" + " : " + level);
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart,
                        new Parameter(FirebaseAnalytics.ParameterLevel, level));
            }else
            {
                MLogs("Firebase not initialized");
            }
        }


        public void LogLevelCompleteEvent(int level)
        {
            if (isInitialized)
            {
                MLogs("EventLevelUp" + " : " + "ParameterLevel" + " : " + level);
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelUp,
                    new Parameter("success", 1),
                    new Parameter(FirebaseAnalytics.ParameterLevel, level));
            }else
            {
                MLogs("Firebase not initialized");
            }
        }

        public void LogLevelFailedEvent(int level)
        {
            if (isInitialized)
            {
                MLogs("EventLevelEnd" + " : " + "ParameterLevel" + " : " + level);
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd,
                        new Parameter("success", 0),
                        new Parameter(FirebaseAnalytics.ParameterLevel, level));
            }else
            {
                MLogs("Firebase not initialized");
            }
        }

        void MLogs(string log)
        {
            Debug.Log("## " + log);
        }
    }
}
