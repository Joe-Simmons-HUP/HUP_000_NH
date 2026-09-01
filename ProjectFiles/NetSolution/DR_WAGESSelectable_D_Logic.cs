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
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.DataLogger;
#endregion

public class DR_WAGESSelectable_D_Logic : BaseNetLogic
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
    private String TitleText;
    private String LabelFont;
    private Int32 LabelFontSize;
    private String LabelFontWeight;
    private Int32 GridTopSpacing;
    private Int32 GridBottomSpacing;
    private Int32 GridLeftSpacing;
    private Int32 GridRightSpacing;
    private Int32 CornerRadius;
    private String ConsBarColor;
    private String GenBarColor;
    private String NetBarColor;
    private String TotalLineColor;
    private string[] fontList = { "arial", "verdana", "tahoma", "trebuchet ms", "times new roman", "georgia", "garamond", "courier new", "brush script mt" };
    private string[] fontWeightList = { "normal", "bold", "bolder", "lighter" };

    private string projectName;
    private Store dataStore;
    private Object[,] resultsData;
    private string[] resultsHeader;
    private string queryWhere;
    private string dataQuery;
    private string data1;
    private string data2;
    private string data3;
    private string data4;

    private CoreFunctions CoreFunctions;

    private string htmlFileUri;
    private string js_Header;
    private string js_Data1;
    private string js_Data2;
    private string js_Data3;
    private string js_Data4;
    private string js_VarLegend;
    private string js_VarData;
    private bool firstData;
    private string js_VarList1;
    private string js_VarList2;
    private string js_eChartsOptions;
    private string htmlBody;
    private string projPath;
    private string jsFilePath;
    private string htmlFilePath;

    public struct WAGESData
    {
        public DateTime dateHour;
        public float consHourTotal;
        public float genHourTotal;
        public float netHourTotal;
        public float runTotal;
    }

    #endregion

    #region SUPPORTING METHODS

    private void loadValues()
    {
        DebugEnable = Owner.GetVariable("DebugEnable").Value;

        if (DebugEnable)
        {
            Log.Info("WAGES Debug Enabled: " + DebugEnable);
            Log.Info("WAGES Loading Values");
        }

        CoreFunctions = new CoreFunctions();

        NodeId RefNodeID = Owner.GetVariable("Model").Value;
        IUAObject WAGESInstance = InformationModel.GetObject(RefNodeID);
        string FullPath = "";
        
        CoreFunctions.GetOwnerPath("InformationModel", WAGESInstance as IUANode, WAGESInstance.BrowseName, out FullPath);
        string[] InstancePathArray = FullPath.Split("/");
        ModelName = InstancePathArray[0];
        InstanceName = FullPath;
        ImageID = Owner.GetVariable("ImageID").Value;

        StartDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/Start").Value;
        EndDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/End").Value;
        Units = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/InformationModel/" + InstanceName + "/CrntSet/EngineeringUnits").Value;
        BackGroundColor = colorString(Owner.GetVariable("BackGroundColor").Value);
        FontColor = colorString(Owner.GetVariable("FontColor").Value);
        TitleText = Owner.GetVariable("TitleText").Value;
        LabelFont = fontList[Owner.GetVariable("LabelFont").Value];
        LabelFontSize = Owner.GetVariable("LabelFontSize").Value;
        LabelFontWeight = fontWeightList[Owner.GetVariable("LabelFontWeight").Value];
        GridTopSpacing = Owner.GetVariable("GridTopSpacing").Value;
        GridBottomSpacing = Owner.GetVariable("GridBottomSpacing").Value;
        GridLeftSpacing = Owner.GetVariable("GridLeftSpacing").Value;
        GridRightSpacing = Owner.GetVariable("GridRightSpacing").Value;
        CornerRadius = Owner.GetVariable("CornerRadius").Value;
        ConsBarColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/WAGES_UI/Data/WAGESColors/ConsBarColor").Value);
        GenBarColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/WAGES_UI/Data/WAGESColors/GenBarColor").Value);
        NetBarColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/WAGES_UI/Data/WAGESColors/NetBarColor").Value);
        TotalLineColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/WAGES_UI/Data/WAGESColors/TotalLineColor").Value);


        projectName = Project.Current.BrowseName;
        projPath = Project.Current.ProjectDirectory;
        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/WAGES/Databases/WAGESDatabase");

        jsFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".js";
        htmlFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".html";
        htmlFileUri = ResourceUri.FromProjectRelativePath("DataReady/eCharts/Runtime/" + ImageID + ".html");

        Owner.GetVariable("ImageID").Value = ImageID;

        if (DebugEnable)
        {
            Log.Info("WAGES Loading Values Complete");
        }
    }

    private static string GetFQNForDataTag(IUANode Node, string FQN, string guid, Boolean DebugEnable)
    {
        if (Node.Owner.BrowseName != guid)
        {
            if (DebugEnable)
            {
                Log.Info("FQN !=:  Owner=" + Node.Owner.BrowseName);
            }
            FQN = Node.Owner.BrowseName + "/" + FQN;
            FQN = GetFQNForDataTag(Node.Owner, FQN, guid, DebugEnable);
        }
        return FQN;
    }

    public string colorString(Color color)
    {
        if (DebugEnable)
        {
            Log.Info("WAGES Converting Color");
        }
        string updatedColor = "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");

        if (DebugEnable)
        {
            Log.Info("WAGES Color Conversion: " + Convert.ToString(color.ARGB) + " Color String: " + updatedColor);
        }
        return updatedColor;
    }

    public static DateTime trimTime(DateTime rawTime)
    {
        string stringTime = rawTime.ToString("yyyy.MM.dd, HH:0:0");
        DateTime truncTime = Convert.ToDateTime(stringTime);
        return truncTime;
    }

    private void readDatabase()
    {
        if (DebugEnable)
        {
            Log.Info("WAGES Read Database");
        }
        queryWhere = "(MachineGUID = '" + ModelName + "') AND (InstanceName = '" + InstanceName + "') AND (Timestamp >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (Timestamp <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";
        if (DebugEnable)
        {
            Log.Info("WAGES Query WHERE: " + queryWhere);
        }
        dataQuery = "SELECT * FROM raI_01_00_0106_WAGES WHERE " + queryWhere + " ORDER BY Timestamp";
        if (DebugEnable == true)
        {
            Log.Info("WAGES Query: " + dataQuery);
        }
        dataStore.Query(dataQuery, out resultsHeader, out resultsData);
        if (DebugEnable == true)
        {
            Log.Info("WAGES Query Result Length: " + resultsData.GetLength(0));
        }
    }

    private void dataStaging()
    {
        Int32 length = resultsData.GetLength(0);

        DateTime startTime = trimTime(Convert.ToDateTime(resultsData[0, 0]));
        DateTime endTime = trimTime(Convert.ToDateTime(resultsData[length - 1, 0]).AddHours(1));
        Int32 timeSpan = Convert.ToInt32(endTime.Subtract(startTime).TotalHours);

        if (DebugEnable)
        {
            Log.Info("Time Span: " + timeSpan.ToString());
        }

        WAGESData[] WAGESData = new WAGESData[timeSpan];

        Int32 i = 0;  //Source index
        Int32 j = 0; //Target index

        float consHourTotal = 0;
        float genHourTotal = 0;
        float netHourTotal = 0;
        float runningTotal = 0;

        if (DebugEnable)
        {
            Log.Info("Start: " + startTime + "  End: " + endTime + "  Entries: " + length.ToString());
        }

        DateTime currentTime = trimTime(Convert.ToDateTime(resultsData[0, 0]));

        while ((i < length) & (j < timeSpan))

        {
            currentTime = trimTime(Convert.ToDateTime(resultsData[i, 0]));
            if (startTime == currentTime)
            {
                WAGESData[j].dateHour = startTime;
                consHourTotal = consHourTotal + (Convert.ToSingle(resultsData[i, 5]));
                genHourTotal = genHourTotal + (Convert.ToSingle(resultsData[i, 7]));
                netHourTotal = netHourTotal + (Convert.ToSingle(resultsData[i, 9]));
                runningTotal = runningTotal + (Convert.ToSingle(resultsData[i, 5]));
                WAGESData[j].consHourTotal = consHourTotal;
                WAGESData[j].genHourTotal = genHourTotal;
                WAGESData[j].netHourTotal = netHourTotal;
                WAGESData[j].runTotal = runningTotal;
                i++;
            }
            else
            {
                j++;
                consHourTotal = 0;
                genHourTotal = 0;
                netHourTotal = 0;

                startTime = startTime.AddHours(1);
                WAGESData[j].dateHour = startTime;
                WAGESData[j].consHourTotal = consHourTotal;
                WAGESData[j].genHourTotal = genHourTotal;
                WAGESData[j].netHourTotal = netHourTotal;
                WAGESData[j].runTotal = runningTotal;
            }
            if (DebugEnable)
            {
                Log.Info("Timestamp: " + startTime + "  Hour Total: " + consHourTotal + "  Rnning Total: " + runningTotal);
            }
        }

        i = 0;
        data1 = "[";
        data2 = "[";
        data3 = "[";
        data4 = "[";

        while (i < WAGESData.Length)
        {
            data1 = data1 + "['" + WAGESData[i].dateHour.ToString() + "', " + WAGESData[i].runTotal.ToString() + "],";
            data2 = data2 + "['" + WAGESData[i].dateHour.ToString() + "', " + WAGESData[i].consHourTotal.ToString() + "],";
            data3 = data3 + "['" + WAGESData[i].dateHour.ToString() + "', " + WAGESData[i].genHourTotal.ToString() + "],";
            data4 = data4 + "['" + WAGESData[i].dateHour.ToString() + "', " + WAGESData[i].netHourTotal.ToString() + "],";
            i++;
        }

        data1 = data1 + "]";
        data2 = data2 + "]";
        data3 = data3 + "]";
        data4 = data4 + "]";
    }

    public void createStrings()
    {
        if (DebugEnable)
        {
            Log.Info("WAGES Creating JS Strings");
        }
        js_VarLegend = "";
        js_VarData = "";

        js_Header = "var dom = document.getElementById('container');\r\nvar myChart = echarts.init(dom, null, {\r\n    renderer: 'canvas',\r\n    useDirtyRect: false\r\n});\r\nvar app = {};\r\nvar option;\r\nvar data = [];\r\nvar startTime = +new Date();\r\nvar categories = [''];";
        js_Data1 = "var data1 = " + data1 + ";";
        js_Data2 = "var data2 = " + data2 + ";";
        js_Data3 = "var data3 = " + data3 + ";";
        js_Data4 = "var data4 = " + data4 + ";";
        js_VarList1 = "var backGroundColor = '" + BackGroundColor + "';\r\nvar fontColor = '" + FontColor + "';\r\nvar titleText = '" + TitleText + "';\r\nvar labelFontSize = " + LabelFontSize + ";\r\nvar labelFont = '" + LabelFont + "';\r\nvar labelWeight = '" + LabelFontWeight + "';\r\nvar gridTopSpacing=" + GridTopSpacing + ";\r\nvar gridBottomSpacing=" + GridBottomSpacing + ";\r\nvar gridLeftSpacing=" + GridLeftSpacing + ";\r\nvar gridRightSpacing=" + GridRightSpacing + ";\r\nvar units='" + Units + "';\r\n";
        js_VarList2 = "var cornerRadius = " + CornerRadius + ";\r\nvar consBarColor = '" + ConsBarColor + "';\r\nvar genBarColor = '" + GenBarColor + "';\r\nvar netBarColor = '" + NetBarColor + "';\r\nvar totalLineColor = '" + TotalLineColor + "';\r\n";
        js_eChartsOptions = "option = {\r\n    backgroundColor: backGroundColor,\r\n    tooltip: {\r\n        trigger: 'axis',\r\n        axisPointer: {\r\n            type: 'cross'\r\n        }\r\n    },\r\n"; 

        js_eChartsOptions = js_eChartsOptions + js_VarLegend;
        js_eChartsOptions = js_eChartsOptions + "grid: {\r\n        top: gridTopSpacing,\r\n        bottom: gridBottomSpacing,\r\n        left: gridLeftSpacing,\r\n        right: gridRightSpacing\r\n    },\r\n    xAxis: {\r\n        type: 'category',\r\n        axisLabel: {\r\n            color: fontColor,\r\n            fontWeight: labelWeight,\r\n            fontFamily: labelFont,\r\n            fontSize: labelFontSize,\r\n        }\r\n    },\r\n    yAxis: [{\r\n            type: 'value',\r\n            name: 'Hourly '+units,\r\n            nameTextStyle: {\r\n              color: fontColor,\r\n              fontWeight: labelWeight ,\r\n              fontFamily: labelFont ,\r\n              fontSize: labelFontSize ,\r\n            },\r\n            min: 0,\r\n            axisLabel: {\r\n                color: fontColor,\r\n                fontWeight: labelWeight,\r\n                fontFamily: labelFont,\r\n                fontSize: labelFontSize,\r\n                overflow: 'truncate',\r\n                formatter: '{value} '+units\r\n            }\r\n        }, {\r\n            type: 'value',\r\n            name: 'Total '+units,\r\n            nameTextStyle: {\r\n              color: fontColor,\r\n              fontWeight: labelWeight ,\r\n              fontFamily: labelFont ,\r\n              fontSize: labelFontSize ,\r\n            },\r\n            min: 0,\r\n            axisLabel: {\r\n                color: fontColor,\r\n                fontWeight: labelWeight,\r\n                fontFamily: labelFont,\r\n                fontSize: labelFontSize,\r\n                overflow: 'truncate',\r\n                formatter: '{value} '+units\r\n            }\r\n        },\r\n    ],\r\n    series: [";
        
        firstData = true;

        if (Owner.GetVariable("EnableRunningTotal").Value == true)
        {
            js_VarData = "\r\n      {\r\n            name: 'Running Total',\r\n            yAxisIndex: 1,\r\n            data: data1,\r\n            type: 'line',\r\n            lineStyle: {\r\n              color: totalLineColor ,\r\n              width: 4 \r\n            },\r\n        }";
            firstData = false;
        }

        if (Owner.GetVariable("EnableConsumption").Value == true)
        {
            if (firstData == false)
            {
                js_VarData = js_VarData + ",";
            }

            js_VarData = js_VarData + "\r\n     {\r\n            name: 'Consumed',\r\n            itemStyle: {\r\n              color: consBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data2,\r\n            type: 'bar'\r\n        }";
            firstData = false;
        }

        if (Owner.GetVariable("EnableGenerated").Value == true)
        {
            if (firstData == false)
            {
                js_VarData = js_VarData + ",";
            }

            js_VarData = js_VarData + "\r\n     {\r\n            name: 'Generated',\r\n            itemStyle: {\r\n              color: genBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data3,\r\n            type: 'bar'\r\n        }";
            firstData = false;
        }

        if (Owner.GetVariable("EnableNetConsumption").Value == true)
        {
            if (firstData == false)
            {
                js_VarData = js_VarData + ",";
            }

            js_VarData = js_VarData + "\r\n     {\r\n            name: 'Net',\r\n            itemStyle: {\r\n              color: netBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data4,\r\n            type: 'bar'\r\n        }";
            firstData = false;
        }

        js_eChartsOptions = js_eChartsOptions + js_VarData;
        js_eChartsOptions = js_eChartsOptions + "\r\n    ]\r\n};\r\nif (option && typeof option === 'object') {\r\n    myChart.setOption(option);\r\n}\r\n\r\nwindow.addEventListener('resize', myChart.resize);";

        htmlBody = "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"height: 100%\">\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n</head>\r\n<body style=\"height: 100%; margin: 0\">\r\n\t<div id=\"container\" style=\"height: 100%\"></div>\r\n\r\n\t<script type=\"text/javascript\" src=\"../jquery.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"../echarts.min.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"./" + ImageID + ".js\"></script>\r\n\r\n</body>\r\n</html>";
        if (DebugEnable)
        {
            Log.Info("WAGES JS Strings Completed");
        }
    }

    public void writeOutputs()
    {
        if (DebugEnable)
        {
            Log.Info("WAGES Write JS Strings to files");
        }
        System.IO.StreamWriter jsFile = new System.IO.StreamWriter(jsFilePath, true);
        jsFile.Flush();
        jsFile.WriteLine(js_Header);
        jsFile.WriteLine(js_Data1);
        jsFile.WriteLine(js_Data2);
        jsFile.WriteLine(js_Data3);
        jsFile.WriteLine(js_Data4);
        jsFile.WriteLine(js_VarList1);
        jsFile.WriteLine(js_VarList2);
        jsFile.WriteLine(js_eChartsOptions);
        jsFile.Close();
        if (DebugEnable)
        {
            Log.Info(jsFilePath + " WAGES File Write Complete");
        }
        System.IO.StreamWriter htmlFile = new System.IO.StreamWriter(htmlFilePath, true);
        htmlFile.Flush();
        htmlFile.WriteLine(htmlBody);
        htmlFile.Close();

        if (DebugEnable)
        {
            Log.Info(htmlFilePath + " WAGES File Write Complete");
        }
    }

    #endregion

    #region MAIN METHODS

    public override void Start()
    {
        Owner.Get<Image>("HorizontalLayout1/Panel2/GraphicPlaceholder").Visible = false;
        Owner.Get<Image>("HorizontalLayout1/Panel2/GraphicPlaceholder").Enabled = false;
        ImageID = "WS_" + Guid.NewGuid().ToString().ToUpper();

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

        readDatabase();

        if (resultsData.GetLength(0) <= 0)
        {
            Owner.Get<Label>("HorizontalLayout1/Panel2/Label2").Visible = true;
            Owner.Get<Label>("HorizontalLayout1/Panel2/Label2").TextColor = Owner.GetVariable("FontColor").Value;
            Owner.Get<WebBrowser>("HorizontalLayout1/Panel2/WebWidget").Visible = false;
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

            Owner.Get<Label>("HorizontalLayout1/Panel2/Label2").Visible = false;
            Owner.Get<WebBrowser>("HorizontalLayout1/Panel2/WebWidget").Visible = true;
            Owner.Get<WebBrowser>("HorizontalLayout1/Panel2/WebWidget").URL = htmlFileUri;
            Owner.Get<WebBrowser>("HorizontalLayout1/Panel2/WebWidget").Refresh();


        }
    }
    #endregion
}
