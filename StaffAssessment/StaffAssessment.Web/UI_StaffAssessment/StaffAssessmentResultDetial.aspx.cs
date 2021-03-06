﻿using StaffAssessment.Service;
using StaffAssessment.Service.StaffAssessment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StaffAssessment.Web.UI_StaffAssessment
{
    public partial class StaffAssessmentResultDetial : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_qtx_efc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "StaffAssessmentResultDetial.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        [WebMethod]
        public static string GetWorkingSectionGrid(string mOrganizationId)
        {
            DataTable table = commonClass.GetWorkingSectionGridList(mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetAssessmentGroupGrid()
        {
            DataTable table = commonClass.GetAssessmentGroupGridTable();
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetWorkingSection(string mOrganizationId)
        {
            DataTable table = StaffAssessmentResultDetialService.GetWorkingSectionList(mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }       
        [WebMethod]
        public static string GetAllAssessmentResult(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            DataTable table = StaffAssessmentResultDetialService.GetAllAssessmentResultTable(mProductionID, mWorkingSectionID, mGroupId, mStartTime, mEndTime, mStatisticalCycle);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetAssessmentResultdetail(string mAssessmentId)
        {
            DataTable table = StaffAssessmentResultDetialService.GetAssessmentResultdetailTable(mAssessmentId);       
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}