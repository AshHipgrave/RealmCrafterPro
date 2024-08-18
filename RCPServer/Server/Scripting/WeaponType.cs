using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Weapon Type Enumeration for an item instance.
    /// </summary>
    public enum WeaponType
    {
        /// <summary>
        /// The ItemInstance isn't a weapon.
        /// </summary>
        NotAWeapon = 0,

        /// <summary>
        /// Weapon is one handed.
        /// </summary>
        OneHanded = 1,

        /// <summary>
        /// Weapon is two handed.
        /// </summary>
        TwoHanded = 2,

        /// <summary>
        /// Weapon is ranged (projectile based).
        /// </summary>
        Ranged = 3
    }
}
