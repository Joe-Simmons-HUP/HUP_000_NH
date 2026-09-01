#region Using directives
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.OPCUAServer;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.UI;
using FTOptix.WebUI;
using System;
using System.Threading;
using UAManagedCore;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using OpcUa = UAManagedCore.OpcUa;
#endregion


    public class HelpSizeControl : BaseNetLogic
    {
        [ExportMethod]
        public void ChangeSize()
        {
            Thread.Sleep(200);
            int MaxScreenWidth;
            bool FullScreen;

        // Insert code to be executed when the user-defined logic is started
            Dialog dialog = InformationModel.Get<Dialog>(Owner.Owner.Owner.Owner.Owner.Owner.NodeId);
            Log.Info(Owner.Owner.Owner.Owner.Owner.Owner.BrowseName);
            Log.Info(dialog.BrowseName);

            Panel panel = InformationModel.Get<Panel>(Owner.Owner.Owner.Owner.Owner.NodeId);
            Log.Info(Owner.Owner.Owner.Owner.Owner.BrowseName);
            Log.Info(panel.BrowseName);

            if (panel.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                dialog.HorizontalAlignment = HorizontalAlignment.Center;
                dialog.VerticalAlignment = VerticalAlignment.Center;

                panel.HorizontalAlignment = HorizontalAlignment.Left;
                panel.Width = 750;
                panel.LeftMargin = 0;
                panel.VerticalAlignment = VerticalAlignment.Top;
                panel.Height = 450;
                panel.TopMargin = 0;
                FullScreen = false;
                Log.Info("SmallScreen");
            }
            else
            {
                dialog.HorizontalAlignment = HorizontalAlignment.Stretch;
                dialog.LeftMargin = 0;
                dialog.VerticalAlignment = VerticalAlignment.Stretch;
                dialog.TopMargin = 0;

                panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                panel.LeftMargin = 0;
                panel.VerticalAlignment = VerticalAlignment.Stretch;
                panel.TopMargin = 0;
                FullScreen = true;
                Log.Info("FullScreen");
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
