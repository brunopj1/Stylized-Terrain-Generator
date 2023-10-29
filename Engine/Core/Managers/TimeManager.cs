using Engine.Core.Managers.Interfaces;
using Engine.Exceptions;

namespace Engine.Core.Managers;

internal class TimeManager : ITimeManager
{
    public double TotalTime { get; private set; }
    public double DeltaTime { get; private set; }
    public ulong CurrentFrame { get; private set; }
    public int FrameRate { get; private set; }

    private int _framesThisSecond = 0;
    private double _elapsedTimeThisSecond = 1; // Early successfull update

    public TimeManager()
    {
        if (ITimeManager.Instance != null) throw new MultipleSingletonException<TimeManager>();
        ITimeManager.Instance = this;
    }

    public bool Update(double elapsedTime)
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
