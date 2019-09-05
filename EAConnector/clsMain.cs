using System;
using System.Windows.Forms;
using Microsoft.CSharp;

using Aras.IOM;
using EAInnovator.Views;
using EAInnovator.Services;
using EAInnovator.Converters;

namespace EnterpriseArchitectInnovator
{
    /// <summary>
    /// The Main class loads the Sparx Systems Enterprise Architect (EA) Add-in.
    /// The menu entries are within the EA Specialize toolbar section
    /// https://sparxsystems.com.au/enterprise_architect_user_guide/14.0/automation/codeexamples.html
    /// </summary>
    public class Main
	{
		public String EA_Connect(EA.Repository Repository) 
		{
			// No special processing required
			return "";
		}

		public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName) 
		{
            /// <summary>
            /// Get EA add-in menu items
            /// <paramref name="Repository"/> Active EA repository
            /// <paramref name="Location"/> (not used)
            /// <paramref name="MenuName"/> Name of the EA menu
            /// </summary>
            switch (MenuName)
			{
				case "":
					return "-&Innovator";
				case "-&Innovator":
					string[] ar = { "Load &Data Model", "-", "Aras Login" };
					return ar;
			}
			return "";
		}

        private bool IsProjectOpen(EA.Repository Repository)
		{
            /// <summary>
            /// The isProjectOpen method verifies if an EA project file (eap) is loaded
            /// <paramref name="Repository"/> Active EA repository
            /// </summary>
            try
            {
				EA.Collection c = Repository.Models;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
		{
            /// <summary>
            /// Determine if the add-in menu shall be active
            /// <paramref name="Repository"/> Active EA repository
            /// <paramref name="Location"/> (not used)
            /// <paramref name="MenuName"/> Name of the EA menu
            /// <paramref name="ItemName"/> Menu item name
            /// <paramref name="IsEnabled"/> Menu item enabled/disabled
            /// <paramref name="IsChecked"/> (not used)
            /// </summary>
            if (IsProjectOpen(Repository))
            {
                // Enable additional menu entries when project is loaded and innovator connection exists
                if (ItemName != "Aras Login" && !InnovatorService._.IsLoggedIn)
                {
                    IsEnabled = false;
                }
                // Enable login to innovator menu entry when project is loaded
                else
                {
                    IsEnabled = true;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }

		public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
		{
            /// <summary>
            /// Menu click event
            /// <paramref name="Repository"/> Active EA repository
            /// <paramref name="Location"/> (not used)
            /// <paramref name="MenuName"/> (not used)
            /// <paramref name="ItemName"/> Menu item name
            /// </summary>
			switch (ItemName)
			{
                case "Load &Data Model":
                    Item dummyItem = null;
                    var converter = new InnovatorConverter(dummyItem);
                    converter.LoadDatamodel(Repository);
                    break;
                case "Aras Login":
                    LoginForm loginForm = new LoginForm();
                    loginForm.ShowDialog();
                    break;
            }
		}

		public void EA_Disconnect()  
		{
			GC.Collect();  
			GC.WaitForPendingFinalizers();   
		}

	}
}
