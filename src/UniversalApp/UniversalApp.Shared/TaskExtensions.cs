
namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
            task.ContinueWith(t =>
            {
                var e = t.Exception;
            });
        }
    }
}
