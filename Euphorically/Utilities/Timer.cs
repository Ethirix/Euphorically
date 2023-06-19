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
        }

        public float Time { get; }
        public bool Completed { get; private set; }
        public float CurrentTime { get; private set; }
    }
}
