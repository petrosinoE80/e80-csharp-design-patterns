using System.Diagnostics;
using System.Reflection;

namespace CSharpCourse.DesignPatterns.Assignments;

internal interface IService
{
    void DoSomething();

    Task DoSomethingAsync();

    Task<bool> GetResultAsync();
}

internal class SlowService : IService
{
    private readonly TimeSpan _delay;

    public SlowService(TimeSpan delay)
    {
        _delay = delay;
    }

    public void DoSomething()
    {
        Thread.Sleep(_delay);
    }

    public async Task DoSomethingAsync()
    {
        await Task.Delay(_delay);
    }

    public async Task<bool> GetResultAsync()
    {
        await Task.Delay(_delay);
        return true;
    }
}

internal class PerformanceMonitoringProxy<T> : DispatchProxy where T : class
{
    private T? _decorated;
    private TimeSpan _threshold;

    private async Task HandleTask(Task task, MethodInfo targetMethod, Stopwatch stopwatch)
    {
        await task;
        RaiseStopWatch(targetMethod, stopwatch);
    }

    private async Task<TResult> HandleTaskWithResult<TResult>(Task<TResult> task, MethodInfo targetMethod, Stopwatch stopwatch)
    {
        TResult result = await task;
        RaiseStopWatch(targetMethod, stopwatch);
        return result;
    }

    private void RaiseStopWatch(MethodInfo? targetMethod, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        if (targetMethod == null) throw new FormatException();

        if (stopwatch.Elapsed > _threshold)
        {
            Console.WriteLine($"Method {targetMethod.Name} took {stopwatch.ElapsedMilliseconds}ms, exceeding the threshold of {_threshold.TotalMilliseconds}ms.");
        }
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) throw new FormatException();

        var stopwatch = Stopwatch.StartNew();
        var result = targetMethod.Invoke(_decorated, args);

        //if it is async
        if (result is Task task)
        {
            var taskType = task.GetType();
            if (taskType.IsGenericType && task.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance) != null)
            {
                //HandleTaskWithResult
                var resultType = taskType.GetGenericArguments()[0];
                var handleMethod = typeof(PerformanceMonitoringProxy<T>).GetMethod(nameof(HandleTaskWithResult),
                    bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(resultType);

                return handleMethod?.Invoke(this, [task, targetMethod, stopwatch]);
            }
            else
            {
                //HandleTaskWithoutResult 
                return HandleTask(task, targetMethod, stopwatch);
            }
        }


        // if it is not async
        stopwatch.Stop();
        if (stopwatch.Elapsed > _threshold)
        {
            Console.WriteLine($"Method {targetMethod.Name} took {stopwatch.ElapsedMilliseconds}ms, exceeding the threshold of {_threshold.TotalMilliseconds}ms.");
        }

        return result;
    }

    public static T? Create(T decorated, TimeSpan threshold)
    {
        var proxy = Create<T, PerformanceMonitoringProxy<T>>() as PerformanceMonitoringProxy<T> ?? throw new FormatException();
        proxy._decorated = decorated;
        proxy._threshold = threshold;
        return proxy as T;
    }
}