using StaffAssessment.Service;
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
    public partial class StaffAssessment : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf", "zc_nxjc_szsc_szsf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                         //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "StaffSignInModify.aspx";   //向web用户控件传递当前调用的页面名称
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
        public static string GetStaffInfo(string mProductionId, string mWorkingSectionID)
        {
            DataTable table = commonClass.GetStaffInfoTable(mProductionId, mWorkingSectionID);
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
        public static string GetAssessmentVersion(string mOrganizationID, string mWorkingSectionID)
        {
            DataTable table = StaffAssessmentService.GetAssessmentVersionTable(mOrganizationID, mWorkingSectionID);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetAssessmentResult(string mProductionID, string mWorkingSectionID, string mStaffId, string mGroupId, string mStartTime, string mEndTime, string mVersionId, string mStatisticalCycle)
        //" { mProductionID + mWorkingSectionID  + mStaffId  + mGroupId + mStartTime + mEndTime + mVersionId  + mStatisticalCycle 
        {
            //获取引领表
            DataTable tzTable = StaffAssessmentService.GetAssessmentResultTableByDay(mProductionID, mWorkingSectionID, mStaffId, mGroupId, mStartTime, mEndTime, mVersionId, mStatisticalCycle);
           //获取考核结果详表
            DataTable detailTable = StaffAssessmentService.GetAssessmentResultDetailTableByDay(mProductionID, mWorkingSectionID, mStaffId, mGroupId, mStartTime, mEndTime, mVersionId, mStatisticalCycle);    
            string tzTablejson = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(tzTable);
            string detailTablejson = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(detailTable);

            return tzTablejson + "&" + detailTablejson;
        }
        [WebMethod]
        public static string CalculateStaffAssessment(string mProductionID, string mWorkingSectionID, string mStaffId, string mStaffName, string mGroupName, string mGroupId, string mStartTime, string mEndTime, string mVersionId, string mStatisticalCycle)
        {
            //生成引领表
            DataTable tzTable = StaffAssessmentService.GetStaffAssessmentTZ(mProductionID, mWorkingSectionID, mStaffId, mStaffName, mGroupId, mGroupName, mStartTime,mEndTime, mVersionId, mStatisticalCycle, mUserName);
            //GetStaffAssessmentCalculateResult
            //获取考核计算之后的表
            DataTable CalculateResult = StaffAssessmentService.GetStaffAssessmentCalculateResult(tzTable, mVersionId, mStatisticalCycle);

            string tzTableJson = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(tzTable);
            string CalculateResultJson = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(CalculateResult);

            return tzTableJson + "&" + CalculateResultJson;
        }
        [WebMethod]
        public static int SaveStaffAssessmentResult(string TzJson, string DetailJson) 
        {
            string[] tzJsons = TzJson.Replace("},{", "}|{").Split('|');
            string[] detailJsons = DetailJson.Replace("},{", "}|{").Split('|');

            DataTable tzTable = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(tzJsons, StaffAssessmentService.tzTableStructure());
            DataTable detailTable = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(detailJsons, StaffAssessmentService.detailTableStructure());
            //返回插入结果
            int result = StaffAssessmentService.InsertStaffAssessmentResult(tzTable, detailTable);

            return result;
        }

        
    }
}