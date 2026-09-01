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
using System.IO;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using FTOptix.OPCUAServer;
using FTOptix.OPCUAClient;
using FTOptix.RAEtherNetIP;
using FTOptix.Report;
using FTOptix.ODBCStore;
using FTOptix.EventLogger;
using FTOptix.DataLogger;
using FTOptix.WebUI;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
#endregion

public class DR_OEEHourLogic : BaseNetLogic
{
    #region DECLARE VARIABLES

    private Boolean DebugEnable;
    private String ImageID;
    //private Boolean RefreshRequest;
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
    private Int32 CornerRadius;
    private string[] fontList = { "arial", "verdana", "tahoma", "trebuchet ms", "times new roman", "georgia", "garamond", "courier new", "brush script mt" };
    private string[] fontWeightList = { "normal", "bold", "bolder", "lighter" };

    private float OEETarget;
    private float AvailTarget;
    private float PerfTarget;
    private float QualTarget;
    private string data1;
    private string data2;
    private string data3;
    private string data4;
    private string data5;
    private string data6;

    private string js_Header;
    private string js_Data1;
    private string js_Data2;
    private string js_Data3;
    private string js_Data4;
    private string js_Data5;
    private string js_Data6;
    private string js_VarList1;
    private string js_eChartsOptions;
    private string htmlBody;
    private string jsFilePath;
    private string htmlFilePath;
    private string htmlFileUri;

    private Store dataStore;
    private Object[,] resultsData;
    private string[] resultsHeader;
    private string queryWhere;
    private string dataQuery;

    private CoreFunctions CoreFunctions;

    #endregion

    #region SUPPORTING METHODS

