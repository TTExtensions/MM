﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTMouseclickSimulator.Core.Actions;
using TTMouseclickSimulator.Core.Environment;

namespace TTMouseclickSimulator.Core.ToontownRewritten.Actions.Keyboard
{
    /// <summary>
    /// An action for pressing a key for a specific amount of time.
    /// </summary>
    public class PressKeyAction : IAction
    {

        private readonly AbstractWindowsEnvironment.VirtualKeyShort keyCode;
        private readonly int timeout;

        public PressKeyAction(AbstractWindowsEnvironment.VirtualKeyShort keyCode, int timeout)
        {
            this.keyCode = keyCode;
            this.timeout = timeout;
        }


        public async Task RunAsync(IInteractionProvider provider)
        {
            provider.PressKey(keyCode);
            await provider.WaitAsync(timeout);
            provider.ReleaseKey(keyCode);
        }
    }
}
