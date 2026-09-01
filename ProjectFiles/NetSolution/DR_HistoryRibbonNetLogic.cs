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
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.Collections.Generic;
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
using FTOptix.Report;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using FTOptix.Alarm;


#endregion

public class DR_HistoryRibbonNetLogic : BaseNetLogic
{
    #region DECLARE VARIABLES

    private Boolean DebugEnable;
    private String ImageID;
    private Int32 ModelNameEnum;
    private String ModelName;
    private Int32 InstanceNameEnum;
    private String InstanceName;
    private DateTime StartDateTime;
    private DateTime EndDateTime;
    private String Units;
    private String BackGroundColor;
    private String FontColor;
    private String TitleEnable;
    private String TitleText;
    private String TitleFont;
    private Int32 TitleFontSize;
    private String TitleFontWeight;
    private Int32 TitleTop;
    private String LabelFont;
    private Int32 LabelFontSize;
    private String LabelFontWeight;
    private Int32 GridTopSpacing;
    private Int32 GridBottomSpacing;
    private Int32 GridLeftSpacing;
    private Int32 GridRightSpacing;
    private Int32 BarHeight;
    private String TimeSliderEnable;
    private Int32 TimeSliderOffset;
    private Int32 ModeCnt;
    private Int32 StateCnt;
    private Int32 ReasonCodeCnt;
    private Int32 CategoryCnt;
    private String[] LocaleList;
    private string Locale;

    private string projectName;
    private Store dataStore;
    private Object[,] resultsData;
    private string[] resultsHeader;
    private string DynamicWHERE;
    private string queryWhere;
    private string dataQuery;
    private string theData;
    private Int32 i;
    private Int32 j;
    private Int32 length;
    private List<string> categoryList;
    private List<string> reasonCodeList;
    private List<string> stateList;
    private List<string> modeList;
    private LocalizedText discKey;
    private List<string> locationID;
    private string translatedText;
    private CoreFunctions CoreFunctions;

    private string htmlFileUri;
    private string js_Header;
    private string js_Data;
    private string js_VarList1;
    private string js_VarList2;
    private string js_eChartsOptions;
    private string htmlBody;
    private string projPath;
    private string jsFilePath;
    private string htmlFilePath;
    string[] fontList = { "arial", "verdana", "tahoma", "trebuchet ms", "times new roman", "georgia", "garamond", "courier new", "brush script mt" };
    private string[] fontWeightList = { "normal", "bold", "bolder", "lighter" };

    private struct dataSet
    {
        public string State;
        public string ReasonCode;
        public DateTime StartTime;
        public DateTime Timestamp;
        public float Duration;
        public string StateColor;
    };

    #endregion

    #region SUPPORTING METHODS

    private void loadValues()
    {
        DebugEnable = Owner.GetVariable("DebugEnable").Value;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Debug Enabled: " + DebugEnable);
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Loading Values");
        }

        CoreFunctions = new CoreFunctions();
        ImageID = Owner.GetVariable("ImageID").Value;

