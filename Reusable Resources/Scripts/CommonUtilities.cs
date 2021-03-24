using UnityEngine;

namespace Utility.Development
{
    public class CommonUtilities
    {
        private static readonly object lockObject = new object();
        private static CommonUtilities instance;

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

        private GameObject player;
        private Transform playerTransform;

        public GameObject Player
        {
            get
            {
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                }

                return player;
            }
        }

        public Transform PlayerTransform
        {
            get
            {
                if (playerTransform == null && Player != null)
                {
                    playerTransform = Player.transform;
                    
                }

                return playerTransform;
            }
        }

        public Vector3 PlayerPosition
        {
            get => PlayerTransform.position;
            set => PlayerTransform.position = value;
        }

        public Quaternion PlayerRotation
        {
            get => PlayerTransform.rotation;
            set => PlayerTransform.rotation = value;
        }
        
        public const string PLAYER_TAG = "Player";
        public const string PLAYER_LAYER = "Player";
        
        private CommonUtilities()
        {
        }
    }
}