using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A Moldy Burger.
/// </summary>
public class MoldyBurger : BaseTrash
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Moldy Burger";
    /// <inheritdoc/>
    public override string Description => "A very Moldy Burger you found in a dumpster. The bread is green and the patty is white.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "moldy_burger");

    #endregion
}
