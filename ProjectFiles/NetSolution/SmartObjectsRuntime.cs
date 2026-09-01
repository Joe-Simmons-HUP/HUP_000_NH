using Cca.So.Optix;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using System;
using System.Reflection;
using UAManagedCore;
using FTOptix.EdgeAppPlatform;
using FTOptix.WebUI;
using FTOptix.DataLogger;

public class SmartObjectsRuntime : BaseNetLogic
{
#nullable enable

    public override void Start()
    {
        Project.Current.OnProjectLoaded += Current_OnProjectLoaded;
    }

    public override void Stop()
    {
        try
        {
            if (runtimeBase is { })
            {
                runtimeBase.Stop();
                runtimeBase?.Dispose();
                runtimeBase = null;

                Project.Current.OnProjectLoaded -= Current_OnProjectLoaded;
            }
        }
        catch (Exception ex)
        {
            Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName, $"{MethodBase.GetCurrentMethod().Name}: {ex.Message}");
        }
    }

    private void Current_OnProjectLoaded(object? sender, ProjectLoadedEvent e)
    {
        runtimeBase = new SoAdsApp(Owner.Parent, LogicObject.Context.Sessions.CurrentSessionHandler);
    }

    private SoAdsApp? runtimeBase;

#nullable restore
}
