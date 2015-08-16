// (c) Copyright Cory Plotts.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using Snoop.Infrastructure;

namespace Snoop
{
	public static class SnoopWindowUtils
	{
		public static Window FindOwnerWindow()
		{
			Window ownerWindow = null;
		    PropertyInfo propInfo = null;

            if (SnoopModes.MultipleDispatcherMode)
            {
                foreach (PresentationSource presentationSource in PresentationSource.CurrentSources)
                {
                    if
                    (
                        presentationSource.RootVisual is Window &&
                        ((Window)presentationSource.RootVisual).Dispatcher.CheckAccess() &&
                        ((Window)presentationSource.RootVisual).Visibility == Visibility.Visible
                    )
                    {
                        ownerWindow = (Window)presentationSource.RootVisual;
                        break;
                    }
                }
            }
            else if (Application.Current != null && (propInfo = Application.Current.GetType().GetProperty("MainCore", BindingFlags.Static | BindingFlags.NonPublic)) != null)
            {
                var mainCore = propInfo.GetValue( Application.Current );
                var mainWin = mainCore.GetType().GetProperty( "ApplicationMainWindow" ).GetValue( mainCore ) as Window;
                if (mainWin != null)
                {
                    //MessageBox.Show("found main window");
                    ownerWindow = mainWin;
                }
            }
            //          var app = Application.Current;
            //      PropertyInfo propInfo = app.GetType().GetProperty( "MainCore", BindingFlags.Static | BindingFlags.NonPublic);

            //     var mainCore = propInfo.GetValue(app);

            //var mainWin = mainCore.GetType().GetProperty( "ApplicationMainWindow" ).GetValue( mainCore ) as Window;
            //if( mainWin != null )
            //{
            //    MessageBox.Show( "found main window" );
            //}
            //ownerWindow = mainWin;

            else if (Application.Current != null)
            {
                if (Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility == Visibility.Visible)
                {
                    // first: set the owner window as the current application's main window, if visible.
                    ownerWindow = Application.Current.MainWindow;
                }
                else
                {
                    // second: try and find a visible window in the list of the current application's windows
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Visibility == Visibility.Visible)
                        {
                            ownerWindow = window;
                            break;
                        }
                    }
                }
            }

            if (ownerWindow == null)
            {
                // third: try and find a visible window in the list of current presentation sources
                foreach (PresentationSource presentationSource in PresentationSource.CurrentSources)
                {
                    if
                    (
                        presentationSource.RootVisual is Window &&
                        ((Window)presentationSource.RootVisual).Visibility == Visibility.Visible
                    )
                    {
                        ownerWindow = (Window)presentationSource.RootVisual;
                        break;
                    }
                }
            }

			return ownerWindow;
		}
	}
}
