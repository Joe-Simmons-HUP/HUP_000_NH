using Cca.So.Optix;
using FTOptix.NetLogic;
using System;
using System.IO;
using System.Reflection;
using UAManagedCore;
using FTOptix.EdgeAppPlatform;
using FTOptix.WebUI;
using FTOptix.DataLogger;

public class SmartObjectsDesigntime : BaseNetLogic
{
    [ExportMethod]
    public void ReadModels()
    {
        try
        {
            SoAdsApp.ReadModels(Owner);
        }
        catch (Exception ex)
        {
            Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName, $"{MethodBase.GetCurrentMethod().Name}: {Owner.BrowseName} {ex.Message}");
        }
    }

    [ExportMethod]
    public void ImportSmartObjects()
    {
        try
        {
            SoAdsApp.ImportSmartObjects(Owner);
        }
        catch (Exception ex)
        {
            Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName, $"{MethodBase.GetCurrentMethod().Name}: {Owner.BrowseName} {ex.Message}");
        }
    }
}
