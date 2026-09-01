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
using FTOptix.RAEtherNetIP;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using FTOptix.Alarm;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.DataLogger;
#endregion

public class BuildFilter : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void BuildWHEREString()
    {
        bool FirstFilterScan = true;
        string FilterSelectPair = "";
        string FilterSelectBookend = "";
        string FilterBookend = "";

        Int32 i = 0;
        foreach (var FilterName in Project.Current.GetObject("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/DynamicFilterTree").Children)
        {
            bool FirstFilterSelectScan = true;
            FilterSelectBookend = "";

            string FilterColumn = FilterName.BrowseName;

            foreach (IUAVariable FilterValue in FilterName.Children)
            {

                if ((!FilterValue.BrowseName.Contains("Select All")) & (FilterValue.Value == true))
                {
                    Log.Info("Enum Value: " + FilterValue.GetVariable("EnumValue").Value);
                    if ((FilterValue.GetVariable("EnumValue").Value == 0)&(FilterValue.BrowseName!="Blank"))
                    {
                        FilterSelectPair = "(" + FilterColumn + "='" + FilterValue.BrowseName + "')";
                    }
                    if ((FilterValue.GetVariable("EnumValue").Value == 0) & (FilterValue.BrowseName == "Blank"))
                    {
                        FilterSelectPair = "(" + FilterColumn + "='')";
                    }
                    if (FilterValue.GetVariable("EnumValue").Value != 0)
                    {
                        FilterSelectPair = "(" + FilterColumn + "='" + FilterValue.GetVariable("EnumValue").Value + "')";
                    }

                    if (FirstFilterSelectScan)
                    {
                        FilterSelectBookend = "(" + FilterSelectPair;
                        FirstFilterSelectScan = false;
                    }
                    else
                    {
                        FilterSelectBookend = FilterSelectBookend + " OR " + FilterSelectPair;
                    }

                }

            }

            if (FirstFilterSelectScan == false)
            {
                FilterSelectBookend = FilterSelectBookend + ")";
            }

            if (FirstFilterSelectScan == false)
            {
                if (FirstFilterScan == true)
                {
                    FilterBookend = FilterSelectBookend;  //"(" + FilterSelectBookend;
                    FirstFilterScan = false;
                }
                else
                {
                    FilterBookend = FilterBookend + " AND " + FilterSelectBookend;
                }
            }
        }
        if (FirstFilterScan == false)
        {
            FilterBookend = " AND " + FilterBookend;  //" AND " + FilterBookend + ")";
        }

        DateTime StartDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/Start").Value;
        DateTime EndDateTime = Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/Data/DateTimeRange/End").Value;

        string FinalWHERE = "(StartTime >= '" + StartDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "') AND (StartTime < '" + EndDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "')" + FilterBookend;

        //Log.Info("Query WHERE: " + FinalWHERE);
        Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/OEE/Data/DynamicFilterWHERE").Value = FinalWHERE;
    }
}
