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
            MonoBehaviourHelper mbHelper = MonoBehaviourHelper.CreateTemporaryMonoBehaviour(null, "Delayer");
            mbHelper.StartCoroutine(ForCoroutine(seconds, action, mbHelper));
        }
        #endregion

        #region ForCoroutine
        private static IEnumerator ForCoroutine(float seconds, Action action, MonoBehaviourHelper helper)
        {
            float timeElapsed = 0f;
            while (timeElapsed < seconds)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            action?.Invoke();
            Object.Destroy(helper.gameObject);
        }
        #endregion

        #region While
        public static void While(Func<bool> conditionFunc, Action action)
        {
            MonoBehaviourHelper mbHelper = MonoBehaviourHelper.CreateTemporaryMonoBehaviour(null, "Delayer");
            mbHelper.StartCoroutine(WhileCoroutine(conditionFunc, action, mbHelper));
        }
        #endregion

        #region WhileCoroutine
        private static IEnumerator WhileCoroutine(Func<bool> conditionFunc, Action action, MonoBehaviourHelper helper)
        {
            while (conditionFunc.Invoke())
            {
                yield return null;
            }
            action.Invoke();
            Object.Destroy(helper.gameObject);
        }
        #endregion
    }
}