﻿using TTMouseclickSimulator.Core.Actions;
using TTMouseclickSimulator.Core.Environment;

namespace TTMouseclickSimulator.Core.ToontownRewritten.Actions.Fishing;

public class SellFishAction : AbstractAction
{
    public SellFishAction()
    {
    }

    public override SimulatorCapabilities RequiredCapabilities
    {
        get => SimulatorCapabilities.MouseInput;
    }

    public override sealed void Run(IInteractionProvider provider)
    {
        var c = new Coordinates(1159, 911);
        MouseHelpers.DoSimpleMouseClick(provider, c);
    }

    public override string ToString()
    {
        return "Sell Fish";
    }
}
