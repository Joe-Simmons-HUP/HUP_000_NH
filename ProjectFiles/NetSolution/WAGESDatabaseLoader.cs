/*
 * ***** Warning *****
DO NOT EDIT!  Edits to this script may cause this script to fail.  
 
=============================================================
 
Disclaimer of Warranty
THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT ARE PROVIDED "AS IS" WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ALL IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, NON-INFRINGEMENT OR OTHER VIOLATION OF RIGHTS. ROCKWELL AUTOMATION DOES NOT WARRANT OR MAKE ANY REPRESENTATIONS REGARDING THE USE, VALIDITY, ACCURACY, OR RELIABILITY OF, OR THE RESULTS OF ANY USE OF, OR OTHERWISE RESPECTING, THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT OR ANY WEB SITE LINKED TO THIS DOCUMENT
 
Limitation of Liability
UNDER NO CIRCUMSTANCE (INCLUDING NEGLIGENCE AND TO THE FULLEST EXTEND PERMITTED BY APPLICABLE LAW) WILL ROCKWELL AUTOMATION BE LIABLE FOR ANY DIRECT, INDIRECT, SPECIAL, INCIDENTAL, PUNITIVE OR CONSEQUENTIAL DAMAGES (INCLUDING WITHOUT LIMITATION, BUSINESS INTERRUPTION, DELAYS, LOSS OF DATA OR PROFIT) ARISING OUT OF THE USE OR THE INABILITY TO USE THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT EVEN IF ROCKWELL AUTOMATION HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES. IF USE OF SUCH MATERIALS RESULTS IN THE NEED FOR SERVICING, REPAIR OR CORRECTION OF USER EQUIPMENT OR DATA, USER ASSUMES ANY COSTS ASSOCIATED THEREWITH.
 
Copyright © Rockwell Automation, Inc.  All Rights Reserved.
 
=============================================================
*/

#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.OPCUAServer;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using System.Threading;
using FTOptix.WebUI;
using FTOptix.OPCUAClient;
using FTOptix.RAEtherNetIP;
using FTOptix.Report;
using FTOptix.ODBCStore;
using FTOptix.EventLogger;
using FTOptix.DataLogger;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
#endregion

public class WAGESDatabaseLoader : BaseNetLogic
{
    private bool DebugEnable = false;
    private bool WAGESFound = false;
    private string machineGUID = "";
    private CoreFunctions CoreFunctions;

    public override void Start()
    {
        CoreFunctions = new CoreFunctions();

        if (LogicObject.GetVariable("DebugEnable").Value)
        {
            DebugEnable = true;
        }

        var infoModel = Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/Global/InformationModel");

        foreach (var child in infoModel.Children)
        {
            machineGUID = child.BrowseName;
            RegisterWAGESTypes(child);
        }
        if (!WAGESFound)
        {
            Log.Warning("DataReady - WAGESDatabaseLoader", "Info Attribute: raI_01_00_0106_WAGES not found.  Logging service not started");
        }


    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined Logic is stopped
        Log.Warning("DataReady - WAGESDatabaseLoader", "Logging service stopped");
    }

    private void RegisterWAGESTypes(IUANode node)  //look for "raI_01_00_0106_WAGES" to determine if this part of the model is Data Ready enabled.
    {
        string DataType = InformationModel.GetObject(node.NodeId).ObjectType.BrowseName;

        if (DebugEnable)
        {
            Log.Info("DataReady - WAGESDatabaseLoader Debug", "Drilling for DR Type raI_01_00_0106_WAGES in: " + node.BrowseName + " of type: " + DataType);
        }

        if (DataType == "raI_01_00_0106_WAGES")   //InfoModelAttributeAsString = Info Attributes
        {
            WAGESFound = true;
            node.GetVariable("Data/_NewDataTrigger").VariableChange += WAGESDataUpdated;
            Log.Info("DataReady - WAGESDatabaseLoader", "DR Type raI_01_00_0106_WAGES found.  Logging service started.");
        }

        foreach (IUANode child in node.Children)
        {
            if (DebugEnable)
            {
                Log.Info("DataReady - WAGESDatabaseLoader Debug", "Found child of " + node.BrowseName + ": " + child.BrowseName);
            }

            if (child is IUAObject)
            {
                RegisterWAGESTypes(child);
            }

        }
        if (DebugEnable)
        {
            Log.Info("DataReady - WAGESDatabaseLoader Debug", "No more children for: " + node.BrowseName);
        }
    }

