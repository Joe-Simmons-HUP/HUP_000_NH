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

using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.OPCUAServer;
using FTOptix.RAEtherNetIP;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.UI;
using FTOptix.WebUI;
using OpcUa = UAManagedCore.OpcUa;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UAManagedCore;
using FTOptix.EventLogger;
using FTOptix.OPCUAClient;
using FTOptix.Report;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using FTOptix.Alarm;

#endregion

public class DR_InstanceMaintEventsLogic : BaseNetLogic
{
    #region DECLARE VARIABLES

    private Boolean DebugEnable;
    //private Boolean RefreshRequest;
    private String ImageID;
    //private String DatabaseBrowsePath;
    private Int32 ModelNameEnum;
    private string ModelName;
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
    private String TranslationsBrowsePath;
    private String MaintEventsColorsBrowsePath;
    private Int32 BarHeight;
    private String TimeSliderEnable;
    private Int32 TimeSliderOffset;
    private Int32 LabelWidth;
    private Int32 SeverityCnt;
    private Int32 DescCnt;
    private String Locale;
    private string[] fontList = { "arial", "verdana", "tahoma", "trebuchet ms", "times new roman", "georgia", "garamond", "courier new", "brush script mt" };
    private string[] fontWeightList = { "normal", "bold", "bolder", "lighter" };

    private string projectName;
    //private string databaseName;
    private Store dataStore;
    private Object[,] resultsData;
    private Object[,] resultsInstances;
    private string[] resultsHeader;
    private string[] resultsHeader2;
    private string queryWhere;
    private string instanceQuery;
    private string dataQuery;
    private string theInstances;
    private string theData;
    private Int32 i;
    private Int32 j;
    private Int32 index;
    private Int32 length;
    private String translationPathName;
    //private String MaintEventsColorsPathName;
    private List<string> severityList;
    private List<string> descriptionList;
    private LocalizedText discKey;
    private List<string> locationID;
    private string translatedText;

    private CoreFunctions CoreFunctions;

    private string js_Header;
    private string js_Data;
    private string js_VarList1;
    private string js_VarList2;
    private string js_eChartsOptions;
    private string htmlBody;
    private string projPath;
    private string jsFilePath;
    private string htmlFilePath;
    private string htmlFileUri;

    private struct dataSet
    {
        public Int32 Index;
        public string Instance;
        public string Severity;
        public string Desc;
        public DateTime StartTime;
        public DateTime Timestamp;
        public float Duration;
        public string SeverityColor;
    };

    #endregion

    #region SUPPORTING METHODS

    private void loadValues()
    {
        DebugEnable = Owner.GetVariable("DebugEnable").Value;

        if (DebugEnable)
        {
            Log.Info("History Ribbon Debug Enabled: " + DebugEnable);
            Log.Info("History Ribbon Loading Values");
        }

        CoreFunctions = new CoreFunctions();

        NodeId RefNodeID = Owner.GetVariable("Model").Value;
        IUAObject MaintEventsInstance = InformationModel.GetObject(RefNodeID);
        string FullPath = "";

        CoreFunctions.GetOwnerPath("InformationModel", MaintEventsInstance as IUANode, MaintEventsInstance.BrowseName, out FullPath);
        string[] InstancePathArray = FullPath.Split("/");
        ModelName = InstancePathArray[0];
        InstanceName = FullPath;

        ImageID = Owner.GetVariable("ImageID").Value;

        ModelNameEnum = Owner.GetVariable("ModelName").Value;
        IUANode EnumObj = InformationModel.Get(Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumLists/InformationModelInstances").NodeId);
        CoreFunctions.GetEnumText(ModelNameEnum, EnumObj, out ModelName);

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
        TranslationsBrowsePath = "raLib_Core/DataReady_Core_V01_00_0106/EnumTranslations/raI_01_00_0106_Translations";
        MaintEventsColorsBrowsePath = "raI_01_00_0106_MaintEvents/ObjectTypes/MaintEventsColors";
        BarHeight = Owner.GetVariable("BarHeight").Value;
        TimeSliderEnable = Owner.GetVariable("TimeSliderEnable").Value.ToString().ToLower().Replace(" (bool)", "");
        TimeSliderOffset = Owner.GetVariable("TimeSliderOffset").Value;
        LabelWidth = Owner.GetVariable("LabelWidth").Value;
        SeverityCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/MESeverity").Value;
        DescCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/MEDescription").Value;
        Locale = Owner.GetVariable("Locale").Value;
        translationPathName = TranslationsBrowsePath.Replace("Root/Data/" + projectName + "/Translation/", "");

        projectName = Project.Current.BrowseName;
        projPath = Project.Current.ProjectDirectory;
        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/MaintEvents/Databases/MaintEventsDatabase");

        jsFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".js";
        htmlFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".html";
        htmlFileUri = ResourceUri.FromProjectRelativePath("DataReady/eCharts/Runtime/" + ImageID + ".html");

        Owner.GetVariable("ImageID").Value = ImageID;

        if (DebugEnable)
        {
            Log.Info("History Ribbon Loading Values Complete");
        }
    }

    private static string GetFQNForDataTag(IUANode Node, string FQN, string guid)
    {
        if (Node.Owner.BrowseName != guid)
        {
            //Log.Info("FQN !=:  Owner="+ Node.Owner.BrowseName);
            FQN = Node.Owner.BrowseName + "/" + FQN;
            FQN = GetFQNForDataTag(Node.Owner, FQN, guid);
        }
        return FQN;
    }

    public string colorString(Color color)
    {
        if (DebugEnable)
        {
            Log.Info("MaintEvents Ribbon Converting Color");
        }
        string updatedColor = "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");

        if (DebugEnable)
        {
            Log.Info("MaintEvents Ribbon Color Convertion: " + Convert.ToString(color.ARGB) + " Color String: " + updatedColor);
        }
        return updatedColor;
    }

