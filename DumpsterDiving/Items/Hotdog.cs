using LemonUI.Elements;

namespace DumpsterDiving.Items;

/// <summary>
/// A standard Hot Dog.
/// </summary>
public class Hotdog : BaseHeal
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Hotdog";
    /// <inheritdoc/>
    public override ScaledTexture Icon => new ScaledTexture("", "");

    #endregion
}
