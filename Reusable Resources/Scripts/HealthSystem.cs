#nullable enable

using UnityEngine;
using Utility.Development;

namespace Utility.Health
{
    #region HealthSystem
    [DisallowMultipleComponent]
    public class HealthSystem : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private int health = 20;
        [SerializeField]
        private int maxHealth = 20;
        public int Health => health;
        public int MaxHealth => maxHealth;
        public bool Invincible { get; set; }
        public bool IsAlive { get; private set; }
        public bool HasFullHealth => Health == MaxHealth;
        public float HealthPercent => (float)health / maxHealth;
        public event TypeSafeEventHandler<HealthSystem, System.EventArgs>? OnDeath;
        public event TypeSafeEventHandler<HealthSystem, OnHealthChangedEventArgs>? OnHealthChanged;
        #endregion

        #region Awake
        private void Awake()
        {
            IsAlive = true;
            Invincible = false;
        }
        #endregion

        #region IncreaseHealth
        public void IncreaseHealth(int amount)
        {
            if (health != maxHealth)
            {
                int previousHealth = health;
                health += amount;
                if (health > maxHealth)
                {
                    health = maxHealth;
                }
                OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(health, previousHealth, OnHealthChangedEventArgs.HealthChange.Increase));
            }
        }
        #endregion

        #region DecreaseHealth
        public void DecreaseHealth(int amount)
        {
            if (!Invincible && IsAlive)
            {
                int previousHealth = health;
                health -= amount;
                if (health <= 0)
                {
                    health = 0;
                    IsAlive = false;
                    OnDeath?.Invoke(this, System.EventArgs.Empty);
                }
                OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(health, previousHealth, OnHealthChangedEventArgs.HealthChange.Decrease));
            }
        }
        #endregion

        #region KillSelf
        /// <summary>
        /// Kills the game object regardless of being invincible.
        /// </summary>
        public void KillSelf()
        {
            int previousHealth = health;
            health = 0;
            IsAlive = false;
            OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(health, previousHealth, OnHealthChangedEventArgs.HealthChange.Decrease));
            OnDeath?.Invoke(this, System.EventArgs.Empty);
        }
        #endregion

        #region OnValidate
        private void OnValidate()
        {
            if (health <= 0)
            {
                health = 1;
            }
            if (maxHealth < health)
            {
                health = maxHealth;
            }
            if (maxHealth <= 0)
            {
                maxHealth = 1;
                health = maxHealth;
            }
        }
        #endregion
    }
    #endregion

    #region OnHealthChangedEventArgs
    public class OnHealthChangedEventArgs : System.EventArgs
    {
        #region Variables
        public enum HealthChange { None, Increase, Decrease }
        public int PreviousHealth { get; }
        public int CurrentHealth { get; }
        public HealthChange HealthChangeData { get; }
        #endregion

        #region Constructor
        public OnHealthChangedEventArgs(int currentHealth, int previousHealth, HealthChange healthChangeData)
        {
            CurrentHealth = currentHealth;
            PreviousHealth = previousHealth;
            HealthChangeData = healthChangeData;
        }
        #endregion
    }
    #endregion
}
