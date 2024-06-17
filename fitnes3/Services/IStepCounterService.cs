using System;
using System.Collections.Generic;
using System.Text;

namespace fitnes3.Services
{
    public interface IStepCounterService
    {
        event EventHandler<StepCountChangedEventArgs> StepCountChanged;
        void Start();
        void Stop();
    }

    public class StepCountChangedEventArgs : EventArgs
    {
        public int StepCount { get; private set; }

        public StepCountChangedEventArgs(int stepCount)
        {
            StepCount = stepCount;
        }
    }
}
