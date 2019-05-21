using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PupilLabs
{
    public class BlinkListener
    {

        public event Action<bool> OnBlinkDetected;

        private RequestController requestCtrl;
        private SubscriptionsController subsCtrl;

        public BlinkListener(SubscriptionsController subsCtrl)
        {
            this.subsCtrl = subsCtrl;
            this.requestCtrl = subsCtrl.requestCtrl;

            requestCtrl.OnConnected += Enable;
            requestCtrl.OnDisconnecting += Disable;

            if (requestCtrl.IsConnected)
            {
                Enable();
            }
        }

        ~BlinkListener()
        {
            requestCtrl.OnConnected -= Enable;
            requestCtrl.OnDisconnecting -= Disable;

            if (requestCtrl.IsConnected)
            {
                Disable();
            }
        }

        public void Enable()
        {
            Debug.Log("Enabling Blink Listener");

            subscriptionsController.SubscribeTo("blinks", CustomReceiveData);

            requestCtrl.StartPlugin(
                "Blink_Detection",
                new Dictionary<string, object> {
                    { "history_length", 0.2f },
                    { "onset_confidence_threshold", 0.5f },
                    { "offset_confidence_threshold", 0.5f }
                }
            );
        }

        public void Disable()
        {
            requestCtrl.StopPlugin("Blink_Detection");

            subscriptionsController.UnsubscribeFrom("blinks", CustomReceiveData);
        }

        void CustomReceiveData(string topic, Dictionary<string, object> dictionary, byte[] thirdFrame = null)
        {
            if (dictionary.ContainsKey("timestamp"))
            {
                // Debug.Log("Blink detected: " + dictionary["timestamp"].ToString());
                OnBlinkDetected(true);
            }
            else OnBlinkDetected(false);
        }
    }
}
