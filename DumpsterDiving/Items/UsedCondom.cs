using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// Used Condom.
/// </summary>
public class UsedCondom : BaseTrash
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Used Condom";
    /// <inheritdoc/>
    public override string Description => "A used condom you found on a dumpster. I don't know why someone would carry this around in their pockets.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "used_condom");

    #endregion
}
