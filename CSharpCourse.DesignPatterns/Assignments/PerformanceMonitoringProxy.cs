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
    private T _decorated;
    private TimeSpan _threshold;

    public static T? Create(T decorated, TimeSpan threshold)
    {
        // Crea un'istanza del proxy come PerformanceMonitoringProxy<T>
        var proxy = Create<T, PerformanceMonitoringProxy<T>>() as PerformanceMonitoringProxy<T> ?? throw new FormatException();

        // Assegna i campi decorati e soglia al proxy
        proxy._decorated = decorated;
        proxy._threshold = threshold;

        return proxy as T;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) throw new FormatException();

        var stopwatch = Stopwatch.StartNew();

        // Controlla se il metodo restituisce un Task
        var result = targetMethod.Invoke(_decorated, args);

        if (result is Task task)
        {
            // Controlla se il Task ha un tipo di risultato
            var taskType = task.GetType();
            if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // Gestisci il Task con risultato
                var resultType = taskType.GetGenericArguments()[0];
                var handleMethod = typeof(PerformanceMonitoringProxy<T>).GetMethod(nameof(HandleTaskWithResult), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(resultType);

                return handleMethod?.Invoke(this, new object[] { task, targetMethod, stopwatch });
            }
            else
            {
                // Gestisci il Task senza risultato
                return HandleTask(task, targetMethod, stopwatch);
            }
        }

        stopwatch.Stop();
        if (stopwatch.Elapsed > _threshold)
        {
            Console.WriteLine($"Method {targetMethod.Name} took {stopwatch.ElapsedMilliseconds}ms, exceeding the threshold of {_threshold.TotalMilliseconds}ms.");
        }

        return result;
    }


    private void RaiseStopWatch(MethodInfo? targetMethod, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        if(targetMethod == null) throw new FormatException();

        if (stopwatch.Elapsed > _threshold)
        {
            Console.WriteLine($"Method {targetMethod.Name} took {stopwatch.ElapsedMilliseconds}ms, exceeding the threshold of {_threshold.TotalMilliseconds}ms.");
        }
    }

    private async Task HandleTask(Task task, MethodInfo targetMethod, Stopwatch stopwatch)
    {
        await task; 
        RaiseStopWatch(targetMethod, stopwatch);
    }

    private async Task<TResult> HandleTaskWithResult<TResult>(Task<TResult> task, MethodInfo targetMethod, Stopwatch stopwatch)
    {
        // Aspetta il completamento del Task e ottieni il risultato
        TResult result = await task;

        // Al termine dell'operazione, ferma il cronometro
        RaiseStopWatch(targetMethod, stopwatch);

        // Restituisci il risultato
        return result;
    }
}
