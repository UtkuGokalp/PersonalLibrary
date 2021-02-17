using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Development
{
    public static class ExtensionMethods
    {
        #region With
        public static Vector2 With(this Vector2 vector2, float? x, float? y)
        {
            if (x != null)
            {
                vector2.x = (float)x;
            }
            if (y != null)
            {
                vector2.y = (float)y;
            }
            return vector2;
        }

        public static Vector3 With(this Vector3 vector3, float? x, float? y, float? z)
        {
            if (x != null)
            {
                vector3.x = (float)x;
            }
            if (y != null)
            {
                vector3.y = (float)y;
            }
            if (z != null)
            {
                vector3.z = (float)z;
            }
            return vector3;
        }

        public static Color With(this Color color, float? r, float? g, float? b, float? a)
        {
            if (r != null)
            {
                color.r = (float)r;
            }
            if (g != null)
            {
                color.g = (float)g;
            }
            if (b != null)
            {
                color.b = (float)b;
            }
            if (a != null)
            {
                float aNotNull = (float)a;
                aNotNull = Mathf.Clamp01(aNotNull);
                color.a = aNotNull;
            }
            return color;
        }
        #endregion

        #region ChangeEulerAngles
        public static Quaternion ChangeEulerAngles(this Quaternion rotation, float? x, float? y, float? z)
        {
            Vector3 eulerAngles = rotation.eulerAngles;
            if (x != null)
            {
                eulerAngles.x = (float)x;
            }
            if (y != null)
            {
                eulerAngles.y = (float)y;
            }
            if (z != null)
            {
                eulerAngles.z = (float)z;
            }
            Quaternion returnValue = new Quaternion();
            returnValue.eulerAngles = eulerAngles;
            return returnValue;
        }
        #endregion

        #region GetAngleTo
        public static float GetAngleTo(this Vector2 currentPosition, Vector2 targetPosition)
        {
            Vector2 dir = (targetPosition - currentPosition).normalized;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        public static float GetAngleTo(this Vector3 currentPosition, Vector2 targetPosition) => GetAngleTo((Vector2)currentPosition, targetPosition);
        #endregion

        #region GetAngle
        public static float GetAngle(this Vector2 direction) => Mathf.Atan2(direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;
        public static float GetAngle(this Vector3 direction) => GetAngle((Vector2)direction);
        #endregion

        #region DirectionTo
        public static Vector2 DirectionTo(this Vector2 from, Vector2 to, bool normalized = true) => normalized ? (to - from).normalized : (to - from);
        public static Vector3 DirectionTo(this Vector3 from, Vector3 to, bool normalized = true) => normalized ? (to - from).normalized : (to - from);
        #endregion

        #region CheckSquaredDistance
        /// <summary>
        /// Checks distance without using square root so that it is more performant. == operator isn't supported.
        /// </summary>
        /// <param name="position">Position to check from.</param>
        /// <param name="checkPosition">Position to check against.</param>
        /// <param name="distance">Target distance.</param>
        /// <param name="smallerCheck">If true, comparasion will be made using smaller than operator. Otherwise bigger than operator will be used.</param>
        /// <param name="equalityCheck">If true, equality check will also be made.</param>
        public static bool CheckSquaredDistance(this Vector3 position, Vector3 checkPosition, float distance, bool smallerCheck, bool equalityCheck)
        {
            float distanceBetweenObjectsSquared = position.DirectionTo(checkPosition, false).sqrMagnitude;
            if (smallerCheck && !equalityCheck) //<
            {
                return distanceBetweenObjectsSquared < distance * distance;
            }
            else if (smallerCheck && equalityCheck) //<=
            {
                return distanceBetweenObjectsSquared <= distance * distance;
            }
            else if (!smallerCheck && !equalityCheck) //>
            {
                return distanceBetweenObjectsSquared > distance * distance;
            }
            else //>=
            {
                return distanceBetweenObjectsSquared >= distance * distance;
            }
        }
        #endregion

        #region IsObjectVisible
        public static bool IsObjectVisible(this Camera camera, Vector2 objectPosition, Vector2 boundaryOffset)
        {
            if (boundaryOffset.x < 0)
            {
                boundaryOffset.x *= -1;
            }
            if (boundaryOffset.y < 0)
            {
                boundaryOffset.y *= -1;
            }

            Transform cameraTransformCache = camera.transform;
            float horizontalSize = camera.aspect * camera.orthographicSize;
            Vector2 leftUp = new Vector2(cameraTransformCache.position.x - horizontalSize, cameraTransformCache.position.y + camera.orthographicSize);
            Vector2 rightBottom = new Vector2(cameraTransformCache.position.x + horizontalSize, cameraTransformCache.position.y - camera.orthographicSize);

            leftUp.x -= boundaryOffset.x;
            leftUp.y += boundaryOffset.y;
            rightBottom.x += boundaryOffset.x;
            rightBottom.y -= boundaryOffset.y;

            if (objectPosition.x > leftUp.x && objectPosition.x < rightBottom.x)
            {
                if (objectPosition.y < leftUp.y && objectPosition.y > rightBottom.y)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetRandomElement
        //Instead of using IEnumerable<T> here, T[] and List<T> are implemented separately in order to avoid the usage of System.Linq.
        public static T GetRandomElement<T>(this T[] array) => array[Random.Range(0, array.Length)];
        public static T GetRandomElement<T>(this List<T> list) => list[Random.Range(0, list.Count)];
        #endregion

        #region Shuffle
        //Instead of using IEnumerable<T> here, T[] and List<T> are implemented separately in order to avoid the usage of System.Linq.
        public static void Shuffle<T>(this T[] array, int shuffleCount = 10)
        {
            for (int i = 0; i < shuffleCount; i++)
            {
                int firstIndex = Random.Range(0, array.Length);
                int secondIndex = Random.Range(0, array.Length);
                T temp = array[firstIndex];
                array[firstIndex] = array[secondIndex];
                array[secondIndex] = temp;
            }
        }

        public static void Shuffle<T>(this List<T> list, int shuffleCount = 10)
        {
            for (int i = 0; i < shuffleCount; i++)
            {
                int firstIndex = Random.Range(0, list.Count);
                int secondIndex = Random.Range(0, list.Count);
                T temp = list[firstIndex];
                list[firstIndex] = list[secondIndex];
                list[secondIndex] = temp;
            }
        }
        #endregion

        #region FadeIn
        public static void FadeIn(this AudioSource audioSource, float fadeTime, float lastVolume = 1, System.Action afterFadingFinished = null)
        {
            MonoBehaviourHelper helper = MonoBehaviourHelper.CreateTemporaryMonoBehaviour(null);
            helper.StartCoroutine(FadeInCoroutine(audioSource, fadeTime, lastVolume, afterFadingFinished, helper));
        }
        #endregion

        #region FadeOut
        public static void FadeOut(this AudioSource audioSource, float fadeTime, System.Action afterFadingFinished = null)
        {
            MonoBehaviourHelper helper = MonoBehaviourHelper.CreateTemporaryMonoBehaviour(null);
            helper.StartCoroutine(FadeOutCoroutine(audioSource, fadeTime, afterFadingFinished, helper));
        }
        #endregion

        #region FadeInCoroutine
        private static IEnumerator FadeInCoroutine(AudioSource audioSource, float fadeTime, float lastVolume, System.Action afterFadingFinished, MonoBehaviourHelper helperToDestroy)
        {
            if (audioSource != null)
            {
                float currentTime = 0;
                float firstVolume = audioSource.volume;

                while (currentTime < fadeTime)
                {
                    audioSource.volume = Mathf.Lerp(firstVolume, lastVolume, currentTime / fadeTime);
                    currentTime += Time.deltaTime;
                    yield return null;
                }

                audioSource.volume = lastVolume;
                afterFadingFinished?.Invoke();
                Object.Destroy(helperToDestroy.gameObject);
            }
        }
        #endregion

        #region FadeOutCoroutine
        private static IEnumerator FadeOutCoroutine(AudioSource audioSource, float fadeTime, System.Action afterFadingFinished, MonoBehaviourHelper helperToDestroy)
        {
            if (audioSource != null)
            {
                float currentTime = 0;
                float firstVolume = audioSource.volume;

                while (currentTime < fadeTime)
                {
                    audioSource.volume = Mathf.Lerp(firstVolume, 0, currentTime / fadeTime);
                    currentTime += Time.deltaTime;
                    yield return null;
                }

                audioSource.volume = 0;
                afterFadingFinished?.Invoke();
                Object.Destroy(helperToDestroy.gameObject);
            }
        }
        #endregion
    }

    #region EmptyArrayException
    public class EmptyArrayException : System.Exception
    {
        public EmptyArrayException() : base("Array did not contain any elements.")
        {

        }
    }
    #endregion
}