        ModelNameEnum = Owner.GetVariable("ModelName").Value;
        IUANode EnumObj = InformationModel.Get(Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumLists/InformationModelInstances").NodeId);
        CoreFunctions.GetEnumText(ModelNameEnum, EnumObj, out ModelName);


        InstanceNameEnum = Owner.GetVariable("InstanceName").Value;
        EnumObj = InformationModel.Get(Project.Current.Get("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/OEE_UI/<PrivateElements>/Data/OEEInstances").NodeId);
        CoreFunctions.GetEnumText(InstanceNameEnum, EnumObj, out InstanceName);


        StartDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/Start").Value;
        EndDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/End").Value;
        Units = Owner.GetVariable("Units").Value;
        BackGroundColor = colorString(Owner.GetVariable("BackGroundColor").Value);
        FontColor = colorString(Owner.GetVariable("FontColor").Value);
        TitleEnable = Owner.GetVariable("TitleEnable").Value.ToString().ToLower().Replace(" (bool)", "");
        TitleText = Owner.GetVariable("TitleText").Value;
        TitleFont = fontList[Owner.GetVariable("TitleFont").Value];
        TitleFontSize = Owner.GetVariable("TitleFontSize").Value;
        TitleFontWeight = fontWeightList[Owner.GetVariable("TitleFontWeight").Value];
        TitleTop = Owner.GetVariable("TitleTop").Value;
        LabelFont = fontList[Owner.GetVariable("LabelFont").Value];
        LabelFontSize = Owner.GetVariable("LabelFontSize").Value;
        LabelFontWeight = fontWeightList[Owner.GetVariable("LabelFontWeight").Value];
        GridTopSpacing = Owner.GetVariable("GridTopSpacing").Value;
        GridBottomSpacing = Owner.GetVariable("GridBottomSpacing").Value;
        GridLeftSpacing = Owner.GetVariable("GridLeftSpacing").Value;
        GridRightSpacing = Owner.GetVariable("GridRightSpacing").Value;
        BarHeight = Owner.GetVariable("BarHeight").Value;
        TimeSliderEnable = Owner.GetVariable("TimeSliderEnable").Value.ToString().ToLower().Replace(" (bool)", "");
        TimeSliderOffset = Owner.GetVariable("TimeSliderOffset").Value;
        ModeCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEMode").Value;
        StateCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEState").Value;
        ReasonCodeCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEReasonCode").Value;
        CategoryCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEECategory").Value;
        Locale = Owner.GetVariable("Locale").Value;


        projectName = Project.Current.BrowseName;
        projPath = Project.Current.ProjectDirectory;
        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/OEE/Databases/OEEDatabase");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "OEE Datastore: " + dataStore.BrowseName);
        }

        jsFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".js";
        htmlFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".html";
        htmlFileUri = ResourceUri.FromProjectRelativePath("DataReady/eCharts/Runtime/" + ImageID + ".html");

