﻿using TTMouseclickSimulator.Core.Actions;
using TTMouseclickSimulator.Core.Environment;

namespace TTMouseclickSimulator.Core.ToontownRewritten.Actions;

/// <summary>
/// An action that just waits using the specified amount of time.
/// </summary>
public class PauseAction : AbstractAction
{
    private readonly int duration;

    public PauseAction(int duration)
    {
        this.duration = duration;
    }

    public override SimulatorCapabilities RequiredCapabilities
    {
        get => default;
    }

    public override sealed void Run(IInteractionProvider provider)
    {
        provider.Wait(this.duration);
    }

    public override string ToString()
    {
        return $"Pause – Duration: {this.duration}";
    }
}