    private void loadValues()
    {
        DebugEnable = Owner.GetVariable("DebugEnable").Value;

        CoreFunctions = new CoreFunctions();

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "Smart Trend Debug Enabled: " + DebugEnable);
            Log.Info("DataReady - OEE By Hour", "Loading Values");
        }


        //RefreshRequest = Owner.GetVariable("RefreshRequest").Value;

        ModelNameEnum = Owner.GetVariable("ModelName").Value;
        IUANode EnumObj = InformationModel.Get(Project.Current.Get("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumLists/InformationModelInstances").NodeId);
        CoreFunctions.GetEnumText(ModelNameEnum, EnumObj, out ModelName);


        InstanceNameEnum = Owner.GetVariable("InstanceName").Value;
        EnumObj = InformationModel.Get(Project.Current.Get("UI/RockwellAutomationLibraries/DataReady_UI_V01_00_0106/OEE_UI/<PrivateElements>/Data/OEEInstances").NodeId);
        CoreFunctions.GetEnumText(InstanceNameEnum, EnumObj, out InstanceName);

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
        CornerRadius = Owner.GetVariable("CornerRadius").Value;
        OEETarget = 0;
        AvailTarget = 0;
        PerfTarget = 0;
        QualTarget = 0;

        StartDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/Start").Value;
        EndDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/End").Value;


        dataStore = Project.Current.Get<Store>("raLib_Core/DataReady_Core_V01_00_0106/OEE/Databases/OEEDatabase");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "OEE Datastore: " + dataStore.BrowseName);
            Log.Info("DataReady - OEE By Hour", "Loading Values Complete");
        }

        jsFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".js";
        htmlFilePath = Project.Current.ProjectDirectory + "/DataReady/eCharts/Runtime/" + ImageID + ".html";
        htmlFileUri = ResourceUri.FromProjectRelativePath("DataReady/eCharts/Runtime/" + ImageID + ".html");

        Owner.GetVariable("ImageID").Value = ImageID;

    }

    public string colorString(Color color)
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "Converting Color");
        }
        string updatedColor = "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "Color Convertion: " + Convert.ToString(color.ARGB) + " Color String: " + updatedColor);
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
            Log.Info("DataReady - OEE By Hour", "Read Database");
        }

        //dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        queryWhere = "(MachineGUID = '" + ModelName + "') AND (StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (Timestamp <= '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')";

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "Query WHERE: " + queryWhere);
        }

        dataQuery = "SELECT MachineGUID, StartTime, ScheduledTime, RunningTime, TotalIdealTime, GoodIdealTime, OEETarget, AvailTarget, PerfTarget, QualTarget FROM raI_01_00_0106_OEE WHERE " + queryWhere;
        dataStore.Query(dataQuery, out resultsHeader, out resultsData);

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE By Hour", "Query: " + dataQuery);
            Log.Info("DataReady - OEE By hour", "Query Result Length: " + resultsData.GetLength(0));
        }


    }

    private void dataStaging()
    {
        //convert first time and last time to hour.
        //Calcualte number of hours.
        //Create Object Array with number entries based on hour count.
        //Iterate thru query array to calc OEE values.
        //Load DB.

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE By Hour", "Staging Started");

        }

        float scheduledTime = 0;
        float runningTime = 0;
        float totalTime = 0;
        float goodTime = 0;
        Int32 i = 0;
        Int32 j = 0;

        DateTime FirstTime = trimTime(StartDateTime);
        DateTime LastTime = trimTime(EndDateTime);
        TimeSpan timerange = LastTime.Subtract(FirstTime);

        DateTime currentHour = FirstTime;
        Int32 entryCount = Convert.ToInt32(timerange.TotalHours) + 1;

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE By Hour", "Input Records: " + resultsData.GetLength(0) + "  Output Records: " + entryCount);
        }
        object[,] OEEHoursData = new object[entryCount, 12];

        //This is iterating inside out.  Need to iterate by output hour and check against data.   There may be more hours than data!

        OEEHoursData[0, 0] = FirstTime.ToUniversalTime();
        OEEHoursData[0, 1] = FirstTime;
        OEEHoursData[0, 3] = FirstTime;


        while (i < resultsData.GetLength(0))
        {

            if (currentHour < trimTime(Convert.ToDateTime(resultsData[0, 1]))) //TimeRange starts before the first stamped data.  Zero everthing out.
            {

                if (DebugEnable == true)
                {
                    Log.Info("DataReady - OEE By Hour", "First Hour in time range is less than first sampled data's Timestamp");
                }

                OEEHoursData[j, 0] = FirstTime.AddHours(j).ToUniversalTime();
                OEEHoursData[j, 1] = FirstTime.AddHours(j);
                OEEHoursData[j, 3] = FirstTime.AddHours(j);
                OEEHoursData[j, 4] = 0;
                OEEHoursData[j, 5] = 0;
                OEEHoursData[j, 6] = 0;
                OEEHoursData[j, 7] = 0;
                OEEHoursData[j, 8] = 0;
                OEEHoursData[j, 9] = 0;
                OEEHoursData[j, 10] = 0;
                OEEHoursData[j, 11] = 0;
            }

            if (currentHour < trimTime(Convert.ToDateTime(resultsData[i, 1]))) //TimeRange starts before the first stamped data.  Zero everthing out.
            {
                if (DebugEnable == true)
                {
                    Log.Info("DataReady - OEE By Hour", "Current trend Hour in time range is less than current sampled data's Timestamp");
                }

                j = j + 1;

                OEEHoursData[j, 0] = FirstTime.AddHours(j).ToUniversalTime();
                OEEHoursData[j, 1] = FirstTime.AddHours(j);
                OEEHoursData[j, 3] = FirstTime.AddHours(j);
                OEEHoursData[j, 4] = 0;
                OEEHoursData[j, 5] = 0;
                OEEHoursData[j, 6] = 0;
                OEEHoursData[j, 7] = 0;
                if (Convert.ToSingle(OEEHoursData[j - 1, 8]) != 0)
                {
                    OEEHoursData[j, 8] = Convert.ToSingle(OEEHoursData[j - 1, 8]);
                }
                else
                {
                    OEEHoursData[j, 8] = 0;
                }

                OEEHoursData[j, 9] = 0;
                OEEHoursData[j, 10] = 0;
                OEEHoursData[j, 11] = 0;
                currentHour = FirstTime.AddHours(j);

                scheduledTime = 0;
                runningTime = 0;
                totalTime = 0;
                goodTime = 0;
            }

            if (currentHour == trimTime(Convert.ToDateTime(resultsData[i, 1])))
            {
                if (DebugEnable == true)
                {
                    Log.Info("DataReady - OEE By Hour", "Current trend Hour in time range is equal to the current sampled data's Timestamp");
                }

                scheduledTime = scheduledTime + Convert.ToSingle(resultsData[i, 2]);
                runningTime = runningTime + Convert.ToSingle(resultsData[i, 3]);
                totalTime = totalTime + Convert.ToSingle(resultsData[i, 4]);
                goodTime = goodTime + Convert.ToSingle(resultsData[i, 5]);

                if (scheduledTime <= 0)
                {
                    OEEHoursData[j, 4] = 100;
                }
                else
                {
                    OEEHoursData[j, 4] = goodTime / scheduledTime * 100;
                }

                if (scheduledTime <= 0)
                {
                    OEEHoursData[j, 5] = 0;
                }
                else
                {
                    OEEHoursData[j, 5] = runningTime / scheduledTime * 100;
                }

                if (runningTime <= 0)
                {
                    OEEHoursData[j, 6] = 0;
                }
                else
                {
                    OEEHoursData[j, 6] = totalTime / runningTime * 100;
                }

                if (totalTime <= 0)
                {
                    OEEHoursData[j, 7] = 0;
                }
                else
                {
                    OEEHoursData[j, 7] = goodTime / runningTime * 100;
                }

                OEEHoursData[j, 8] = Convert.ToSingle(resultsData[i, 6]);
                if (Convert.ToSingle(resultsData[i, 6]) != 0)
                {
                    OEETarget = Convert.ToSingle(resultsData[i, 6]);
                }

                OEEHoursData[j, 9] = Convert.ToSingle(resultsData[i, 7]);
                AvailTarget = Convert.ToSingle(resultsData[i, 7]);
                OEEHoursData[j, 10] = Convert.ToSingle(resultsData[i, 8]);
                PerfTarget = Convert.ToSingle(resultsData[i, 8]);
                OEEHoursData[j, 11] = Convert.ToSingle(resultsData[i, 9]);
                QualTarget = Convert.ToSingle(resultsData[i, 9]);

                i++;
            }

            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE By Hour", "j=" + j + " OEE Hours Data Array=" + OEEHoursData.GetLength(0) + " x " + OEEHoursData.GetLength(1));
                //Log.Info("OEEHoursData[" + j + ",4]=  " + OEEHoursData[j, 4].ToString());
            }
        }

        j = j + 1;

        while (j < entryCount)
        {
            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE By Hour", "Current trend Hour in time range is greater than the last sampled data's Timestamp");
            }

            OEEHoursData[j, 0] = FirstTime.AddHours(j).ToUniversalTime();
            OEEHoursData[j, 1] = FirstTime.AddHours(j);
            OEEHoursData[j, 3] = FirstTime.AddHours(j);  //StartTime
            OEEHoursData[j, 4] = 0;  //OEE
            OEEHoursData[j, 5] = 0;  //Avail
            OEEHoursData[j, 6] = 0;  //Perf
            OEEHoursData[j, 7] = 0;  //Qual
            OEEHoursData[j, 8] = OEETarget;  //OEE Target
            OEEHoursData[j, 9] = AvailTarget;
            OEEHoursData[j, 10] = PerfTarget;
            OEEHoursData[j, 11] = QualTarget;

            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE By Hour", "OEEHoursData[" + j + ",4]=  " + OEEHoursData[j, 4].ToString());
            }

            j++;
        }
        ;

        i = 0;
        data1 = "[";
        data2 = "[";
        data3 = "[";
        data4 = "[";
        data5 = "[";
        data6 = "[";

        if (DebugEnable == true)
        {
            Log.Info("DataReady - OEE By Hour", "OEE Hours Data length: " + OEEHoursData.GetLength(0));
        }

        while (i < OEEHoursData.GetLength(0))
        {
            if (DebugEnable == true)
            {
                Log.Info("DataReady - OEE By Hour", "i=" + i);
                Log.Info("DataReady - OEE By Hour", "i=" + i + "  " + OEEHoursData[i, 4].ToString());
            }

            data1 = data1 + "['" + OEEHoursData[i, 3].ToString() + "', " + OEEHoursData[i, 4].ToString() + "],";
            data2 = data2 + "['" + OEEHoursData[i, 3].ToString() + "', " + OEEHoursData[i, 5].ToString() + "],";
            data3 = data3 + "['" + OEEHoursData[i, 3].ToString() + "', " + OEEHoursData[i, 6].ToString() + "],";
            data4 = data4 + "['" + OEEHoursData[i, 3].ToString() + "', " + OEEHoursData[i, 7].ToString() + "],";
            data5 = data5 + "['" + OEEHoursData[i, 3].ToString() + "', " + OEEHoursData[i, 8].ToString() + "],";

            if (Convert.ToSingle(OEEHoursData[i, 8]) - 10 > 0)
            {
                data6 = data6 + "['" + OEEHoursData[i, 3].ToString() + "', " + (Convert.ToSingle(OEEHoursData[i, 8]) - 5) + "],";
            }
            else
            {
                data6 = data6 + "['" + OEEHoursData[i, 3].ToString() + "', " + 0 + "],";
            }

            i++;
        }

        data1 = data1 + "]";
        data2 = data2 + "]";
        data3 = data3 + "]";
        data4 = data4 + "]";
        data5 = data5 + "]";
        data6 = data6 + "]";
    }

    public void createStrings()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "OEE Hour Creating JS Strings");
        }
        js_Header = "var dom = document.getElementById('container');\r\nvar myChart = echarts.init(dom, null, {\r\n    renderer: 'canvas',\r\n    useDirtyRect: false\r\n});\r\nvar app = {};\r\nvar option;\r\nvar data = [];\r\nvar startTime = +new Date();\r\nvar categories = [''];";
        js_Data1 = "var data1 = " + data1 + ";";
        js_Data2 = "var data2 = " + data2 + ";";
        js_Data3 = "var data3 = " + data3 + ";";
        js_Data4 = "var data4 = " + data4 + ";";
        js_Data5 = "var data5 = " + data5 + ";";
        js_Data6 = "var data6 = " + data6 + ";";
        js_VarList1 = "var backGroundColor = '"+ BackGroundColor + "';\r\nvar fontColor = '"+ FontColor + "';\r\nvar titleEnable = true;\r\nvar titleText = 'OEE By Hour';\r\nvar titleTop = 5;\r\nvar titleFontSize = 30;\r\nvar titleFont = 'arial';\r\nvar titleWeight = 'normal';\r\nvar labelFontSize = 15;\r\nvar labelFont = 'arial';\r\nvar labelWeight = 'normal';\r\nvar gridTopSpacing=100;\r\nvar gridBottomSpacing=50;\r\nvar gridLeftSpacing=100;\r\nvar gridRightSpacing=100;\r\nvar units='kWh';\r\n\r\nvar cornerRadius = 10;\r\nvar oeeBarColor = '#008000';\r\nvar availBarColor = '#f08d00';\r\nvar perfBarColor = 'purple';\r\nvar qualBarColor = '#5470c6';\r\nvar targetLineColor='orange';\r\nvar warningLineColor='red';";
        js_eChartsOptions = "option = {\r\n    backgroundColor: backGroundColor,\r\n    tooltip: {\r\n        trigger: 'axis',\r\n        axisPointer: {\r\n            type: 'cross'\r\n        }\r\n    },\r\nlegend: {\r\n    data: ['OEE', 'Availability','Performance','Quality', 'Target','Warning'],\r\n    top: gridTopSpacing-50,\r\n    textStyle:{color: fontColor}\r\n  },\r\n    title: [{\r\n            show: titleEnable,\r\n            text: titleText,\r\n            top: titleTop,\r\n            left: 'center',\r\n            textStyle: {\r\n                color: fontColor,\r\n                fontWeight: titleWeight,\r\n                fontFamily: titleFont,\r\n                fontSize: titleFontSize\r\n            }\r\n        },\r\n    ],\r\n    grid: {\r\n        top: gridTopSpacing,\r\n        bottom: gridBottomSpacing,\r\n        left: gridLeftSpacing,\r\n        right: gridRightSpacing\r\n    },\r\n    xAxis: {\r\n        type: 'category',\r\n        axisLabel: {\r\n            color: fontColor,\r\n            fontWeight: labelWeight,\r\n            fontFamily: labelFont,\r\n            fontSize: labelFontSize,\r\n        }\r\n    },\r\n    yAxis: [{\r\n            type: 'value',\r\n            nameTextStyle: {\r\n              color: fontColor,\r\n              fontWeight: labelWeight ,\r\n              fontFamily: labelFont ,\r\n              fontSize: labelFontSize ,\r\n            },\r\n            min: 0,\r\n            max: 120,\r\n            axisLabel: {\r\n                color: fontColor,\r\n                fontWeight: labelWeight,\r\n                fontFamily: labelFont,\r\n                fontSize: labelFontSize,\r\n                overflow: 'truncate',\r\n                formatter: '{value}'+'%'\r\n            }\r\n        }\r\n    ],\r\n    series: [{\r\n            name: 'OEE',\r\n            itemStyle: {\r\n              color: oeeBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data1,\r\n            type: 'bar'\r\n        }, {\r\n            name: 'Availability',\r\n            itemStyle: {\r\n              color: availBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data2,\r\n            type: 'bar'\r\n        }, {\r\n            name: 'Performance',\r\n            itemStyle: {\r\n              color: perfBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data3,\r\n            type: 'bar'\r\n        }, {\r\n            name: 'Quality',\r\n            itemStyle: {\r\n              color: qualBarColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data4,\r\n            type: 'bar'\r\n        }, {\r\n            name: 'Target',\r\n            itemStyle: {\r\n              color: targetLineColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data5,\r\n            type: 'line'\r\n        }, {\r\n            name: 'Warning',\r\n            itemStyle: {\r\n              color: warningLineColor,\r\n              borderRadius: cornerRadius ,\r\n            },\r\n            data: data6,\r\n            type: 'line'\r\n        }\r\n    ]\r\n};\r\nif (option && typeof option === 'object') {\r\n    myChart.setOption(option);\r\n}\r\n\r\nwindow.addEventListener('resize', myChart.resize);";
        htmlBody = "<!DOCTYPE html>\r\n<html lang=\"en\" style=\"height: 100%\">\r\n<head>\r\n  <meta charset=\"utf-8\">\r\n</head>\r\n<body style=\"height: 100%; margin: 0\">\r\n\t<div id=\"container\" style=\"height: 100%\"></div>\r\n\r\n\t<script type=\"text/javascript\" src=\"../jquery.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"../echarts.min.js\"></script>\r\n\t<script type=\"text/javascript\" src=\"./" + ImageID + ".js\"></script>\r\n\r\n</body>\r\n</html>";
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "OEE Hour JS Strings Completed");
        }
    }

    public void writeOutputs()
    {
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", "OEE Hour Write JS Strings to files");
        }
        System.IO.StreamWriter jsFile = new System.IO.StreamWriter(jsFilePath, true);
        jsFile.Flush();
        jsFile.WriteLine(js_Header);
        jsFile.WriteLine(js_Data1);
        jsFile.WriteLine(js_Data2);
        jsFile.WriteLine(js_Data3);
        jsFile.WriteLine(js_Data4);
        jsFile.WriteLine(js_Data5);
        jsFile.WriteLine(js_Data6);
        jsFile.WriteLine(js_VarList1);
        //jsFile.WriteLine(js_VarList2);
        jsFile.WriteLine(js_eChartsOptions);
        jsFile.Close();
        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", jsFilePath + " OEE Hour File Write Complete");
        }
        System.IO.StreamWriter htmlFile = new System.IO.StreamWriter(htmlFilePath, true);
        htmlFile.Flush();
        htmlFile.WriteLine(htmlBody);
        htmlFile.Close();

        if (DebugEnable)
        {
            Log.Info("DataReady - OEE By Hour", htmlFilePath + " OEE Hour File Write Complete");
        }
    }

    //string[] insertHeader = { "Timestamp", "LocalTimestamp", "MachineGuid", "StartTime", "OEE", "Availability", "Performance", "Quality", "OEETarget", "AvailTarget", "PerfTarget", "QualTarget" };

    #endregion

    #region MAIN METHODS

    public override void Start()
    {
        Owner.Get<Image>("GraphicPlaceholder").Visible = false;
        Owner.Get<Image>("GraphicPlaceholder").Enabled = false;

        ImageID = "OH_" + Guid.NewGuid().ToString().ToUpper();

        Owner.GetVariable("ImageID").Value = ImageID;
        buildChart();
        Owner.GetVariable("RefreshDataWidgets").VariableChange += buildChart_VariableChange;
        //updateOEEHour();
    }

    public override void Stop()
    {
        Owner.GetVariable("RefreshDataWidgets").VariableChange -= buildChart_VariableChange;

        CoreFunctions.RemoveGUIDFiles(jsFilePath);
        CoreFunctions.RemoveGUIDFiles(htmlFilePath);
    }

    private void buildChart_VariableChange(object sender, VariableChangeEventArgs e)
    {
        buildChart();
    }

    [ExportMethod]
    public void buildChart()
    {
        loadValues();

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

            writeOutputs();

            Owner.Get<Label>("Label2").Visible = false;
            Owner.Get<WebBrowser>("WebWidget").Visible = true;
            Owner.Get<WebBrowser>("WebWidget").URL = htmlFileUri;
            Owner.Get<WebBrowser>("WebWidget").Refresh();
        }

    }

    #endregion
}