        Owner.GetVariable("ImageID").Value = ImageID;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Loading Values Complete");
        }
    }

    public string colorString(Color color)
    {
        string updatedColor = "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        return updatedColor;
    }

    private void getLookupLists()
    {
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

    private void readDatabase()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Read Database");
        }

        DynamicWHERE = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/DynamicFilterWHERE").Value;

        if (DynamicWHERE == "")
        {
            queryWhere = "(MachineGUID = '" + ModelName + "') AND (StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (Timestamp <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";
        }
        else
        {
            queryWhere = "(MachineGUID = '" + ModelName + "') AND " + DynamicWHERE;
        }

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Query WHERE: " + queryWhere);
        }

        dataQuery = "SELECT State, ReasonCode, StartTime, Timestamp, Duration, StateColor FROM raI_01_00_0106_OEE WHERE " + queryWhere + " ORDER BY StartTime";

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Query: " + dataQuery);
        }

        dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Query Result Length: " + resultsData.GetLength(0));
        }
    }

    private void dataStaging()
    {
        try
        {
            dataSet resultSet = new dataSet();
            dataSet[] resultList = new dataSet[resultsData.GetLength(0)];

            i = 0;
            j = 0;
            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE Hist Ribbon Debug", "State: " + Convert.ToInt32(resultsData[i, 0]));
            }
            resultSet.State = stateList[Convert.ToInt32(resultsData[i, 0])];
            resultSet.ReasonCode = reasonCodeList[Convert.ToInt32(resultsData[i, 1])];
            resultSet.StartTime = Convert.ToDateTime(resultsData[i, 2]);
            resultSet.Timestamp = Convert.ToDateTime(resultsData[i, 3]);
            resultSet.Duration = Convert.ToSingle(resultsData[i, 4]);
            resultSet.StateColor = Convert.ToString(resultsData[i, 5]);
            i = i + 1;


            while (i <= resultsData.GetLength(0) - 1)
            {
                DateTime OldTimestamp = Convert.ToDateTime(resultsData[i - 1, 3]);
                DateTime NewStartTime = Convert.ToDateTime(resultsData[i, 2]);
                Double TimeDeltaMS = NewStartTime.Subtract(OldTimestamp).TotalMilliseconds;

                //  Instead of comparing States, compare Timestamp and Start Times.
                if ((TimeDeltaMS < 50) & (Convert.ToInt32(resultsData[i, 0]) == Convert.ToInt32(resultsData[i - 1, 0])) & (Convert.ToInt32(resultsData[i, 1]) == Convert.ToInt32(resultsData[i - 1, 1])))
                {
                    //Log.Info("States Match and small delta:  i="+i+"  delta="+TimeDeltaMS);
                    resultSet.Timestamp = Convert.ToDateTime(resultsData[i, 3]);
                    resultSet.Duration = resultSet.Duration + Convert.ToSingle(resultsData[i, 4]);
                }
                else
                {
                    resultList[j] = resultSet;
                    //Log.Info("No Match or large delta:  i=" + i + "  delta=" + TimeDeltaMS);
                    j = j + 1;
                    resultSet.State = stateList[Convert.ToInt32(resultsData[i, 0])];
                    resultSet.ReasonCode = reasonCodeList[Convert.ToInt32(resultsData[i, 1])]; ;
                    resultSet.StartTime = Convert.ToDateTime(resultsData[i, 2]);
                    resultSet.Timestamp = Convert.ToDateTime(resultsData[i, 3]);
                    resultSet.Duration = Convert.ToSingle(resultsData[i, 4]);
                    resultSet.StateColor = Convert.ToString(resultsData[i, 5]);
                }
                i = i + 1;
            }
            resultList[j] = resultSet;
            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE Hist Ribbon Debug", "ResultList build completed");
            }
            length = j + 1;
            i = 0;
            j = 0;
            theData = "[\r\n";
            while (j < length - 1)
            {
                theData = theData + "{state: '" + resultList[j].State + ":  " + resultList[j].ReasonCode + "', starttime: '" + resultList[j].StartTime + "', endtime: '" + resultList[j].Timestamp + "', duration: '" + resultList[j].Duration + "', color: '" + resultList[j].StateColor + "'},\r\n";
                j = j + 1;
            }
            theData = theData + "{state: '" + resultList[j].State + ":  " + resultList[j].ReasonCode + "', starttime: '" + resultList[j].StartTime + "', endtime: '" + resultList[j].Timestamp + "', duration: '" + resultList[j].Duration + "', color: '" + resultList[j].StateColor + "'}\r\n]";
            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE Hist Ribbon Debug", "Staging Complete");
            }
        }
        catch (Exception e)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", e.ToString());
        }
    }

    public void createStrings()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Creating JS Strings");
        }
        js_Header = "var dom = document.getElementById('container');\r\nvar myChart = echarts.init(dom, null, {\r\n    renderer: 'canvas',\r\n    useDirtyRect: false\r\n});\r\nvar app = {};\r\nvar option;\r\nvar data = [];\r\nvar startTime = '"+ StartDateTime.ToString() + "';\r\nvar endTime = '"+ EndDateTime.ToString() + "';\r\nvar categories = [''];";
        js_Data = "var stateEvent = " + theData + ";";
        js_VarList1 = "var backGroundColor = '" + BackGroundColor + "';\r\nvar fontColor = '" + FontColor + "';\r\nvar titleEnable = " + TitleEnable + ";\r\nvar titleText = '" + TitleText + "';\r\nvar titleTop = " + TitleTop + ";\r\nvar titleFontSize = " + TitleFontSize + ";\r\nvar titleFont = '" + TitleFont + "';\r\nvar titleWeight = '" + TitleFontWeight + "';\r\nvar labelFontSize = " + LabelFontSize + ";\r\nvar labelFont = '" + LabelFont + "';\r\nvar labelWeight = '" + LabelFontWeight + "';\r\nvar gridTopSpacing=" + GridTopSpacing + ";\r\nvar gridBottomSpacing=" + GridBottomSpacing + ";\r\nvar gridLeftSpacing=" + GridLeftSpacing + ";\r\nvar gridRightSpacing=" + GridRightSpacing + ";\r\n";
        js_VarList2 = "var barHeight = " + BarHeight + ";\r\nvar sliderEnable = '" + TimeSliderEnable + "';\r\nvar sliderOffset = " + TimeSliderOffset + ";\r\n";
        js_eChartsOptions = "var dataCount = stateEvent.length;\r\n\r\n// Unpack stateEvent data\r\ncategories.forEach(function (category, index) {\r\n    for (var i = 0; i < dataCount; i++) {\r\n        var stateItem = stateEvent[i];\r\n        const datetempstart = new Date(stateItem.starttime);\r\n        var msecstart = datetempstart.getTime();\r\n        const datetempend = new Date(stateItem.endtime);\r\n        var msecend = datetempend.getTime();\r\n        data.push({\r\n            name: stateItem.state,\r\n            value: [index, msecstart, msecend, stateItem.duration * 1000],\r\n            itemStyle: {\r\n                normal: {\r\n                    color: stateItem.color\r\n                }\r\n            }\r\n        });\r\n    }\r\n});\r\n\r\nfunction renderItem(params, api) {\r\n    var categoryIndex = api.value(0);\r\n    var start = api.coord([api.value(1), categoryIndex]);\r\n    var end = api.coord([api.value(2), categoryIndex]);\r\n    var height = api.size([0, 1])[1] * .8; //scales the bar to the grid height\r\n    var rectShape = echarts.graphic.clipRectByRect({\r\n        x: start[0],\r\n        y: start[1] - height / 2,\r\n        width: end[0] - start[0],\r\n        height: height\r\n    }, {\r\n        x: params.coordSys.x,\r\n        y: params.coordSys.y,\r\n        width: params.coordSys.width,\r\n        height: params.coordSys.height\r\n    });\r\n    return (\r\n        rectShape && {\r\n        type: 'rect',\r\n        transition: ['shape'],\r\n        shape: rectShape,\r\n        style: api.style()\r\n    });\r\n}\r\n\r\noption = {\r\n    backgroundColor: backGroundColor,\r\n    tooltip: {\r\n        formatter: function (params) {\r\n            return params.marker + params.name + ': ' + ((params.value[3] / 60000).toFixed(3)) + ' Min';\r\n        }\r\n    },\r\n    title:\r\n    [{\r\n            show: titleEnable,\r\n            text: titleText,\r\n            top: titleTop,\r\n            left: 'center',\r\n            textStyle: {\r\n                color: fontColor,\r\n                fontWeight: titleWeight,\r\n                fontFamily: titleFont,\r\n                fontSize: titleFontSize\r\n            }\r\n        },\r\n    ],\r\n    dataZoom:\r\n    [{\r\n\r\n            show: sliderEnable,\r\n            type: 'slider',\r\n            filterMode: 'weakFilter',\r\n            showDataShadow: false,\r\n            //bottom: gridBottomSpacing-75,\r\n            top: gridTopSpacing + barHeight + sliderOffset,\r\n            labelFormatter: ''\r\n        }, {\r\n            type: 'inside',\r\n            filterMode: 'weakFilter'\r\n        },\r\n    ],\r\n    grid: {\r\n        top: gridTopSpacing,\r\n        height: barHeight,\r\n        left: gridLeftSpacing,\r\n        right: gridRightSpacing\r\n    },\r\n    xAxis: {\r\n        type:'time',\r\n        boundaryGap: ['0%', '0%'],\r\n        min: Date(startTime).getTime,\r\n        max: Date(endTime).getTime,\r\n        axisTick: {\r\n            show: true,\r\n            inside: false,\r\n            length: 5,\r\n            lineStyle: {\r\n                color: fontColor,\r\n                width: 2\r\n            },\r\n        },\r\n        axisLabel: {\r\n            color: fontColor,\r\n            hideOverlap: true,\r\n            fontWeight: labelWeight,\r\n            fontFamily: labelFont,\r\n            fontSize: labelFontSize,\r\n            formatter: function (val) {\r\n                const d = new Date(val);\r\n                let yr=d.getFullYear();\r\n                let mn=d.getMonth()+1;\r\n                let dy=d.getDate();\r\n                let h = (\"0\" + d.getHours()).slice(-2);\r\n                let m = (\"0\" + d.getMinutes()).slice(-2);\r\n                let s = (\"0\" + d.getSeconds()).slice(-2);\r\n                let x = \" AM\";\r\n                if (h >= 12) {\r\n                    x = \" PM\";\r\n                }\r\n                if (h > 12) {\r\n                    h = h - 12;\r\n                }\r\n                let time = mn+\"/\"+dy+\"/\"+yr+\" \" + h + \":\" + m + \":\" + s + x;\r\n                return time;\r\n            }\r\n        }\r\n    },\r\n    yAxis: {\r\n        axisLine: {\r\n            show: false,\r\n        },\r\n        axisTick: {\r\n            show: false,\r\n        },\r\n        data: categories\r\n    },\r\n    series:\r\n    [{\r\n            type: 'custom',\r\n            renderItem: renderItem,\r\n            itemStyle: {\r\n                opacity: 1\r\n            },\r\n            encode: {\r\n                x: [1, 2],\r\n                y: 0\r\n            },\r\n            data: data\r\n        }\r\n    ],\r\n};\r\n\r\nif (option && typeof option === 'object') {\r\n    myChart.setOption(option);\r\n}\r\n\r\nwindow.addEventListener('resize', myChart.resize);";
        htmlBody = "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"height: 100%\">\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n</head>\r\n<body style=\"height: 100%; margin: 0\">\r\n\t<div id=\"container\" style=\"height: 100%\"></div>\r\n\r\n\t<script type=\"text/javascript\" src=\"../jquery.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"../echarts.min.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"./" + ImageID + ".js\"></script>\r\n\r\n</body>\r\n</html>";
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon JS Strings Completed");
        }
    }

    public void writeOutputs()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", "History Ribbon Write JS Strings to files");
        }
        System.IO.StreamWriter jsFile = new System.IO.StreamWriter(jsFilePath, true);
        jsFile.Flush();
        jsFile.WriteLine(js_Header);
        jsFile.WriteLine(js_Data);
        jsFile.WriteLine(js_VarList1);
        jsFile.WriteLine(js_VarList2);
        jsFile.WriteLine(js_eChartsOptions);
        jsFile.Close();
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", jsFilePath + " History Ribbon File Write Complete");
        }
        System.IO.StreamWriter htmlFile = new System.IO.StreamWriter(htmlFilePath, true);
        htmlFile.Flush();
        htmlFile.WriteLine(htmlBody);
        htmlFile.Close();

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Hist Ribbon Debug", htmlFilePath + " History Ribbon File Write Complete");
        }
    }

    #endregion

    #region MAIN METHODS

    public override void Start()
    {
        Owner.Get<Image>("GraphicPlaceholder").Visible = false;
        Owner.Get<Image>("GraphicPlaceholder").Enabled = false;
        ImageID = "HR_" + Guid.NewGuid().ToString().ToUpper();
        Owner.GetVariable("ImageID").Value = ImageID;
        buildChart();

        //Owner.GetVariable("RefreshDataWidgets").VariableChange += buildChart_VariableChange;
        Owner.GetVariable("RefreshDataWidgets").VariableChange += buildChart_VariableChange;
    }

    public override void Stop()
    {
        // remove bind to refresh value
        //Owner.GetVariable("RefreshDataWidgets").VariableChange -= buildChart_VariableChange;
        Owner.GetVariable("RefreshDataWidgets").VariableChange += buildChart_VariableChange;
        CoreFunctions.RemoveGUIDFiles(jsFilePath);
        CoreFunctions.RemoveGUIDFiles(htmlFilePath);
    }

    public void buildChart_VariableChange(object sender, VariableChangeEventArgs e)
    {
        buildChart();
    }

    [ExportMethod]
    public void buildChart()
    {
        loadValues();

        getLookupLists();

        readDatabase();

        if (resultsData.GetLength(0) <= 1)
        {
            Owner.Get<Label>("Label2").Visible = true;
            Owner.Get<Label>("Label2").TextColor = Owner.GetVariable("FontColor").Value;
            Owner.Get<WebBrowser>("WebWidget").Visible = false;
        }
        if (resultsData.GetLength(0) > 1)
        {
            dataStaging();

            createStrings();

            CoreFunctions.RemoveGUIDFiles(jsFilePath);
            CoreFunctions.RemoveGUIDFiles(htmlFilePath);

            CoreFunctions.CreateGUIDFiles(jsFilePath);
            CoreFunctions.CreateGUIDFiles(htmlFilePath);

            //createGUIDFiles();

            writeOutputs();

            Owner.Get<Label>("Label2").Visible = false;
            Owner.Get<WebBrowser>("WebWidget").Visible = true;
            Owner.Get<WebBrowser>("WebWidget").URL = htmlFileUri;
            Owner.Get<WebBrowser>("WebWidget").Refresh();

        }

    }


    #endregion
}
