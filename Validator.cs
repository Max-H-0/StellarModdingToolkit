using System;

namespace StellarModdingToolkit;

public static class Validator
{
    public static T Expect<T>(this T? nullable)
    {
        if (nullable is not T value) throw new ArgumentNullException(nameof(nullable));

        return value;
    }

    public static T Expect<T>(this T? nullable, string error)
    {
        if (nullable is not T value) throw new ArgumentNullException(nameof(nullable), error);

        return value;
    }
}
