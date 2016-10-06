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
    public partial class AssessmentGroup : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
        }
        [WebMethod]
        public static string GetQueryData()
        {
            DataTable table = AssessmentGroupService.GetQueryDataTable();
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }

        [WebMethod]
        public static int AddWorkingSection(string mName, string mStatisticalcycle,  string mRemark)
        {

            int result = AssessmentGroupService.InsertWorkingSection(mName, mStatisticalcycle,mUserName , mRemark);
            return result;
        }
        [WebMethod]
        public static int deleteWorkingSection(string mCreateTime)
        {
            int result = AssessmentGroupService.deleteWorkingSection(mCreateTime);
            return result;

        }
        [WebMethod]
        public static int EditWorkingSection(string mName, string mStatisticalcycle, string mRemark, string mGroupId)
        {
            int result = AssessmentGroupService.EditWorkingSections(mName, mStatisticalcycle, mUserName, mRemark, mGroupId);
            return result;
        }

    }
}