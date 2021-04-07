using UnityEngine;
using Utility.Development;

namespace Utility.FPS
{
    public class FPSAimController : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private LayerMask aimLayer;
        private Transform aimedTransform;
        public event TypeSafeEventHandler<FPSAimController, OnAimedEventArgs> OnAimed;
        public event TypeSafeEventHandler<FPSAimController, OnAimLostEventArgs> OnAimIsLost;
        #endregion

        #region Update
        private void Update()
        {
            Ray ray = Camera.main.ViewportPointToRay(Vector2.one * 0.5f);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayer, QueryTriggerInteraction.Ignore);

            if (hit)
            {
                if (aimedTransform == null)
                {
                    aimedTransform = hitInfo.transform;
                    OnAimed?.Invoke(this, new OnAimedEventArgs(hitInfo));
                }
            }
            else
            {
                if (!(aimedTransform is null))
                {
                    OnAimIsLost?.Invoke(this, new OnAimLostEventArgs(aimedTransform));
                    aimedTransform = null;
                }
            }
        }
        #endregion

        #region OnAimedEventArgs
        public class OnAimedEventArgs : System.EventArgs
        {
            public RaycastHit AimInfo { get; }

            public OnAimedEventArgs(RaycastHit aimInfo)
            {
                AimInfo = aimInfo;
            }
        }
        #endregion

        #region OnAimLostEventArgs
        public class OnAimLostEventArgs : System.EventArgs
        {
            public Transform TransformAimIsLostFrom { get; }

            public OnAimLostEventArgs(Transform transformAimIsLostFrom)
            {
                TransformAimIsLostFrom = transformAimIsLostFrom;
            }
        }
        #endregion
    }
}
