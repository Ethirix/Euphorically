namespace Euphorically.Utilities
{
    internal class Timer
    {
        public Timer(float time)
        {
            Time = time;
        }

        public void Update(float time)
        {
            CurrentTime += time;

            if (CurrentTime >= Time)
                Completed = true;
        }

        public void Restart()
        {
            CurrentTime = 0;
            Completed = false;
        }

        public void Restart(float time)
        {
            CurrentTime = 0;
            Time = time;
            Completed = false;
        }

        public void ForceComplete()
        {
            Completed = true;
        }

        public float Time { get; private set; }
        public bool Completed { get; private set; } = true;
        public float CurrentTime { get; private set; }
    }
}
