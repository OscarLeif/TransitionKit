using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtaGames.TransitionKit.runtime
{
    public static class Extensions 
    {
        public static T getOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if(component==null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}
