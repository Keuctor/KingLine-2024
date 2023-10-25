using System.Diagnostics;


public class Time
{
    public const int TARGET_FPS = 30;

    private readonly Stopwatch stopwatch = new();
    private ulong nextTickId = 0;

    // The stopwatch time right now
    public double Now => stopwatch.Elapsed.TotalSeconds;
    // The stopwatch time at the beginning of this tick
    public double TickTime { get; private set; }
    // The ID of the current tick
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
