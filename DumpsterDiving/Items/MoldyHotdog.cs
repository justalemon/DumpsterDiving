using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A Moldy Hotdog.
/// </summary>
public class MoldyHotdog : BaseTrash
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Moldy Hot Dog";
    /// <inheritdoc/>
    public override string Description => "A very Moldy Hot Dog you found in a dumpster. The white part is not mayo.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "moldy_hotdog");

    #endregion
}
