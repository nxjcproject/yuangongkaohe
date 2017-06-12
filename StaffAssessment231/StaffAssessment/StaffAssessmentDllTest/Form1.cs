using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SqlServerDataAdapter;
namespace StaffAssessmentDllTest
{
    public partial class Form1 : Form
    {
        private SqlServerDataFactory _dataFactory;
        public Form1()
        {
            InitializeComponent();
            string m_DBConnectionString = "Data Source=192.168.101.212;Initial Catalog=NXJC;User ID=sa;Password=nxjcjt@!2015";
            _dataFactory = new SqlServerDataFactory(m_DBConnectionString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable m_WorkingTimeTable = GetWorkingTime();
            DataTable m_AssessmentItemsTable = GetAssessmentItems();
            DataTable m_AssessmentItemsResult = StaffAssessment.Function_AssessmentCaculate.GetStaffAssessment(m_WorkingTimeTable, m_AssessmentItemsTable, _dataFactory);
        }
        private DataTable GetAssessmentItems()
        {
            string m_Sql = @"SELECT B.*
                                  FROM  tz_Assessment A, assessment_AssessmentDetail B
                                  where A.OrganizationID = 'zc_nxjc_byc_byf'
                                  and A.KeyId = B.KeyId
                                  and A.KeyId = '3FFB87B8-EC9F-46CD-8457-29069D5508B5'";
            try
            {
                DataTable m_AssessmentItemsTable = _dataFactory.Query(m_Sql);
                return m_AssessmentItemsTable;
            }
            catch
            {
                return null;
            }
        }
        private DataTable GetWorkingTime()
        {
            DataTable m_WorkingTimeTable = new DataTable();
            m_WorkingTimeTable.Columns.Add("StartTime",typeof(DateTime));
            m_WorkingTimeTable.Columns.Add("EndTime", typeof(DateTime));

            m_WorkingTimeTable.Rows.Add("2016-10-01 00:00:00","2016-10-01 08:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-02 08:00:00", "2016-10-02 16:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-03 16:00:00", "2016-10-04 23:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-04 00:00:00", "2016-10-04 08:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-05 08:00:00", "2016-10-05 16:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-06 16:00:00", "2016-10-07 00:00:00");
            return m_WorkingTimeTable;
        }
    }
}
