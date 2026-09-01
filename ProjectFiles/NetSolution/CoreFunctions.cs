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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using Cca.So.Optix.Extensions;
using FTOptix.Alarm;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.UI;
using FTOptix.WebUI;
using Google.Protobuf.WellKnownTypes;
using UAManagedCore;
using FTOptix.ODBCStore;
using FTOptix.InfluxDBStoreLocal;
using FTOptix.InfluxDBStore;
using FTOptix.InfluxDBStoreRemote;
using FTOptix.DataLogger;
using static System.Net.Mime.MediaTypeNames;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using OpcUa = UAManagedCore.OpcUa;
using Struct = UAManagedCore.Struct;
#endregion
public class CoreFunctions : BaseNetLogic
{
    
    private LocalizedText discKey;
    private string translatedText;


    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

#region DataReady Info Model Lists

    public void UpdateEnumDisplayText(IUANode EnumObj, string[] enumTextList, Int32 ModelCount)
    {
        try
        {
            // Convert enumTextList to Tuple.
            List<(int, string, string, string, string)> enumerationDataCollection = new List<(int, string, string, string, string)>();

            if(ModelCount <= 0)
            {
                Log.Warning("CoreFunctions - UpdateEnumDisplayText", "No models found, setting "+EnumObj.BrowseName+" to Not Used");
                var enumData = (0, "Not Used", "en-US", "", "");
                enumerationDataCollection.Add(enumData);
            }
            else
            {
                for (int i = 0; i < enumTextList.Length; i++)
                {
                    if (i < ModelCount)
                    {
                        var enumData = (i, enumTextList[i], "en-US", "", "");
                        Log.Info("CoreFunctions - UpdateEnumDisplayText", "Adding: "+i+" - "+EnumObj.BrowseName+"."+enumTextList[i]);
                        enumerationDataCollection.Add(enumData);
                    }
                }    
            }

            if (EnumObj.GetVariable("EnumValues") != null)
            {
                Log.Warning("CoreFunctions - UpdateEnumDisplayText", "Deleting old enumerations for "+EnumObj.BrowseName);
                EnumObj.GetVariable("EnumValues").Delete();
            }
            
            NodeId newEnumerationNodeId = NodeId.Random(EnumObj.Owner.NodeId.NamespaceIndex);                   // Generate a NodeId for the new Enumeration
            
            List<EnumField> newEnumerationFields = new List<EnumField>();                                       // For enumeration, you need to create 
            List<Struct> newEnumerationValues = new List<Struct>();                                             // the reference structures and the values to be assigned

            foreach (var enumerationData in enumerationDataCollection)
            {
                LocalizedText displayValue = new LocalizedText(enumerationData.Item2, enumerationData.Item3);   // Generate the localizedText of display Value with Item 2 (value) and Item 3 (LocaleId)
                LocalizedText description = new LocalizedText(enumerationData.Item4, enumerationData.Item5);    // Generate the localizedText of description with Item 4 (value) and Item 5 (LocaleId)
                List<object> newValues = new List<object>                                                       // Generate the Struct containing the enumeration values in the order Key, DisplayValue and Description
                {
                    enumerationData.Item1,
                    displayValue,
                    description
                };
                newEnumerationValues.Add(new Struct(OpcUa.DataTypes.EnumValueType, newValues.AsReadOnly()));
                newEnumerationFields.Add(new EnumField($"Value{enumerationData.Item1}", enumerationData.Item1, displayValue, description));             // Generate the EnumField called Value<key> (ex Value0) containing the values of the enumeration in the order Key, DisplayValue and Description
            }

            EnumDefinition newEnumerationDefinition = new EnumDefinition(EnumObj.BrowseName, newEnumerationNodeId, newEnumerationFields.AsReadOnly());  // Generate a new EnumDefinition with the same nodeId and browseName of the new enumeration
            IUADataType newEnumeration = EnumObj as IUADataType;                                                // Generate the new Enumeration (is a DataType)
            IUAVariable newEnumValuesVariable = InformationModel.MakeVariable(new QualifiedName(0, "EnumValues"), OpcUa.DataTypes.EnumValueType, OpcUa.VariableTypes.BaseDataVariableType, new uint[1] { (uint)newEnumerationValues.Count });          // Generate the variable EnumValues, which contains all the values of the enumeration that you will see in the IDE.  This sets the number of enum entries based on "newEnumerationValues.Count".
            newEnumValuesVariable.Value = new UAValue(newEnumerationValues.ToArray());                          // Fill the variable EnumValues with the values passed in the method
            newEnumeration.Add(newEnumValuesVariable);                                                          // Finalize by adding the variable to the enumeration and the latter to its owner
        }
        catch (Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - UpdateEnumDisplayText", "Error updating Enum Display Name text: " + e.Message+" in "+className+"."+methodName);
        }          
    }

