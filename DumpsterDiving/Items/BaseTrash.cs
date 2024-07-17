using PlayerCompanion;
using System;

namespace DumpsterDiving.Items;

/// <summary>
/// The base item for all of the Trash.
/// </summary>
public abstract class BaseTrash : StackableItem
{
    #region Constructor

    /// <summary>
    /// Creates a new Trash item.
    /// </summary>
    public BaseTrash()
    {
        Used += BaseTrash_Used;
    }

    #endregion

    #region Events

    private void BaseTrash_Used(object sender, EventArgs e)
    {
        // Just delete the item
        Remove();
    }

    #endregion
}
