using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainGenerator.Engine.Util;

internal class FpsCounter
{
    private int _frame = 0;
    private double _elapsedTime = 1; // Early successfull update

    public int FrameRate { get; private set; }

    public bool Update(double elapsedTime)
    {
        _frame++;
        _elapsedTime += elapsedTime;

        if (_elapsedTime >= 1)
        {
            FrameRate = _frame;
            _frame = 0;
            _elapsedTime = 0;
            return true;
        }

        return false;
    }
}
