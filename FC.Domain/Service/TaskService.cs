using FC.Domain._Util;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Domain.Service
{
    public interface ITaskService
    {
        CancellationTokenSource CancellationTokenSource { get; set; }
        IList<Task> Tasks { get; set; }
        CountDownTimer CountDownCancellationTokenSource { get; set; }

        public ReplaySubject<string> TimeLeftMsStrReplay { get; set; }

        public ReplaySubject<bool> CanceledReplay { get; set; }

        void CancelAll();

        void SetTimeout(int timeout);
    }

    public class TaskService : ITaskService
    {
        private const int Milliseconds = 150;

        private readonly CountDownTimer countDown;

        public CountDownTimer CountDownCancellationTokenSource { get; set; }

        private CancellationTokenSource _cancellationTokenSource;

        private readonly CancellationTokenSource _cancellationTokenSourceCanceled;

        public ReplaySubject<bool> CanceledReplay { get; set; }

        public IList<Task> Tasks { get; set; }

        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (countDown.IsRunnign)
                {
                    CanceledReplay.OnNext(true);
                    countDown.Reset();
                    countDown.Start();
                    return _cancellationTokenSourceCanceled;
                }
                CanceledReplay.OnNext(false);
                return _cancellationTokenSource;
            }
            set
            {
                _cancellationTokenSource = value;
                CancellationsTokenSource.Add(value);
            }
        }

        public ReplaySubject<string> TimeLeftMsStrReplay { get; set; }

        private readonly IList<CancellationTokenSource> CancellationsTokenSource;

        public TaskService()
        {
            CancellationsTokenSource = new List<CancellationTokenSource>();

            TimeLeftMsStrReplay = new ReplaySubject<string>();

            CanceledReplay = new ReplaySubject<bool>();

            Tasks = new List<Task>();

            CountDownCancellationTokenSource = new CountDownTimer
            {
                StepMs = 77
            };

            countDown = new CountDownTimer
            {
                StepMs = 10,
                CountDownFinished = () =>
                {
                    CanceledReplay.OnNext(false);
                }
            };

            CountDownCancellationTokenSource.TimeChanged += () =>
            {
                TimeLeftMsStrReplay.OnNext(CountDownCancellationTokenSource.TimeLeftMsStr);
            };

            _cancellationTokenSourceCanceled = new CancellationTokenSource();

            _cancellationTokenSourceCanceled.Cancel();
        }

        public void CancelAll()
        {
            CanceledReplay.OnNext(true);
            countDown.SetTime(Milliseconds);
            countDown.Reset();
            countDown.Start();
            foreach (CancellationTokenSource cts in CancellationsTokenSource)
            {
                if (cts is null)
                {
                    continue;
                }
                if (cts.IsCancellationRequested)
                {
                    cts.Dispose();
                    continue;
                }
                cts.Cancel();
            }
            CancellationsTokenSource.Clear();
        }

        public void SetTimeout(int timeout)
        {
            CountDownCancellationTokenSource.SetTime(timeout);
            CountDownCancellationTokenSource.Reset();
            CountDownCancellationTokenSource.Start();
        }
    }
}