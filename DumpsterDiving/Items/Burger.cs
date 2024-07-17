using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A standard Hamburger.
/// </summary>
public class Burger : BaseHeal
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Burger";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "burger");

    #endregion
}
