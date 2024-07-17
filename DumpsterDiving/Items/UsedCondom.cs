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
    public override ScaledTexture Icon => new ScaledTexture("", "");

    #endregion
}
