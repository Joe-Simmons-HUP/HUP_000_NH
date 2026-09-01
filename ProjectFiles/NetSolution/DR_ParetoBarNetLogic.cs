/*
 * ***** Warning *****
DO NOT EDIT!  Edits to this script may cause this script to fail.  
 
=============================================================
 
Disclaimer of Warranty
THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT ARE PROVIDED "AS IS" WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ALL IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, NON-INFRINGEMENT OR OTHER VIOLATION OF RIGHTS. ROCKWELL AUTOMATION DOES NOT WARRANT OR MAKE ANY REPRESENTATIONS REGARDING THE USE, VALIDITY, ACCURACY, OR RELIABILITY OF, OR THE RESULTS OF ANY USE OF, OR OTHERWISE RESPECTING, THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT OR ANY WEB SITE LINKED TO THIS DOCUMENT
 
Limitation of Liability
UNDER NO CIRCUMSTANCE (INCLUDING NEGLIGENCE AND TO THE FULLEST EXTEND PERMITTED BY APPLICABLE LAW) WILL ROCKWELL AUTOMATION BE LIABLE FOR ANY DIRECT, INDIRECT, SPECIAL, INCIDENTAL, PUNITIVE OR CONSEQUENTIAL DAMAGES (INCLUDING WITHOUT LIMITATION, BUSINESS INTERRUPTION, DELAYS, LOSS OF DATA OR PROFIT) ARISING OUT OF THE USE OR THE INABILITY TO USE THE MATERIALS PROVIDED OR REFERENCED BY WAY OF THIS DOCUMENT EVEN IF ROCKWELL AUTOMATION HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES. IF USE OF SUCH MATERIALS RESULTS IN THE NEED FOR SERVICING, REPAIR OR CORRECTION OF USER EQUIPMENT OR DATA, USER ASSUMES ANY COSTS ASSOCIATED THEREWITH.
 
Copyright � Rockwell Automation, Inc.  All Rights Reserved.
 
=============================================================
*/

#region Using directives

using UAManagedCore;
using System;
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

public class DR_ParetoBarNetLogic : BaseNetLogic
{
    #region DECLARE VARIABLES

    private Boolean DebugEnable;
    //private Boolean RefreshRequest;
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
    private Int32 BarTop;
    private Int32 BarHeight;
    private Int32 FilterStateValue;
    private Int32 FilterState2;
    private Int32 FilterState3;
    private Int32 FilterState4;
    private Int32 FilterState5;
    private Int32 ModeCnt;
    private Int32 StateCnt;
    private Int32 ReasonCodeCnt;
    private Int32 CategoryCnt;
    private String TranslationsBrowsePath;
    private String[] LocaleList;
    private string Locale;

    private string[] fontList = { "arial", "verdana", "tahoma", "trebuchet ms", "times new roman", "georgia", "garamond", "courier new", "brush script mt" };
    private string[] fontWeightList = { "normal", "bold", "bolder", "lighter" };

    private string projectName;
    private Store dataStore;
    private Object[,] resultsData;
    private string[] resultsHeader;
    private string queryWhere;
    private string dataQuery;
    private string theData;
    private string theCategories;
    private Int32 i;
    private String translationName;
    private List<string> categoryList;
    private List<string> reasonCodeList;
    private List<string> stateList;
    private List<string> modeList;
    private LocalizedText discKey;
    private List<string> locationID;
    private string translatedText;
    private CoreFunctions CoreFunctions;

    private string js_Header;
    private string js_Data;
    private String js_Categories;
    private string js_VarList1;
    private string js_VarList2;
    private string js_eChartsOptions;
    private string htmlBody;
    private string projPath;
    private string jsFilePath;
    private string htmlFilePath;
    private string htmlFileUri;

    #endregion

    #region SUPPORTING METHODS

    private void loadValues()
    {
        DebugEnable = Owner.GetVariable("DebugEnable").Value;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Debug Enabled: " + DebugEnable);
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Loading Values");
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
        BarTop = Owner.GetVariable("BarTop").Value;
        BarHeight = Owner.GetVariable("BarHeight").Value;
        FilterStateValue = Owner.GetVariable("FilterStateValue").Value;
        FilterState2 = Owner.GetVariable("FilterState2").Value;
        FilterState3 = Owner.GetVariable("FilterState3").Value;
        FilterState4 = Owner.GetVariable("FilterState4").Value;
        FilterState5 = Owner.GetVariable("FilterState5").Value;
        ModeCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEMode").Value;
        StateCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEState").Value;
        ReasonCodeCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEEReasonCode").Value;
        CategoryCnt = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/OEECategory").Value;
        Locale = Owner.GetVariable("Locale").Value;

        projectName = Project.Current.BrowseName;
        TranslationsBrowsePath = "raLib_Core/DataReady_Core_V01_00_0106/EnumTranslations/raI_01_00_0106_Translations";
        translationName = TranslationsBrowsePath.Replace("Root/Data/" + projectName + "/raLib_Core/DataReady_Core_V01_00_0106/EnumTranslations/", "");

        projPath = Project.Current.ProjectDirectory;
        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/OEE/Databases/OEEDatabase");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "OEE Datastore: " + dataStore.BrowseName);
        }

        jsFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".js";
        htmlFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".html";
        htmlFileUri = ResourceUri.FromProjectRelativePath("DataReady/eCharts/Runtime/" + ImageID + ".html");

        Owner.GetVariable("ImageID").Value = ImageID;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Loading Values Complete");
        }
    }

    public string colorString(Color color)
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Converting Color");
        }
        string updatedColor = "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Color Convertion: " + Convert.ToString(color.ARGB) + " Color String: " + updatedColor);
        }
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
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Read Database");
        }
        queryWhere = "(MachineGUID = '" + ModelName + "') AND (State<>'" + FilterStateValue + "') AND (State<>'" + FilterState2 + "') AND (State<>'" + FilterState3 + "') AND (State<>'" + FilterState4 + "') AND (State<>'" + FilterState5 + "') AND (StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (StartTime < '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Query WHERE: " + queryWhere);
        }
        dataQuery = "SELECT MachineGUID, State, ReasonCode, SUM(Duration) AS TotalDuration, StateColor FROM raI_01_00_0106_OEE WHERE " + queryWhere + "  GROUP BY ReasonCode, MachineGUID, StateColor ORDER BY TotalDuration DESC LIMIT 10";
        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Query: " + dataQuery);
        }
        dataStore.Query(dataQuery, out resultsHeader, out resultsData);
        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Query Result Length: " + resultsData.GetLength(0));
        }
    }

    private void dataStaging()
    {
        i = 0;
        theCategories = "";
        theData = "";
        while (i < resultsData.GetLength(0) - 1)
        {
            theCategories = theCategories + "'" + reasonCodeList[Convert.ToInt32(resultsData[i, 2])] + "',\r\n ";
            i = i + 1;
        }
        theCategories = theCategories + "'" + reasonCodeList[Convert.ToInt32(resultsData[i, 2])] + "'\r\n ";

        i = 0;
        while (i < resultsData.GetLength(0) - 1)
        {
            theData = theData + "{value: " + (Convert.ToSingle(resultsData[i, 3]) / 60 + .000001) + ".toFixed(3), itemStyle: {color: '" + Convert.ToString(resultsData[i, 4]) + "'}, name: '" + reasonCodeList[Convert.ToInt32(resultsData[i, 2])] + "'},";
            i = i + 1;
        }
        theData = theData + "{value: " + (Convert.ToSingle(resultsData[i, 3]) / 60 + .000001) + ".toFixed(3), itemStyle: {color: '" + Convert.ToString(resultsData[i, 4]) + "'}, name: '" + reasonCodeList[Convert.ToInt32(resultsData[i, 2])] + "'}";
    }

    public void createStrings()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Creating JS Strings");
        }
        js_Header = "var dom = document.getElementById('container');\r\nvar myChart = echarts.init(dom, null, {\r\n    renderer: 'canvas',\r\n    useDirtyRect: false\r\n});\r\nvar app = {};\r\nvar option;\r\nvar data = [];\r\nvar startTime = +new Date();\r\nvar categories = [''];";
        js_Data = "var theData = [" + theData + "];";
        js_Categories = "var theLabels = [" + theCategories + "];";
        js_VarList1 = "var backGroundColor = '" + BackGroundColor + "';\r\nvar fontColor = '" + FontColor + "';\r\nvar titleEnable = " + TitleEnable + ";\r\nvar titleTop = " + TitleTop + ";\r\nvar titleFontSize = " + TitleFontSize + ";\r\nvar titleFont = '" + TitleFont + "';\r\nvar titleWeight = '" + TitleFontWeight + "';\r\nvar labelFontSize = " + LabelFontSize + ";\r\nvar labelFont = '" + LabelFont + "';\r\nvar labelWeight = '" + LabelFontWeight + "';\r\nvar gridTopSpacing=" + GridTopSpacing + ";\r\nvar gridBottomSpacing=" + GridBottomSpacing + ";\r\nvar gridLeftSpacing=" + GridLeftSpacing + ";\r\nvar gridRightSpacing=" + GridRightSpacing + ";\r\n";
        js_VarList2 = "var titleText = '" + TitleText + "';\r\nvar units ='" + Units + "';\r\nvar BarHeight = " + BarHeight + ";";
        js_eChartsOptions = "option = {\r\n    backgroundColor: backGroundColor,\r\n    title: {\r\n        show: titleEnable,\r\n        top: titleTop,\r\n        left: 'center',\r\n        text: titleText+' ('+units+')',\r\n        textStyle: {\r\n            color: fontColor,\r\n            fontWeight: titleWeight,\r\n            fontFamily: titleFont,\r\n            fontSize: titleFontSize\r\n        },\r\n    },\r\n    xAxis: {\r\n        type: 'value',\r\n        splitNumber: 1,\r\n        name: units,\r\n        nameLocation: 'middle' ,\r\n        nameTextStyle: {\r\n          color: fontColor,\r\n          fontWeight: labelWeight ,\r\n          fontFamily: labelFont ,\r\n          fontSize: labelFontSize,\r\n          verticalAlign: 'top',\r\n          lineHeight: 50,\r\n          align:'center'\r\n        },\r\n        axisLabel: {\r\n            color: fontColor,\r\n            fontWeight: labelWeight,\r\n            fontFamily: labelFont,\r\n            fontSize: labelFontSize,\r\n            overflow: 'truncate',\r\n            formatter: function (value) {\r\n             return value.toFixed(1);\r\n            }\r\n        },\r\n    },\r\n    yAxis: {\r\n        type: 'category',\r\n        axisLabel: {\r\n            color: fontColor,\r\n            fontWeight: labelWeight,\r\n            fontFamily: labelFont,\r\n            fontSize: labelFontSize,\r\n            width: gridLeftSpacing,\r\n            overflow: 'truncate'\r\n        },\r\n        inverse: true,\r\n        data: theLabels\r\n    },\r\n    tooltip: {\r\n        show: true,\r\n        formatter: '{b0}: {c0}'+\" \"+units\r\n    },\r\n    grid: {\r\n        top: gridTopSpacing,\r\n        bottom: gridBottomSpacing,\r\n        left: gridLeftSpacing,\r\n        right: gridRightSpacing\r\n    },\r\n    series:\r\n    [{\r\n            type: 'bar',\r\n            barWidth: BarHeight,\r\n            itemStyle: {\r\n                borderRadius: 15,\r\n                borderColor: '#fff',\r\n                borderWidth: 2\r\n            },\r\n            emphasis: {\r\n                itemStyle: {\r\n                    shadowBlur: 10,\r\n                    shadowOffsetX: 0,\r\n                    shadowColor: 'rgba(0, 0, 0, 0.5)'\r\n                }\r\n            },\r\n            data: theData\r\n        }\r\n    ]\r\n};\r\n\r\nif (option && typeof option === 'object') {\r\n    myChart.setOption(option);\r\n};\r\n\r\nwindow.addEventListener('resize', myChart.resize);";
        htmlBody = "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"height: 100%\">\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n</head>\r\n<body style=\"height: 100%; margin: 0\">\r\n\t<div id=\"container\" style=\"height: 100%\"></div>\r\n\r\n\t<script type=\"text/javascript\" src=\"../jquery.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"../echarts.min.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"./" + ImageID + ".js\"></script>\r\n\r\n</body>\r\n</html>";
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar JS Strings Completed");
        }
    }

    public void writeOutputs()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", "Pareto Bar Write JS Strings to files");
        }
        System.IO.StreamWriter jsFile = new System.IO.StreamWriter(jsFilePath, true);
        jsFile.Flush();
        jsFile.WriteLine(js_Header);
        jsFile.WriteLine(js_Data);
        jsFile.WriteLine(js_Categories);
        jsFile.WriteLine(js_VarList1);
        jsFile.WriteLine(js_VarList2);
        jsFile.WriteLine(js_eChartsOptions);
        jsFile.Close();
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", jsFilePath + " Pareto Bar File Write Complete");
        }
        System.IO.StreamWriter htmlFile = new System.IO.StreamWriter(htmlFilePath, true);
        htmlFile.Flush();
        htmlFile.WriteLine(htmlBody);
        htmlFile.Close();

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Pareto Bar Debug", htmlFilePath + " Pareto Bar File Write Complete");
        }
    }

    #endregion

    #region MAIN METHODS

    public override void Start()
    {
        Owner.Get<Image>("GraphicPlaceholder").Visible = false;
        Owner.Get<Image>("GraphicPlaceholder").Enabled = false;

        ImageID = "PBC_" + Guid.NewGuid().ToString().ToUpper();

        Owner.GetVariable("ImageID").Value = ImageID;
        buildChart();
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
        if (resultsData.GetLength(0) <= 0)
        {
            Owner.Get<Label>("Label2").Visible = true;
            Owner.Get<Label>("Label2").TextColor = Owner.GetVariable("FontColor").Value;
            Owner.Get<WebBrowser>("WebWidget").Visible = false;
        }
        if (resultsData.GetLength(0) > 0)
        {
            dataStaging();

            createStrings();

            CoreFunctions.RemoveGUIDFiles(jsFilePath);
            CoreFunctions.RemoveGUIDFiles(htmlFilePath);

            CoreFunctions.CreateGUIDFiles(jsFilePath);
            CoreFunctions.CreateGUIDFiles(htmlFilePath);

            writeOutputs();

            Owner.Get<Label>("Label2").Visible = false;
            Owner.Get<WebBrowser>("WebWidget").Visible = true;
            Owner.Get<WebBrowser>("WebWidget").URL = htmlFileUri;
            Owner.Get<WebBrowser>("WebWidget").Refresh();
        }
    }

    #endregion
}
