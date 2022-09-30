using FC.Domain._Base;
using System;
using System.Diagnostics;
using System.Timers;

namespace FC.Domain._Util
{
    public class CountDownTimer : ModelBase
    {
        public Stopwatch _stpWatch = new();

        public Action TimeChanged;
        public Action CountDownFinished;

        public bool IsRunnign => Timer.Enabled;

        public double StepMs
        {
            get => Timer.Interval;
            set => Timer.Interval = value;
        }

        private readonly Timer Timer = new();

        private TimeSpan Max = TimeSpan.FromMilliseconds(30000);

        public TimeSpan TimeLeft => (Max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds) > 0 ? TimeSpan.FromMilliseconds(Max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds) : TimeSpan.FromMilliseconds(0);

        private bool MustStop => (Max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds) < 0;

        public string TimeLeftStr => TimeLeft.ToString(@"\mm\:ss");

        public string TimeLeftMsStr => TimeLeft.ToString(@"mm\:ss\.fff");

        private void TimerTick(object sender, EventArgs e)
        {
            TimeChanged?.Invoke();

            if (MustStop)
            {
                CountDownFinished?.Invoke();
                _stpWatch.Stop();
                Timer.Enabled = false;
            }
        }

        public CountDownTimer(int milliseconds)
        {
            SetTime(milliseconds);
            Init();
        }

        public CountDownTimer(TimeSpan ts)
        {
            SetTime(ts);
            Init();
        }

        public CountDownTimer()
        {
            Init();
        }

        private void Init()
        {
            StepMs = 1000;
            Timer.Elapsed += new ElapsedEventHandler(TimerTick);
        }

        public void SetTime(TimeSpan ts)
        {
            Max = ts;
            TimeChanged?.Invoke();
        }

        public void SetTime(int milliseconds = 0)
        {
            SetTime(TimeSpan.FromMilliseconds(milliseconds));
        }

        public void Start()
        {
            Timer.Start();
            _stpWatch.Start();
        }

        public void Pause()
        {
            Timer.Stop();
            _stpWatch.Stop();
        }

        public void Stop()
        {
            Reset();
            Pause();
        }

        public void Reset()
        {
            _stpWatch.Reset();
        }

        public void Restart()
        {
            _stpWatch.Reset();
            Timer.Start();
        }

        public new void Dispose()
        {
            Timer.Dispose();
        }
    }
}