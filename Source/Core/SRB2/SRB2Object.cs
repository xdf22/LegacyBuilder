
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.SRB2
{
	public struct SRB2Object
	{
        #region ================== Constants

        public const int MF_SOLID = 0x2;
        public const int MF_SPAWNCEILING = 0x100;

		#endregion
		
		#region ================== Variables
		
		public readonly string name;
        public readonly string sprite;
        public readonly string category;
        public readonly string[] states;
        public readonly int mapThingNum;
        public readonly int radius;
        public readonly int height;
        public readonly int flags;
        public readonly Dictionary<string, string> flagstext;
        public readonly string angletext;
        public readonly string parametertext;
        public readonly string flagsvaluetext;
        public readonly bool arrow;
        public readonly bool tagthing;

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        internal SRB2Object(string name, string sprite, string category, string[] states, int mapThingNum, int radius, int height, int flags,
            Dictionary<string, string> flagstext, string angletext, string parametertext, string flagsvaluetext, bool arrow, bool tagthing)
		{
            this.name = name;
            this.sprite = sprite;
            this.category = category;
            this.states = states;
            this.mapThingNum = mapThingNum;
            this.radius = radius;
            this.height = height;
            this.flags = flags;
            this.flagstext = flagstext;
            this.angletext = angletext;
            this.parametertext = parametertext;
            this.flagsvaluetext = flagsvaluetext;
            this.arrow = arrow;
            this.tagthing = tagthing;
		}

        #endregion

        #region ================== Methods
        internal bool Blocking()
        {
            return (flags & MF_SOLID) == MF_SOLID;
        }

        internal bool Hangs()
        {
            return (flags & MF_SPAWNCEILING) == MF_SPAWNCEILING;
        }
        #endregion
    }
}
