#region Using directives
using System;
using System.Threading;
using System.Collections.Generic;
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
using FTOptix.OPCUAServer;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class LanguageUpdate : BaseNetLogic
{
    public override void Start()
    {
        RefreshLanguages();
    }

    public override void Stop()
    {
        
    }

    [ExportMethod]
    public void RefreshLanguages() 
    {
        Thread.Sleep(250);
        CoreFunctions CoreFunctions;
        CoreFunctions = new CoreFunctions();

        string[] OEEModeModels;
        string[] OEEStateModels;
        string[] OEEReasonCodeModels;
        string[] OEECategoryModels;
        string[] MESeverityModels;
        string[] MEDescriptionModels;
        string[] MECategoryModels;

        string Locale;
        List<string> locationID;
        Locale = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/LanguageString").Value;//Owner.GetVariable("Locale").Value;
        locationID = new List<string>();
        locationID.Add(Locale);

        int Index = LogicObject.NodeId.NamespaceIndex;
        CoreFunctions.UpdateLookupList(locationID, Index, "OEEMode", out OEEModeModels);
        CoreFunctions.UpdateLookupList(locationID, Index, "OEEState", out OEEStateModels);
        CoreFunctions.UpdateLookupList(locationID, Index, "OEEReasonCode", out OEEReasonCodeModels);
        CoreFunctions.UpdateLookupList(locationID, Index, "OEECategory", out OEECategoryModels);
        CoreFunctions.UpdateLookupList(locationID, Index, "MESeverity", out MESeverityModels);
        CoreFunctions.UpdateLookupList(locationID, Index, "MEDescription", out MEDescriptionModels);
        CoreFunctions.UpdateLookupList(locationID, Index, "MECategory", out MECategoryModels);

        NodeId KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEModeConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Used", KVConvNodeID, OEEModeModels);

        KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEStateConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Used", KVConvNodeID, OEEStateModels);

        KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEReasonCodeConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Used", KVConvNodeID, OEEReasonCodeModels);

        KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEECategoryConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Used", KVConvNodeID, OEECategoryModels);

        KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/MaintEvents/Data/MESeverityConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Active", KVConvNodeID, MESeverityModels);

        KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/MaintEvents/Data/MEDescriptionConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Active", KVConvNodeID, MEDescriptionModels);

        KVConvNodeID = Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/MaintEvents/Data/MECategoryConverter").NodeId;
        CoreFunctions.UpdateEnumConverter(1, "Not Active", KVConvNodeID, MECategoryModels);
    }
}
