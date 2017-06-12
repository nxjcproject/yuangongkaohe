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
    public partial class IndexConfigure : WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "IndexConfigure.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 7;
            }
        }
        [WebMethod]
        public static string GetAssessmentObjects(string myOrganizationId, string myAssessmentId, string myType, string myValueType )
        {
            Model_StandardIndexObjects indexTable = IndexConfigureService.GetIndexDataTable(myOrganizationId,myAssessmentId,myType, myValueType );
            DataTable table = indexTable.StandardIndexObjectsTable;
            if (table.Rows.Count > 0)
            {
                table.Columns["Name"].ColumnName = "text";
            }
            string json = "";
            if (indexTable.StructureName.Equals("tree"))
            {
                json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "ProcessLevelCode");
            }
            else if (indexTable.StructureName.Equals("grid"))
            {
                table.Columns.Add("ProcessLevelCode", typeof(string));
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["ProcessLevelCode"] = "P" + (i + 1).ToString("00");
                }
                json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "ProcessLevelCode");

            }
            //string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "ProcessLevelCode");
            return json;
        }
        [WebMethod]
        public static string GetAssessmentCatalogue()
        {
            DataTable table = commonClass.GetAssessmentCatalogueTable();
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string SaveIndex(string json, string assessmentId, string mOrganizationId)
        {

            string myName = mUserName;
            IndexConfigureService.SaveIndexId(json, assessmentId, myName, mOrganizationId);
            return "success";
        }
    }
}