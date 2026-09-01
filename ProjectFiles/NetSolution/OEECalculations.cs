/*
 * ***** Warning *****
DO NOT EDIT!  Edits to this script may cause this script to fail.  
 
=============================================================
 
Disclaimer of Warranty
THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT ARE PROVIDED "AS IS" WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
INCLUDING WITHOUT LIMITATION, ALL IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, NON-INFRINGEMENT OR OTHER VIOLATION OF RIGHTS. 
ROCKWELL AUTOMATION DOES NOT WARRANT OR MAKE ANY REPRESENTATIONS REGARDING THE USE, VALIDITY, ACCURACY, OR RELIABILITY OF, OR THE RESULTS OF ANY USE OF, 
OR OTHERWISE RESPECTING, THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT OR ANY WEB SITE LINKED TO THIS DOCUMENT
 
Limitation of Liability
UNDER NO CIRCUMSTANCE (INCLUDING NEGLIGENCE AND TO THE FULLEST EXTEND PERMITTED BY APPLICABLE LAW) WILL ROCKWELL AUTOMATION BE LIABLE FOR ANY 
DIRECT, INDIRECT, SPECIAL, INCIDENTAL, PUNITIVE OR CONSEQUENTIAL DAMAGES (INCLUDING WITHOUT LIMITATION, BUSINESS INTERRUPTION, DELAYS, LOSS OF DATA OR PROFIT) 
ARISING OUT OF THE USE OR THE INABILITY TO USE THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT EVEN IF ROCKWELL AUTOMATION HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES. 
IF USE OF SUCH MATERIALS RESULTS IN THE NEED FOR SERVICING, REPAIR OR CORRECTION OF USER EQUIPMENT OR DATA, USER ASSUMES ANY COSTS ASSOCIATED THEREWITH.
 
Copyright © Rockwell Automation, Inc.  All Rights Reserved.
 
=============================================================
*/

#region Using directives
using System;
using System.Linq;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.RAEtherNetIP;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using System.ComponentModel;
using FTOptix.Alarm;
using System.Collections.Generic;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.DataLogger;
#endregion

public class OEECalculations : BaseNetLogic
{
    private String ModelName;
    private Int32 ModelNameEnum;
    private DateTime StartDateTime;
    private DateTime EndDateTime;

    private Store dataStore;
    private Object[,] resultsData;
    private string[] resultsHeader;
    private string queryWhere;
    private string dataQuery;

    
    private struct QueryInfo
    {
        public string ID;
        public DateTime BeginningTime;
        public DateTime EndingTime;
        public bool Found;
    }

    public override void Start()
    {
        
        
        Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/RefreshDataWidgets").VariableChange += readDatabase;
        
    }

    public override void Stop()
    {
        Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/RefreshDataWidgets").VariableChange -= readDatabase;
    }

