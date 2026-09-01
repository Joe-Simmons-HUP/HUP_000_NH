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
using UAManagedCore;
using System;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.WebUI;
using FTOptix.UI;
using FTOptix.Store;
using FTOptix.SQLiteStore;
using FTOptix.RAEtherNetIP;
using FTOptix.OPCUAServer;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.CommunicationDriver;
using FTOptix.EventLogger;
using FTOptix.OPCUAClient;
using System.Threading;
using FTOptix.Report;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using FTOptix.Alarm;

#endregion

public class DateTimeManager : BaseNetLogic
{
    private Boolean DebugEnable;

    private PeriodicTask periodicTaskDTUpdate;

    public override void Start()
    {
        Log.Info("DataReady - DateTimeUpdate", "DateTimeUpdate Initialized");

        DebugEnable = LogicObject.GetVariable("DebugEnable").Value;

        if (DebugEnable)
        {
            Log.Info("DataReady - DateTimeUpdate","Update Tine Range Debug Enabled");
        }

        var dtUpdateRequest = InformationModel.GetVariable(Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/UpdateDateTime").NodeId);
        dtUpdateRequest.VariableChange += dtUpdateRequest_VariableChange;

        if (Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshEnable").Value == true)
        {
            Int32 updateRate = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshRateSec").Value * 1000;
            periodicTaskDTUpdate = new PeriodicTask(TimeRangeUpdate, updateRate, LogicObject);
            periodicTaskDTUpdate.Start();
        }
        
        var dtAutoUpdateEnable = InformationModel.GetVariable(Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshEnable").NodeId);
        dtAutoUpdateEnable.VariableChange += dtAutoUpdateEnable_VariableChange;

        var dtAutoRefreshTime = InformationModel.GetVariable(Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshRateSec").NodeId);
        dtAutoRefreshTime.VariableChange += dtAutoRefreshTime_VariableChange;


    }

    public override void Stop()
    {
        periodicTaskDTUpdate.Dispose();
        periodicTaskDTUpdate = null;

        Log.Warning("DataReady - DateTimeUpdate", "DateTimeUpdate Disposed");
    }

    private void dtUpdateRequest_VariableChange(object sender, VariableChangeEventArgs e)
    {
        var dtUpdateRequest = InformationModel.GetVariable(Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/UpdateDateTime").NodeId);
        if (dtUpdateRequest.Value)
        {
            TimeRangeUpdate();
            dtUpdateRequest.Value = false;
        }

    }

    private void dtAutoUpdateEnable_VariableChange(object sender, VariableChangeEventArgs e)
    {
        if (Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshEnable").Value == true) 
        {
            Int32 updateRate = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshRateSec").Value * 1000;
            periodicTaskDTUpdate = new PeriodicTask(TimeRangeUpdate, updateRate, LogicObject);
            periodicTaskDTUpdate.Start();
            
        }
        if (Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshEnable").Value == false)
        {
            periodicTaskDTUpdate.Dispose();
            periodicTaskDTUpdate = null;
        }
    }

    private void dtAutoRefreshTime_VariableChange(object sender, VariableChangeEventArgs e)
    {
        if (Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshEnable").Value == true)
        {
            periodicTaskDTUpdate.Dispose();
            periodicTaskDTUpdate = null;
            
            Thread.Sleep(1000);

            Int32 updateRate = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/AutoRefreshRateSec").Value * 1000;
            periodicTaskDTUpdate = new PeriodicTask(TimeRangeUpdate, updateRate, LogicObject);
            periodicTaskDTUpdate.Start();

        }
        
    }

    [ExportMethod]
    public void TimeRangeUpdate()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - DateTimeUpdate", "Update Tine Range");
        }

        var dateTimeRange = InformationModel.Get(Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange").NodeId);
        var refreshDataWidgets = InformationModel.GetVariable(Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/RefreshDataWidgets").NodeId);
        

        int Range = dateTimeRange.GetVariable("Range").Value;
        DateTime StartDT = dateTimeRange.GetVariable("Start").Value;
        DateTime EndDT = dateTimeRange.GetVariable("End").Value;


        if (Range <= 1)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddHours(-1);
        }
        if (Range == 2)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddHours(-3);
        }
        if (Range == 3)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddHours(-5);
        }
        if (Range == 4)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddHours(-8);
        }
        if (Range == 5)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddDays(-1);
        }
        if (Range == 6)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddDays(-7);
        }
        if (Range == 7)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddDays(-15);
        }
        if (Range == 8)
        {
            EndDT = DateTime.Now;
            StartDT = EndDT.AddDays(-30);
        }
        if (Range == 9)
        {
            //Custom Range set directly in the UI.
        }
        if (Range == 10)
        {
            EndDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ShiftID/Current/EndingTime").Value;
            StartDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ShiftID/Current/BeginningTime").Value;
        }
        if (Range == 11)
        {
            EndDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ShiftID/Previous/EndingTime").Value;
            StartDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ShiftID/Previous/BeginningTime").Value;
        }
        if (Range == 12)
        {
            EndDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/OrderID/Current/EndingTime").Value;
            StartDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/OrderID/Current/BeginningTime").Value;
        }
        if (Range == 13)
        {
            EndDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/OrderID/Previous/EndingTime").Value;
            StartDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/OrderID/Previous/BeginningTime").Value;
        }
        if (Range == 14)
        {
            EndDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ProductID/Current/EndingTime").Value;
            StartDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ProductID/Current/BeginningTime").Value;
        }
        if (Range == 15)
        {
            EndDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ProductID/Previous/EndingTime").Value;
            StartDT = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/QuickFilter/ProductID/Previous/BeginningTime").Value;
        }


        if (StartDT >= EndDT)
        {
            StartDT = EndDT;
        }

        dateTimeRange.GetVariable("Start").Value = StartDT;
        dateTimeRange.GetVariable("End").Value = EndDT;

        DateTime StartDTUTC = StartDT.ToUniversalTime();
        DateTime EndDTUTC = EndDT.ToUniversalTime();

        dateTimeRange.GetVariable("StartUTC").Value = StartDTUTC;
        dateTimeRange.GetVariable("EndUTC").Value = EndDTUTC;


        if (refreshDataWidgets.Value == false)
        {
            refreshDataWidgets.Value = true;
        }
        else
        {
            refreshDataWidgets.Value = false;
        }

        if (DebugEnable)
        {
            Log.Info("DataReady - DateTimeUpdate", "Time Selector Range Selected: " + Range + "  Start DT: " + StartDT + "  End DT: " + EndDT);
        }
    }
}
