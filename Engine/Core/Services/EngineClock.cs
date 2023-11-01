namespace Engine.Core.Services;

public class EngineClock
{
    public float TotalTime { get; private set; } = 0;
    public float DeltaTime { get; private set; } = 0;
    public ulong CurrentFrame { get; private set; } = 0;
    public bool NewSecond { get; private set; } = false;

    private double _elapsedTimeThisSecond = 1;

    public void Update(float elapsedTime)
    {
        NewSecond = false;
        TotalTime += elapsedTime;
        DeltaTime = elapsedTime;
        CurrentFrame++;

        _elapsedTimeThisSecond += elapsedTime;
        if (_elapsedTimeThisSecond >= 1)
        {
            NewSecond = true;
            _elapsedTimeThisSecond = 0;
        }
    }
}
