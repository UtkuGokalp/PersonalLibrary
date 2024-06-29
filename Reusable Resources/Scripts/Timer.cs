#nullable enable

using Utility.Development;

namespace DevsThatJam
{
    public class Timer
    {
        #region Variables
        private float targetTime;
        private float currentTime;
        public event TypeSafeEventHandler<Timer, System.EventArgs>? OnTimerFinished;
        #endregion

        #region Constructors
        public Timer(float timeInSeconds)
        {
            targetTime = timeInSeconds;
            OnTimerFinished = null;
        }

        public Timer(float timeInSeconds, TypeSafeEventHandler<Timer, System.EventArgs> onTimerFinished)
        {
            targetTime = timeInSeconds;
            OnTimerFinished += onTimerFinished;
        }
        #endregion

        #region Tick
        public void Tick(float elapsedTime)
        {
            currentTime += elapsedTime;
            if (currentTime > targetTime)
            {
                OnTimerFinished?.Invoke(this, System.EventArgs.Empty);
            }
        }
        #endregion

        #region Reset
        public void Reset()
        {
            currentTime = 0f;
        }
        #endregion
    }
}
