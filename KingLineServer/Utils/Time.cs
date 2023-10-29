using System.Diagnostics;


public class Time
{
    public const int TARGET_FPS = 30;

    private readonly Stopwatch stopwatch = new();
    private ulong nextTickId = 0;

    public double Now => stopwatch.Elapsed.TotalSeconds;
    public double TickTime { get; private set; }
    public ulong TickId { get; private set; }

    public void Start()
    {
        stopwatch.Start();
    }

    public bool ShouldTick()
    {
        bool shouldTick = Now * TARGET_FPS > nextTickId;

        if (shouldTick)
        {
            TickTime = Now;
            TickId = nextTickId++;
        }

        return shouldTick;
    }
}
