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
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
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

public class DR_FullGaugeNetLogic : BaseNetLogic
{
    #region DECLARE VARIABLES

    private Boolean DebugEnable;
    //private Boolean RefreshRequest;
    private String ImageID;
    private Int32 ModelNameEnum;
    private String ModelName;
    private Int32 InstanceNameEnum;
    private String InstanceName;
    private Int32 Component;
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
    private string[] fontList = { "arial", "verdana", "tahoma", "trebuchet ms", "times new roman", "georgia", "garamond", "courier new", "brush script mt" };
    private string[] fontWeightList = { "normal", "bold", "bolder", "lighter" };

    private float MinValue;
    private float MaxValue;
    private Int32 OuterRadius;
    private Int32 InnerRadius;
    private Int32 CornerRadius;

    private Int32 MajorTickCount;
    private Int32 MinorTickCount;
    private Int32 TickDistance;
    private Int32 TickLength;
    private float TargetThreshold;
    private String DetailFont;
    private Int32 DetailFontSize;
    private String DetailFontWeight;
    private Int32 DetailVertOffset;
    private Int32 DetailHorzOffset;
    private String TargetColor;
    private String WarningColor;
    private String AlertColor;

    private float AlertValue;
    private float WarningValue;
    private float TargetValue;
    private float ActualValue;

    private string projectName;
    private Store dataStore;
    private Object[,] resultsData;
    private string[] resultsHeader;
    private string queryWhere;
    private string dataQuery;
    private CoreFunctions CoreFunctions;

