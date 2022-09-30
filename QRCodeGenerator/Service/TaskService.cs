using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QRCodeGenerator.Service
{
    public interface ITaskService
    {
        CancellationTokenSource CancellationTokenSource { get; set; }
        IList<Task> Tasks { get; set; }

        void CancelAll();
    }

    public class TaskService : ITaskService
    {
        public CancellationTokenSource CancellationTokenSource { set; get; }

        public IList<Task> Tasks { get; set; }

        public TaskService()
        {
            Tasks = new List<Task>();
        }

        public void CancelAll()
        {
            foreach (Task task in Tasks)
            {
                task.Dispose();
            }
        }
    }
}