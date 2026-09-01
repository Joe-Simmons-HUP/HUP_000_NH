#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.OPCUAServer;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.Alarm;
using FTOptix.Recipe;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.TwinCAT;
using FTOptix.CommunicationDriver;
using FTOptix.WebUI;
#endregion

public class SplashPanelLogic : BaseNetLogic
{
    public override void Start()
    {
        var delayDuration = GetSplashPanelParameter("Delay");
        if (delayDuration == null)
            return;

        switchPanelTask = new DelayedTask(TryChangePanel, delayDuration.Value, LogicObject);
        switchPanelTask.Start();
    }

    private void TryChangePanel()
    {
        try
        {
            var panelLoader = Owner.Owner as PanelLoader;
            if (panelLoader == null)
            {
                Log.Error("SplashPanelLogic", "Missing PanelLoader, The SplashPanel must be set in a PanelLoader Panel property");
                return;
            }

            var nextPanelNode = GetSplashPanelParameter("NextPanel");
            if (nextPanelNode == null)
                return;

            var panelType = InformationModel.Get<ContainerType>(nextPanelNode.Value);
            if (panelType == null)
            {
                Log.Error("SplashPanelLogic", "Could not find Next panel");
                return;
            }

            panelLoader.ChangePanel(panelType);
        }
        catch (Exception e)
        {
            Log.Error("SplashPanelLogic", $"Failed changing panel {e.Message}");
            return;
        }
    }

    private IUAVariable GetSplashPanelParameter(string parameterName)
    {
        var parameter = Owner.GetVariable(parameterName);
        if (parameter == null)
        {
            Log.Error("SplashPanelLogic", $"Missing variable {parameterName}");
            return null;
        }
        if (parameter.Value == null)
        {
            Log.Error("SplashPanelLogic", $"Missing {parameterName} variable value");
            return null;
        }
        return parameter;
    }

    private DelayedTask switchPanelTask;
}
