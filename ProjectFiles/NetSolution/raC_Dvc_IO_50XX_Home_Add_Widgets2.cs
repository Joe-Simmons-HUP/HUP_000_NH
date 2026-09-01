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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Diagnostics.Tracing;
using FTOptix.OPCUAClient;
using FTOptix.System;
using FTOptix.SerialPort;
using FTOptix.DataLogger;
#endregion

public class raC_Dvc_IO_50XX_Home_Add_Widgets2 : BaseNetLogic
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
    
    private Dictionary<int, Tuple<string, int>> CatNo_50xx = new Dictionary<int, Tuple<string, int>>() {
        {0,Tuple.Create("5094-IB16",16)},
        {1,Tuple.Create("5094-IB16XT",16)},
        {2,Tuple.Create("5094-OB16",16)},
        {3,Tuple.Create("5094-OB16XT",16)},
        {4,Tuple.Create("5094-OW8I",8)},
        {5,Tuple.Create("5094-OW8IXT",8)},
        {6,Tuple.Create("5094-IA16",16)},
        {7,Tuple.Create("5094-IA16XT",16)},
        {8,Tuple.Create("5094-IM8",8)},
        {9,Tuple.Create("5094-IM8XT",8)},
        {10,Tuple.Create("5094-OA16",16)},
        {11,Tuple.Create("5094-OA16XT",16)},
        {12,Tuple.Create("5094-OB8",8)},
        {13,Tuple.Create("5094-OB8XT",8)},
        {14,Tuple.Create("5094-IB32",32)},
        {15,Tuple.Create("5094-IB32XT",32)},
        {16,Tuple.Create("5094-OB32",32)},
        {17,Tuple.Create("5094-OB32XT",32)},
        {18,Tuple.Create("5094-IF8",8)},
        {19,Tuple.Create("5094-IF8XT",8)},
        {20,Tuple.Create("5094-OF8",8)},
        {21,Tuple.Create("5094-OF8XT",8)},
        {22,Tuple.Create("5094-IY8_IR",8)},
        {23,Tuple.Create("5094-IY8_IT",8)},
        {24,Tuple.Create("5094-IJ2I",2)},
        {25,Tuple.Create("5094-IJ2IXT",2)},
        {26,Tuple.Create("5094-IF8IH",8)},
        {28,Tuple.Create("5094-IF8IHXT",8)},
        {30,Tuple.Create("5094-OF8IH",8)},
        {32,Tuple.Create("5094-OF8IHXT",8)},
        {34,Tuple.Create("5094-IB16S",24)},
        {35,Tuple.Create("5094-IB16SXT",24)},
        {36,Tuple.Create("5094-OB16S",32)},
        {37,Tuple.Create("5094-OB16SXT",32)},
        {38,Tuple.Create("5094-OW4IS",4)},
        {39,Tuple.Create("5094-OW4ISXT",4)},
        {40,Tuple.Create("5094-IJ2IS",2)},
        {41,Tuple.Create("5094-IJ2ISXT",2)},
        {42,Tuple.Create("5094-HSC",14)},
        {43,Tuple.Create("5094-HSCXT",14)},
        {44,Tuple.Create("5069-IA16",16)},
        {45,Tuple.Create("5069-IB16",16)},
        {46,Tuple.Create("5069-IB16F",16)},
        {47,Tuple.Create("5069-IB6F-3W",6)},
        {48,Tuple.Create("5069-OA16",16)},
        {49,Tuple.Create("5069-OB16",16)},
        {50,Tuple.Create("5069-OB16F",16)},
        {51,Tuple.Create("5069-OB8",8)},
        {52,Tuple.Create("5069-OW16",16)},
        {53,Tuple.Create("5069-OW4I",4)},
        {54,Tuple.Create("5069-OX4I",4)},
        {55,Tuple.Create("5069-IF8",8)},
        {56,Tuple.Create("5069-IY4",4)},
        {57,Tuple.Create("5069-OF4",4)},
        {58,Tuple.Create("5069-OF8",8)},
        {59,Tuple.Create("5069-IF4IH",4)},
        {61,Tuple.Create("5069-OF4IH",4)},
        {63,Tuple.Create("5069-IB8S_Safety",12)},
        {64,Tuple.Create("5069-IB8S_SafetyMuting",14)},
        {65,Tuple.Create("5069-OBV8S",16)},
        {67,Tuple.Create("5069-HSC2XOB4",14)},
        {68,Tuple.Create("50xx-SERIAL",1)},
        {71,Tuple.Create("5094-Adapter",0)},
        {72,Tuple.Create("5069-Adapter",0)},
        {81,Tuple.Create("5034-IB16",16)},
        {82,Tuple.Create("5034-OB16",16)},
        {83,Tuple.Create("5034-IB8",8)},
        {84,Tuple.Create("5034-OB8",8)},
        {85,Tuple.Create("5034-IF4",4)},
        {86,Tuple.Create("5034-OF4",4)},
        {89,Tuple.Create("5094-IF4IHS",4)},
        {91,Tuple.Create("5094-OF4IHS",4)},
        {93,Tuple.Create("5094-IF4IHSXT",4)},
        {95,Tuple.Create("5094-OF4IHSXT",4)},
        {1501,Tuple.Create("5034-UB8",8)},
        {1502,Tuple.Create("5034-UB8F",8)},
        {601,Tuple.Create("5034-OW4I",4)},
        {602,Tuple.Create("5034-OB4",4)},
        {701,Tuple.Create("5034-IF8C",8)},
        {702,Tuple.Create("5034-IF8V",8)},
        {703,Tuple.Create("5034-IRT4I",4)},
        {801,Tuple.Create("5034-IF4IH",4)},
        {802,Tuple.Create("5034-IF4IHXT",4)},
        {1001,Tuple.Create("5034-OF4IH",4)},
        {1101,Tuple.Create("5034-IB8S",12)},
        {1201,Tuple.Create("5034-OB8S",16)},
        {1002,Tuple.Create("5034-OF4IHXT",4)},
        {1301,Tuple.Create("5034-ENC",8)},
    };

