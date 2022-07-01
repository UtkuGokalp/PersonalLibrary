#nullable enable

using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace Utility.Development
{
    public static class Delay
    {
        #region For
        public static void For(float seconds, Action action)
        {
            MonoBehaviourHelper mbHelper = MonoBehaviourHelper.CreateTemporaryMonoBehaviour(seconds + 1, "Delayer");
            mbHelper.StartCoroutine(ForCoroutine(seconds, action, mbHelper));
        }
        #endregion

        #region ForCoroutine
        private static IEnumerator ForCoroutine(float seconds, Action action, MonoBehaviourHelper helper)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
            Object.Destroy(helper.gameObject);
        }
        #endregion
    }
}