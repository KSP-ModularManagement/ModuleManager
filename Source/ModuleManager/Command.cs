using System;

namespace ModuleManager
{
    public enum Command
    {
        Insert,

        Delete,         // - or !

        Edit,           // @

        Replace,        // %

        Copy,           // + or $

        Rename,         // |

        Paste,          // #

        Special,        // *

        Create          // &
    }
}
