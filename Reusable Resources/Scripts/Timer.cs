#nullable enable

namespace Utility.Development
{
    public class Timer
    {
        #region Variables
        private bool increasing;
        private float targetTime;
        private float currentTime;
        public event TypeSafeEventHandler<Timer, System.EventArgs>? OnTimerFinished;
        public float ElapsedTime => increasing ? currentTime : targetTime - currentTime;
        public float RemainingTime => increasing ? targetTime - currentTime : currentTime;
        #endregion

        #region Constructors
        public Timer(float timeInSeconds, bool increasing, TypeSafeEventHandler<Timer, System.EventArgs>? onTimerFinished)
        {
            targetTime = timeInSeconds;
            this.increasing = increasing;
            OnTimerFinished += onTimerFinished;

            if (increasing)
            {
                currentTime = 0;
            }
            else
            {
                currentTime = targetTime;
            }
        }
        #endregion

        #region Tick
        public void Tick(float elapsedTime)
        {
            if (increasing)
            {
                currentTime += elapsedTime;
                if (currentTime >= targetTime)
                {
                    OnTimerFinished?.Invoke(this, System.EventArgs.Empty);
                }
            }
            else
            {
                currentTime -= elapsedTime;
                if (currentTime <= 0)
                {
                    OnTimerFinished?.Invoke(this, System.EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Reset
        public void Reset()
        {
            if (increasing)
            {
                currentTime = 0f;
            }
            else
            {
                currentTime = targetTime;
            }
        }
        #endregion
    }
}