    private void getLookupLists()
    {
        severityList = new List<string>();
        descriptionList = new List<string>();

        locationID = new List<string>();

        severityList.Add("Not Used");
        descriptionList.Add("Not Used");

        if (DebugEnable)
        {
            Log.Info("Locale: " + Locale);
        }

        locationID.Add(Locale);
        i = 1;
        while (i <= SeverityCnt)
        {
            discKey = new LocalizedText(LogicObject.NodeId.NamespaceIndex, "MESeverity" + i);
            translatedText = InformationModel.LookupTranslation(discKey, locationID).Text;
            severityList.Add(translatedText);
            if (DebugEnable)
            {
                Log.Info("Severity: " + i + "  " + translatedText);
            }
            i++;
        }
        i = 1;
        while (i <= DescCnt)
        {
            discKey = new LocalizedText(LogicObject.NodeId.NamespaceIndex, "MEDescription" + i);
            translatedText = InformationModel.LookupTranslation(discKey, locationID).Text;
            descriptionList.Add(translatedText);
            if (DebugEnable)
            {
                Log.Info("Description: " + i + "  " + translatedText);
            }
            i++;
        }
    }

    private void readDatabase()
    {
        if (DebugEnable)
        {
            Log.Info("Read Database for Categories");
        }
        queryWhere = "(MachineGUID = '" + ModelName + "') AND (InstanceName = '" + InstanceName + "') AND (Severity<>'0') AND (Timestamp >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (Timestamp <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";
        if (DebugEnable)
        {
            Log.Info("Query WHERE: " + queryWhere);
        }
        instanceQuery = "SELECT InstanceName FROM raI_01_00_0106_MaintEvents WHERE " + queryWhere + " ORDER BY InstanceName";
        dataQuery = "SELECT StartTime, Timestamp, InstanceName, Severity, Description, Duration, SeverityColor FROM raI_01_00_0106_MaintEvents WHERE " + queryWhere + " ORDER BY InstanceName, Timestamp";
        if (DebugEnable == true)
        {
            Log.Info("Category Query: " + instanceQuery);
            Log.Info("Data Query: " + dataQuery);
        }
        dataStore.Query(instanceQuery, out resultsHeader, out resultsInstances);
        dataStore.Query(dataQuery, out resultsHeader2, out resultsData);
        if (DebugEnable == true)
        {
            Log.Info("Query Result Length: " + resultsData.GetLength(0) + "  Query Result Width: " + resultsData.GetLength(1));
        }
    }

