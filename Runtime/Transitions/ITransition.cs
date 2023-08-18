using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtaGames.TransitionKit.runtime
{
    public interface ITransition
    {
        public void Update();
        public IEnumerator YieldTransition();
    }

    //Probably Need 2 more for Begin and Completed
    public enum TransitionState
    {
        StateIn, Hold, HoldDelay, StateOut
    }

    //Async Loading. 
    public enum LoadState
    {
        Begin, Loading, Completed
    }
}
