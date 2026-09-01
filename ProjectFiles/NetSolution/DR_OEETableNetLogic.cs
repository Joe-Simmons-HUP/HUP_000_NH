#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.Retentivity;
using FTOptix.RAEtherNetIP;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using System.Collections.Generic;
using FTOptix.ODBCStore;
using FTOptix.EventLogger;
using FTOptix.DataLogger;
using FTOptix.WebUI;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
using FTOptix.OPCUAServer;
#endregion

public class DR_OEETableNetLogic : BaseNetLogic
{
    private Boolean DebugEnable;
    private String Locale;
    private Int32 ModeCnt;
    private Int32 StateCnt;
    private Int32 ReasonCodeCnt;
    private Int32 CategoryCnt;
    private List<string> categoryList;
    private List<string> reasonCodeList;
    private List<string> stateList;
    private List<string> modeList;
    private LocalizedText discKey;
    private List<string> locationID;
    private string translatedText;
    private string DynamicWHERE;
    private string queryWhere;
    private Int32 i;

    private CoreFunctions CoreFunctions;


    private struct TargetTableStruct
    {
        public DateTime StartTime { get; set; }
        public DateTime Timestamp { get; set; }
        public String MachineGuid { get; set; }
        public String InstanceName { get; set; }
        public String AssetID { get; set; }
        public Int32 ScheduledStatus { get; set; }
        public Int32 RunningStatus { get; set; }
        public Single ScheduledTime { get; set; }
        public Single RunningTime { get; set; }
        public Single TotalIdealTime { get; set; }
        public Single GoodIdealTime { get; set; }
        public Single TotalCount { get; set; }
        public Single TotalCountDelta { get; set; }
        public Single GoodCount { get; set; }
        public Single GoodCountDelta { get; set; }
        public Single RejectCount1 { get; set; }
        public Single RejectCount1Delta { get; set; }
        public Single RejectCount2 { get; set; }
        public Single RejectCount2Delta { get; set; }
        public Single RejectCount3 { get; set; }
        public Single RejectCount3Delta { get; set; }
        public Single RejectCount4 { get; set; }
        public Single RejectCount4Delta { get; set; }
        public String Mode { get; set; }
        public String State { get; set; }
        public String ReasonCode { get; set; }
        public String Category { get; set; }
        public Single Duration { get; set; }
        public Single IdealCycleTime { get; set; }
        public Single ActualCycleTime { get; set; }
        public Single UnitRatio { get; set; }
        public String ShiftID { get; set; }
        public String ProductID { get; set; }
        public String OperatorID { get; set; }
        public String OrderID { get; set; }
        public String ContextVar1 { get; set; }
        public String ContextVar2 { get; set; }
        public String ContextVar3 { get; set; }
        public String ContextVar4 { get; set; }
        public Single OEETarget { get; set; }
        public Single AvailTarget { get; set; }
        public Single PerfTarget { get; set; }
        public Single QualTarget { get; set; }
        public String StateColor { get; set; }
        public Int32 StateEnum { get; set; }

    }

