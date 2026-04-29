#if DEVELOPMENT_ESSENTIALS_RUNTIME_UNI_TASK
using System;
using Cysharp.Threading.Tasks;
using DevelopmentEssentials.Extensions.CS;
using DevelopmentEssentials.Extensions.Unity.ExtendedLogger;
using Sirenix.OdinInspector;
using UnityEngine;
#if DEVELOPMENT_ESSENTIALS_RUNTIME_ODIN_INSPECTOR
using static Sirenix.OdinInspector.Units;
#endif

namespace DevelopmentEssentials {

    /// For when we need an event that only fires once and can be subscribed to and awaited if necessary.
    [Serializable]
    public class SingleEvent<T> {

        [NonSerialized]
        /// !!! USE <see cref="Lock"/> !!!
        private object _lock; // Protection for multi-threading
        private object Lock => _lock ??= new(); // Because domain reloads wipe non-serialized fields

        [NonSerialized]
        private Action<T> onCompleted; // Storage for pending handlers

        [NonSerialized]
        private UniTaskCompletionSource<T> _utcs;
        private UniTaskCompletionSource<T> utcs => _utcs ??= new();

        [NonSerialized]
        private bool started;

#if DEVELOPMENT_ESSENTIALS_RUNTIME_ODIN_INSPECTOR
        [Unit(Second)]
#endif
        [SerializeField] private int timeoutSeconds;
        [SerializeField] private T timedOutTriggerValue;

        public SingleEvent(int timeoutSeconds = int.MaxValue) => this.timeoutSeconds = timeoutSeconds;

        public async void StartTimeout() {
            try {
                if (started)
                    return;

                started                   = true;
                (bool timedOut, T result) = await utcs.Task.TimeoutWithoutException(TimeSpan.FromSeconds(timeoutSeconds), DelayType.Realtime);
                Trigger(timedOut ? timedOutTriggerValue : result);
            }
            catch (Exception e) {
                e.LogEx();
            }
        }

        public void Trigger(T t) {
            Action<T> subscribers; // Temp holder for thread safety

            lock (Lock) { // Atomic completion and capture
                if (!utcs.TrySetResult(t)) // Set result if not already set (prevents multiple triggers)
                    return; // Exit if already triggered

                subscribers = onCompleted; // Grab current queue
                onCompleted = null; // Wipe queue to prevent re-fire
            }

            subscribers.SafeInvoke(t); // Execute all subscribers. ?. to avoid the exception if none
        }

        // Allows: await instance;
        public UniTask<T>.Awaiter GetAwaiter() {
            StartTimeout();
            return utcs.Task.GetAwaiter();
        }

        // Allows: instance += Action;
        public static SingleEvent<T> operator +(SingleEvent<T> e, Action<T> callback) {
            if (e == null) return null;

            lock (e.Lock) {
                if (e.utcs.Task.Status == UniTaskStatus.Succeeded) {
                    callback?.SafeInvoke(e.utcs.Task.GetAwaiter().GetResult());
                }
                else {
                    e.onCompleted -= callback; // Prevent duplicates
                    e.onCompleted += callback;
                    e.StartTimeout();
                }
            }

            return e;
        }

        // Allows: instance -= Action;
        public static SingleEvent<T> operator -(SingleEvent<T> e, Action<T> callback) {
            if (e == null)
                return null;

            lock (e.Lock) {
                e.onCompleted -= callback;
            }

            return e;
        }

    }

}
#endif