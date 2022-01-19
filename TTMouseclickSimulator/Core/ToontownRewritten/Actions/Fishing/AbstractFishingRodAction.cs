﻿using System;
using System.Diagnostics;

using TTMouseclickSimulator.Core.Actions;
using TTMouseclickSimulator.Core.Environment;

namespace TTMouseclickSimulator.Core.ToontownRewritten.Actions.Fishing;

public abstract class AbstractFishingRodAction : AbstractAction
{
    /// <summary>
    /// Coordinates to use when we check for a dialog that indicates that a fish
    /// has been caught.
    /// Those coordinates are adapted from the old tt mouse click simulator.
    /// </summary>
    private static readonly Coordinates[] fishResultDialogCoordinates =
    {
        new Coordinates(1023, 562),
        new Coordinates(634, 504),
        new Coordinates(564, 100)
    };

    /// <summary>
    /// Coordinates to use when we check for an error dialog. All of these coordinates must have the
    /// fish dialog color.
    /// </summary>
    private static readonly Coordinates[] fishErrorDialogCoordinates =
    {
        new Coordinates(530, 766),
        new Coordinates(530, 490),
        new Coordinates(896, 690)
    };

    private static readonly ScreenshotColor fishDialogColor = new(255, 255, 191);

    public AbstractFishingRodAction()
    {
    }

    public override SimulatorCapabilities RequiredCapabilities
    {
        get => SimulatorCapabilities.MouseInput | SimulatorCapabilities.CaptureScreenshot;
    }

    // This is determined by the class type, not by the instance so implement it
    // as abstract property instead of a field. This avoids it being serialized.
    /// <summary>
    /// The timeout value that should be used when waiting for the fish
    /// result dialog after finishing casting the rod.
    /// </summary>
    protected abstract int WaitingForFishResultDialogTime { get; }

    public override sealed void Run(IInteractionProvider provider)
    {
        // Cast the fishing rod
        this.OnActionInformationUpdated("Casting…");

        this.StartCastFishingRod(provider);
        this.FinishCastFishingRod(provider);

        // Then, wait until we find a window displaying the caught fish
        // or the specified number of seconds has passed.
        this.OnActionInformationUpdated("Waiting for the fish result dialog…");

        var sw = new Stopwatch();
        sw.Start();

        bool found = false;
        while (!found && sw.ElapsedMilliseconds <= this.WaitingForFishResultDialogTime)
        {
            provider.Wait(500);

            // Get a current screenshot.
            var screenshot = provider.GetCurrentWindowScreenshot();

            foreach (var c in fishResultDialogCoordinates)
            {
                var cc = screenshot.WindowPosition.ScaleCoordinates(
                    c, MouseHelpers.ReferenceWindowSize);
                var col = screenshot.GetPixel(cc);

                if (this.CompareColor(fishDialogColor, col, 10))
                {
                    // OK, we caught a fish, so break from the loop.
                    found = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Clicks on the fishing rod button.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    protected void StartCastFishingRod(IInteractionProvider provider)
    {
        var coords = new Coordinates(800, 846);
        var pos = provider.GetCurrentWindowPosition();

        coords = pos.ScaleCoordinates(
            coords,
            MouseHelpers.ReferenceWindowSize);

        // Move the mouse and press the button.
        provider.MoveMouse(coords);
        provider.PressMouseButton();

        provider.Wait(300);

        this.CheckForFishErrorDialog(provider);
    }

    protected bool CompareColor(
        ScreenshotColor refColor,
        ScreenshotColor actualColor,
        Tolerance tolerance)
    {
        // Simply compare the discrepancy of the R, G and B values
        // of each color.
        for (int i = 0; i < 3; i++)
        {
            byte bRef = refColor.GetValueFromIndex(i);
            byte bAct = actualColor.GetValueFromIndex(i);

            if (!(Math.Abs(bRef - bAct) <= tolerance.GetValueFromIndex(i)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Detects a fish bubble and then casts the fishing rod by moving the mouse to the
    /// desired position and releaseing the mouse button.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    protected abstract void FinishCastFishingRod(IInteractionProvider provider);

    private void CheckForFishErrorDialog(IInteractionProvider provider)
    {
        // Check if a dialog appeared, which means we don't have any more jelly beans or
        // the fish bucket is full.
        var screenshot = provider.GetCurrentWindowScreenshot();
        bool foundDialog = true;

        foreach (var point in fishErrorDialogCoordinates)
        {
            if (!this.CompareColor(fishDialogColor, screenshot.GetPixel(
                screenshot.WindowPosition.ScaleCoordinates(point, MouseHelpers.ReferenceWindowSize)), 4))
            {
                foundDialog = false;
                break;
            }
        }

        if (foundDialog)
        {
            throw new InvalidOperationException(
                "Either your fish bucket is full or you don't have any more jellybeans for bait.");
        }
    }

    public struct Tolerance
    {
        public byte ToleranceR { get; }
        public byte ToleranceG { get; }
        public byte ToleranceB { get; }

        public Tolerance(byte toleranceR, byte toleranceG, byte toleranceB)
        {
            this.ToleranceR = toleranceR;
            this.ToleranceG = toleranceG;
            this.ToleranceB = toleranceB;
        }

        public Tolerance(byte tolerance)
            : this(tolerance, tolerance, tolerance)
        {
        }

        public byte GetValueFromIndex(int index)
        {
            return index switch
            {
                0 => this.ToleranceR,
                1 => this.ToleranceG,
                2 => this.ToleranceB,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
        }

        public static implicit operator Tolerance(byte value) => new(value);
    }
}
