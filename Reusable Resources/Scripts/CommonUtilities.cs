#nullable enable

using UnityEngine;

namespace Utility.Development
{
    public class CommonUtilities
    {
        private static readonly object lockObject = new object();
        private static CommonUtilities? instance;

        public static CommonUtilities Instance
        {
            get
            {
                lock (lockObject)
                {
                    return instance ??= new CommonUtilities();
                }
            }
        }

        private GameObject? player;
        private Transform? playerTransform;
        private Camera? mainCamera;

        public GameObject? Player
        {
            get
            {
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                    if (player == null)
                    {
                        Debug.LogWarning("No player was found. Check if you tagged the player with the \"Player\" tag.");
                    }
                }

                return player;
            }
        }

        public Transform? PlayerTransform
        {
            get
            {
                if (Player == null)
                {
                    return null;
                }

                if (playerTransform == null && Player != null)
                {
                    playerTransform = Player.transform;
                }

                return playerTransform;
            }
        }

        public Vector3? PlayerPosition
        {
            get
            {
                if (PlayerTransform == null)
                {
                    return null;
                }
                return PlayerTransform.position;
            }
            set
            {
                if (PlayerTransform != null)
                {
                    if (value.HasValue)
                    {
                        PlayerTransform.position = value.Value;
                    }
                }
            }
        }

        public Quaternion? PlayerRotation
        {
            get
            {
                if (PlayerTransform == null)
                {
                    return null;
                }
                return PlayerTransform.rotation;
            }
            set
            {
                if (PlayerTransform != null)
                {
                    if (value.HasValue)
                    {
                        PlayerTransform.rotation = value.Value;
                    }
                }
            }
        }

        public Camera MainCamera
        {
            get
            {
                if (mainCamera == null)
                {
                    mainCamera = Camera.main;
                }

                return mainCamera;
            }
        }

        public const string PLAYER_TAG = "Player";
        public const string PLAYER_LAYER_NAME = "Player";

        /// <summary>
        /// Gets the mouse position in 2D relative to the main camera.
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMousePosition2D()
        {
            return GetMousePosition2D(Instance.MainCamera);
        }

        public static Vector2 GetMousePosition2D(Camera relativeTo)
        {
            return relativeTo.ScreenToWorldPoint(Input.mousePosition);
        }

        /// <summary>
        /// Returns the amount of movement an object should move with in order to reach it's target properly no matter the frame rate. The returned value is generally used by multiplying it with a normalized movement vector.
        /// </summary>
        public static float GetFrameRateIndependantMoveAmount(float speed, float distanceLeftToTarget, float deltaTime)
        {
            float moveAmount = speed * deltaTime;
            moveAmount = Mathf.Clamp(moveAmount, 0, distanceLeftToTarget);
            return moveAmount;
        }

        public static Vector2 GetCameraViewportSize(Camera camera)
        {
            //Camera.ortographicSize is camera's height / 2
            //Camera.aspect is camera's width / camera's height
            //When these two equations are reordered, the following calculations
            //give us the camera viewport size.
            float cameraViewportHeight = camera.orthographicSize * 2;
            float cameraViewportWidth = camera.aspect * cameraViewportHeight;
            return new Vector2(cameraViewportWidth, cameraViewportHeight);
        }

        private CommonUtilities()
        {
        }
    }
}