    private void WAGESDataUpdated(object sender, VariableChangeEventArgs e)
    {

        Int32 i = 0;
        bool matchDT = true;
        var parent = e.Variable.Owner;
        double hbDT = parent.GetVariable("AssetID").SourceTimestamp.Ticks;//e.Variable.SourceTimestamp.Ticks;

        if (DebugEnable)
        {
            Log.Info("DataReady - WAGES Event", "New data detected for Logging on " + e.Variable.Owner.Owner.BrowseName);
        }

        while (i < 10)
        {
            Thread.Sleep(50);
            hbDT = parent.GetVariable("AssetID").SourceTimestamp.Ticks;//e.Variable.SourceTimestamp.Ticks;
            foreach (var child in parent.Children)
            {
                if ((child.BrowseName != "_NewDataTrigger") & (child.BrowseName != "Symbol name") & (child.BrowseName != "Info Attributes"))
                {
                    var childVar = child as IUAVariable;
                    double childVarTimeticks = childVar.SourceTimestamp.Ticks;
                    double tickDelta = Math.Abs(childVarTimeticks - hbDT);
                    if (DebugEnable)
                    {
                        Log.Info("DataReady - WAGES Debug", "Tick Delta: " + tickDelta);
                    }
                    if (tickDelta > 200)
                    {
                        matchDT = false;
                        if (DebugEnable)
                        {
                            Log.Info("DataReady - WAGES Debug", "Data Timestamp mismatch at " + childVar.BrowseName + ".  Step: " + i + ": " + tickDelta + " " + hbDT + " <> " + childVarTimeticks);
                        }
                    }
                }
            }
            i++;
            if (matchDT)
            {
                i = 10;
            }
        }
        if (DebugEnable)
        {
            Log.Info("DataReady - WAGES Debug", "Valid Data set?:  " + matchDT);
        }
        if (!matchDT)
        {
            Log.Warning("DataReady - WAGES", "Data Timestamp mismatch.  Data will not be Logged.");
        }
        if (matchDT)
        {
            WAGESLogToDatabase(parent);
        }

        LimitWAGESDBSize();
    }