    private void dataStaging()
    {
        //Need to add code to create list of instances and map into resultSet

        //Stage Instance info first.

        i = 0;

        string[] notifyInstances = new string[resultsInstances.GetLength(0)];
        while (i < resultsInstances.GetLength(0))
        {
            notifyInstances[i] = resultsInstances[i, 0].ToString();

            if (DebugEnable)
            {
                Log.Info("Notify Instances: " + resultsInstances[i, 0].ToString());
            }

            i++;
        }

        string[] instanceList = notifyInstances.Distinct().ToArray();

        if (DebugEnable)
        {
            Log.Info("Notify Category Distinct Cnt: " + instanceList.GetLength(0));
        }

        i = 0;
        theInstances = "";
        theInstances = "[";
        while (i < instanceList.GetLength(0))
        {
            theInstances = theInstances + "'" + instanceList[i] + "',";
            i = i + 1;

            if (DebugEnable)
            {
                Log.Info("Notify Categories: " + theInstances);
            }
        }
        theInstances = theInstances + "]";
        if (DebugEnable)
        {
            Log.Info("Notify Categories: " + theInstances);
        }
        theInstances = "['']";

        //

        dataSet resultSet = new dataSet();
        dataSet[] resultList = new dataSet[resultsData.GetLength(0)];
        index = 0;
        i = 0;
        j = 0;

        resultSet.Index = index;
        resultSet.Instance = Convert.ToString(resultsData[i, 2]);
        resultSet.Severity = severityList[Convert.ToInt32(resultsData[i, 3])];
        resultSet.Desc = descriptionList[Convert.ToInt32(resultsData[i, 4])];
        resultSet.StartTime = Convert.ToDateTime(resultsData[i, 0]);
        resultSet.Timestamp = Convert.ToDateTime(resultsData[i, 1]);
        resultSet.Duration = Convert.ToSingle(resultsData[i, 5]);
        resultSet.SeverityColor = Convert.ToString(resultsData[i, 6]);
        resultList[i] = resultSet;
        i = i + 1;


        while (i < resultsData.GetLength(0))
        {

            if (resultSet.Instance != Convert.ToString(resultsData[i, 2]))
            {
                index = index + 1;
            }
            resultSet.Index = index;
            resultSet.Instance = Convert.ToString(resultsData[i, 2]);
            resultSet.Severity = severityList[Convert.ToInt32(resultsData[i, 3])];
            resultSet.Desc = descriptionList[Convert.ToInt32(resultsData[i, 4])];
            resultSet.StartTime = Convert.ToDateTime(resultsData[i, 0]);
            resultSet.Timestamp = Convert.ToDateTime(resultsData[i, 1]);
            resultSet.Duration = Convert.ToSingle(resultsData[i, 5]);
            resultSet.SeverityColor = Convert.ToString(resultsData[i, 6]);
            resultList[i] = resultSet;
            i = i + 1;
        }

        i = 0;
        j = 0;

        length = resultList.GetLength(0);

        theData = "[\r\n";
        while (j < length - 1)
        {
            theData = theData + "{index: " + resultList[j].Index + ", state: '" + resultList[j].Severity + ":  " + resultList[j].Desc + "', starttime: '" + resultList[j].StartTime + "', endtime: '" + resultList[j].Timestamp + "', duration: '" + resultList[j].Duration + "', color: '" + resultList[j].SeverityColor + "'},\r\n";
            j = j + 1;
        }
        theData = theData + "{index: " + resultList[j].Index + ", state: '" + resultList[j].Severity + ":  " + resultList[j].Desc + "', starttime: '" + resultList[j].StartTime + "', endtime: '" + resultList[j].Timestamp + "', duration: '" + resultList[j].Duration + "', color: '" + resultList[j].SeverityColor + "'}\r\n]";

        BarHeight = BarHeight * instanceList.GetLength(0);
    }