    private void readDatabase(object sender, VariableChangeEventArgs e)
    {
        StartDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/Start").Value;
        EndDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/End").Value;
        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/OEE/Databases/OEEDatabase");

        ModelNameEnum = 0;// Owner.GetVariable("ModelName").Value;
        IUANode EnumObj = InformationModel.Get(Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumLists/InformationModelInstances").NodeId);
        IUAVariable enumChild = InformationModel.GetVariable(EnumObj.Children[0].NodeId);
        Struct[] enumChildStructs = (Struct[])enumChild.Value.Value;
        LocalizedText thetext = (LocalizedText)enumChildStructs[ModelNameEnum].Values[1];
        ModelName = thetext.Text;

        queryWhere = "(MachineGUID = '" + ModelName + "') AND (StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (Timestamp <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";

        dataQuery = "SELECT MachineGUID, SUM(ScheduledTime) AS TotalScheduledTime, SUM(RunningTime) AS TotalRunningTime, SUM(TotalIdealTime) AS TotalTotalTime ,SUM(GoodIdealTime) AS TotalGoodTime FROM raI_01_00_0106_OEE WHERE " + queryWhere;

        dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        if (resultsData.GetLength(0) > 0)
        {
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/OEE").Value = Convert.ToSingle(resultsData[0,4])/ Convert.ToSingle(resultsData[0, 1])*100;
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/Availability").Value = Convert.ToSingle(resultsData[0, 2]) / Convert.ToSingle(resultsData[0, 1]) * 100;
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/Performance").Value = Convert.ToSingle(resultsData[0, 3]) / Convert.ToSingle(resultsData[0, 2]) * 100;
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/Quality").Value = Convert.ToSingle(resultsData[0, 4]) / Convert.ToSingle(resultsData[0, 3]) * 100;
        }
        else
        {
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/OEE").Value = 0;
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/Availability").Value = 0;
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/Performance").Value = 0;
            Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/OEEValues/Quality").Value = 0;
        }

        // query shift, order, and product, starttime, and timestamp.

        dataQuery = "SELECT StartTime, Timestamp, ShiftID FROM raI_01_00_0106_OEE ORDER BY TimeStamp DESC";

        dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        List<QueryInfo> QueryList = new List<QueryInfo>();
        
        for (int i = 0; i < resultsData.GetLength(0); i++)
        {
            QueryInfo Result = new QueryInfo();
            Result.BeginningTime = Convert.ToDateTime(resultsData[i, 0]);
            Result.EndingTime = Convert.ToDateTime(resultsData[i, 1]);
            Result.ID = resultsData[i, 2].ToString();
            QueryList.Add(Result);
        }

        QueryInfo[] Results = new QueryInfo[2];
        NodeId ShiftFilterNodeId= Owner.Owner.GetObject("Data/QuickFilter/ShiftID").NodeId;
        GetFilterData(QueryList, ShiftFilterNodeId, out Results);


        dataQuery = "SELECT StartTime, Timestamp, OrderID FROM raI_01_00_0106_OEE ORDER BY TimeStamp DESC";

        dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        QueryList = new List<QueryInfo>();

        for (int i = 0; i < resultsData.GetLength(0); i++)
        {
            QueryInfo Result = new QueryInfo();
            Result.BeginningTime = Convert.ToDateTime(resultsData[i, 0]);
            Result.EndingTime = Convert.ToDateTime(resultsData[i, 1]);
            Result.ID = resultsData[i, 2].ToString();
            QueryList.Add(Result);
        }

        Results = new QueryInfo[2];
        ShiftFilterNodeId = Owner.Owner.GetObject("Data/QuickFilter/OrderID").NodeId;
        GetFilterData(QueryList, ShiftFilterNodeId, out Results);

        dataQuery = "SELECT StartTime, Timestamp, ProductID FROM raI_01_00_0106_OEE ORDER BY TimeStamp DESC";

        dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        QueryList = new List<QueryInfo>();

        for (int i = 0; i < resultsData.GetLength(0); i++)
        {
            QueryInfo Result = new QueryInfo();
            Result.BeginningTime = Convert.ToDateTime(resultsData[i, 0]);
            Result.EndingTime = Convert.ToDateTime(resultsData[i, 1]);
            Result.ID = resultsData[i, 2].ToString();
            QueryList.Add(Result);
        }

        Results = new QueryInfo[2];
        ShiftFilterNodeId = Owner.Owner.GetObject("Data/QuickFilter/ProductID").NodeId;
        GetFilterData(QueryList, ShiftFilterNodeId, out Results);

    }

    private void GetFilterData(List<QueryInfo> SourceData, NodeId ModelTargetNodeId, out QueryInfo[] Results)
    {
        var Modeltarget=InformationModel.GetObject(ModelTargetNodeId);
        QueryInfo CurrentResults = new QueryInfo();
        QueryInfo PreviousResults = new QueryInfo();
        
        bool EndOfCurrent = false;
        bool StartOfCurrent=false;
        bool EndOfPrevious = false;
        bool StartOfPrevious = false;
        
        CurrentResults.Found = false;
        PreviousResults.Found = false;

        //int i = 1;

        if (SourceData.Count == 1)
        {
            StartOfCurrent = true;
            EndOfCurrent = true;
            StartOfPrevious = false;
            EndOfPrevious = false;

            CurrentResults.Found=true;
            CurrentResults.BeginningTime = SourceData[0].BeginningTime;
            CurrentResults.EndingTime = SourceData[0].EndingTime;
            CurrentResults.ID = SourceData[0].ID;
            PreviousResults.Found = false;
        }

        if (SourceData.Count > 1)
        {
            StartOfCurrent = true;
            CurrentResults.Found = true;
            CurrentResults.EndingTime = SourceData[0].EndingTime;
            CurrentResults.ID = SourceData[0].ID;

            for (int i = 1; i < SourceData.Count; i++)
            {
                //TimeSpan DeltaTime = SourceData[i-1].BeginningTime.Subtract(SourceData[i].EndingTime);
                //Log.Info("Delta Time: "+DeltaTime.TotalMilliseconds.ToString());

                if (SourceData[i].ID != SourceData[i - 1].ID)
                {
                    if (EndOfCurrent & !EndOfPrevious)
                    {
                        EndOfPrevious = true;
                        PreviousResults.BeginningTime = SourceData[i - 1].BeginningTime;
                    }
                    if (!EndOfCurrent)
                    {
                        EndOfCurrent = true;
                        CurrentResults.BeginningTime = SourceData[i - 1].BeginningTime;
                        StartOfPrevious = true;
                        PreviousResults.Found = true;
                        PreviousResults.ID = SourceData[i].ID;
                        PreviousResults.EndingTime = SourceData[i].EndingTime;
                    }
                }
            }
        }
        
        if (StartOfCurrent & !EndOfCurrent) 
        {
            CurrentResults.BeginningTime = SourceData[SourceData.Count-1].BeginningTime;
            CurrentResults.Found = true;
            EndOfCurrent = true;
        }
        
        if (StartOfPrevious & !EndOfPrevious)
        {
            PreviousResults.BeginningTime = SourceData[SourceData.Count - 1].BeginningTime;
            PreviousResults.Found = true;
            EndOfPrevious = true;
        }

        if (CurrentResults.Found)
        {
            Modeltarget.GetVariable("Current/ID").Value = CurrentResults.ID;
            Modeltarget.GetVariable("Current/BeginningTime").Value = CurrentResults.BeginningTime;
            Modeltarget.GetVariable("Current/EndingTime").Value = CurrentResults.EndingTime;
            Modeltarget.GetVariable("Current/Enable").Value = CurrentResults.Found;
        }
        else
        {
            Modeltarget.GetVariable("Current/ID").Value = "No Data";
            Modeltarget.GetVariable("Current/BeginningTime").Value = DateTime.Now;
            Modeltarget.GetVariable("Current/EndingTime").Value = DateTime.Now;
            Modeltarget.GetVariable("Current/Enable").Value = CurrentResults.Found;
        }
            
        
        if (PreviousResults.Found) 
        {
            Modeltarget.GetVariable("Previous/ID").Value = PreviousResults.ID;
            Modeltarget.GetVariable("Previous/BeginningTime").Value = PreviousResults.BeginningTime;
            Modeltarget.GetVariable("Previous/EndingTime").Value = PreviousResults.EndingTime;
            Modeltarget.GetVariable("Previous/Enable").Value = PreviousResults.Found;
        }
        else
        {
            Modeltarget.GetVariable("Previous/ID").Value = "No Data";
            Modeltarget.GetVariable("Previous/BeginningTime").Value = DateTime.Now;
            Modeltarget.GetVariable("Previous/EndingTime").Value = DateTime.Now;
            Modeltarget.GetVariable("Previous/Enable").Value = PreviousResults.Found;
        }


        Results = new QueryInfo[2];
        Results[0]=CurrentResults;
        Results[1]=PreviousResults;
    }
}
