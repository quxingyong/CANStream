﻿/*
 *	This file is part of CANStream.
 *
 *	CANStream program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU General Public License as published by
 *	the Free Software Foundation, either version 3 of the License, or
 *	(at your option) any later version.
 *
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU General Public License for more details.
 *
 *	You should have received a copy of the GNU General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 *	CANStream Copyright © 2013-2016 whilenotinfinite@gmail.com
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

using Ctrl_GraphWindow;

using NumberBaseConversion;

namespace CANStream
{
	/// <summary>
	/// CANStream project common functions
	/// </summary>
	public static class CANStreamTools
	{
		#region Public members
		
		public static string MyDocumentPath; //MyDocuement folder path
		public static string CsDataPath; //Users\xxx\AppData\Local
		
		public static RecordConversionOption TraceConversionOptions;
		
		#endregion
		
		#region Private members
		
		private static int CurrentColorId;
		
		private static CS_RecordEvent oEventCurrent = null;
		private static CS_RecordSession oSessionCurrent = null;
		
		#endregion
		
		#region Public methods
		
		public static string[] GetBinaryRowData(int ParamLength, long lRowValue)
        {
            string sBinValue = NumberBaseConverter.Dec2Bin(lRowValue);

            while (sBinValue.Length < ParamLength)
            {
                sBinValue = "0" + sBinValue;
            }

            string[] BinRowValue = new string[ParamLength];

            int i = ParamLength - 1;
            while (i >= 0)
            {
                BinRowValue[i] = sBinValue.Substring(i, 1);
                i--;
            }

            return (BinRowValue);
        }
		
		public static bool CreateFolderIfItDoesNotExist(string FolderPath)
		{
			if(!(Directory.Exists(FolderPath)))
			{
				string[] PathDetails=FolderPath.Split(char.Parse("\\"));
				string PathTmp=PathDetails[0];
				
				for(int i=0;i<PathDetails.Length;i++)
				{
					if(i==0) //First, check if the drive is ready... if not, we're fucked !
					{
						DriveInfo Drv=new DriveInfo(PathTmp);
						if(!(Drv.IsReady))
						{
							return(false);
						}
					}
					else
					{
						PathTmp=PathTmp+"\\"+PathDetails[i];
						
						if(!(Directory.Exists(PathTmp)))
						{
							Directory.CreateDirectory(PathTmp);
						}
					}
				}
			}
			
			return(true);
		}
		
		public static void ResetTrcFileInfoEventSession()
		{
			oEventCurrent = null;
			oSessionCurrent = null;
		}
		
		public static PcanTrcFileInfo[] GetTrcFileInfoList(string FolderPath)
		{
			List<PcanTrcFileInfo> FolderTrcFilesInfo = new List<PcanTrcFileInfo>();
			
			DirectoryInfo FolderInfo = new DirectoryInfo(FolderPath);
			DirectoryInfo[] SubDirsInfo = FolderInfo.GetDirectories();
			FileInfo[] TrcFiles = FolderInfo.GetFiles("*.trc");
			
			//Update event
			if (File.Exists(FolderPath + "\\EventDetails.xml"))
			{
				oEventCurrent = new CS_RecordEvent();
				
				if (!(oEventCurrent.Load_RecordEventInformationFile(FolderPath + "\\EventDetails.xml")))
				{
					oEventCurrent = null;
				}
			}
			
			//Update session
			if (File.Exists(FolderPath + "\\SessionDetails.xml"))
			{
				oSessionCurrent = new CS_RecordSession();
				
				if (!(oSessionCurrent.Load_RecordSessionInformationFile(FolderPath + "\\SessionDetails.xml")))
				{
					oSessionCurrent = null;
				}
			}
			
			//Look for trc files
			if (TrcFiles.Length > 0)
			{
				foreach (FileInfo oTrc in TrcFiles)
				{
					PcanTrcFileInfo oTrcInfo = new PcanTrcFileInfo();
					
					oTrcInfo.TrcFileInfo = oTrc;
					
					if (!(oEventCurrent == null || oSessionCurrent == null))
					{
						oTrcInfo.TrcFileEvent = oEventCurrent;
						oTrcInfo.TrcFileSession = oSessionCurrent;
					}
					
					FolderTrcFilesInfo.Add(oTrcInfo);
				}
			}
			
			//Look in sub-directories
			if (SubDirsInfo.Length > 0)
			{
				foreach (DirectoryInfo oSubDir in SubDirsInfo)
				{
					FolderTrcFilesInfo.AddRange(GetTrcFileInfoList(oSubDir.FullName));
				}
			}
			
            return (FolderTrcFilesInfo.ToArray());
		}
		
		public static int GetParamByteLength(int ParamLength)
		{
			double dLen=ParamLength/8;
			int iLen=(int) dLen;
			
			if(!(dLen==iLen))
			{
				iLen++;
			}
				
			return(iLen);
		}
		
		public static int GetRGBColor(Color ColorSelected)
        {
            string R = NumberBaseConverter.Dec2Hex(Convert.ToInt32(ColorSelected.R));
            string G = NumberBaseConverter.Dec2Hex(Convert.ToInt32(ColorSelected.G));
            string B = NumberBaseConverter.Dec2Hex(Convert.ToInt32(ColorSelected.B));

            if (R.Length < 2)
            {
                R = "0" + R;
            }

            if (G.Length < 2)
            {
                G = "0" + G;
            }

            if (B.Length < 2)
            {
                B = "0" + B;
            }

            string RGB = R + G + B;
            return (Convert.ToInt32(NumberBaseConverter.Hex2Dec(RGB)));
            
        }

        public static int GetRandomColor()
        {
            CurrentColorId++;
			
            if(CurrentColorId == 16)
            {
            	CurrentColorId=0;
            }
            
            switch (CurrentColorId)
            {
                case 0:
                    return (GetRGBColor(Color.Red));
                case 1:
                    return (GetRGBColor(Color.Cyan));
                case 2:
                    return (GetRGBColor(Color.LightGreen));
                case 3:
                    return (GetRGBColor(Color.Yellow));
                case 4:
                    return (GetRGBColor(Color.Pink));
                case 5:
                    return (GetRGBColor(Color.Blue));
                case 6:
                    return (GetRGBColor(Color.Green));
                case 7:
                    return (GetRGBColor(Color.Orange));
                case 8:
                    return (GetRGBColor(Color.Purple));
                case 9:
                    return (GetRGBColor(Color.LightBlue));
                case 10:
                    return (GetRGBColor(Color.GreenYellow));
                case 11:
                    return (GetRGBColor(Color.OrangeRed));
                case 12:
                    return (GetRGBColor(Color.Salmon));
                case 13:
                    return (GetRGBColor(Color.SteelBlue));
                case 14:
                    return (GetRGBColor(Color.DarkGreen));
                case 15:
                    CurrentColorId = -1;
                    return (GetRGBColor(Color.Olive));
                default:
                    CurrentColorId = -1;
                    return (GetRGBColor(Color.Magenta));
            }
        }
		
        public static void ResetRandomColor()
        {
        	CurrentColorId=-1;
        }

        #region Cycle plotting methods for Ctrl_GraphWindow
       
        public static GraphWindowProperties Get_CycleGraphicSetup(CANStreamCycle oCycle)
        {
            GraphWindowProperties oGraphProps = new GraphWindowProperties();

            if (!(oCycle.oCanNodesMap==null))
            {
                oGraphProps.GraphLayoutMode = GraphicWindowLayoutModes.Parallel;

                foreach (CANMessage oMsg in oCycle.oCanNodesMap.Messages)
                {
                    if (oMsg.RxTx == CanMsgRxTx.Tx)
                    {
                        foreach (CANParameter oParam in oMsg.Parameters)
                        {
                            oGraphProps.Create_Serie(oParam.Name);

                            GraphSerieProperties oSerieProps = oGraphProps.Get_SerieByName(oParam.Name);

                            if(!(oSerieProps==null))
                            {
                                oSerieProps.YAxis.AxisTitleVisible = true;
                            }
                        }
                    }
                }

                return (oGraphProps);
            }

            return (null);
        }

        #endregion

        public static void Init_RecordConversionOption(string BasePath)
        {
        	TraceConversionOptions = new RecordConversionOption();
			TraceConversionOptions.SourceFileFolder = BasePath + CANStreamConstants.StackDirectory;
			TraceConversionOptions.OutputFileFolder = BasePath + CANStreamConstants.DataDirectory;
			TraceConversionOptions.SourceFileBackUpFolder = BasePath + CANStreamConstants.RawDirectory;
			TraceConversionOptions.TrcFileList = null;
            TraceConversionOptions.OutputFileFormat = RecordConversionFormat.Xml; //RecordConversionFormat.Text;
        }
        
        public static string GetScaledFileSize(long FileSize)
        {
        	double ScaledSize = FileSize;
        	int iScale = 0;
        	string sScale = "";
        	
        	while (ScaledSize > 1024)
        	{
        		ScaledSize /= 1024;
        		iScale++;
        	}
        	
        	switch (iScale)
        	{	
        		case 1: //Kilo Byte
        			
        			sScale = "KB";
        			break;
        			
        		case 2: //Mega Byte
        			
        			sScale = "MB";
        			break;
        			
        		case 3: //Giga Byte
        			
        			sScale = "GB";
        			break;
        			
        		case 4: //Tera Byte
        			
        			sScale = "TB";
        			break;
        			
        		default:
        			
        			sScale = "Bytes";
        			ScaledSize = FileSize;
        			break;
        	}
        	
        	return(Math.Round(ScaledSize, 3).ToString() + " " + sScale);
        }

        public static Type CS_GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// Convert a CANStream SignalFormatProperties class into a Ctrl_GraphWindow GraphSerieValueFormat class
        /// </summary>
        /// <param name="oSigFormat">CANStream SignalFormatProperties object to convert</param>
        /// <returns>Ctrl_GraphWindow GraphSerieValueFormat object</returns>
        public static GraphSerieValueFormat Convert_CSSignalFormatToSerieValueFormat(SignalFormatProperties oSigFormat)
        {
            GraphSerieValueFormat oSerieFormat = new GraphSerieValueFormat();

            switch (oSigFormat.FormatType)
            {
                case SignalValueFormat.Binary:

                    oSerieFormat.Format = GraphSerieLegendFormats.Binary;
                    break;

                case SignalValueFormat.Decimal:

                    oSerieFormat.Format = GraphSerieLegendFormats.Decimal;
                    oSerieFormat.Decimals = oSigFormat.Decimals;
                    break;

                case SignalValueFormat.Enum:

                    oSerieFormat.Format = GraphSerieLegendFormats.Enum;

                    foreach (EnumerationValue sSigEnum in oSigFormat.Enums)
                    {
                        GraphSerieEnumValue sSerieEnum = new GraphSerieEnumValue();

                        sSerieEnum.Value = sSigEnum.Value;
                        sSerieEnum.Text = sSigEnum.Text;

                        oSerieFormat.Enums.Add(sSerieEnum);
                    }

                    break;
                case SignalValueFormat.Hexadecimal:

                    oSerieFormat.Format = GraphSerieLegendFormats.Hexadecimal;
                    break;

                default:

                    oSerieFormat.Format = GraphSerieLegendFormats.Auto;
                    break;
            }

            return (oSerieFormat);
        }

        /// <summary>
        /// Convert a CANStream SignalAlarmsProperties class into a list of Ctrl_GraphWindow GraphReferenceLine class
        /// </summary>
        /// <param name="oSigAlarms">CANStream SignalAlarmsProperties object to convert</param>
        /// <returns>List of Ctrl_GraphWindow GraphReferenceLine class</returns>
        public static List<GraphReferenceLine> Convert_CSAlarmsToSerieReferenceLines(SignalAlarmsProperties oSigAlarms)
        {
            List<GraphReferenceLine> oSerieRefLines = new List<GraphReferenceLine>();

            if (oSigAlarms.Enabled)
            {
                GraphReferenceLine oLine;
                int iLineKey = 0;

                if (oSigAlarms.AlarmLimitMin.Enabled)
                {
                    oLine = GetSerieReferenceLineFromAlarm(oSigAlarms.AlarmLimitMin);
                    oLine.ReferenceTitle = "Alarm Min";
                    oLine.iKey = iLineKey;
                    iLineKey++;

                    oSerieRefLines.Add(oLine);
                }

                if (oSigAlarms.AlarmLimitMax.Enabled)
                {
                    oLine = GetSerieReferenceLineFromAlarm(oSigAlarms.AlarmLimitMax);
                    oLine.ReferenceTitle = "Alarm Max";
                    oLine.iKey = iLineKey;
                    iLineKey++;

                    oSerieRefLines.Add(oLine);
                }

                if (oSigAlarms.WarningLimitMin.Enabled)
                {
                    oLine = GetSerieReferenceLineFromAlarm(oSigAlarms.WarningLimitMin);
                    oLine.ReferenceTitle = "Warning Min";
                    oLine.iKey = iLineKey;
                    iLineKey++;

                    oSerieRefLines.Add(oLine);
                }

                if (oSigAlarms.WarningLimitMax.Enabled)
                {
                    oLine = GetSerieReferenceLineFromAlarm(oSigAlarms.WarningLimitMax);
                    oLine.ReferenceTitle = "Warning Max";
                    oLine.iKey = iLineKey;
                    iLineKey++;

                    oSerieRefLines.Add(oLine);
                }
            }

            return (oSerieRefLines);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Convert a CANStream SignalAlarmValue structure into a Ctrl_GraphWindow GraphReferenceLine class
        /// </summary>
        /// <param name="sAlarm">CANStream SignalAlarmValue structure to convert</param>
        /// <returns>Ctrl_GraphWindow GraphReferenceLine class</returns>
        private static GraphReferenceLine GetSerieReferenceLineFromAlarm(SignalAlarmValue sAlarm)
        {
            GraphReferenceLine oRefLine = new GraphReferenceLine();

            oRefLine.ReferenceValue = sAlarm.Value;
            oRefLine.ReferenceStyle.LineColor = sAlarm.BackColor;
            oRefLine.ReferenceTitlePosition = ScreenPositions.Left;

            return (oRefLine);
        }

        #endregion
    }
}