    private string js_Header;
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
            Log.Info("DataReady - OEE Full Gauge Debug", "Smart Trend Debug Enabled: " + DebugEnable);
            Log.Info("DataReady - OEE Full Gauge Debug", "Loading Values");
        }

        CoreFunctions = new CoreFunctions();
        ImageID = Owner.GetVariable("ImageID").Value;

        ModelNameEnum = Owner.GetVariable("ModelName").Value;
        IUANode EnumObj = InformationModel.Get(Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumLists/InformationModelInstances").NodeId);
        CoreFunctions.GetEnumText(ModelNameEnum, EnumObj, out ModelName);

        InstanceNameEnum = Owner.GetVariable("InstanceName").Value;
        EnumObj = InformationModel.Get(Project.Current.Get("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/OEE_UI/<PrivateElements>/Data/OEEInstances").NodeId);
        CoreFunctions.GetEnumText(InstanceNameEnum, EnumObj, out InstanceName);

        Component = Owner.GetVariable("Component").Value;

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

        MinValue = Owner.GetVariable("MinValue").Value;
        MaxValue = Owner.GetVariable("MaxValue").Value;
        OuterRadius = Owner.GetVariable("OuterRadius").Value;
        InnerRadius = Owner.GetVariable("InnerRadius").Value;
        CornerRadius = Owner.GetVariable("CornerRadius").Value;

        MajorTickCount = Owner.GetVariable("MajorTickCount").Value;
        MinorTickCount = Owner.GetVariable("MinorTickCount").Value;
        TickDistance = Owner.GetVariable("TickDistance").Value;
        TickLength = Owner.GetVariable("TickLength").Value;
        TargetThreshold = Owner.GetVariable("TargetThreshold").Value;
        DetailFont = fontList[Owner.GetVariable("DetailFont").Value];
        DetailFontSize = Owner.GetVariable("DetailFontSize").Value;
        DetailFontWeight = fontWeightList[Owner.GetVariable("DetailFontWeight").Value];
        DetailVertOffset = Owner.GetVariable("DetailVertOffset").Value;
        DetailHorzOffset = Owner.GetVariable("DetailHorzOffset").Value;
        TargetColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/OEE_UI/Data/OEEColors/Target").Value);
        WarningColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/OEE_UI/Data/OEEColors/Warning").Value);
        AlertColor = colorString(Project.Current.GetVariable("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/OEE_UI/Data/OEEColors/Alert").Value);

        projectName = Project.Current.BrowseName;
        projPath = Project.Current.ProjectDirectory;
        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/OEE/Databases/OEEDatabase");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "OEE Datastore: " + dataStore.BrowseName);
        }

        jsFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".js";
        htmlFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".html";
        htmlFileUri = ResourceUri.FromProjectRelativePath("DataReady/eCharts/Runtime/" + ImageID + ".html");

        Owner.GetVariable("ImageID").Value = ImageID;

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Loading Values Complete");
        }
    }

    public string colorString(Color color)
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Converting Color");
        }
        string updatedColor = "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Color Convertion: " + Convert.ToString(color.ARGB) + " Color String: " + updatedColor);
        }
        return updatedColor;
    }

    private void readDatabase()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Read Database");
        }
        queryWhere = "(MachineGUID = '" + ModelName + "') AND (StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (Timestamp <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Query WHERE: " + queryWhere);
        }

        if (Component == 0)
        {
            dataQuery = "SELECT MachineGUID, SUM(ScheduledTime) AS TotalScheduledTime, SUM(GoodIdealTime) AS TotalGoodTime FROM raI_01_00_0106_OEE WHERE " + queryWhere;
        }
        if (Component == 1)
        {
            dataQuery = "SELECT MachineGUID, SUM(ScheduledTime) AS TotalScheduledTime, SUM(RunningTime) AS TotalRunningTime FROM raI_01_00_0106_OEE WHERE " + queryWhere;
        }
        if (Component == 2)
        {
            dataQuery = "SELECT MachineGUID, SUM(RunningTime) AS TotalRunningTime, SUM(TotalIdealTime) AS TotalTotalTime FROM raI_01_00_0106_OEE WHERE " + queryWhere;
        }
        if (Component == 3)
        {
            dataQuery = "SELECT MachineGUID, SUM(TotalIdealTime) AS TotalTotalTime, SUM(GoodIdealTime) AS TotalGoodTime FROM raI_01_00_0106_OEE WHERE " + queryWhere;
        }

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Query: " + dataQuery);
        }
        dataStore.Query(dataQuery, out resultsHeader, out resultsData);
        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Query Result Length: " + resultsData.GetLength(0));
        }
    }

    private void dataStaging()
    {
        ActualValue = ((Convert.ToSingle(resultsData[0, 2]) / Convert.ToSingle(resultsData[0, 1])) * 100);
        AlertValue = (270 / (MaxValue - MinValue)) * (TargetThreshold - 5 - MinValue);
        WarningValue = (270 / (MaxValue - MinValue)) * (5);
        TargetValue = (270 / (MaxValue - MinValue)) * (MaxValue - TargetThreshold);
    }

    public void createStrings()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Creating JS Strings");
        }
        js_Header = "var dom = document.getElementById('container');\r\nvar myChart = echarts.init(dom, null, {\r\n    renderer: 'canvas',\r\n    useDirtyRect: false\r\n});\r\nvar app = {};\r\nvar option;\r\n";
        js_VarList1 = "var backGroundColor = '" + BackGroundColor + "';\r\nvar fontColor = '" + FontColor + "';\r\nvar titleEnable = " + TitleEnable + ";\r\nvar titleTop = " + TitleTop + ";\r\nvar titleFontSize = " + TitleFontSize + ";\r\nvar titleFont = '" + TitleFont + "';\r\nvar titleWeight = '" + TitleFontWeight + "';\r\nvar labelFontSize = " + LabelFontSize + ";\r\nvar labelFont = '" + LabelFont + "';\r\nvar labelWeight = '" + LabelFontWeight + "';\r\nvar gridTopSpacing=" + GridTopSpacing + ";\r\nvar gridBottomSpacing=" + GridBottomSpacing + ";\r\nvar gridLeftSpacing=" + GridLeftSpacing + ";\r\nvar gridRightSpacing=" + GridRightSpacing + ";\r\n";
        js_VarList2 = "var  titleText = '" + TitleText + "';\r\nvar  minValue = " + MinValue + ";\r\nvar maxValue = " + MaxValue + ";\r\nvar  outerRadius = '" + OuterRadius + "%';\r\nvar  innerRadius = '" + InnerRadius + "%';\r\nvar  cornerRadius = " + CornerRadius + ";\r\nvar  majorTickCount = " + MajorTickCount + ";\r\nvar  minorTickCount = " + MinorTickCount + ";\r\nvar  tickDistance = " + TickDistance + ";\r\nvar  tickLength = " + TickLength + ";\r\nvar  targetThreshold = " + TargetThreshold + ";\r\nvar  detailFont = '" + DetailFont + "';\r\nvar  detailFontSize = " + DetailFontSize + ";\r\nvar  detailFontWeight = '" + DetailFontWeight + "';\r\nvar  detailVertOffset = " + DetailVertOffset + ";\r\nvar  detailHorzOffset = " + DetailHorzOffset + ";\r\nvar  targetColor = '" + TargetColor + "';\r\nvar  warningColor = '" + WarningColor + "';\r\nvar  alertColor = '" + AlertColor + "';\r\nvar actualValue = " + ActualValue + ";\r\nvar alertValue = " + AlertValue + ";\r\nvar warningValue = " + WarningValue + ";\r\nvar targetValue = " + TargetValue + ";\r\nvar valueBackground = targetColor;\r\nif (actualValue<(targetThreshold-10)) \r\n{\r\n  valueBackground=alertColor;\r\n};\r\nif (actualValue>=targetThreshold-10)\r\n{\r\n  valueBackground=warningColor;\r\n};\r\nif (actualValue>=targetThreshold)\r\n{\r\n  valueBackground=targetColor;\r\n};";
        js_eChartsOptions = "option = {\r\n  backgroundColor: backGroundColor,\r\n  title: {\r\n        show: titleEnable,\r\n        top: titleTop,\r\n        left: 'center',\r\n        text: titleText,\r\n        textStyle: {\r\n            color: fontColor,\r\n            fontWeight: titleWeight,\r\n            fontFamily: titleFont,\r\n            fontSize: titleFontSize\r\n        },\r\n    },\r\n    grid: {\r\n        top: gridTopSpacing,\r\n        bottom: gridBottomSpacing,\r\n        left: gridLeftSpacing,\r\n        right: gridRightSpacing\r\n    },\r\n  series: [\r\n    {\r\n      //inverse: true,\r\n      name: 'Access From',\r\n      type: 'pie',\r\n      radius: [innerRadius, outerRadius],\r\n      startAngle: 225,\r\n      avoidLabelOverlap: false,\r\n      itemStyle: {\r\n          borderRadius: cornerRadius,\r\n          borderColor: backGroundColor,\r\n          borderWidth: 2\r\n      }, //border radius needs to be a variable\r\n      label: {\r\n          show: false\r\n      },\r\n      labelLine: {\r\n          show: false\r\n      },\r\n      data: [{\r\n          value: alertValue,\r\n          name: 'Alert',\r\n          itemStyle: {\r\n              color: alertColor,\r\n          }\r\n      }, {\r\n          value: warningValue,\r\n          name: 'Warning',\r\n          itemStyle: {\r\n              color: warningColor,\r\n          }\r\n      }, {\r\n          value: targetValue,\r\n          name: 'Target',\r\n          itemStyle: {\r\n              color: targetColor,\r\n          }\r\n      }, //values are addative to start angle...like a stacked bar.\r\n      {\r\n          value: 90,\r\n          name: 'Null',\r\n          itemStyle: {\r\n              color: 'transparent',\r\n              borderColor: 'transparent'\r\n          }\r\n      } // This line is static.\r\n      ]\r\n    },\r\n    {\r\n      type: 'gauge',\r\n      startAngle: 225,\r\n      endAngle: -45,\r\n      min: minValue,\r\n      max: maxValue,\r\n      radius: innerRadius, //Matches with pie radius\r\n      axisLine: {\r\n          show: false,\r\n          lineStyle: {\r\n            color: fontColor,\r\n            width: 5\r\n          }\r\n      },\r\n      pointer: {\r\n          icon: 'path://M12.8,0.7l12,40.1H0.7L12.8,0.7z',\r\n          length: '18%',\r\n          width: '12%',\r\n          offsetCenter: [0, '-83%'],\r\n          itemStyle: {\r\n              color: fontColor\r\n          }\r\n      },\r\n      splitNumber: majorTickCount, //Major Tick count\r\n      splitLine: {\r\n          show: true,\r\n          distance: tickDistance,\r\n          length: tickLength,\r\n          lineStyle: {\r\n              color: fontColor,\r\n              width: 1\r\n          }\r\n      }, //Major Ticks\r\n      axisTick: {\r\n          show: true,\r\n          distance: tickDistance,\r\n          length: tickLength/2,\r\n          splitNumber: minorTickCount,\r\n          lineStyle: {\r\n              color: fontColor,\r\n              width: 1\r\n          }\r\n      }, //Minor ticks\r\n      axisLabel: {\r\n          show: true,\r\n          color: fontColor,\r\n          distance: 15,\r\n          fontWeight: labelWeight,\r\n          fontFamily: labelFont,\r\n          fontSize: labelFontSize\r\n      },\r\n      data: [{\r\n          detail: {\r\n              top: 'center',\r\n              left: 'center',\r\n              valueAnimation: true,\r\n              offsetCenter: [detailHorzOffset, detailVertOffset],\r\n              formatter: '{value}%',\r\n              color: fontColor,\r\n              backgroundColor: valueBackground,\r\n              borderColor: valueBackground ,\r\n              borderWidth: 2 ,\r\n              borderType: 'solid' ,\r\n              borderDashOffset: 0 ,\r\n              borderRadius: 50 ,\r\n              width: '100%' ,\r\n              height: '30%' ,\r\n              fontWeight: detailFontWeight,\r\n              fontFamily: detailFont,\r\n              fontSize: detailFontSize\r\n          },\r\n          value: actualValue.toFixed(2)\r\n        }\r\n        ]\r\n    }\r\n    ]\r\n};\r\nif (option && typeof option === 'object') {\r\n    myChart.setOption(option);\r\n}\r\n\r\nwindow.addEventListener('resize', myChart.resize);";
        htmlBody = "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"height: 100%\">\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n</head>\r\n<body style=\"height: 100%; margin: 0\">\r\n\t<div id=\"container\" style=\"height: 100%\"></div>\r\n\r\n\t<script type=\"text/javascript\" src=\"../jquery.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"../echarts.min.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"./" + ImageID + ".js\"></script>\r\n\r\n</body>\r\n</html>";
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "JS Strings Completed");
        }
    }

    public void writeOutputs()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", "Write JS Strings to files");
        }
        System.IO.StreamWriter jsFile = new System.IO.StreamWriter(jsFilePath, true);
        jsFile.Flush();
        jsFile.WriteLine(js_Header);
        //jsFile.WriteLine(js_Data);
        jsFile.WriteLine(js_VarList1);
        jsFile.WriteLine(js_VarList2);
        jsFile.WriteLine(js_eChartsOptions);
        jsFile.Close();
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", jsFilePath + " File Write Complete");
        }
        System.IO.StreamWriter htmlFile = new System.IO.StreamWriter(htmlFilePath, true);
        htmlFile.Flush();
        htmlFile.WriteLine(htmlBody);
        htmlFile.Close();

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE Full Gauge Debug", htmlFilePath + " File Write Complete");
        }
    }

    #endregion

    #region MAIN METHODS

    public override void Start()
    {
        Owner.Get<Image>("GraphicPlaceholder").Visible = false;
        Owner.Get<Image>("GraphicPlaceholder").Enabled = false;

        ImageID = "FG_" + Guid.NewGuid().ToString().ToUpper();

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
        if (Convert.ToSingle(resultsData[0, 1]) <= 0)
        {
            Owner.Get<Label>("Label2").Visible = true;
            Owner.Get<Label>("Label2").TextColor = Owner.GetVariable("FontColor").Value;
            Owner.Get<WebBrowser>("WebWidget").Visible = false;
        }
        if (Convert.ToSingle(resultsData[0, 1]) > 0)
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
