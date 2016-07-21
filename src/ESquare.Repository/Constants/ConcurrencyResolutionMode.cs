using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESquare.Repository.Constants
{
    public enum ConcurrencyResolutionMode
    {
        /// <summary>
        /// Throws exception
        /// </summary>
        None,

        /// <summary>
        /// Uses database values
        /// </summary>
        DatabaseWin,

        /// <summary>
        /// Uses client values
        /// </summary>
        ClientWin
    }
}
