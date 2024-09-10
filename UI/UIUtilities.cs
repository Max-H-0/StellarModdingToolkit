using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StellarModdingToolkit.UI;

public static class UIUtilities
{
    public static void AddRange(this VisualElement visualElement, IEnumerable<VisualElement> children)
    {
        foreach (var child in children)
        {
            visualElement.Add(child);
        }
    }
}
