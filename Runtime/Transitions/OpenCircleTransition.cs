using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtaGames.TransitionKit.runtime
{
    public class OpenCircleTransition : MonoBehaviour, ITransition
    {
        private void Awake()
        {
            
        }

        public void Update()
        {
            
        }

        public IEnumerator YieldTransition()
        {
            yield return null;
        }
    }
}
