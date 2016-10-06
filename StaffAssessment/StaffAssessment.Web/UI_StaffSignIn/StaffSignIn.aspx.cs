using StaffAssessment.Service;
using StaffAssessment.Service.StaffSignIn;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StaffAssessment.Web.UI_StaffSignIn
{
    public partial class StaffSignIn : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "StaffSignIn.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        [WebMethod]
        public static string GetWorkingSection(string mOrganizationID)
        {
            DataTable table = commonClass.GetWorkingSectionList(mOrganizationID);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetStaffIn(string mOrganizationID, string mWorkingSectionID)
        {
            DataTable table = StaffSignInService.GetWorkingSectionTable(mOrganizationID, mWorkingSectionID);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetStaffInfo(string mOrganizationID, string team)
        {
            DataTable table = StaffSignInService.GetStaffInfoTable(mOrganizationID, team);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetHistoryStaffSignInData(string mOrganizationID, string itemID, string mStartTime, string mEndTime)
     //" {mOrganizationID:'" + mOrganizationID + "',  itemID:'" + itemID + "', mStartTime:'" + mStartTime + "', mEndTime:'" + mEndTime + "'}",           
        {
            DataTable table = StaffSignInService.GetHistoryStaffSignInTable(mOrganizationID, itemID, mStartTime, mEndTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static int Save(string mworkingId, string organizationId, string mvDate, string mShift, string itemId)
        {
            int result = StaffSignInService.Save(mworkingId, organizationId, mvDate, mShift, itemId);
            return result;
        }
        [WebMethod]
        public static int InsertSignIn(string mOrganizationID, string mVdate, string mStaffId, string mWorkingSectionID, string mShifts) 
    //" {mOrganizationID:'" + mOrganizationID + "',mVdate:'" + mVdate + "',mStaffId:'" + itemId + "',mWorkingSectionID:'" + mWorkingSectionID + "',mShifts:'" + mShiftsID + "'}",
        {
            int result = StaffSignInService.InsertSignIn(mOrganizationID, mVdate, mStaffId, mWorkingSectionID,mShifts,mUserName);
            return result;
        
        }
    }
}