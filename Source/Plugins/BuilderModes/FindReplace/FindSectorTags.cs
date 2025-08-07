
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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Sector Tag", BrowseButton = false)]
	internal class FindSectorTags : BaseFindSector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		//mxd. Prefixes usage
		public override string UsageHint
		{
			get
			{
				return "Supported prefixes:" + System.Environment.NewLine
				   + "\"<=\" for values less or equal to given value;" + System.Environment.NewLine
				   + "\">=\" for values greater or equal to given value;" + System.Environment.NewLine
				   + "\"<\" for values less than given value;" + System.Environment.NewLine
				   + "\">\" for values greater than given value." + System.Environment.NewLine
				   + "\"!\" or \"!=\" for values not equal to given value.";
			}
		}

		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replacetag = 0;
			if(replace)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replacetag)) replacewith = null;
				if(replacetag < General.Map.FormatInterface.MinTag) replacewith = null;
				if(replacetag > General.Map.FormatInterface.MaxTag) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Check for comparison operators
			string comparer = "";
			if (value[0].ToString() == ">" || value[0].ToString() == "<" || value[0].ToString() == "!")
			{
				comparer = value.Substring(0, 1);
				if (value.Length > 1)
					if (value[1].ToString() == "=")
						comparer = value.Substring(0, 2);
				value = value.Remove(0, comparer.Length);
			}

			if (value.Length == 0)
				return objs.ToArray();

			// Interpret the number given
			int tag;
			if(int.TryParse(value, out tag))
			{
				// Where to search?
				ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

				// Go for all sectors
				foreach(Sector s in list)
				{
					// Tag matches?
					int index = -1;
					for (int i = 0; i < s.Tags.Count; i++)
						if ((s.Tags[i] == tag && comparer.ToString() == "") ||
							(s.Tags[i] > tag && comparer.ToString() == ">") ||
							(s.Tags[i] < tag && comparer.ToString() == "<") ||
							(s.Tags[i] >= tag && comparer.ToString() == ">=") ||
							(s.Tags[i] <= tag && comparer.ToString() == "<=") ||
							(s.Tags[i] != tag && (comparer.ToString() == "!" || comparer.ToString() == "!=")))
							index = s.Tags[i];
					if (index != -1)
					{
						// Replace
						if(replace)
						{
							//mxd. Make a copy of tags, otherwise BeforePropsChange will be triggered after tag changes
							List<int> tags = new List<int>(s.Tags);
							tags[index] = replacetag;
							s.Tags = tags.Distinct().ToList(); // We don't want duplicates
						}

						// Add to list
						SectorEffectInfo info = General.Map.Config.GetSectorEffectInfo(s.Effect);
						if(!info.IsNull)
							objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (" + info.Title + ")"));
						else
							objs.Add(new FindReplaceObject(s, "Sector " + s.Index));
					}
				}
			}
			
			return objs.ToArray();
		}
		
		#endregion
	}
}
