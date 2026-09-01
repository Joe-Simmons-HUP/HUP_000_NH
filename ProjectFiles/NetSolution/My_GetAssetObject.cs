#region Using directives
using System;
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
using FTOptix.Alarm;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.DataLogger;
#endregion

public class My_GetAssetObject : BaseNetLogic
{
    IUAObject assetObj = null;

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        //Get the referenced Asset
        assetObj = GetReferencedObject("tcSDK1_DialogBox", "Ref_Model");
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    private IUAObject GetReferencedObject(string aliasName, string variableName)
    {
        if (aliasName == null) { throw new ArgumentNullException("aliasName"); }
        if (variableName == null) { throw new ArgumentNullException("variableName"); }

        IUAObject referencedObject = null;

        IUANode dialogBoxAlias = Owner.GetAlias(aliasName);
        IUAVariable assetRef = dialogBoxAlias.GetVariable(variableName);
        referencedObject = InformationModel.Get<IUAObject>(assetRef.Value);

        return referencedObject;
    }
}
