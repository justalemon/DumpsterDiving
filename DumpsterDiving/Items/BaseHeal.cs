using GTA;
using PlayerCompanion;
using System;

namespace DumpsterDiving.Items
{
    /// <summary>
    /// Represents an item that can heal the player.
    /// </summary>
    public abstract class BaseHeal : StackableItem
    {
        #region Constructor

        /// <summary>
        /// Creates a new Healing item.
        /// </summary>
        public BaseHeal()
        {
            Used += BaseHeal_Used;
        }

        #endregion

        #region Events

        private void BaseHeal_Used(object sender, EventArgs e)
        {
            // Heal the player
            Game.Player.Character.HealthFloat = Game.Player.Character.MaxHealthFloat;
            // And reduce the count by one
            Count -= 1;
            // If is equal or lower than 0, discard the item
            if (Count <= 0)
            {
                Remove();
            }
        }

        #endregion
    }
}
