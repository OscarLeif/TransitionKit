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
}