    public void createStrings()
    {
        if (DebugEnable)
        {
            Log.Info("History Ribbon Creating JS Strings");
        }
        js_Header = "var dom = document.getElementById('container');\r\nvar myChart = echarts.init(dom, null, {\r\n    renderer: 'canvas',\r\n    useDirtyRect: false\r\n});\r\nvar app = {};\r\nvar option;\r\nvar data = [];\r\nvar startTime = +new Date();\r\nvar categories = " + theInstances + ";";
        js_Data = "var stateEvent = " + theData + ";";
        js_VarList1 = "var backGroundColor = '" + BackGroundColor + "';\r\nvar fontColor = '" + FontColor + "';\r\nvar titleEnable = " + TitleEnable + ";\r\nvar titleText = '" + TitleText + "';\r\nvar titleTop = " + TitleTop + ";\r\nvar titleFontSize = " + TitleFontSize + ";\r\nvar titleFont = '" + TitleFont + "';\r\nvar titleWeight = '" + TitleFontWeight + "';\r\nvar labelFontSize = " + LabelFontSize + ";\r\nvar labelFont = '" + LabelFont + "';\r\nvar labelWeight = '" + LabelFontWeight + "';\r\nvar gridTopSpacing=" + GridTopSpacing + ";\r\nvar gridBottomSpacing=" + GridBottomSpacing + ";\r\nvar gridLeftSpacing=" + GridLeftSpacing + ";\r\nvar gridRightSpacing=" + GridRightSpacing + ";\r\n";
        js_VarList2 = "var barHeight = " + BarHeight + ";\r\nvar sliderEnable = '" + TimeSliderEnable + "';\r\nvar sliderOffset = " + TimeSliderOffset + ";\r\nvar labelWidth = " + LabelWidth + ";\r\n";
        js_eChartsOptions = "var dataCount = stateEvent.length;\r\n\r\ncategories.forEach(function (category, index) {\r\n    for (var i = 0; i < dataCount; i++) {\r\n        var stateItem = stateEvent[i];\r\n        const indexid = stateItem.index;\r\n        const datetempstart = new Date(stateItem.starttime);\r\n        var msecstart = datetempstart.getTime();\r\n        const datetempend = new Date(stateItem.endtime);\r\n        var msecend = datetempend.getTime();\r\n        data.push({\r\n            name: stateItem.state,\r\n            value: [indexid, msecstart, msecend, stateItem.duration * 60000],\r\n            itemStyle: {\r\n                normal: {\r\n                    color: stateItem.color\r\n                }\r\n            }\r\n        });\r\n    }\r\n});\r\n\r\nfunction renderItem(params, api) {\r\n    var categoryIndex = api.value(0);\r\n    var start = api.coord([api.value(1), categoryIndex]);\r\n    var end = api.coord([api.value(2), categoryIndex]);\r\n    var height = api.size([0, 1])[1] * .8; //scales the bar to the grid height\r\n    var rectShape = echarts.graphic.clipRectByRect({\r\n        x: start[0],\r\n        y: start[1] - height / 2,\r\n        width: end[0] - start[0],\r\n        height: height\r\n    }, {\r\n        x: params.coordSys.x,\r\n        y: params.coordSys.y,\r\n        width: params.coordSys.width,\r\n        height: params.coordSys.height\r\n    });\r\n    return (\r\n        rectShape && {\r\n        type: 'rect',\r\n        transition: ['shape'],\r\n        shape: rectShape,\r\n        style: api.style()\r\n    });\r\n}\r\n\r\noption = {\r\n    backgroundColor: backGroundColor,\r\n    tooltip: {\r\n        formatter: function (params) {\r\n            return params.marker + params.name + ': ' + ((params.value[3] / 3600000).toFixed(3)) + ' Min';\r\n        }\r\n    },\r\ndataZoom:\r\n    [{\r\n\r\n            show: sliderEnable,\r\n            type: 'slider',\r\n            filterMode: 'weakFilter',\r\n            showDataShadow: false,\r\n            //bottom: gridBottomSpacing-75,\r\n            top: gridTopSpacing + barHeight + sliderOffset,\r\n            labelFormatter: ''\r\n        }, {\r\n            type: 'inside',\r\n            filterMode: 'weakFilter'\r\n        },\r\n    ],\r\n    grid: {\r\n        top: gridTopSpacing,\r\n        height: barHeight,\r\n        left: labelWidth,\r\n        right: gridRightSpacing\r\n    },\r\n    xAxis: {\r\n        min: Date(stateEvent[0].starttime).getTime,\r\n        scale: true,\r\n        axisTick: {\r\n            show: true,\r\n            inside: false,\r\n            length: 5,\r\n            lineStyle: {\r\n                color: fontColor,\r\n                width: 2\r\n            },\r\n        },\r\n        axisLabel: {\r\n            color: fontColor,\r\n            hideOverlap: true,\r\n            fontWeight: labelWeight,\r\n            fontFamily: labelFont,\r\n            fontSize: labelFontSize,\r\n            formatter: function (val) {\r\n                const d = new Date(val);\r\n                let yr=d.getFullYear();\r\n                let mn=d.getMonth()+1;\r\n                let dy=d.getDate();\r\n                let h = (\"0\" + d.getHours()).slice(-2);\r\n                let m = (\"0\" + d.getMinutes()).slice(-2);\r\n                let s = (\"0\" + d.getSeconds()).slice(-2);\r\n                let x = \" AM\";\r\n                if (h >= 12) {\r\n                    x = \" PM\";\r\n                }\r\n                if (h > 12) {\r\n                    h = h - 12;\r\n                }\r\n                let time = mn+\"/\"+dy+\"/\"+yr+\" \" + h + \":\" + m + \":\" + s + x;\r\n                return time;\r\n            }\r\n        }\r\n    },\r\n    yAxis: {\r\n        axisLine: {\r\n            show: false,\r\n        },\r\n        axisTick: {\r\n            show: false,\r\n        },\r\n        data: categories,\r\n        axisLabel: {\r\n\twidth: labelWidth,\r\n\tcolor: fontColor,\r\n\tfontWeight: labelWeight,\r\n\tfontFamily: labelFont,\r\n\toverflow: 'break' ,\r\n\tfontSize: labelFontSize\r\n        }\r\n    },\r\n    series:\r\n    [{\r\n            type: 'custom',\r\n            renderItem: renderItem,\r\n            itemStyle: {\r\n                opacity: 1\r\n            },\r\n            encode: {\r\n                x: [1, 2],\r\n                y: 0\r\n            },\r\n            data: data\r\n        }\r\n    ],\r\n};\r\n\r\nif (option && typeof option === 'object') {\r\n    myChart.setOption(option);\r\n}\r\n\r\nwindow.addEventListener('resize', myChart.resize);";
        htmlBody = "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"height: 100%\">\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n</head>\r\n<body style=\"height: 100%; margin: 0\">\r\n\t<div id=\"container\" style=\"height: 100%\"></div>\r\n\r\n\t<script type=\"text/javascript\" src=\"../jquery.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"../echarts.min.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"./" + ImageID + ".js\"></script>\r\n\r\n</body>\r\n</html>";
        if (DebugEnable)
        {
            Log.Info("History Ribbon JS Strings Completed");
        }
    }

