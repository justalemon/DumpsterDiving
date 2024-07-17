using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A standard Hamburger.
/// </summary>
public class Burger : BaseHeal
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Dumpster Burger";
    /// <inheritdoc/>
    public override string Description => "A brand new Burger you found in a dumpster. A rich kid from Vinewood probably threw it away because of the gluten.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "burger");

    #endregion
}
