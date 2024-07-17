using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A Boot.
/// </summary>
public class Boot : BaseTrash
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Boot";
    /// <inheritdoc/>
    public override string Description => "A lightly used Boot you found in a dumpster. Someone might have the other one.";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "boot");

    #endregion
}
