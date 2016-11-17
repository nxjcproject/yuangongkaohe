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
    public partial class AssessmentVersionDefine : WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "AssessmentVersionDefine.aspx";   //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        [WebMethod]
        public static string GetWorkingSection(string mOrganizationId)
        {
            DataTable table = AssessmentVersionDefineService.GetWorkingSectionList(mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetWorkingSectionGrid(string mOrganizationId)
        {
            DataTable table = commonClass.GetWorkingSectionGridList(mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetAssessmentVersionDefine(string mOrganizationId, string mWorkingSectionItemID) 
      //" {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + mWorkingSectionID + "'}";
        {
            DataTable table = AssessmentVersionDefineService.GetAssessmentVersionDefine(mOrganizationId, mWorkingSectionItemID);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
        [WebMethod]
        public static string GetProcessLine(string mOrganizationId) 
        {
            DataTable table = commonClass.GetProcessLine(mOrganizationId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            //string[] para = { "OrganizationID", "text" };
            //string json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "LevelCode");
            return json;      
        }
        [WebMethod]
        public static string GetAssessmentObjects(string myType, string myValueType, string myOrganizationId) 
     //"{myType:'" + myType + "',myValueType:'" + myValueType + "',myOrganizationId:'" + myOrganizationId + "'}",
        {
            Model_CalculateObjects calculateObjects = AssessmentVersionDefineService.GetCalculateObjects(myType, myValueType, myOrganizationId);
            DataTable table = calculateObjects.CalculateObjectsTable;
            if (table.Rows.Count>0)
            {
                table.Columns["Name"].ColumnName = "text";
            }          
            string json = "";
            if (calculateObjects.StructureName.Equals("tree"))
            {
                json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "ProcessLevelCode");
            }
            else if (calculateObjects.StructureName.Equals("grid"))
            {
                table.Columns.Add("ProcessLevelCode", typeof(string));
                for (int i=0; i < table.Rows.Count;i++ )
                {
                    table.Rows[i]["ProcessLevelCode"] = "P" + (i + 1).ToString("00");
                }
                json = EasyUIJsonParser.TreeGridJsonParser.DataTableToJsonByLevelCode(table, "ProcessLevelCode");

            }
            return json;         
        }
         [WebMethod]
        public static int AddAssessmentVersion(string mOrganizationId, string mWorkingSectionItemID, string mName,string mType, string mRemark) 
 //" {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + eWorkingSectionID + "',mName:'" + eName + "',mType:'" + eType  + "',mRemark:'" + eRemark + "'}";      
        {
            //string mName = mUserName;
            int result = AssessmentVersionDefineService.ToAddAssessmentVersion(mOrganizationId, mWorkingSectionItemID, mName, mType, mUserName, mRemark);
            return result;
        }
        /// <summary>
        /// 编辑操作
        /// </summary>
        /// <param name="mProductionID"></param>
        /// <param name="mWorkingSectionID"></param>
        /// <param name="mName"></param>
        /// <param name="mType"></param>
        /// <param name="mCreator"></param>
        /// <param name="mRemark"></param>
        /// <param name="mIsAdd"></param>
        /// <returns></returns>
         [WebMethod]
         public static int EditAssessmentVersion(string mOrganizationId, string mWorkingSectionID, string mName, string mType, string mRemark, string mKeyId)
//" {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + eWorkingSectionID + "',mName:'" + eName + "',mType:'" + eType + "',mRemark:'" + eRemark + "',mKeyId:'" + eKeyId + "'}";           
         {
             int result = AssessmentVersionDefineService.ToEditAssessmentVersion(mOrganizationId, mWorkingSectionID, mName, mType, mUserName, mRemark, mKeyId);
             return result;
         }
        [WebMethod]
         public static string GetAssessmentVersionDetail(string mKeyId)
        {
            DataTable table = AssessmentVersionDefineService.GetAssessmentVersionDetailTable(mKeyId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
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
        public static int AddAssessmentDetail(string mOrganizationID,string mKeyId, string mAssessmentId, string mAssessmentName, string mObjectId,string mObjectName, string mWeightedValue, string mBestValue, string mWorstValue,string mStandardValue,string mStandardScore, string mScoreFactor,string mMaxScore,string mMinScore,string mEnabled)
                //"{mOrganizationID:'" + myOrganizationId
                //+ "',mKeyId:'" + meditContrastId
                //+ "',mAssessmentId:'" + eAssessmentObjectId
                //+ "',mAssessmentName'" + eAssessmentName
                //+ "',mObjectId:'" + eObjectId
                //+ "',mObjectName:'" + eObjectName
                //+ "',mWeightedValue:'" + eWeightedValue
                //+ "',mBestValue:'" + eBestValue
                //+ "',mWorstValue:'" + eWorstValue
                //+ "',mStandardValue:'" + eStandardValue
                //+ "',mStandardScore:'" + eStandardScore
                //+ "',mScoreFactor:'" + eScoreFactor
                //+ "',mMaxScore:'" + eMaxScore
                //+ "',mMinScore:'" + eMinScore
                //+ "',mEnabled:'" + eEnabled + "'}";
        {
            int result = AssessmentVersionDefineService.ToAddAssessmentDetail(mOrganizationID, mKeyId, mAssessmentId,mAssessmentName, mObjectId,mObjectName, mWeightedValue, mBestValue, mWorstValue, mStandardValue, mStandardScore, mScoreFactor, mMaxScore, mMinScore,mEnabled);
            return result;      
        }
        [WebMethod]
        public static int UptateGetAssessmentDetail(string mOrganizationID, string mKeyId, string mAssessmentId,string mAssessmentName, string mObjectId,string mObjectName, string mWeightedValue, string mBestValue, string mWorstValue,string mStandardValue,string mStandardScore, string mScoreFactor,string mMaxScore,string mMinScore ,string mEnabled, string mId) 
        {
            int result = AssessmentVersionDefineService.ToEditAssessmentDetail(mOrganizationID, mKeyId, mAssessmentId, mAssessmentName, mObjectId, mObjectName, mWeightedValue, mBestValue, mWorstValue, mStandardValue, mStandardScore, mScoreFactor, mMaxScore, mMinScore, mEnabled, mId);
            return result;       
        }
        [WebMethod]
        public static int DeleteAssessmentDetail(string mId)
        {
            int result = AssessmentVersionDefineService.ToDeleteAssessmentDetail(mId);
            return result;   
        }
        [WebMethod] 
        public static int DeleteAssessmentVersion(string mKeyId)
        {
            int result = AssessmentVersionDefineService.ToDeleteAssessmentVersion(mKeyId);
            return result;          
        }
        [WebMethod]
        public static string GetIndex(string myOrganizationId, string mAssessmentId, string mObjectId)
        {
            DataTable table = AssessmentVersionDefineService.GetStandardIndexIndex(myOrganizationId, mAssessmentId, mObjectId);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table);
            return json;
        }
    }
}