#region Using directives
using System;
using System.Threading;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.UI;
using FTOptix.WebUI;
using UAManagedCore;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.DataLogger;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class OEEDialog_btnNav_Logic : BaseNetLogic
{
    private CoreFunctions coreFunctions;

    public override void Start()
    {
        //UpdatePaths();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    private void UpdatePaths()
    {
        Thread.Sleep(50);
        string Path = Owner.GetVariable("FullPath").Value;
        NodeId PathNode = Project.Current.GetObject(Path).NodeId;
        Owner.Get<NodePointer>("Ref_Model").Value = PathNode;
        //Log.Info(Owner.Get<NodePointer>("Ref_Model").Value);
    }

    [ExportMethod]
    public void NavExplicit()
    {
        //UpdatePaths();

        //Thread.Sleep(100);
        DialogType commonDb = null;
        IUAObject lPanel = null;
        IUAObject launchAliasObj = null;

        try
        {
            // Get button object
            lPanel = InformationModel.Get<Panel>(Owner.NodeId);//Owner.Owner.GetObject(this.Owner.BrowseName);
            //Log.Info(lPanel.BrowseName);
            // Make Launch Object that will contain aliases
            launchAliasObj = InformationModel.MakeObject("LaunchAliasObj");
            //Log.Info("launchAliasObj Created");
        }
        catch
        {
            Log.Warning(this.GetType().Name, "Error getting owner object");
            return;
        }


        // Get each alias from Launch Button and add them into Launch Object, and assign NodeId values 
        foreach (var inpTag in lPanel.Children)
        {
            if (inpTag.BrowseName.Contains("Ref_"))  // & !inpTag.BrowseName.Contains("Ref_DialogBox") & (inpTag.GetType() == typeof(UAVariable)))
            {
                // Make a variable with same name as alias of type NodeId
                var newVar = InformationModel.MakeVariable(inpTag.BrowseName, OpcUa.DataTypes.NodeId);
                try
                {
                    // Assign alias value to new variable
                    newVar.Value = ((UAManagedCore.UAVariable)inpTag).Value;
                }
                catch
                {
                    //If no value is assigned to a Ref_ input, annunciate that it is missing a node assignment
                    Log.Warning(this.GetType().Name, "Missing node assignment to variable: " + inpTag.BrowseName);
                }

                // Add variable int launch object
                //Log.Info(newVar.Value.ToString());
                launchAliasObj.Add(newVar);
            }

            else if (inpTag.BrowseName.Contains("Cfg_DialogBox"))
            {
                try
                {
                    // Assign dialog box to launch
                    commonDb = (DialogType)InformationModel.Get(((UAVariable)inpTag).Value);
                    //Log.Info("Called Dialog: " + commonDb.BrowseName);
                }
                catch
                {
                    //If no or bad value is assigned to Cfg_DialogBox, annunciate that dialog box is not found
                    Log.Warning(this.GetType().Name, "Unable to find Node assigned to Cfg_DialogBox");
                }
            }
        }


        // Launch the faceplate
        try
        {
            // Launch DialogBox passing Launch Object that contains the aliases as an alias 
            UICommands.OpenDialog(lPanel, commonDb, launchAliasObj.NodeId);
        }
        catch
        {
            Log.Warning(this.GetType().Name, "Failed to launch dialog box specified by Cfg_DialogBox '" + commonDb.BrowseName + "'");
            return;
        }



        // If configured, close the dialog box containing launch button
        //try
        //{
        //    bool cfgCloseCurrent = lPanel.GetVariable("Cfg_CloseCurrentDisplay").Value;
        //    if (cfgCloseCurrent)
        //    {
        //        CloseCurrentDB(Owner);
        //    }
        //}
        //catch
        //{
        //    Log.Warning(this.GetType().Name, "Failed to close current dialog box");
        //}
    }
    public void CloseCurrentDB(IUANode inputNode)
    {
        // if input node is of type Dialog, close it
        if (inputNode.GetType().BaseType.BaseType == typeof(Dialog))
        {
            // close dialog box
            ((Dialog)inputNode).Close();
            return;
        }
        // if input node is Main Window, no dialog box was found, return
        if (inputNode.GetType() == typeof(MainWindow))
        {
            return;
        }
        // continue search for Dialog or Main Window
        CloseCurrentDB(inputNode.Owner);
    }
}
