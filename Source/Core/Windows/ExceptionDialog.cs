#region ================== Namespaces

using System;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ExceptionDialog : Form
	{
		private readonly bool cannotContinue;
		private readonly string logPath;
		
		public ExceptionDialog(UnhandledExceptionEventArgs e) 
		{
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			Exception ex = (Exception)e.ExceptionObject;
			errorDescription.Text = "Error in " + ex.Source + ":";
            string sysinfo = GetSystemInfo();
            using (StreamWriter sw = File.CreateText(logPath)) 
			{
                sw.Write(sysinfo + GetExceptionDescription(ex));
            }

			errorMessage.Text = ex.Message + Environment.NewLine + ex.StackTrace;
			cannotContinue = true;  //cannot recover from this...
		}

		public ExceptionDialog(ThreadExceptionEventArgs e) 
		{
			InitializeComponent();

			logPath = Path.Combine(General.SettingsPath, @"GZCrash.txt");
			errorDescription.Text = "Error in " + e.Exception.Source + ":";
			string sysinfo = GetSystemInfo();
			using(StreamWriter sw = File.CreateText(logPath)) 
			{
				sw.Write(sysinfo + GetExceptionDescription(e.Exception));
			}

			errorMessage.Text = sysinfo + "********EXCEPTION DETAILS********" + Environment.NewLine 
				+ e.Exception.Message + Environment.NewLine + e.Exception.StackTrace;
		}

		public void Setup() 
		{
			bContinue.Enabled = !cannotContinue;

			string[] titles = {
								  "Legacy Builder was killed by Eggman's nefarious TV magic.",
                                  "Legacy Builder was killed by an environmental hazard.",
                                  "Legacy Builder drowned.",
                                  "Legacy Builder was crushed.",
                                  "Legacy Builder fell into a bottomless pit.",
                                  "Legacy Builder asphyxiated in space.",
                                  "Legacy Builder died.",
                                  "Legacy Builder's playtime with heavy objects killed Legacy Builder.",
								  "Resynching...",
								  "You have been banned from the server.",
								  "SIGSEGV - segment violation",
								  "[Eggman laughing]",
								  "[Armageddon pow]",
								  "[Dying]",
								  "I'm outta here...",
								  "GAME OVER",
								  "SONIC MADE A BAD FUTURE IN Legacy Builder",
								  "Sonic arrived just in time to see what little of the 'ruins' were left.",
								  "The natural beauty of the zone had been obliterated.",
								  "I'm putting my foot down.",
								  "All of this is over. You will be left with ashes.",
								  "some doofus gave us an empty string?",
								  "unfortunate player falls into spike?!",
								  "Ack! Metal Sonic shouldn't die! Cut the tape, end recording!",
								  "ALL YOUR RINGS ARE BELONG TO US!",
								  "Hohohoho!! *B^D",
								  "So that's it. I was so busy playing SRB2 I never noticed... but... everything's gone...",
								  "Tails! You made the engines quit!",
								  
								  // Some less well known SSN/SRB2 quotes
								  "No! This can't be true!",
								  "You little bugger!",
								  "And there were the times when I despised you...",
								  "*SLAM!*",
								  "The flowers green, neither are the flowers",
								  "Which jerk wrote the script?",
								  "Our powers are useless against this...", // APL script
								  "YOU! You git!",
								  "Uh, didn't the world get destroyed?",
								  "Phew... it was just a dream..."
							  };
			this.Text = titles[new Random().Next(0, titles.Length - 1)];
		}

		private static string GetSystemInfo()
		{
			string result = "***********SYSTEM INFO***********" + Environment.NewLine;
			
			// Get OS name
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
			foreach(ManagementBaseObject mo in searcher.Get())
			{
				result += "OS: " + mo["Caption"] + Environment.NewLine;
				break;
			}

			// Get GPU name
			searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
			foreach(ManagementBaseObject mo in searcher.Get())
			{
				PropertyData bpp = mo.Properties["CurrentBitsPerPixel"];
				PropertyData description = mo.Properties["Description"];
				if(bpp != null && description != null && bpp.Value != null)
				{
					result += "GPU: " + description.Value + Environment.NewLine;
					break;
				}
			}

			// Get Zone Builder version
			result += "Legacy Builder: v" + General.ThisAssembly.GetName().Version.Major + "." + General.ThisAssembly.GetName().Version.Minor + Environment.NewLine + Environment.NewLine;

			return result;
		}

		private static string GetExceptionDescription(Exception ex) 
		{
            // Add to error logger
            General.WriteLogLine("***********************************************************");
            General.ErrorLogger.Add(ErrorType.Error, ex.Source + ": " + ex.Message);
            General.WriteLogLine("***********************************************************");

            string message = "********EXCEPTION DETAILS********"
							 + Environment.NewLine + ex.Source + ": " + ex.Message + Environment.NewLine + ex.StackTrace;

			if(File.Exists(General.LogFile)) 
			{
				try 
				{
					string[] lines = File.ReadAllLines(General.LogFile);
					message += Environment.NewLine + Environment.NewLine + "***********ACTIONS LOG***********";
					for(int i = lines.Length - 1; i > -1; i--) 
						message += Environment.NewLine + lines[i];
				} catch(Exception) { }
			}

			return message;
		}

		private void reportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) 
		{
			if(!File.Exists(logPath)) return;
			System.Diagnostics.Process.Start("explorer.exe", @"/select, " + logPath);
			reportLink.LinkVisited = true;
		}

		private void threadLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) 
		{
			try 
			{
				System.Diagnostics.Process.Start("https://git.do.srb2.org/STJr/ZoneBuilder/issues");
			} 
			catch(Exception) 
			{
				MessageBox.Show("Unable to open URL...");
			}
			
			threadLink.LinkVisited = true;
		}

		private void bContinue_Click(object sender, EventArgs e) 
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void bQuit_Click(object sender, EventArgs e)
		{
			if(General.Map != null) General.Map.SaveMapBackup();
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void bToClipboard_Click(object sender, EventArgs e) 
		{
			errorMessage.SelectAll();
			errorMessage.Copy();
			errorMessage.DeselectAll();
		}
	}
}
