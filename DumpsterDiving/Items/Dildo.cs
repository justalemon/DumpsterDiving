using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A Dildo.
/// </summary>
public class Dildo : BaseTrash
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Used Dildo";
    /// <inheritdoc/>
    public override string Description => "A used Dildo you found while looting a dumpster. Still does the same thing from 1992.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "dildo");

    #endregion
}
