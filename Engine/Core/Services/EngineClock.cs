namespace Engine.Core.Services;

public class EngineClock
{
    public float TotalTime { get; private set; } = 0;
    public float DeltaTime { get; private set; } = 0;
    public ulong CurrentFrame { get; private set; } = 0;
    public int FrameRate { get; private set; } = 0;

    private int _framesThisSecond = 0;
    private double _elapsedTimeThisSecond = 1;

    public bool Update(float elapsedTime)
    {
        TotalTime += elapsedTime;
        DeltaTime = elapsedTime;
        CurrentFrame++;

        _elapsedTimeThisSecond += elapsedTime;
        _framesThisSecond++;

        if (_elapsedTimeThisSecond >= 1)
        {
            FrameRate = _framesThisSecond;
            _framesThisSecond = 0;
            _elapsedTimeThisSecond = 0;
            return true;
        }

        return false;
    }
}
