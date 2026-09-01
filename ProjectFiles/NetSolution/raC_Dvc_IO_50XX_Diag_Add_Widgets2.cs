#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.UI;
using FTOptix.CoreBase;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using FTOptix.OPCUAClient;
using FTOptix.System;
using FTOptix.SerialPort;
using FTOptix.DataLogger;
#endregion

public class raC_Dvc_IO_50XX_Diag_Add_Widgets2 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        Add_Widgets(2);
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        DeleteWidgets();
    }

    private Dictionary<int, Tuple<string, int>> CatNo_50XX = new Dictionary<int, Tuple<string, int>>() {
        {34,Tuple.Create("5094-IB16S",1)},
        {35,Tuple.Create("5094-IB16SXT",1)},
        {36,Tuple.Create("5094-OB16S",1)},
        {37,Tuple.Create("5094-OB16SXT",1)},
        {38,Tuple.Create("5094-OW4IS",1)},
        {39,Tuple.Create("5094-OW4ISXT",1)},
        {40,Tuple.Create("5094-IJ2IS",1)},
        {41,Tuple.Create("5094-IJ2ISXT",1)},
        {63,Tuple.Create("5069-IB8S_Safety",1)},
        {64,Tuple.Create("5069-IB8S_SafetyMuting",1)},
        {65,Tuple.Create("5069-OBV8S",1)},
        {89,Tuple.Create("5094-IF4IHS",1)},
        {91,Tuple.Create("5094-OF4IHS",1)},
        {93,Tuple.Create("5094-IF4IHSXT",1)},
        {95,Tuple.Create("5094-OF4IHSXT",1)},
        {1101,Tuple.Create("5034-IB8S",1)},
        {1201,Tuple.Create("5034-OB8S",1)},
    };

[ExportMethod]
public void Add_Widgets(int instCount) {
    Container targetContainer = Owner.Get<ColumnLayout>("grp_Home/ChannelSts/ScrollView1/grp_Channels");
    
    IUANode Ref_Tag = null;
    IUANode Ref_Cat = null;
    IUANode Ref_Output = null;
    var dialogBoxAlias = Owner.GetAlias("raSDK1_DialogBox");
    foreach (var aliasChild in dialogBoxAlias.Children) {
        try {
            if (aliasChild.BrowseName == "Ref_Tag" || aliasChild.BrowseName == "Ref_Input") {
                Ref_Tag = InformationModel.Get(((UAVariable)aliasChild).Value);
            } else if (aliasChild.BrowseName == "Ref_Cat") {
                Ref_Cat = InformationModel.Get(((UAVariable)aliasChild).Value);
            } else if (aliasChild.BrowseName == "Ref_Output") {
                Ref_Output = InformationModel.Get(((UAVariable)aliasChild).Value);
            }
        } catch (Exception ex) {
            Log.Error($"Error retrieving node: {ex.Message}");
        }
    }

    var uaVariable1 = Ref_Cat as UAVariable;
    var Cat_Number = uaVariable1?.Value;
    
   

    var uaVariable = Ref_Tag as UAVariable;
    var variableType = uaVariable?.VariableType;
    var Data_Type = variableType?.BrowseName;
    
    //Banner update
    try
    {
        if (Data_Type.StartsWith("AB:5000_SDI16:I:0") || Data_Type.StartsWith("AB:5000_SFI2:I:0") || Data_Type.StartsWith("AB:5000_SDO8:I:0") || Data_Type.StartsWith("AB:5000_SDO16:I:0")
        || Data_Type.StartsWith("AB:5000_SDO4:I:0") || Data_Type.StartsWith("AB:5000_SDI8:I:0") || Data_Type.StartsWith("AB:5000_SAI4_SSV:I:0")
        || Data_Type.StartsWith("AB:5000_SAO4:I:0") || Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0") || Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("FaultTag").Value = "ConnectionFaulted";
        } else {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("FaultTag").Value = "Fault";
        }
    }
    catch (Exception ex)
    {
        Log.Error($"Problem with Banner or FaultTag {ex.Message}");
    }
    
    // Step 1: Find out diCount
    int diCount = 0;
    try {
        //diCount = GetInstCountForVariableType(Cat_Number);
        diCount = (CatNo_50XX.TryGetValue(Cat_Number, out var foundTuple)) ? foundTuple.Item2 : 0;
    } catch (Exception ex) {
        Log.Error($"Error getting instance count: {ex.Message}");
        return; // Exit method if error occurs
    }
    
        // Step 4: If diCount is less than or equal to 8, add widgets directly to the targetContainer
        for (int j = 0; j < diCount; j++) {
            try {
                 if (Data_Type.StartsWith("AB:5000_SDI16:I:0") || Data_Type.StartsWith("AB:5000_SDI8:I:0") || Data_Type.StartsWith("AB:5000_SAI4_SSV:I:0")
                 || Data_Type.StartsWith("AB:5000_SAO4:I:0") || Data_Type.StartsWith("AB:5000_SFI2:I:0") || Data_Type.StartsWith("AB:5000_SDO8:I:0")
                  || Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0") || Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDI16_I_0_50XX_Diag>("AIWidget" + j.ToString());
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="DiagnosticSequenceCount";
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString()+"Fault";
						newWidgetDI.GetVariable("Cfg_Label").Value = "DiagnosticActive";
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_SDO16:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDO16_I_0_50XX_Diag>("AOWidget" + j.ToString());
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="DiagnosticSequenceCount";
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString()+"Fault";
						newWidgetDI.GetVariable("Cfg_Label").Value = "DiagnosticActive";
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith( "AB:5000_SDO4:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDO4_I_0_50XX_Diag>("AOWidget" + j.ToString());
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="DiagnosticSequenceCount";
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString()+"Fault";
						newWidgetDI.GetVariable("Cfg_Label").Value = "DiagnosticActive";
                        targetContainer.Add(newWidgetDI);
                    }
                }
            } catch (Exception ex) {
                Log.Error($"Error adding widget directly to target container: {ex.Message}");
            }
        }
    }
    public void DeleteWidgets()
{
    // Get the target container
    var targetContainer = Owner.Get<ColumnLayout>("grp_Home/ChannelSts/ScrollView1/grp_Channels");

    // Check if the container is not null before proceeding
    if (targetContainer != null)
    {
        // Clear the children collection
        targetContainer.Children.Clear();
    }
    else
    {
        Log.Warning("Target container not found.");
    }
}
}