    private void WAGESLogToDatabase(IUANode parent)
    {
        try
        {
            string[] labels = new string[15];
            object[,] values = new object[1, 15];
            Int32 j = 0;

            string instanceName = parent.BrowseName;
            string fqnName;
            CoreFunctions.GetOwnerPath("InformationModel", parent, instanceName, out fqnName);
            fqnName = fqnName.Replace("/Data", "");

            foreach (var child in parent.Children)
            {
                if ((child.BrowseName != "_NewDataTrigger") & (child.BrowseName != "Symbol name") & (child.BrowseName != "Info Attributes"))
                {
                    var childVar = child as IUAVariable;
                    labels[j] = childVar.BrowseName;
                    values[0, j] = CoreFunctions.GetValueByDataType(childVar.Value, childVar.DataType);
                    if (DebugEnable)
                    {
                        Log.Info("DataReady - WAGES Debug", childVar.BrowseName + " " + childVar.Value + " " + childVar.DataType);
                    }
                    j++;
                }
            }
            if (DebugEnable)
            {
                Log.Info("DataReady - WAGES Debug", "Calculated Values Starting: " + j);
            }

            labels[j] = "Timestamp";
            values[0, j] = parent.GetVariable("_NewDataTrigger").SourceTimestamp.ToLocalTime();
            j = j + 1;

            if (DebugEnable)
            {
                Log.Info("DataReady - WAGES Debug", "Calculated Times: " + j);
            }

            labels[j] = "InstanceName";
            values[0, j] = fqnName;
            j = j + 1;

            labels[j] = "MachineGuid";
            values[0, j] = machineGUID;

            if (DebugEnable)
            {
                Log.Info("DataReady - WAGES Debug", "Variable Collection Completed: " + j);
            }

            Store drStore = InformationModel.Get<Store>(Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/WAGES/Databases/WAGESDatabase").NodeId);
            if (DebugEnable)
            {
                Log.Info("DataReady - WAGES Debug", "Store: " + drStore.BrowseName);
            }
            Table WAGESTable = drStore.Tables.Get<Table>("raI_01_00_0106_WAGES");
            if (DebugEnable)
            {
                Log.Info("DataReady - WAGES Debug", "Table: " + WAGESTable.BrowseName);
            }

            if (DebugEnable)
            {
                j = 0;
                while (j < 12)
                {
                    Log.Info("DataReady - WAGES Debug", "Data to be written to the DB: " + labels[j] + " " + values[0, j].ToString());
                    j = j + 1;
                }
            }

            Int32 childCount = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DRInfoModelsPresent/WAGES").Children.Count;
            if (childCount > 0)
            {

                foreach (var child in Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DRInfoModelsPresent/WAGES").Children)
                {

                    NodePointer targetTableNodeId = child as NodePointer;

                    Table targetTable = InformationModel.Get<Table>(targetTableNodeId.Value);

                    targetTable.Insert(labels, values);

                }
            }
        }
        catch (Exception e)
        {
            Log.Error("DataReady - WAGES", "Data Logging Failed: " + e.StackTrace + " " + e.Message);
        }
    }

    [ExportMethod]
    public void DeleteWAGES()
    {
        Store drStore = InformationModel.Get<Store>(Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/WAGES/Databases/WAGESDatabase").NodeId);
        NodeId TargetNodeId = drStore.Tables.Get("raI_01_00_0106_WAGES").NodeId;
        CoreFunctions.DeleteDatabaseTable(TargetNodeId);

    }

    [ExportMethod]
    public void LimitWAGESDBSize()
    {
        Store drStore = InformationModel.Get<Store>(Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/WAGES/Databases/WAGESDatabase").NodeId);
        object[,] resultSet;
        string[] header;
        drStore.Query("SELECT Timestamp FROM raI_01_00_0106_WAGES ORDER BY Timestamp", out header, out resultSet);
        Int32 timelength = resultSet.GetLength(0);

        float MaxTimeHorizon = LogicObject.GetVariable("TargetTimeHorizon_Days").Value;
        Int32 TimeHorizone = 0;
        int CurrentRowCount = resultSet.GetLength(0);
        float CurrentDBSize = Convert.ToSingle(CurrentRowCount) * 132 / 1000000;
        uint MaxRowCount = Convert.ToUInt32(MaxTimeHorizon * 288);
        if (MaxRowCount < 1000) { MaxRowCount = 1000; }
        float MaxDBSize = Convert.ToSingle(MaxRowCount + 1) * 132 / 1000000;
        LogicObject.GetVariable("MaxRowCount").Value = MaxRowCount + 1;
        LogicObject.GetVariable("MaxTableSize_MB").Value = MaxDBSize;

        if (timelength != 0)
        {
            DateTime MinDT = DateTime.Now;
            DateTime MaxDT = DateTime.Now;

            LogicObject.GetVariable("CurrentRowCount").Value = CurrentRowCount;
            LogicObject.GetVariable("CurrentTableSize_MB").Value = CurrentDBSize;

            MinDT = Convert.ToDateTime(resultSet[0, 0]);
            MaxDT = Convert.ToDateTime(resultSet[timelength - 1, 0]);
            TimeHorizone = Convert.ToInt32(MaxDT.Subtract(MinDT).TotalDays);
            if (TimeHorizone == 0) { TimeHorizone = 1; }
            LogicObject.GetVariable("CurrentTimeHorizon_Days").Value = TimeHorizone;
        }
        else
        {
            LogicObject.GetVariable("CurrentRowCount").Value = 0;
            LogicObject.GetVariable("CurrentTableSize_MB").Value = 0;
            LogicObject.GetVariable("CurrentTimeHorizon_Days").Value = 0;
        }

        if (DebugEnable)
        {
            Log.Info("DataReady - WAGESDatabaseLoader", "Time Horizon (Days) - Target Max: " + MaxTimeHorizon + "  Current: " + TimeHorizone);
            Log.Info("DataReady - WAGESDatabaseLoader", "DB Size (MB) - Target Max: " + MaxDBSize + "  Current: " + CurrentDBSize);
            Log.Info("DataReady - WAGESDatabaseLoader", "DB Row Count - Max (Calculated as 1 Event/Min for " + MaxTimeHorizon + " Days): " + MaxRowCount + "  Current: " + CurrentRowCount);
        }

    }

}
