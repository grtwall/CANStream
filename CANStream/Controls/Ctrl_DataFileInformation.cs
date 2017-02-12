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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ctrl_GraphWindow;

namespace CANStream
{
    public partial class Ctrl_DataFileInformation : UserControl
    {
        #region Public properties

        /// <summary>
        /// GW_DataFile object showed in the control
        /// </summary>
        public GW_DataFile DataFile
        {
            get
            {
                return (oDataFile);
            }

            set
            {
                oDataFile = value;
                Show_DataFileInformation();
            }
        }

        #endregion

        #region Private enums

        private enum ControlEditType
        {
            Format = 0,
            ReferenceLines=1,
        }

        #endregion

        #region Private members

        private GW_DataFile oDataFile;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ctrl_DataFileInformation()
        {
            InitializeComponent();

            oDataFile = null;
        }

        #region Control events

        #region cCL_FileChannelList

        private void cCL_FileChannelList_DataChannelClicked(object sender, ChannelClickEventArgs e)
        {
            Show_DataChannelProperties(e.ChannelName);
        }

        private void cCL_FileChannelList_DataChannelSelectionChanged(object sender, ChannelSelectionChangedEventArgs e)
        {
            Show_DataChannelProperties(e.ChannelName);
        }

        #endregion

        #region GV_DataChannelProperties

        private void GV_DataChannelProperties_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==1)
            {
                DataGridViewCell oCell = GV_DataChannelProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];

                switch(e.RowIndex)
                {
                    case 2: //Format
                        {
                            Create_EditCommand(oCell, ControlEditType.Format);
                        }
                        break;

                    case 3: //Reference lines
                        {
                            Create_EditCommand(oCell, ControlEditType.ReferenceLines);
                        }
                        break;

                    default:

                        //Nothing to do
                        break;
                }
            }
        }

        private void GV_DataChannelProperties_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                switch (e.RowIndex)
                {
                    case 2: //Format
                        {
                            GV_DataChannelProperties.Controls.RemoveByKey("Cmd_EditFormat");
                        }
                        break;

                    case 3: //Reference lines
                        {
                            GV_DataChannelProperties.Controls.RemoveByKey("Cmd_EditReferenceLines");
                        }
                        break;

                    default:

                        //Nothing to do
                        break;
                }
            }
        }

        private void Cmd_Edit_ChannelFormat_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Cmd_Edit_ChannelReferenceLines_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Private methodes

        /// <summary>
        /// Show the GW_DataFile object inforamtion into the current Ctrl_DataFileInformation control
        /// </summary>
        private void Show_DataFileInformation()
        {
            //File information
            Txt_FileStartTime.Text = oDataFile.DataStartTime.ToLongDateString() + " " 
                                     + oDataFile.DataStartTime.ToLongTimeString();

            Txt_FileComment.Text = oDataFile.UserComment;

            //File custom properties
            CS_RecordUserInfoCollection oFileCustomProperties = new CS_RecordUserInfoCollection();

            foreach (GW_XmlDataFileCustomProperty oProp in oDataFile.XmlDataFileCustomProperties)
            {
                CS_RecordUserInfo sProp = Convert_XmlDataFileProperty_To_RecordUserInfo(oProp);

                if(!(sProp.Title.Equals("")))
                {
                    oFileCustomProperties.Informations.Add(sProp);
                }
            }

            cRUI_FileCustomProperties.Set_UserInformations(oFileCustomProperties);

            //Channels properties
            cCL_FileChannelList.Clear_ChannelList();
            
            foreach(GW_DataChannel oChan in oDataFile.Channels)
            {
                cCL_FileChannelList.Add_ChannelNameWithDescription(oChan.Name, oChan.Description);
            }

            cCL_FileChannelList.Show_ChannelList();

        }

        /// <summary>
        /// Convert a GW_XmlDataFileCustomProperty object into a CS_RecordUserInfo structure
        /// </summary>
        /// <param name="oXmlProp">GW_XmlDataFileCustomProperty to be converted</param>
        /// <returns>CS_RecordUserInfo created from the GW_XmlDataFileCustomProperty given as argument</returns>
        private CS_RecordUserInfo Convert_XmlDataFileProperty_To_RecordUserInfo(GW_XmlDataFileCustomProperty oXmlProp)
        {
            CS_RecordUserInfo sRecordUserInfo = new CS_RecordUserInfo();

            sRecordUserInfo.Title = "";
            sRecordUserInfo.Value = "";

            if (!(oXmlProp == null))
            {
                sRecordUserInfo.Title = oXmlProp.Name;
                sRecordUserInfo.Value = oXmlProp.PropertyValue.ToString();
            }

            return (sRecordUserInfo);
        }

        /// <summary>
        /// Show the properties of a data channel into the channel properties grid
        /// </summary>
        /// <param name="ChannelName">Name of the channel to show</param>
        private void Show_DataChannelProperties(string ChannelName)
        {
            GV_DataChannelProperties.Rows.Clear();

            GW_DataChannel oChan = oDataFile.Get_DataChannel(ChannelName);

            if (!(oChan == null))
            {
                GV_DataChannelProperties.Rows.Add(4);

                GV_DataChannelProperties.Rows[0].Cells[0].Value = "Unit";
                GV_DataChannelProperties.Rows[0].Cells[1].Value = oChan.Unit;

                GV_DataChannelProperties.Rows[1].Cells[0].Value = "Description";
                GV_DataChannelProperties.Rows[1].Cells[1].Value = oChan.Description;

                GV_DataChannelProperties.Rows[2].Cells[0].Value = "Format";
                GV_DataChannelProperties.Rows[2].Cells[1].Value = oChan.GraphicFormat.Get_StringValueFormat();
                GV_DataChannelProperties.Rows[2].Cells[1].ReadOnly = true;

                GV_DataChannelProperties.Rows[3].Cells[0].Value = "Reference lines";

                if(oChan.ChannelReferenceLines.Count>0)
                {
                    GV_DataChannelProperties.Rows[3].Cells[1].Value = "Reference lines";
                }
                else
                {
                    GV_DataChannelProperties.Rows[3].Cells[1].Value = "No reference lines";
                }

                GV_DataChannelProperties.Rows[3].Cells[1].ReadOnly = true;
            }
        }

        private void Create_EditCommand(DataGridViewCell oCell, ControlEditType EditType)
        {
            Button Btn = new Button();
            string CmdName;

            switch (EditType)
            {
                case ControlEditType.Format:

                    Btn.Name = "Cmd_EditFormat";
                    Btn.Click += Cmd_Edit_ChannelFormat_Click;
                    break;

                case ControlEditType.ReferenceLines:

                    Btn.Name = "Cmd_EditReferenceLines";
                    Btn.Click += Cmd_Edit_ChannelReferenceLines_Click;
                    break;

                default: //Unknown property type

                    return;
            }

            Btn.Text = "...";
            Btn.Width = oCell.Size.Height;
            Btn.Height = oCell.Size.Height;

            Rectangle sCellRec = GV_DataChannelProperties.GetCellDisplayRectangle(oCell.RowIndex, oCell.ColumnIndex, true);
            GV_DataChannelProperties.Controls.Add(Btn);
            Btn.Left = sCellRec.Right - Btn.Width;
            Btn.Top = sCellRec.Top;
        }

        #endregion
    }
}
