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
	/// Summary description for Class1.
	/// </summary>
	public class Main
	{
		public String EA_Connect(EA.Repository Repository) 
		{
			// No special processing required
			return "";
		}

		public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
		{
			MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
		}

		public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName) 
		{
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

		bool IsProjectOpen(EA.Repository Repository)
		{
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
            if (IsProjectOpen(Repository))
            {
                // Enable open/save menu entries when project is loaded and innovator connection exists
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
			switch( ItemName )
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
