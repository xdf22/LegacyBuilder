
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
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.IO
{
    internal class SRB2MapSetIO : DoomMapSetIO
    {
        #region ================== Constants
        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        public SRB2MapSetIO(WAD wad, MapManager manager) : base(wad, manager)
        {
            translucentLineTypes = new Dictionary<int, float>() {
                { 900, 0.9f }, // translucent
                { 901, 0.8f },
                { 902, 0.7f },
                { 903, 0.6f },
                { 904, 0.5f },
                { 905, 0.4f },
                { 906, 0.3f },
                { 907, 0.2f },
                { 908, 0.1f },
                { 910, 1.0f }, // additive
                { 911, 0.9f },
                { 912, 0.8f },
                { 913, 0.7f },
                { 914, 0.6f },
                { 915, 0.5f },
                { 916, 0.4f },
                { 917, 0.3f },
                { 918, 0.2f },
                { 919, 0.1f },
                { 920, 1.0f }, // subtractive
                { 921, 0.9f },
                { 922, 0.8f },
                { 923, 0.7f },
                { 924, 0.6f },
                { 925, 0.5f },
                { 926, 0.4f },
                { 927, 0.3f },
                { 928, 0.2f },
                { 929, 0.1f },
                { 930, 1.0f }, // reverse subtractive
                { 931, 0.9f },
                { 932, 0.8f },
                { 933, 0.7f },
                { 934, 0.6f },
                { 935, 0.5f },
                { 936, 0.4f },
                { 937, 0.3f },
                { 938, 0.2f },
                { 939, 0.1f },
                { 940, 0.5f }  // modulate, placeholder value
            };

            startTypes = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 };
        }

        #endregion

        #region ================== Properties
        public override bool HasThingHeight { get { return true; } }
        public override bool HasLinedefParameters { get { return false; } }
        public override bool HasTranslucent3DFloors { get { return true; } }
        public override int SlopeVertexType { get { return 750; } }
        public override int MaxThingHeight { get { return 4095; } }
        public override int MinThingHeight { get { return 0; } }
        public override int ColormapType { get { return 606; } }
        public override int FlatAlignmentType { get { return 7; } }
        public override int HorizonLineType { get { return 41; } }
        public override int AxisType { get { return 1700; } }
        public override int AxisTransferType { get { return 1701; } }
        public override int AxisTransferLineType { get { return 1702; } }
        public override int WaypointType { get { return 753; } }
        #endregion

        #region ================== Reading

        // This reads the THINGS from WAD file
        protected override void ReadThings(MapSet map, int firstindex)
        {
            // Get the lump from wad file
            Lump lump = wad.FindLump("THINGS", firstindex);
            if (lump == null) throw new Exception("Could not find required lump THINGS!");

            // Prepare to read the items
            MemoryStream mem = new MemoryStream(lump.Stream.ReadAllBytes());
            int num = (int)lump.Stream.Length / 10;
            BinaryReader reader = new BinaryReader(mem);

            // Read items from the lump
            map.SetCapacity(0, 0, 0, 0, map.Things.Count + num);
            for (int i = 0; i < num; i++)
            {
                // Read properties from stream
                int x = reader.ReadInt16();
                int y = reader.ReadInt16();
                int angle = reader.ReadInt16();
                int type = reader.ReadUInt16();
                int flags = reader.ReadUInt16();

                // Make string flags
                Dictionary<string, bool> stringflags = new Dictionary<string, bool>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, string> f in manager.Config.ThingFlags)
                {
                    int fnum;
                    if (int.TryParse(f.Key, out fnum)) stringflags[f.Key] = ((flags & fnum) == fnum);
                }

                // MascaraSnake: SRB2 stores Z position in upper 12 bits of flags. Read Z position and remove it from flags.
                int z = flags >> 4;
                flags &= 0xF;

                // Create new item
                Thing t = map.CreateThing();
                t.Update(type, x, y, z, angle, 0, 0, 1.0f, 1.0f, stringflags, 0, 0, new int[Thing.NUM_ARGS]);
            }

            // Done
            mem.Dispose();
        }
        #endregion
    }
}