    public void writeOutputs()
    {
        if (DebugEnable)
        {
            Log.Info("History Ribbon Write JS Strings to files");
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
            Log.Info(jsFilePath + " History Ribbon File Write Complete");
        }
        System.IO.StreamWriter htmlFile = new System.IO.StreamWriter(htmlFilePath, true);
        htmlFile.Flush();
        htmlFile.WriteLine(htmlBody);
        htmlFile.Close();

        if (DebugEnable)
        {
            Log.Info(htmlFilePath + " History Ribbon File Write Complete");
        }
    }

    #endregion

    #region MAIN METHODS

    public override void Start()
    {
        Owner.Get<Image>("ScrollView1/GraphicPlaceholder").Visible = false;
        Owner.Get<Image>("ScrollView1/GraphicPlaceholder").Enabled = false;

        ImageID = "ME_" + Guid.NewGuid().ToString().ToUpper();

        Owner.GetVariable("ImageID").Value = ImageID;
        buildChart();
        //Owner.GetVariable("RefreshRequest").VariableChange += buildChart_VariableChange;
        Owner.GetVariable("RefreshDataWidgets").VariableChange += buildChart_VariableChange;
    }

    public override void Stop()
    {
        Owner.GetVariable("RefreshDataWidgets").VariableChange -= buildChart_VariableChange;

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
            Owner.Get<WebBrowser>("ScrollView1/WebWidget").Visible = false;
        }
        if (resultsData.GetLength(0) > 1)
        {
            dataStaging();

            createStrings();

            CoreFunctions.RemoveGUIDFiles(jsFilePath);
            CoreFunctions.RemoveGUIDFiles(htmlFilePath);

            CoreFunctions.CreateGUIDFiles(jsFilePath);
            CoreFunctions.CreateGUIDFiles(htmlFilePath);

            writeOutputs();

            Owner.Get<Label>("Label2").Visible = false;
            Owner.Get<WebBrowser>("ScrollView1/WebWidget").Visible = true;
            Owner.Get<WebBrowser>("ScrollView1/WebWidget").URL = htmlFileUri;
            Owner.Get<WebBrowser>("ScrollView1/WebWidget").Refresh();

        }

    }

    #endregion
}
