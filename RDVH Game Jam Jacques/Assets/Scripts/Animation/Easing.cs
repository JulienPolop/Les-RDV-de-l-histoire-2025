using System;

public static class Easing
{
    // Linear
    public static readonly Func<float, float> Linear = x => x;

    // Quadratic
    public static readonly Func<float, float> EaseInQuad = x => x * x;
    public static readonly Func<float, float> EaseOutQuad = x => x * (2 - x);
    public static readonly Func<float, float> EaseInOutQuad = x => x < 0.5f ? 2 * x * x : -1 + (4 - 2 * x) * x;

    // Cubic
    public static readonly Func<float, float> EaseInCubic = x => x * x * x;
    public static readonly Func<float, float> EaseOutCubic = x => 1 + (--x) * x * x;
    public static readonly Func<float, float> EaseInOutCubic = x => x < 0.5f ? 4 * x * x * x : (x - 1) * (2 * x - 2) * (2 * x - 2) + 1;

    // Quartic
    public static readonly Func<float, float> EaseInQuart = x => x * x * x * x;
    public static readonly Func<float, float> EaseOutQuart = x => 1 - (--x) * x * x * x;
    public static readonly Func<float, float> EaseInOutQuart = x => x < 0.5f ? 8 * x * x * x * x : 1 - 8 * (--x) * x * x * x;

    // Quintic
    public static readonly Func<float, float> EaseInQuint = x => x * x * x * x * x;
    public static readonly Func<float, float> EaseOutQuint = x => 1 + (--x) * x * x * x * x;
    public static readonly Func<float, float> EaseInOutQuint = x => x < 0.5f ? 16 * x * x * x * x * x : 1 + 16 * (--x) * x * x * x * x;

    // --- Elastic ---
    public static readonly Func<float, float> EaseInElastic = x =>
    {
        if (x == 0 || x == 1) return x;
        return -(float)Math.Pow(2, 10 * (x - 1)) * (float)Math.Sin((x - 1.1f) * 5 * Math.PI);
    };

    public static readonly Func<float, float> EaseOutElastic = x =>
    {
        if (x == 0 || x == 1) return x;
        return (float)Math.Pow(2, -10 * x) * (float)Math.Sin((x - 0.1f) * 5 * Math.PI) + 1;
    };

    public static readonly Func<float, float> EaseInOutElastic = x =>
    {
        if (x == 0 || x == 1) return x;
        x *= 2;
        if (x < 1)
            return -0.5f * (float)Math.Pow(2, 10 * (x - 1)) * (float)Math.Sin((x - 1.1f) * 5 * Math.PI);
        return (float)Math.Pow(2, -10 * (x - 1)) * (float)Math.Sin((x - 1.1f) * 5 * Math.PI) * 0.5f + 1;
    };

    // --- Back ---
    public static readonly Func<float, float> EaseInBack = x =>
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        return c3 * x * x * x - c1 * x * x;
    };

    public static readonly Func<float, float> EaseOutBack = x =>
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        return 1 + c3 * (float)Math.Pow(x - 1, 3) + c1 * (float)Math.Pow(x - 1, 2);
    };

    public static readonly Func<float, float> EaseInOutBack = x =>
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        return x < 0.5f
            ? (float)(Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
            : (float)(Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    };

    // --- Bounce ---
    public static readonly Func<float, float> EaseOutBounce = x =>
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1 / d1) return n1 * x * x;
        else if (x < 2 / d1) return n1 * (x -= 1.5f / d1) * x + 0.75f;
        else if (x < 2.5f / d1) return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        else return n1 * (x -= 2.625f / d1) * x + 0.984375f;
    };

    public static readonly Func<float, float> EaseInBounce = x =>
        1 - EaseOutBounce(1 - x);

    public static readonly Func<float, float> EaseInOutBounce = x =>
        x < 0.5f
            ? (1 - EaseOutBounce(1 - 2 * x)) * 0.5f
            : (1 + EaseOutBounce(2 * x - 1)) * 0.5f;
}
