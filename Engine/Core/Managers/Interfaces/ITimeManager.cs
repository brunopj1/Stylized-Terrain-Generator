using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.Managers.Interfaces;

public interface ITimeManager
{
    public static ITimeManager? Instance { get; protected set; } = null;

    double TotalTime { get; }
    double DeltaTime { get; }
    ulong CurrentFrame { get; }
    public int FrameRate { get; }
}
