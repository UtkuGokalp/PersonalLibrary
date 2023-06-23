#nullable enable

using System;
using UnityEngine;

namespace Utility.Development
{
    public static class Easing
    {
        #region Ease
        /// <summary>
        /// Eases a float with the given easing function. The output is not clamped. 
        /// </summary>
        public static float Ease(float initial, float target, float t, Func<float, float> easingFunc)
        {
            return Mathf.LerpUnclamped(initial, target, easingFunc(t));
        }

        /// <summary>
        /// Eases a Vector2 with the given easing function. The output is not clamped. 
        /// </summary>
        public static Vector2 Ease(Vector2 initial, Vector2 target, float t, Func<float, float> easingFunc)
        {
            return Ease((Vector3)initial, (Vector3)target, t, easingFunc);
        }

        
        /// <summary>
        /// Eases a Vector3 with the given easing function. The output is not clamped. 
        /// </summary>
        public static Vector3 Ease(Vector3 initial, Vector3 target, float t, Func<float, float> easingFunc)
        {
            return new Vector3(Ease(initial.x, target.x, t, easingFunc),
                               Ease(initial.y, target.y, t, easingFunc),
                               Ease(initial.z, target.z, t, easingFunc));
        }
        #endregion
    }
}