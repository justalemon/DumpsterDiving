using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A Moldy Hotdog.
/// </summary>
public class MoldyHotdog : BaseTrash
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Moldy Hotdog";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("lemon_dumpsterdiving", "moldy_hotdog");

    #endregion
}
