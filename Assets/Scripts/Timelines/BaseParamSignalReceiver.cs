using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BaseParamSignalReceiver<T> : MonoBehaviour, INotificationReceiver
{
    public SignalAssetEventPair[] signalAssetEventPairs;

    [Serializable]
    public class SignalAssetEventPair
    {
        public SignalAsset signalAsset;
        public ParameterizedEvent events;

        [Serializable]
        public class ParameterizedEvent : UnityEvent<PlayableGraph, T> { }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is BaseParamEmitter<T> typedEmitter)
        {
            var matches = signalAssetEventPairs.Where(x => ReferenceEquals(x.signalAsset, typedEmitter.asset));
            foreach (var m in matches)
            {
                m.events.Invoke(origin.GetGraph(), typedEmitter.parameter);
            }
        }
    }
}

