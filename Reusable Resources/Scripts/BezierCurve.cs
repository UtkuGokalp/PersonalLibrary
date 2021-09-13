#nullable enable

using UnityEngine;
using System.Collections.Generic;

namespace Utility.Math
{
    public static class BezierCurve
    {
        /// <summary>
        /// Returns point at t in cubic bezier curve.
        /// </summary>
        public static Vector3 GetPoint(Vector3 start, Vector3 peak, Vector3 end, float t)
        {
            t = Mathf.Clamp01(t);
            Vector3 start_peakLerp = Vector3.Lerp(start, peak, t);
            Vector3 peak_endLerp = Vector3.Lerp(peak, end, t);
            Vector3 bezierLerp = Vector3.Lerp(start_peakLerp, peak_endLerp, t);
            return bezierLerp;
        }

        /// <summary>
        /// Returns point at t in quadratic bezier curve.
        /// </summary>
        public static Vector3 GetPoint(Vector3 start, Vector3 peak1, Vector3 peak2, Vector3 end, float t)
        {
            t = Mathf.Clamp01(t);
            Vector3 start_peak1Lerp = Vector3.Lerp(start, peak1, t);
            Vector3 peak1_peak2Lerp = Vector3.Lerp(peak1, peak2, t);
            Vector3 peak2_endLerp = Vector3.Lerp(peak2, end, t);

            Vector3 bezierLerp1 = Vector3.Lerp(start_peak1Lerp, peak1_peak2Lerp, t);
            Vector3 bezierLerp2 = Vector3.Lerp(peak1_peak2Lerp, peak2_endLerp, t);

            return Vector3.Lerp(bezierLerp1, bezierLerp2, t);
        }

        /// <summary>
        /// Returns point at t in bezier curve with n points.
        /// </summary>
        public static Vector3 GetPoint(float t, params Vector3[] points)
        {
            t = Mathf.Clamp01(t);
            if (points.Length < 3)
            {
                throw new System.ArgumentException("Points must have at least 3 points.");
            }

            List<Vector3> currentPoints = new List<Vector3>(points);
            List<Vector3> lerpedPoints = new List<Vector3>();

            while (currentPoints.Count > 2)
            {
                for (int i = 0; i < currentPoints.Count - 2; i++)
                {
                    Vector3 lerpedPoint1 = Vector3.Lerp(currentPoints[i], currentPoints[i + 1], t);
                    Vector3 lerpedPoint2 = Vector3.Lerp(currentPoints[i + 1], currentPoints[i + 2], t);
                    Vector3 lerpedPoint_Result = Vector3.Lerp(lerpedPoint1, lerpedPoint2, t);
                    lerpedPoints.Add(lerpedPoint_Result);
                }
                currentPoints.Clear();
                currentPoints.AddRange(lerpedPoints);
                lerpedPoints.Clear();
            }

            if (currentPoints.Count == 1)
            {
                return currentPoints[0];
            }
            else //if (lerpedPoints.Count == 2)
            {
                return Vector3.Lerp(currentPoints[0], currentPoints[1], t);
            }
        }
    }
}