[ExportMethod]
public void Add_Widgets(int instCount) {
    Container targetContainer = Owner.Get<ColumnLayout>("grp_Home/ChannelSts/ScrollView1/grp_Channels");
    
    IUANode Ref_Tag = null;
    IUANode Ref_Cat = null;
    IUANode Ref_Output = null;
    IUANode Ref_HART00 = null;
    
    var dialogBoxAlias = Owner.GetAlias("raSDK1_DialogBox");
    foreach (var aliasChild in dialogBoxAlias.Children) {
        try {
            if (aliasChild.BrowseName == "Ref_Tag" || aliasChild.BrowseName == "Ref_Input") {
                Ref_Tag = InformationModel.Get(((UAVariable)aliasChild).Value);
            } else if (aliasChild.BrowseName == "Ref_Cat") {
                Ref_Cat = InformationModel.Get(((UAVariable)aliasChild).Value);
            } else if (aliasChild.BrowseName == "Ref_Output") {
                Ref_Output = InformationModel.Get(((UAVariable)aliasChild).Value);
            }else if (aliasChild.BrowseName == "Ref_HART00") {
                Ref_HART00 = InformationModel.Get(((UAVariable)aliasChild).Value);
            }
            }  catch (Exception ex) {
            Log.Error($"Error retrieving node: {ex.Message}");
        }
    }

    var uaVariable1 = Ref_Cat as UAVariable;
    var Cat_Number = uaVariable1?.Value;

    var uaVariable = Ref_Tag as UAVariable;
    var variableType = uaVariable?.VariableType;
    var Data_Type = variableType?.BrowseName;
    
    //Ref_HART
    var uaVariable2 = Ref_HART00 as UAVariable;
    var variableType1 = uaVariable2?.VariableType;
    var Data_Type1 = variableType1?.BrowseName;
    
    //Banner update : Module Fault
    try
    {
        if (Data_Type.StartsWith("AB:5000_DI16:I:0") || Data_Type.StartsWith("AB:5000_DI8:I:0") || Data_Type.StartsWith("AB:5000_DI32:I:0") || Data_Type.StartsWith("AB:5000_DO16_Diag:I:0")
        || Data_Type.StartsWith("AB:5000_DO8:I:0") || Data_Type.StartsWith("AB:5000_DO16:I:0") || Data_Type.StartsWith("AB:5000_DO8_Diag:I:0") || Data_Type.StartsWith("AB:5000_DO32_Diag:I:0")
        || Data_Type.StartsWith("AB:5000_AI8:I:0") || Data_Type.StartsWith("AB:5000_AO8:I:0") || Data_Type.StartsWith("AB:5000_AI8CJ:I:0") || Data_Type.StartsWith("AB:5000_FI2:I:0")
        || Data_Type.StartsWith("AB:5000_AI4:I:0")
        || Data_Type.StartsWith("AB:5000_AI4CJ:I:0") || Data_Type.StartsWith("AB:5000_DI6:I:0") || Data_Type.StartsWith("AB:5000_DO4:I:0") || Data_Type.StartsWith("AB:5000_AO4:I:0")
        || Data_Type.StartsWith("AB:5000_ModbusSlave:I:1") || Data_Type.StartsWith("AB:5000_ModbusMaster_Status_2CC6E45D:I:1") || Data_Type.StartsWith("AB:5000_ASCII:I:1")
        || Data_Type.StartsWith("AB:5000_SFI2:I:0") || Data_Type.StartsWith("AB:5000_DI8_C11111111:I:0") || Data_Type.StartsWith("AB:5000_UB_4210842108:I:0") || Data_Type.StartsWith("AB:5000_UB_842108421:I:0") || Data_Type.StartsWith("AB:5000_UB_39CE739CE7:I:0") || Data_Type.StartsWith("AB:5000_UB_1084210842:I:0") || Data_Type.StartsWith("AB:5000_UB_MAOC_492492:I:0") || Data_Type.StartsWith("AB:5000_UB_5294A5294A:I:0") || Data_Type.StartsWith("AB:5000_DO4_Diag:I:0") || Data_Type.StartsWith("AB:5000_ModbusMaster_Status_AABC3631:I:2")
        || Data_Type.StartsWith("AB:5000_DO8_C55555555:I:0") || Data_Type.StartsWith("AB:5000_DO8_C66666666:I:0")|| Data_Type.StartsWith("AB:5000_DI8_C33333333:I:0") || Data_Type.StartsWith("AB:5000_DO8_Scheduled_CAAAA:I:0")|| Data_Type.StartsWith("AB:5000_ModbusSlave:I:2") || Data_Type.StartsWith("AB:5000_ASCII:I:2") || Data_Type.StartsWith("AB:5000_AI4CJ4:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("FaultTag").Value = "ConnectionFaulted";
        } else if (Data_Type.StartsWith("AB:5000_HSC2:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_HSC>("grp_Home/Banner_HSC").GetVariable("FaultTag").Value = "ConnectionFaulted";
        } else if (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_ENC>("grp_Home/Banner_ENC").GetVariable("FaultTag").Value = "ConnectionFaulted";
        }else if ( Data_Type.StartsWith("AB:5000_SDI16:I:0") || Data_Type.StartsWith("AB:5000_SDO16:I:0") || Data_Type.StartsWith("AB:5000_SAI4_SSV:I:0")
         || Data_Type.StartsWith("AB:5000_SAO4:I:0") || Data_Type.StartsWith("AB:5000_SDO4:I:0") || Data_Type.StartsWith("AB:5000_SDI8:I:0") || Data_Type.StartsWith("AB:5000_SDO8:I:0")
          || Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0") || Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0")) {
           Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("FaultTag").Value = "ConnectionFaulted";
        }
    }
    catch (Exception ex)
    {
        Log.Error($"Problem with Banner or FaultTag {ex.Message}");
    }

    // Step 1: Find out Catlog Number
    int CatNo = 0;
    try {
        //diCount = GetInstCountForVariableType(Cat_Number);
        CatNo = (CatNo_50xx.TryGetValue(Cat_Number, out var foundTuple)) ? Cat_Number : 0;
    } catch (Exception ex) {
        Log.Error($"Error getting Cat Number: {ex.Message}");
        return; // Exit method if error occurs
    }
    // Step 2: Find out diCount
    int diCount = 0;
    try {
        //diCount = GetInstCountForVariableType(Cat_Number);
        diCount = (CatNo_50xx.TryGetValue(Cat_Number, out var foundTuple)) ? foundTuple.Item2 : 0;
    } catch (Exception ex) {
        Log.Error($"Error getting instance count: {ex.Message}");
        return; // Exit method if error occurs
    }

    
   
    // Step 3: Calculate the number of Accordions required
    int accordionCount = 0;
    if (Data_Type.StartsWith("AB:5000_SDO4:I:0")) {
        accordionCount = 2;
    }
    else if (Data_Type.StartsWith("AB:5000_HSC2:I:0") || ((CatNo == 64) & (Data_Type.StartsWith("AB:5000_SDI8:I:0")))){
        accordionCount = 3;
    }else if (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")){
        accordionCount = 4;
    }
        else
        {
            accordionCount = (diCount + 7) / 8; // Calculate the number of Accordions needed
        }

    // Channel fault banner
    for (int i = 0; i < diCount; i++) {
      try
    {
        if (Data_Type.StartsWith("AB:5000_DI16:I:0") || Data_Type.StartsWith("AB:5000_DI8:I:0") || Data_Type.StartsWith("AB:5000_DO16:I:0") || Data_Type.StartsWith("AB:5000_DO8_Diag:I:0") || Data_Type.StartsWith("AB:5000_DI32:I:0")
        || Data_Type.StartsWith("AB:5000_DO32_Diag:I:0") || Data_Type.StartsWith("AB:5000_DI6:I:0") || Data_Type.StartsWith("AB:5000_DO16_Diag:I:0")
        || Data_Type.StartsWith("AB:5000_DO4:I:0") || Data_Type.StartsWith("AB:5000_DO8:I:0") || Data_Type.StartsWith("AB:5000_DI8_C11111111:I:0") || Data_Type.StartsWith("AB:5000_UB_4210842108:I:0") || Data_Type.StartsWith("AB:5000_UB_39CE739CE7:I:0") || Data_Type.StartsWith("AB:5000_UB_1084210842:I:0") || Data_Type.StartsWith("AB:5000_UB_842108421:I:0")  || Data_Type.StartsWith("AB:5000_UB_MAOC_492492:I:0")  || Data_Type.StartsWith("AB:5000_UB_5294A5294A:I:0") || Data_Type.StartsWith("AB:5000_DO4_Diag:I:0")
         || Data_Type.StartsWith("AB:5000_DO8_C55555555:I:0") || Data_Type.StartsWith("AB:5000_DO8_C66666666:I:0")|| Data_Type.StartsWith("AB:5000_DI8_C33333333:I:0") || Data_Type.StartsWith("AB:5000_DO8_Scheduled_CAAAA:I:0") || Data_Type.StartsWith("AB:5000_AI4CJ4:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "Pt" + i.ToString("D2") + "/Fault";//Pt00.Fault
        }else if (Data_Type.StartsWith("AB:5000_AI8:I:0") || Data_Type.StartsWith("AB:5000_AO8:I:0") || Data_Type.StartsWith("AB:5000_FI2:I:0") || Data_Type.StartsWith("AB:5000_SFI2:I:0") || Data_Type.StartsWith("AB:5000_AI4:I:0")  
         || Data_Type.StartsWith("AB:5000_AO4:I:0") || ((CatNo == 22) & Data_Type.StartsWith("AB:5000_AI8CJ:I:0")) || ((CatNo == 56) & Data_Type.StartsWith("AB:5000_AI4CJ:I:0"))) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "Ch" + i.ToString("D2") + "/Fault";//Ch00.Fault
        }else if ((CatNo == 23) & (Data_Type.StartsWith("AB:5000_AI8CJ:I:0"))) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "CJCh" + i.ToString("D2") + "/Fault";//CJCh00.Fault
        }else if (Data_Type.StartsWith("AB:5000_ModbusMaster_Status_2CC6E45D:I:1") || Data_Type.StartsWith("AB:5000_ModbusMaster_Status_AABC3631:I:2")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "Fault";//.Fault
        }else if (Data_Type.StartsWith("AB:5000_ModbusSlave:I:1") || Data_Type.StartsWith("AB:5000_ModbusSlave:I:2")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "Slave/Fault";//Slave.Fault
        }else if (Data_Type.StartsWith("AB:5000_ASCII:I:1") || Data_Type.StartsWith("AB:5000_ASCII:I:2")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner>("grp_Home/Banner").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "ASCII/Fault";//ASCII.Fault
        }else if (Data_Type.StartsWith("AB:5000_SAI4_SSV:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "Ch" + i.ToString("D2")+"/Ch/Fault";//Ch00.Ch.Fault  
        }else if (Data_Type.StartsWith("AB:5000_SAO4:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ i.ToString("D2")).Value = "Ch" + i.ToString("D2")+"/Fault";//Ch00.Fault  
        }/*else if (Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_ENC>("grp_Home/Banner_ENC").GetVariable("Counter00_Fault").Value = "SSI" + i.ToString("D2")+"/Fault";//SSI00.Fault  
        }else if (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0")) {
            Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_ENC>("grp_Home/Banner_ENC").GetVariable("Counter00_Fault").Value = "Counter" + i.ToString("D2")+"/Fault";//Counter00.Fault  
        }*/
    
    }
    catch (Exception ex)
    {
        Log.Error($"Problem with Banner or Ch_FaultTag {ex.Message}");
    }  
    }

    // Step 4: If diCount is greater than 8, create Accordions
    if (accordionCount > 1) {
        for (int i = 0; i < accordionCount; i++) {
            try {
                // Create a new accordion panel
                Container accordionPanel = InformationModel.Make<Accordion>("AccordionPanel" + i.ToString());
                accordionPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                accordionPanel.VerticalAlignment = VerticalAlignment.Top;
                accordionPanel.BottomMargin = 8;
                targetContainer.Add(accordionPanel);
                
                // Add header label
                Label Header_Label = InformationModel.Make<Label>("AccordionPanel_HL" + i.ToString());
                var Header_Height = InformationModel.Make<Label>("AccordionPanel_HL" + i.ToString());
                accordionPanel.Get<AccordionHeader>("Header")?.Add(Header_Label);
                accordionPanel.Get<AccordionHeader>("Header")?.Add(Header_Height);
                Header_Height.Height = 30;
                Header_Label.TopMargin = 4;
                
                // Add content column layout
                ColumnLayout Content_CL = InformationModel.Make<ColumnLayout>("AccordionPanel_CL" + i.ToString());
                Content_CL.HorizontalAlignment = HorizontalAlignment.Stretch;
                Content_CL.VerticalAlignment = VerticalAlignment.Top;
                Content_CL.VerticalGap = 0; Content_CL.LeftMargin = 8; Content_CL.TopMargin = 8; Content_CL.RightMargin = 8; Content_CL.BottomMargin = 8;
                accordionPanel.Get<AccordionContent>("Content")?.Add(Content_CL);
                
                // Set the title text
                var HL = accordionPanel.Get<Label>("Header/AccordionPanel_HL" + i.ToString());
                string title = "";
                 int startIndex = 0; int endIndex = 0;
                if (((i == 0) || (i == 1)) & (Data_Type.StartsWith("AB:5000_SDO4:I:0")))
                {
                     startIndex = 0;
                     endIndex = 4;    
                }else if ((i == 2) & (CatNo == 64) & (Data_Type.StartsWith("AB:5000_SDI8:I:0")))
                {
                    startIndex = 2;
                     endIndex = 4;
                }else if ((i == 2) & (Data_Type.StartsWith("AB:5000_SDI16:I:0")))
                {
                    startIndex = 0;
                    endIndex = Math.Min(startIndex + 8, diCount);
                }else if ((i == 0) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")))//ENC Counter;SSI
                {
                    startIndex = 0;
                    endIndex = 1;
                }else if ((i == 1) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")))//ENC INPUT
                {
                    startIndex = 0;
                    endIndex = 1;
                }else if ((i == 2) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")))//ENC OUTPUT
                {
                    startIndex = 0;
                    endIndex = 2;
                }else if ((i == 3) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")))// ENC WINDOW
                {
                    startIndex = 0;
                    endIndex = 4;
                }else if (((i == 2) & (Data_Type.StartsWith("AB:5000_SDO16:I:0"))) || ((i == 1) & (Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0"))))
                {
                    startIndex = 0;
                    endIndex = Math.Min(startIndex + 8, diCount); 
                }else if ((i == 3) & (Data_Type.StartsWith("AB:5000_SDO16:I:0")))
                {
                    startIndex = 8;
                    endIndex = Math.Min(startIndex + 8, diCount);
                }else if ((i == 0) & (Data_Type.StartsWith("AB:5000_HSC2:I:0")))
                {
                    startIndex = 0;
                    endIndex = 2;
                }else if ((i == 1) & (Data_Type.StartsWith("AB:5000_HSC2:I:0") || Data_Type.StartsWith("AB:5000_SDI8:I:0") || Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0")))
                {
                    startIndex = 0;
                    endIndex = 4;
                }else if (((i == 2) & Data_Type.StartsWith("AB:5000_HSC2:I:0")) || ((i == 1) & Data_Type.StartsWith("AB:5000_SDO8:I:0")))
                {
                    startIndex = 0;
                    endIndex = Math.Min(startIndex + 8, diCount);
                }else
                {
                   startIndex = i * 8;
                   endIndex = Math.Min(startIndex + 8, diCount);
                }
                
                if (Data_Type.StartsWith("AB:5000_DI16:I:0") || Data_Type.StartsWith("AB:5000_DI32:I:0") || ((i < 2) & Data_Type.StartsWith("AB:5000_SDI16:I:0")) || ((i < 1) & Data_Type.StartsWith("AB:5000_SDI8:I:0"))
                 || ((i < 1) & Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0")) || ((i == 1) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")))) {
                    title = " Inputs - Ch" + startIndex.ToString() + "..." + (endIndex - 1).ToString();
                }else if (((i == 2) & Data_Type.StartsWith("AB:5000_SDI16:I:0")) || ((i == 1) & Data_Type.StartsWith("AB:5000_SDI8:I:0")) || ((i == 1) & (Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0"))))  {
                    title = " Test Output - Ch" + startIndex.ToString() + "..." +  (endIndex -1).ToString();
                }else if (((i == 1) & Data_Type.StartsWith("AB:5000_SDO8:I:0")) || ((i > 1) & Data_Type.StartsWith("AB:5000_SDO16:I:0")) || ((i == 1) & (Data_Type.StartsWith("AB:5000_SDO4:I:0")))
                 || ((i == 1) & (Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0"))))  {
                    title = " Readback - Ch" + startIndex.ToString() + "..." +  (endIndex -1).ToString();
                }else if (Data_Type.StartsWith("AB:5000_DO16_Diag:I:0") || Data_Type.StartsWith("AB:5000_DO16:I:0") || Data_Type.StartsWith("AB:5000_DO32_Diag:I:0") || ((i < 2) & Data_Type.StartsWith("AB:5000_SDO16:I:0"))
                || ((i == 0) & Data_Type.StartsWith("AB:5000_SDO4:I:0")) || ((i == 1) & Data_Type.StartsWith("AB:5000_HSC2:I:0")) || ((i == 0) & Data_Type.StartsWith("AB:5000_SDO8:I:0"))
                 || ((i == 0) & Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0")) || ((i == 2) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")))) {
                    title = " Output - Ch" + startIndex.ToString() + "..." +  (endIndex - 1).ToString();
                }else if ((i == 0) & (Data_Type.StartsWith("AB:5000_HSC2:I:0" ) || Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0" ))) {
                    title = " Counters - Ch" + startIndex.ToString() + "..." +  (endIndex - 1).ToString();
                }else if ((i == 0) & Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0" )) {
                    title = " SSI - " + startIndex.ToString() + "..." +  (endIndex - 1).ToString();
                }
                else if ((i == 2) & (Data_Type.StartsWith("AB:5000_HSC2:I:0")) || (i == 3) & (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0") || Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0"))) {
                    title = " Windows - Ch" + startIndex.ToString() + "..." +  (endIndex - 1).ToString();
                }else if ((i == 2) & (CatNo == 64) & (Data_Type.StartsWith("AB:5000_SDI8:I:0"))) {
                    title = " Muting Lamp Outputs - " + startIndex.ToString() + "..." +  (endIndex - 1).ToString();
                }
                HL.Text = title;
                HL.Style = "Heading";

                //Safety Banner
                for (int k = 0; k < diCount; k++) {
                try
                { 
                if ((i < 2) && (k < 16) && Data_Type.StartsWith("AB:5000_SDI16:I:0")) 
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ k.ToString("D2")).Value = "Pt" + k.ToString("D2")+"/Status";//Pt00.Status
                }
                else if ((i < 2) && (k < 16) && Data_Type.StartsWith("AB:5000_SDO16:I:0"))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ k.ToString("D2")).Value = "Pt" + k.ToString("D2")+"/Status";//Pt00.Status  
                }else if ((i < 1) && (k < 4) && Data_Type.StartsWith("AB:5000_SDO4:I:0"))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ k.ToString("D2")).Value = "Pt" + k.ToString("D2")+"/Status";//Pt00.Status  
                }else if ((i < 1) && (k < 8) && Data_Type.StartsWith("AB:5000_SDI8:I:0"))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ k.ToString("D2")).Value = "Pt" + k.ToString("D2")+"/Status";//Pt00.Status 
                }else if (((i < 1) && (k < 8) && Data_Type.StartsWith("AB:5000_SDO8:I:0")) || ((i < 1) && (k < 8) && Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0")))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ k.ToString("D2")).Value = "Pt" + k.ToString("D2")+"/Status";//Pt00.Status
                }
                else if ((i < 1) && (k < 8) && Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0"))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_FaultTag"+ k.ToString("D2")).Value = "Pt" + k.ToString("D2")+"/Status";//Pt00.Status  
                }
                }
                catch (Exception ex)
                {
                Log.Error($"Problem with Banner or Ch_FaultTag {ex.Message}");
                }}

                //Safety Banner: TestOutput
                for (int k = 0; k < diCount; k++) {
                try
                { 
                if ((i == 2) && (k < 8) && Data_Type.StartsWith("AB:5000_SDI16:I:0")) 
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_TestOutputFaultTag"+ k.ToString("D2")).Value = "Test" + k.ToString("D2")+"/Status";//Test00.Status
                }else if ((i == 1) && (k < 4) && Data_Type.StartsWith("AB:5000_SDI8:I:0"))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_TestOutputFaultTag"+ k.ToString("D2")).Value = "Test" + k.ToString("D2")+"/Status";//Test00.Status 
                }else if ((i == 1) && (k < 4) && Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0"))
                {
                Owner.Get<raC_5_07_raC_Dvc_50XX_XXXX_Banner_Safety>("grp_Home/Banner_Safety").GetVariable("Ch_TestOutputFaultTag"+ k.ToString("D2")).Value = "Test" + k.ToString("D2")+"/Status";//Test00.Status 
                }
                }
                catch (Exception ex)
                {
                Log.Error($"Problem with Banner or Ch_FaultTag {ex.Message}");
                }}

                // Add widgets to the accordion container
                for (int j = startIndex; j < endIndex; j++) {
                    if (Data_Type.StartsWith("AB:5000_DI16:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DI16_I_0>("DIWidget" + j.ToString());//5094-IB16
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";  //Pt00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault"; //Pt00.Fault
                            Content_CL?.Add(newWidgetDI);
                        }
                    } else if (Data_Type.StartsWith("AB:5000_DI32:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DI32_I_0>("DIWidget" + j.ToString());//5094-IB32
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";  //Pt00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault"; //Pt00.Fault
                            Content_CL?.Add(newWidgetDI);
                        }
                    } else if (Data_Type.StartsWith("AB:5000_DO16_Diag:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DO16_Diag_I_0>("DOWidget" + j.ToString());//5094-OB16;  5069-OB16 
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";  //Pt00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault"; //Pt00.Fault
                            Content_CL?.Add(newWidgetDI);
                        }
                    } else if (Data_Type.StartsWith("AB:5000_DO16:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DO16_I_0>("AOWidget" + j.ToString());//5094-OA16; 5069-OA16
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";  //Pt00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault"; //Pt00.Fault
                            Content_CL?.Add(newWidgetDI);
                        }
                    }else if (Data_Type.StartsWith("AB:5000_DO32_Diag:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DO32_Diag_I_0>("AOWidget" + j.ToString());//5094-OB32
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";  //Pt00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault"; //Pt00.Fault
                            Content_CL?.Add(newWidgetDI);
                        }
                    }else if (Data_Type.StartsWith("AB:5000_SDI16:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDI16_I_0_50XX_Home>("AIWidget" + j.ToString());//5094-IB16S
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==2) 
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Test" + j.ToString("D2"); //Test00.Uncertain                       
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Test" + j.ToString("D2")+"/Readback"; //Test00.Readback
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Test" + j.ToString("D2") + "/Status"; //Test00.Status                                                   
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        }
                        Content_CL?.Add(newWidgetDI);
                        }
                    }else if ((CatNo == 63) & Data_Type.StartsWith("AB:5000_SDI8:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDI16_I_0_50XX_Home>("AIWidget" + j.ToString());//5069-IB8S
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==1) 
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Test" + j.ToString("D2"); //Test00.Uncertain                       
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Test" + j.ToString("D2")+"/Readback"; //Test00.Readback
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Test" + j.ToString("D2") + "/Status"; //Test00.Status                                                   
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        }
                        Content_CL?.Add(newWidgetDI);
                        }
                    }else if ((CatNo == 64) & Data_Type.StartsWith("AB:5000_SDI8:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDI8_I_0_Muting_50XX_Home>("AIWidget" + j.ToString());//5069-IB8S_SafetyMuting
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==2) 
                        {                       
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Output/Muting" + j.ToString("D2")+"/Data"; //Muting02.Data 
                        newWidgetDI.GetVariable("Cfg_Fault").Value = false;                                                   
                        }else if (i==1) 
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Test" + j.ToString("D2"); //Test00.Uncertain                       
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Tag/Test" + j.ToString("D2")+"/Readback"; //Test00.Readback
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Test" + j.ToString("D2") + "/Status"; //Test00.Status 
                        newWidgetDI.GetVariable("Cfg_Fault").Value = true;                                                  
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Tag/Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        newWidgetDI.GetVariable("Cfg_Fault").Value = true;
                        }
                        Content_CL?.Add(newWidgetDI);
                        }
                    }else if (Data_Type.StartsWith("AB:5000_SDI8_NoDiag:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDI8_NoDiag_I_0_Home>("AIWidget" + j.ToString());//5034-IB8S
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==1) 
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Test" + j.ToString("D2"); //Test00.Uncertain                       
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Test" + j.ToString("D2")+"/Readback"; //Test00.Readback
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Test" + j.ToString("D2") + "/Status"; //Test00.Status                                                   
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        }
                        Content_CL?.Add(newWidgetDI);
                        }
                    }else if (Data_Type.StartsWith("AB:5000_SDO8_NoDiag:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDO8_NoDiag_I_0_50XX_Home>("AOWidget" + j.ToString());//5034-OB8S
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==1)
                        {                        
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2")+"/Readback"; //Pt00.Readback 
                        newWidgetDI.GetVariable("Cfg_Fault").Value = false;                                     
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Output/" + "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        newWidgetDI.GetVariable("Cfg_Fault").Value = true;
                        }
                        Content_CL?.Add(newWidgetDI);
                    
                        }
                    }else if (Data_Type.StartsWith("AB:5000_SDO16:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDO16_I_0_50XX_Home>("AOWidget" + j.ToString());//5094-OB16S
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if ((i==2) || (i==3))
                        {                        
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2")+"/Readback"; //Pt00.Readback 
                        newWidgetDI.GetVariable("Cfg_Fault").Value = false;                                     
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Output/" + "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        newWidgetDI.GetVariable("Cfg_Fault").Value = true;
                        }
                        Content_CL?.Add(newWidgetDI);
                    
                        }
                    }else if (Data_Type.StartsWith("AB:5000_SDO8:I:0")) { 
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDO8_I_0_50XX_Home>("AOWidget" + j.ToString());//5069-OBV8S-SafetyBipolar
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==1)
                        {                        
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2")+"/Readback"; //Pt00.Readback 
                        newWidgetDI.GetVariable("Cfg_Fault").Value = false;                                     
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Output/" + "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        newWidgetDI.GetVariable("Cfg_Fault").Value = true;
                        }
                        Content_CL?.Add(newWidgetDI);
                    
                        }
                    }else if (Data_Type.StartsWith("AB:5000_SDO4:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SDO4_I_0_50XX_Home>("AOWidget" + j.ToString());//5094-OW4IS
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");                                                                   
                        if (i==1)
                        {                          
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2")+"/Readback"; //Pt00.Readback
                        newWidgetDI.GetVariable("Cfg_Fault").Value = false;                                      
                        }
                        else
                        {
                        newWidgetDI.GetVariable("Cfg_Alarm").Value = "Pt" + j.ToString("D2"); //Pt00.Uncertain         
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Ref_Output/" + "Pt" + j.ToString("D2")+"/Data"; //Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ref_Tag/" + "Pt" + j.ToString("D2") + "/Status"; //Pt00.Status
                        newWidgetDI.GetVariable("Cfg_Fault").Value = true;
                        }
                        Content_CL?.Add(newWidgetDI);
                        }
                    }else if (Data_Type.StartsWith("AB:5000_HSC2:I:0")) {
                        if (i == 0) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_HSC2_I_0_Counter>("AIWidget" + j.ToString());//5094-HSC1
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Counter" + j.ToString("D2")+"/Fault";//Counter00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 1) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_HSC2_I_0>("AIWidget" + j.ToString());//5094-HSC2
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Output" + j.ToString("D2")+"/Data";//Output00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Output" + j.ToString("D2")+"/Fault";//Output00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 2) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_HSC2_I_0_3>("AIWidget" + j.ToString());//5094-HSC3
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Window" + j.ToString("D2")+"/InWindow";//Window00.InWindow
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Window" + j.ToString("D2")+"/NotANumber";//Window00.NotANumber
                            Content_CL?.Add(newWidgetDI);
                            }
                        }
                        
                    }else if (Data_Type.StartsWith("AB:5000_ENC_C_I1_O11:I:0")) {
                        if (i == 0) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_Counter>("AIWidget" + j.ToString());//5034-ENC
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Counter" + j.ToString("D2")+"/Fault";//Counter00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 1) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_1>("AIWidget" + j.ToString());//5034-ENC1
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Input" + j.ToString("D2")+"/Data";//Input00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Input" + j.ToString("D2")+"/Fault";//Input00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 2) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_2>("AIWidget" + j.ToString());//5034-ENC2
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Output" + j.ToString("D2")+"/Data";//Output00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Output" + j.ToString("D2")+"/Fault";//Output00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 3) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_3>("AIWidget" + j.ToString());//5034-ENC3
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Window" + j.ToString("D2")+"/InWindow";//Window00.InWindow
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Window" + j.ToString("D2")+"/NotANumber";//Window00.NotANumber
                            Content_CL?.Add(newWidgetDI);
                            }
                        }
                        
                    }else if (Data_Type.StartsWith("AB:5000_ENC_S_I1_O11:I:0")) {
                        if (i == 0) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC_S_I1_O11_I_0>("AIWidget" + j.ToString());//5034-ENC SSI
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="SSI" + j.ToString("D2")+"/Data";//SSI00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "SSI" + j.ToString("D2")+"/Fault";//SSI00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 1) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_1>("AIWidget" + j.ToString());//5034-ENC1
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Input" + j.ToString("D2")+"/Data";//Input00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Input" + j.ToString("D2")+"/Fault";//Input00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 2) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_2>("AIWidget" + j.ToString());//5034-ENC2
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Output" + j.ToString("D2")+"/Data";//Output00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Output" + j.ToString("D2")+"/Fault";//Output00.Fault
                            Content_CL?.Add(newWidgetDI);
                            }
                        }else if (i == 3) {
                            var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ENC1_DI1_DO2_00660101_I_0_3>("AIWidget" + j.ToString());//5034-ENC3
                            if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Window" + j.ToString("D2")+"/InWindow";//Window00.InWindow
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Window" + j.ToString("D2")+"/NotANumber";//Window00.NotANumber
                            Content_CL?.Add(newWidgetDI);
                            }
                        }
                        
                    }   
                }
            } catch (Exception ex) {
                Log.Error($"Error creating accordion panel: {ex.Message}");
            }
        }
    } else {
        // Step 5: If diCount is less than or equal to 8, add widgets directly to the targetContainer
        for (int j = 0; j < diCount; j++) {
            try {
                if (Data_Type.StartsWith("AB:5000_DI8:I:0") || Data_Type.StartsWith("AB:5000_DI6:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DI8_I_0>("DIWidget" + j.ToString());//5094-IM8; 5069-IB6F-3W
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";//Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault";//Pt00.Fault
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_DO8:I:0") || Data_Type.StartsWith("AB:5000_DO4:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DO8_I_0>("DOWidget" + j.ToString());//5094-OW8I; 5069-OW4I
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";//Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault";//Pt00.Fault
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_DO8_C55555555:I:0") || Data_Type.StartsWith("AB:5000_DO8_C66666666:I:0") || Data_Type.StartsWith("AB:5000_UB_842108421:I:0") || Data_Type.StartsWith("AB:5000_UB_MAOC_492492:I:0") || Data_Type.StartsWith("AB:5000_UB_1084210842:I:0") || Data_Type.StartsWith("AB:5000_UB_39CE739CE7:I:0")  || Data_Type.StartsWith("AB:5000_UB_MAOC_492492:I:0") || Data_Type.StartsWith("AB:5000_DO8_Scheduled_CAAAA:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DO8_C55555555_I_0>("DOWidget" + j.ToString());//5034-UB8_OUT;5034-UB8F_OUT
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";//Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault";//Pt00.Fault
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_DO8_Diag:I:0") || Data_Type.StartsWith("AB:5000_DO4_Diag:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DO8_Diag_I_0>("DOWidget" + j.ToString());//5094-OB8; 5069-OB8; 5034-OB4
                    if (newWidgetDI != null)
                        {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value = "Pt" + j.ToString("D2") + "/Data";//Pt00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2") + "/Fault";//Pt00.Fault
                            targetContainer.Add(newWidgetDI);
                        }
                }else if (Data_Type.StartsWith("AB:5000_AI8:I:0") || Data_Type.StartsWith("AB:5000_AI4:I:0")) {
                    if ((CatNo == 18 ) || (CatNo == 19 ) || (CatNo == 55 ) || (CatNo == 85 ) || (CatNo == 701 ) || (CatNo == 702 )) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AI8_I_0>("AIWidget" + j.ToString()); //5094-IF8; 5069-IF8; 5034-IF8C; 5034-IF8V
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                            targetContainer.Add(newWidgetDI);
                        }
                    } else if (((CatNo == 26) || (CatNo == 28) || (CatNo == 59) || (CatNo == 801) || (CatNo == 802)) && Data_Type1.StartsWith("AB:5000_HART4:I:0"))   {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AI8_I_0_HART>("AIWidget" + j.ToString());//5094-IF8IH; 5069-IF4IH_HART; 5034-IF4IH_HART
	                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                        newWidgetDI.GetVariable("Cfg_Connection").Value = "/";//HART
                        targetContainer.Add(newWidgetDI);
                    }
				} else if (((CatNo == 26) || (CatNo == 28) || (CatNo == 59) || (CatNo == 801) ||  (CatNo == 802)) && Data_Type1.StartsWith("AB:5000_HART_PAX:I:0")){
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AI8_I_0_HART>("AIWidget" + j.ToString());//5094-IF8IH-PlantPAx; 5069-IF4IH_PlantPAx; 5034-IF4IH_PlantPAx
	                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                        newWidgetDI.GetVariable("Cfg_Connection").Value = "/PAxDevice/";//PlantPAx
                        targetContainer.Add(newWidgetDI);
                    }
				}
            }else if (Data_Type.StartsWith("AB:5000_AO8:I:0") || Data_Type.StartsWith("AB:5000_AO4:I:0")) {
                    if ((CatNo == 20 ) || (CatNo == 21 ) || (CatNo == 57 ) || (CatNo == 58 ) || (CatNo == 86 )) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AO8_I_0>("AIWidget" + j.ToString()); //5094-OF8; 5069-OF4; 5069-OF8
                        if (newWidgetDI != null) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                            targetContainer.Add(newWidgetDI);
                        }
                    } else if (((CatNo == 30) || (CatNo == 32) || (CatNo == 61) || (CatNo == 1001) || (CatNo == 1002)) && Data_Type1.StartsWith("AB:5000_HART4:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AO8_I_0_HART>("AIWidget" + j.ToString());//5094-OF8H; 5069-OF4IH_HART; 5034-OF4IH_HART
	                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                        newWidgetDI.GetVariable("Cfg_Connection").Value = "/";//HART
                        targetContainer.Add(newWidgetDI);
                    }
				} else if (((CatNo == 30) || (CatNo == 32) || (CatNo == 61) || (CatNo == 1001) || (CatNo == 1002)) && Data_Type1.StartsWith("AB:5000_HART_PAX:I:0")) {
                        var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AO8_I_0_HART>("AIWidget" + j.ToString());//5094-OF8H-PlantPAx; 5069-OF4IH_PlantPAx; 5034-OF4IH_PlantPAx
	                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                        newWidgetDI.GetVariable("Cfg_Connection").Value = "/PAxDevice/";//PlantPAx
                        targetContainer.Add(newWidgetDI);
                    }
				}
            }else if (Data_Type.StartsWith("AB:5000_AI8CJ:I:0") || Data_Type.StartsWith("AB:5000_AI4CJ:I:0") || Data_Type.StartsWith("AB:5000_AI4CJ4:I:0")) {
                if ((CatNo == 22) || (CatNo == 56) || (CatNo == 703)) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AI8CJ_I_0_IR>("AIWidget" + j.ToString());//5094-IY8-IR;5069-IY4;5034-IRT4I
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault 
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (CatNo == 23) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_AI8CJ_I_0_IT>("AIWidget" + j.ToString());//5094-IY8-IT
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="CJCh" + j.ToString("D2")+"/Temperature";//CJCh00.Temperature
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "CJCh" + j.ToString("D2")+"/Fault";//CJCh00.Fault
                        targetContainer?.Add(newWidgetDI);
                    }
                }
            }else if (Data_Type.StartsWith("AB:5000_FI2:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_FI2_I_0>("AIWidget" + j.ToString());//5094-IJ2I
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Frequency";//Ch00.Frequency
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                        targetContainer.Add(newWidgetDI);
                    }    
                }else if (Data_Type.StartsWith("AB:5000_SFI2:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SFI2_I_0>("AIWidget" + j.ToString());//5094-IJ2IS
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Frequency";//Ch00.Frequency
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_ModbusSlave:I:1") || Data_Type.StartsWith("AB:5000_ModbusSlave:I:2")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ModbusSlave_I_1>("AIWidget" + j.ToString());//
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_ModbusMaster_Status_2CC6E45D:I:1") || Data_Type.StartsWith("AB:5000_ModbusMaster_Status_AABC3631:I:2")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ModbusMaster_Status>("AIWidget" + j.ToString());//
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_ASCII:I:1") || Data_Type.StartsWith("AB:5000_ASCII:I:2")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_ASCII_I_1>("AIWidget" + j.ToString());//
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        targetContainer.Add(newWidgetDI);
                    }
                }else if (Data_Type.StartsWith("AB:5000_SAI4_SSV:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SAI4_SSV_I_0>("AIWidget" + j.ToString());//5094-IF4IHS
                    if (newWidgetDI != null) {
                        if (((CatNo == 89) || (CatNo == 93)) && Data_Type1.StartsWith("AB:5000_HART4:I:0")){
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Ch/Data";//Ch00.Ch.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Ch/Fault";//Ch00.Ch.Fault
                            newWidgetDI.GetVariable("Cfg_Connection").Value = "/";//HART
                            targetContainer.Add(newWidgetDI);   
                        }
                        else if (((CatNo == 89) || (CatNo == 93)) && Data_Type1.StartsWith("AB:5000_HART_PAX:I:0")){
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Ch/Data";//Ch00.Ch.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Ch/Fault";//Ch00.Ch.Fault
                            newWidgetDI.GetVariable("Cfg_Connection").Value = "/PAxDevice/";//PlantPAx
                            targetContainer.Add(newWidgetDI);
                        }
                    }
                }else if (Data_Type.StartsWith("AB:5000_SAO4:I:0")) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_SAO4_I_0>("AIWidget" + j.ToString());//5094-OF4IHS_HART
                    if (newWidgetDI != null) {
                        if (((CatNo == 91) || (CatNo == 95)) && Data_Type1.StartsWith("AB:5000_HART4:I:0")) {
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                            newWidgetDI.GetVariable("Cfg_Connection").Value = "/";//HART
                            targetContainer.Add(newWidgetDI);   
                        }
                        else if (((CatNo == 91) || (CatNo == 95)) && Data_Type1.StartsWith("AB:5000_HART_PAX:I:0")){
                            newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                            newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                            newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                            newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Ch" + j.ToString("D2")+"/Data";//Ch00.Data
                            newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Ch" + j.ToString("D2")+"/Fault";//Ch00.Fault
                            newWidgetDI.GetVariable("Cfg_Connection").Value = "/PAxDevice/";//PlantPAx
                            targetContainer.Add(newWidgetDI);
                        }
                    }
                }else if (Data_Type.StartsWith("AB:5000_DI8_C11111111:I:0") || Data_Type.StartsWith("AB:5000_UB_4210842108:I:0") || Data_Type.StartsWith("AB:5000_UB_5294A5294A:I:0") || Data_Type.StartsWith("AB:5000_UB_5294A5294A:I:0") || Data_Type.StartsWith("AB:5000_DO8_C66666666:I:0") || Data_Type.StartsWith("AB:5000_DI8_C33333333:I:0")  ) {
                    var newWidgetDI = InformationModel.Make<raC_5_07_raC_Dvc_AB_5000_DI8_C11111111_I_0>("DIWidget" + j.ToString());//5034-UB8_INPUT
                    if (newWidgetDI != null) {
                        newWidgetDI.VerticalAlignment = VerticalAlignment.Top;
                        newWidgetDI.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newWidgetDI.GetVariable("Cfg_ChannelNo").Value = j.ToString("D2");
                        newWidgetDI.GetVariable("Channel_DataTag_Member").Value ="Pt" + j.ToString("D2")+"/Data";//Pt00.Data
                        newWidgetDI.GetVariable("Channel_FaultTag_Member").Value = "Pt" + j.ToString("D2")+"/Fault";//Pt00.Fault
                        targetContainer.Add(newWidgetDI);
                    }
                }
               
                }catch (Exception ex) {
                Log.Error($"Error adding widget directly to target container: {ex.Message}");
            }
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
