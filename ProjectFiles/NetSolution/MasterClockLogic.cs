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
using CoreBase = FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using FTOptix.UI;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.EventLogger;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.OPCUAServer;
using FTOptix.WebUI;
using FTOptix.OPCUAClient;
using FTOptix.Report;
using FTOptix.ODBCStore;
using FTOptix.DataLogger;
using FTOptix.Alarm;
#endregion

public class MasterClockLogic : BaseNetLogic
{
	public override void Start()
	{
		periodicTask = new PeriodicTask(UpdateTime, 1000, LogicObject);
		periodicTask.Start();
	}

	public override void Stop()
	{
		periodicTask.Dispose();
		periodicTask = null;
	}

	private void UpdateTime()
	{
		DateTime localTime = DateTime.Now;
		DateTime utcTime = DateTime.UtcNow;
		LogicObject.GetVariable("Time").Value = localTime;
		LogicObject.GetVariable("Time/Year").Value = localTime.Year;
		LogicObject.GetVariable("Time/Month").Value = localTime.Month;
		LogicObject.GetVariable("Time/Day").Value = localTime.Day;
		LogicObject.GetVariable("Time/Hour").Value = localTime.Hour;
		LogicObject.GetVariable("Time/Minute").Value = localTime.Minute;
		LogicObject.GetVariable("Time/Second").Value = localTime.Second;
		LogicObject.GetVariable("UTCTime").Value = utcTime;
		LogicObject.GetVariable("UTCTime/Year").Value = utcTime.Year;
		LogicObject.GetVariable("UTCTime/Month").Value = utcTime.Month;
		LogicObject.GetVariable("UTCTime/Day").Value = utcTime.Day;
		LogicObject.GetVariable("UTCTime/Hour").Value = utcTime.Hour;
		LogicObject.GetVariable("UTCTime/Minute").Value = utcTime.Minute;
		LogicObject.GetVariable("UTCTime/Second").Value = utcTime.Second;
	}

	private PeriodicTask periodicTask;
}
