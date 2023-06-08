using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtaGames.TransitionKit.runtime
{
    public static class TransitionUtils 
    {
        public static float LerpUnscaled(float from, float to, float timeDuration, ref float timeCounter, out bool complete)
        {
            complete = false;
            timeDuration = timeDuration <= 0 ? 0.001f : timeDuration;
            timeCounter = Mathf.Clamp(timeCounter + Time.unscaledDeltaTime, 0, timeDuration);
            float value = Mathf.Lerp(from, to, timeCounter / timeDuration);
            if (timeCounter >= timeDuration)
            {
                timeCounter = 0;
                complete = true;
                return to;
            }
            return value;
        }
    }
}
