using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A standard Hot Dog.
/// </summary>
public class Hotdog : BaseHeal
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Dumpster Hot Dog";
    /// <inheritdoc/>
    public override string Description => "A brand new Hot Dog you found in a dumpster. Someone forgot to add some ketchup but that's it.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "hotdog");

    #endregion
}
