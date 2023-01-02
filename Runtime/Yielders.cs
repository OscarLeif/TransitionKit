using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtaGames.TransitionKit
{
    public static class Yielders
    {
        static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);
        static Dictionary<float, WaitForSecondsRealtime> _timeIntervalRealTime = new Dictionary<float, WaitForSecondsRealtime>(100);

        static readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { return _endOfFrame; }
        }

        static readonly WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();

        /// <summary>
        /// Cache version for new WaitForFixedUpdate();
        /// </summary>
        public static WaitForFixedUpdate FixedUpdate
        {
            get { return _fixedUpdate; }
        }

        public static IEnumerator WaitForFrames(int frameCount = 1)
        {
            if (frameCount <= 0)
            {
                Debug.LogWarning("Frame counter cannot be less than 0");
                frameCount = 1;
            }

            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }

        /// <summary>
        /// Return Cache version of "new WaitForSeconds"
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static WaitForSeconds Get(float seconds)
        {
            if (!_timeInterval.ContainsKey(seconds))
                _timeInterval.Add(seconds, new WaitForSeconds(seconds));
            return _timeInterval[seconds];
        }

        public static WaitForSecondsRealtime GetRealTime(float seconds)
        {
            if (!_timeIntervalRealTime.ContainsKey(seconds))
            {
                _timeIntervalRealTime.Add(seconds, new WaitForSecondsRealtime(seconds));
            }
            return _timeIntervalRealTime[seconds];
        }

        public static int WaitSecondCounter()
        {
            return _timeInterval.Count;
        }
    }
}