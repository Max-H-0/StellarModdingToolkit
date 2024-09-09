using System;

namespace StellarModdingToolkit.UI;

internal partial class ResizeableElement
{
    [Flags]
    public enum ResizeMode : byte
    {
        None        = 0b_0000_0000,

        Left        = 0b_0000_1000,
        Right       = 0b_0000_0100,
        Top         = 0b_0000_0010,
        Bottom      = 0b_0000_0001,

        TopLeft     = 0b_0000_1010,
        TopRight    = 0b_0000_0110,
        BottomLeft  = 0b_0000_1001,
        BottomRight = 0b_0000_0101,
        
        Move        = 0b_0001_0000,
    }
}