    public void UpdateStringConverter(NodeId KVCNodeID, string[] ModelList, Int32 ModelCount)
    {
        try
        {
            var Converter = InformationModel.Get(KVCNodeID);

            foreach (var child in Converter.Children)
            {
                if (child.BrowseName == "Pairs")
                {
                    var pairs = child.Children;

                    foreach (var pair in pairs)
                    {
                        //Log.Warning(pair.BrowseName);
                        pair.Delete();

                    }

                    if(ModelCount <= 0)
                    {
                        IUAObject newPair = InformationModel.MakeObject($"Pair0", FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                        IUAVariable newKey = newPair.GetVariable("Key");
                        IUAVariable newValue = newPair.GetVariable("Value");
                        newKey.DataType = OpcUa.DataTypes.Int16;
                        newKey.Value = 0;
                        newValue.DataType = OpcUa.DataTypes.String;
                        newValue.Value = "Not Used";
                        child.Add(newPair);
                        Log.Info("CoreFunctions - UpdateStringConverter", "Created: Pair0 - Not Used");
                    }
                    else
                    {
                        Int32 ElementCnt = ModelList.GetLength(0);
                        for (int i = 0; i < ElementCnt; i++)
                        {
                            IUAObject newPair = InformationModel.MakeObject($"Pair{i}", FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                            IUAVariable newKey = newPair.GetVariable("Key");
                            IUAVariable newValue = newPair.GetVariable("Value");
                            newKey.DataType = OpcUa.DataTypes.Int16;
                            newKey.Value = i;
                            newValue.DataType = OpcUa.DataTypes.String;
                            newValue.Value = ModelList[i];
                            if(ModelList[i] != "")
                            {
                                child.Add(newPair);
                                Log.Info("CoreFunctions - UpdateStringConverter", "Created: Pair" + i + " - " + newValue.Value);
                            }
                            
                        }    
                    }
                    
                }
            }
        }
        catch (Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - UpdateStringConverter", "Error updating String converter: " + e.Message+" in "+className+"."+methodName);
        }
    }

    public void UpdateNodeIDConverter(NodeId KVCNodeID, string[] ModelList, Int32 ModelCount)
    {
        try
        {
            var Converter = InformationModel.Get(KVCNodeID);

            foreach (var child in Converter.Children)
            {
                if (child.BrowseName == "Pairs")
                {
                    var pairs = child.Children;

                    foreach (var pair in pairs)
                    {
                        //Log.Warning(pair.BrowseName);
                        pair.Delete();

                    }

                    if(ModelCount <= 0)
                    {
                        IUAObject newPair = InformationModel.MakeObject($"Pair0", FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                        IUAVariable newKey = newPair.GetVariable("Key");
                        IUAVariable newValue = newPair.GetVariable("Value");
                        
                        newKey.DataType = OpcUa.DataTypes.Int16;
                        newKey.Value = 0;
                        
                        newValue.DataType = OpcUa.DataTypes.NodeId;
                        string FullPath = "raLib_Core/DataReady_Core_V01_00_0106/Global/Data/NotUsed_DoNotDelete";
                        NodeId ModelNodeId = Project.Current.Get(FullPath).NodeId;
                        newValue.Value = ModelNodeId;
                        child.Add(newPair);
                        Log.Info("CoreFunctions - UpdateNodeIDConverter", "Created: Pair0 - Not Used");

                    }
                    else
                    {
                        for (int i = 0; i < ModelList.GetLength(0); i++)
                        {
                            if (ModelList[i] != "")
                            {
                                IUAObject newPair = InformationModel.MakeObject($"Pair{i}", FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                                IUAVariable newKey = newPair.GetVariable("Key");
                                IUAVariable newValue = newPair.GetVariable("Value");
                                
                                newKey.DataType = OpcUa.DataTypes.Int16;
                                newKey.Value = i;

                                newValue.DataType = OpcUa.DataTypes.NodeId;
                                string FullPath = "raLib_Core/DataReady_Core_V01_00_0106/Global/InformationModel/" + ModelList[i];
                                NodeId ModelNodeId = Project.Current.Get(FullPath).NodeId;
                                newValue.Value = ModelNodeId;
                                child.Add(newPair);

                                Log.Info("CoreFunctions - UpdateNodeIDConverter", "Created: Pair" + i + " - " + ModelList[i]);
                            }
                        }    
                    }                
                }
            }
        }
        catch (Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - UpdateNodeIDConverter", "Error updating NodeID converter: " + e.Message+" in "+className+"."+methodName);
        }
    }

#endregion    

    public void CreateTranslatedList(string Locale, int NamespaceIndex, Int32 EnumCount, string EnumPrefix, out List<string> TranslatedList)
    {
        List<string> LocationID=new List<string>();
        List<string> TempTranslatedList = ["Not Used"];
        
        try
        {   
            LocationID.Add(Locale);
            LocalizedText discKey;
            string translatedText;
            
            for (int i = 1; i <= EnumCount; i++)
            {
                discKey = new LocalizedText(NamespaceIndex, EnumPrefix + i);
                translatedText = InformationModel.LookupTranslation(discKey, LocationID).Text;
                TempTranslatedList.Add(translatedText);
                
            }
        }
        catch (Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - CreateTranslatedList", "Error creating Translated List: " + e.Message+" in "+className+"."+methodName);
        }

        TranslatedList = TempTranslatedList;
    }
    
    public void GetEnumText(int EnumPointer, IUANode EnumListNodeID,out string EnumText)
    {     
        LocalizedText theText = null;
        try
        {
            IUAVariable enumChild = InformationModel.GetVariable(EnumListNodeID.Children[0].NodeId);
            Struct[] enumChildStructs = (Struct[])enumChild.Value.Value;
            theText = (LocalizedText)enumChildStructs[EnumPointer].Values[1];            
        }
        catch (Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - GetEnumText", "Error getting Enum Text: " + e.Message+" in "+className+"."+methodName);
        }
        EnumText = theText.Text;
    }

    public void UpdateEnumConverter(Int32 StartIndex, string SkippedIndexText, NodeId ConverterNodeID, string[] ModelList)
    {
        try
        {
            var Converter = InformationModel.Get(ConverterNodeID);

            foreach (var child in Converter.Children)
            {
                if (child.BrowseName == "Pairs")
                {
                    var pairs = child.Children;
                    
                    foreach (var pair in pairs)
                    {
                        pair.Delete();
                    }
                    
                    Int32 ElementCnt=ModelList.GetLength(0);

                    for (int i = 0; i < ElementCnt; i++)
                    {
                        IUAObject newPair = InformationModel.MakeObject($"Pair{i}", FTOptix.CoreBase.ObjectTypes.ValueMapPair);
                        IUAVariable newKey = newPair.GetVariable("Key");
                        IUAVariable newValue = newPair.GetVariable("Value");
                        newKey.DataType = OpcUa.DataTypes.Int16;
                        newKey.Value = i;
                        newValue.DataType = OpcUa.DataTypes.String;
                        if (i < StartIndex)
                        {
                            newValue.Value = SkippedIndexText;                        
                        }
                        if (i >= StartIndex)
                        {
                            newValue.Value = ModelList[i];
                        }
                        child.Add(newPair);
                    }
                }
            }
        }
        catch (Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - UpdateEnumConverter", "Error updating Enum converter: " + e.Message+" in "+className+"."+methodName);
        }

        
    }

    public void UpdateLookupList(List<string> locationID, int NamespaceIndex, string ListName, out string[] ModelArray)
    {
        Int32 i = 1;
        List<string> ModelList = new List<string>();
        LocalizedText discKey;
        try
        {
            ModelList.Add("Not Used");

            while (i <= 20000)
            {
                discKey = new LocalizedText(NamespaceIndex, ListName + i);
                if (InformationModel.LookupTranslation(discKey, locationID).HasTranslation == true)
                {
                    //Log.Warning("DataReady - AutoConfig - UpdateModelList", "Found: " + ListName + "" + i + " - " + InformationModel.LookupTranslation(discKey, locationID).Text);
                    ModelList.Add(InformationModel.LookupTranslation(discKey, locationID).Text);
                    i++;
                }
                else
                {
                    Project.Current.GetVariable("raLib_Core/DataReady_Core_V01_00_0106/Global/EnumTranslations/LookupLists/" + ListName).Value = i - 1;
                    i = 20001;
                }

            }

        }
        catch(Exception e)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Log.Error("CoreFunctions - UpdateLookupList", "Error updating Lookup List: " + e.Message+" in "+className+"."+methodName);
        }
        
        ModelArray = ModelList.ToArray();
    }

    public object GetValueByDataType(UAValue value, NodeId dataType)
    {
        // NOTE: ordered by usage frequency.

        if (dataType == UAManagedCore.OpcUa.DataTypes.Int32)
        {
            return Convert.ToInt32(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.Float)
        {
            return Convert.ToSingle(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.String)
        {
            return Convert.ToString(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.Double)
        {
            return Convert.ToDouble(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.Int16)
        {
            return Convert.ToInt16(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.Int64)
        {
            return Convert.ToInt64(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.Boolean)
        {
            return Convert.ToBoolean(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.Byte)
        {
            return Convert.ToByte(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.DateTime)
        {
            return Convert.ToDateTime(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.UInt16)
        {
            return Convert.ToUInt16(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.UInt32)
        {
            return Convert.ToUInt32(value.Value);
        }

        if (dataType == UAManagedCore.OpcUa.DataTypes.UInt64)
        {
            return Convert.ToUInt64(value.Value);
        }

        return null;
    }

    public void RemoveGUIDFiles(string FileWithPathToRemove)
    {
        if (File.Exists(FileWithPathToRemove))
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FileWithPathToRemove);
        }
    }

    public void CreateGUIDFiles(string FileWithPathToCreate)
    {
        System.IO.File.Create(FileWithPathToCreate).Close();
    }

    public void GetOwnerPath(string PathRoot,IUANode parent, string instanceName, out string OwnerPath)
    {
        
        OwnerPath = instanceName;
        if (parent.Owner.BrowseName != PathRoot)
        {
            instanceName = parent.Owner.BrowseName + "/" + instanceName;
            GetOwnerPath(PathRoot, parent.Owner, instanceName, out OwnerPath);
        }
        
    }
    
    public void DeleteDatabaseTable( NodeId TableNodeID)
    {
        Table TargeTable = InformationModel.Get<Table>(TableNodeID);
        Store DataStore = InformationModel.Get<Store>(TargeTable.Owner.Owner.NodeId);
        object[,] resultSet;
        string[] header;
        DataStore.Query($@"DELETE FROM "+ TargeTable.BrowseName, out header, out resultSet);
    }

}
