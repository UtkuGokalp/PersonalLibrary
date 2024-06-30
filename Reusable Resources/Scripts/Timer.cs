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
        public event TypeSafeEventHandler<Timer, TimerTickEventArgs>? OnTimerTicked;
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
        public void Tick(float deltaTime)
        {
            if (increasing)
            {
                currentTime += deltaTime;
                if (currentTime >= targetTime)
                {
                    OnTimerFinished?.Invoke(this, System.EventArgs.Empty);
                }
            }
            else
            {
                currentTime -= deltaTime;
                if (currentTime <= 0)
                {
                    OnTimerFinished?.Invoke(this, System.EventArgs.Empty);
                }
            }
            OnTimerTicked?.Invoke(this, new TimerTickEventArgs(RemainingTime, ElapsedTime, deltaTime));
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

    #region TimerTickEventArgs
    public class TimerTickEventArgs : System.EventArgs
    {
        public float RemainingTime { get; }
        public float ElapsedTime { get; }
        public float DeltaTime { get; }

        public TimerTickEventArgs(float remainingTime, float elapsedTime, float deltaTime)
        {
            RemainingTime = remainingTime;
            ElapsedTime = elapsedTime;
            DeltaTime = deltaTime;
        }
    }
    #endregion
}