    public override void Start()
    {


        DebugEnable = Owner.GetVariable("DebugEnable").Value;

        CoreFunctions = new CoreFunctions();

        Owner.GetVariable("RefreshDataWidgets").VariableChange += updateUIDB;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEEUILoader", "OEEUILoader Initialized");
        }
    }

    public override void Stop()
    {
        Owner.GetVariable("RefreshDataWidgets").VariableChange -= updateUIDB;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEEUILoader", "OEEUILoader Stopped");
        }
    }

    private void getLookupLists()
    {

        ModeCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEMode").Value;
        StateCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEState").Value;
        ReasonCodeCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEReasonCode").Value;
        CategoryCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEECategory").Value;
        Locale = Owner.GetVariable("Locale").Value;

        int NamespaceIndex = LogicObject.NodeId.NamespaceIndex;
        locationID = new List<string>();

        modeList = new List<string>();
        modeList.Add("Not Used");
        CoreFunctions.CreateTranslatedList(Locale, NamespaceIndex, ModeCnt, "OEEMode", out modeList);
                
        stateList = new List<string>();
        stateList.Add("Not Used");
        CoreFunctions.CreateTranslatedList(Locale, NamespaceIndex, StateCnt, "OEEState", out stateList);

        reasonCodeList = new List<string>();
        reasonCodeList.Add("Not Used");
        CoreFunctions.CreateTranslatedList(Locale, NamespaceIndex, ReasonCodeCnt, "OEEReasonCode", out reasonCodeList);

        categoryList = new List<string>();
        categoryList.Add("Not Used");
        CoreFunctions.CreateTranslatedList(Locale, NamespaceIndex, CategoryCnt, "OEECategory", out categoryList);
    }

    private void updateUIDB(object sender, VariableChangeEventArgs g)
    {
        
        getLookupLists();
        
        object[,] resultSet;
        string[] header;
        object[,] updatedSet;
        string[] updatedHeader;

        DateTime StartDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/Start").Value;
        DateTime EndDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/End").Value;

        IUANode EnumObj = InformationModel.Get(Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumLists/InformationModelInstances").NodeId);
        IUAVariable enumChild = InformationModel.GetVariable(EnumObj.Children[0].NodeId);
        Struct[] enumChildStructs = (Struct[])enumChild.Value.Value;
        LocalizedText theText = (LocalizedText)enumChildStructs[0].Values[1];
        string ModelName = theText.Text;

        DynamicWHERE = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/DynamicFilterWHERE").Value;

        if (DynamicWHERE == "")
        {
            queryWhere = "(MachineGUID = '" + ModelName + "') AND (StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (StartTime <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";
        }
        else
        {
            queryWhere = "(MachineGUID = '" + ModelName + "') AND " + DynamicWHERE;
        }

        try
        {
            string dataQuery = "SELECT * FROM raI_01_00_0106_OEE WHERE " + queryWhere + " ORDER BY StartTime";

            Store UIStore = InformationModel.Get<Store>(Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/OEE/Databases/OEEDatabase").NodeId);
            Table SourceTable = UIStore.Tables.Get("raI_01_00_0106_OEE");
            Table DestinationTable = UIStore.Tables.Get("OEE_UI");

            UIStore.Query("DELETE FROM OEE_UI", out header, out resultSet);
            UIStore.Query(dataQuery, out header, out resultSet);
            
            TargetTableStruct SourceDBRow = new TargetTableStruct();
            List<TargetTableStruct> SourceDBTable = new List<TargetTableStruct>();
            var TargetStructType = typeof(TargetTableStruct);

            object Boxed = SourceDBRow;
            for (int i = 0; i < resultSet.GetLength(0); i++)
            {
                foreach (var prop in TargetStructType.GetProperties())
                {
                    //Log.Info("Logging Column: "+prop.Name+" of Row:" + i);
                    if ((prop.Name != "Mode") & (prop.Name != "State") & (prop.Name != "ReasonCode") & (prop.Name != "Category") & (prop.Name != "StateEnum") & (prop.Name != "Duration"))
                    {
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            prop.SetValue(Boxed, Convert.ToDateTime(resultSet[i, Array.IndexOf(header, prop.Name)]));
                        }
                        if (prop.PropertyType == typeof(Int32))
                        {
                            prop.SetValue(Boxed, Convert.ToInt32(resultSet[i, Array.IndexOf(header, prop.Name)]));
                        }
                        if (prop.PropertyType == typeof(Single))
                        {
                            prop.SetValue(Boxed, Convert.ToSingle(resultSet[i, Array.IndexOf(header, prop.Name)]));
                        }
                        if (prop.PropertyType == typeof(String))
                        {
                            prop.SetValue(Boxed, Convert.ToString(resultSet[i, Array.IndexOf(header, prop.Name)]));
                        }
                    }
                    if ((prop.Name == "Mode") | (prop.Name == "State") | (prop.Name == "ReasonCode") | (prop.Name == "Category"))
                    {
                        Int32 pointer = 0;
                        if (prop.Name == "Mode")
                        {
                            pointer = Convert.ToInt32(resultSet[i, Array.IndexOf(header, prop.Name)]);
                            //Log.Info("Pointer i= "+ modeList[pointer]);
                            prop.SetValue(Boxed, modeList[pointer]);
                        }
                        if (prop.Name == "State")
                        {
                            pointer = Convert.ToInt32(resultSet[i, Array.IndexOf(header, prop.Name)]);
                            prop.SetValue(Boxed, stateList[pointer]);
                        }
                        if (prop.Name == "ReasonCode")
                        {
                            pointer = Convert.ToInt32(resultSet[i, Array.IndexOf(header, prop.Name)]);
                            prop.SetValue(Boxed, reasonCodeList[pointer]);
                        }
                        if (prop.Name == "Category")
                        {
                            pointer = Convert.ToInt32(resultSet[i, Array.IndexOf(header, prop.Name)]);
                            prop.SetValue(Boxed, categoryList[pointer]);
                        }
                    }
                    if (prop.Name == "StateEnum")
                    {
                        Int32 pointer = Convert.ToInt32(resultSet[i, Array.IndexOf(header, "State")]);
                        prop.SetValue(Boxed, pointer);
                    }
                    if (prop.Name == "Duration")
                    {
                        Single pointer = Convert.ToSingle(resultSet[i, Array.IndexOf(header, "Duration")]) / 60;
                        prop.SetValue(Boxed, pointer);
                    }
                }
                SourceDBRow = (TargetTableStruct)Boxed;
                SourceDBTable.Add(SourceDBRow);
            }

            TargetTableStruct TargetDBRow = new TargetTableStruct();
            List<TargetTableStruct> TargetDBTable = new List<TargetTableStruct>();


            TargetDBRow = SourceDBTable[0];

            for (int x = 1; x < SourceDBTable.Count; x++)
            {
                //  Add code to check for gap in time
                double TimeDeltaMsec = SourceDBTable[x].StartTime.Subtract(SourceDBTable[x - 1].Timestamp).TotalMilliseconds;
                if ((TargetDBRow.Mode == SourceDBTable[x].Mode) & (TargetDBRow.State == SourceDBTable[x].State) & (TargetDBRow.ReasonCode == SourceDBTable[x].ReasonCode) & (TargetDBRow.Category == SourceDBTable[x].Category) & (TimeDeltaMsec < 50))
                {
                    TargetDBRow.Timestamp = SourceDBTable[x].Timestamp;
                    TargetDBRow.Duration = TargetDBRow.Duration + SourceDBTable[x].Duration;
                }
                else
                {
                    TargetDBTable.Add(TargetDBRow);
                    TargetDBRow = SourceDBTable[x];
                }
            }

            Int32 RowCount = TargetDBTable.Count;
            var props = TargetStructType.GetProperties();
            Int32 ColumnCount = props.Length;

            updatedSet = new object[RowCount, ColumnCount];
            updatedHeader = new string[ColumnCount];

            for (int x = 0; x < ColumnCount; x++)
            {
                updatedHeader[x] = props[x].Name;
            }

            for (int x = 0; x < RowCount; x++)
            {
                for (int y = 0; y < ColumnCount; y++)
                {
                    updatedSet[x, y] = props[y].GetValue(TargetDBTable[x]);

                }
            }

            UIStore.Insert("OEE_UI", updatedHeader, updatedSet);
            Owner.Get<DataGrid>("ScrollView1/DataGrid1").Refresh();
            
        }
        catch (Exception e)
        {
            Log.Info("DR_OEETableNetLogic - updateUIDB", e.Message + " - " + e.StackTrace);
        }
    }
}
