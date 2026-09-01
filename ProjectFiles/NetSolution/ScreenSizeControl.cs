#region Using directives
using System;
using System.Threading;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.CoreBase;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.CommunicationDriver;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.DataLogger;
#endregion

public class ScreenSizeControl : BaseNetLogic
{
    [ExportMethod]
    public void ChangeSize()
    {
        int MaxScreenWidth;
        bool FullScreen;

        // Insert code to be executed when the user-defined logic is started
        Dialog dialog = InformationModel.Get<Dialog>(Owner.Owner.Owner.Owner.Owner.Owner.NodeId);
        Log.Info(Owner.Owner.Owner.Owner.Owner.Owner.BrowseName);
        Log.Info(dialog.BrowseName);
        if (dialog.HorizontalAlignment == HorizontalAlignment.Stretch)
        {
            dialog.HorizontalAlignment = HorizontalAlignment.Center;
            dialog.VerticalAlignment = VerticalAlignment.Center;
            FullScreen = false;
        }
        else
        {
            dialog.HorizontalAlignment = HorizontalAlignment.Stretch;
            dialog.LeftMargin = 0;
            dialog.VerticalAlignment = VerticalAlignment.Stretch;
            dialog.TopMargin = 0;
            FullScreen = true;
        }
                
        MaxScreenWidth = Project.Current.GetVariable("UI/MainWindow/Width").Value;

        LogicObject.GetVariable("_MaxScreenWidth").Value = MaxScreenWidth;
        LogicObject.GetVariable("_FullScreen").Value = FullScreen;

    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}
