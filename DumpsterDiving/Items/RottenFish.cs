using LemonUI.Elements;

namespace DumpsterDiving.Items
{
    /// <summary>
    /// A Rotten Fish.
    /// </summary>
    /// <remarks>
    /// We specify that the fish is rotten because other mods might add useable fishes.
    /// </remarks>
    public class RottenFish : BaseTrash
    {
        #region Properties

        /// <inheritdoc/>
        public override string Name => "Rotten Fish";
        /// <inheritdoc/>
        public override ScaledTexture Icon => new ScaledTexture("", "");

        #endregion
    }
}
