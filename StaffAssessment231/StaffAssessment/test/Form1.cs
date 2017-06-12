using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SqlServerDataAdapter;
namespace test
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
        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, StaffAssessment.Model_CaculateItems> m_Temp = new Dictionary<string, StaffAssessment.Model_CaculateItems>();
            StaffAssessment.Model_CaculateItems m_TempItem = new StaffAssessment.Model_CaculateItems();
            m_TempItem.OrganizaitonId = "zc_nxjc_qtx_efc_clinker02";
            m_TempItem.Type = "DCSTagAvg";
            m_TempItem.CaculateItemDetail = new List<StaffAssessment.Model_CaculateItemDetail>();

            StaffAssessment.Model_CaculateItemDetail m_TempDetail1 = new StaffAssessment.Model_CaculateItemDetail();
            m_TempDetail1.AssessmentId = "ddsadB10M_CUN_M";
            m_TempDetail1.ObjectId = "B10M_CUN_M";
            m_TempItem.CaculateItemDetail.Add(m_TempDetail1);
            m_Temp.Add("afsdf", m_TempItem);

            DataTable m_DataTime = new DataTable();
            m_DataTime.Columns.Add("StartTime", typeof(DateTime));
            m_DataTime.Columns.Add("EndTime", typeof(DateTime));
            m_DataTime.Rows.Add(new object[]{"2016-11-10 11:22:32", "2016-11-10 11:32:42"});

            StaffAssessment.Function_AssessmentCaculate.CaculateItemValue(ref m_Temp, m_DataTime, _dataFactory);

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
                m_AssessmentItemsTable.Rows.Add(new object[] { "aaa", "ProcessElectricityConsumption", "工序电耗", "rawMaterialsPreparation", "生料制备", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 20, 16, 1, 10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "bbb", "ComprehensiveCoalConsumption", "综合煤耗", "clinker", "熟料", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 20, 21, 1, 10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "ccc", "ComprehensiveEnergyConsumption", "综合能耗", "cementmill", "水泥", "zc_nxjc_byc_byf_cementmill01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 20, 110, 1, 10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "ddd", "ComparableEnergyConsumption", "可比综合能耗", "cementmill", "水泥", "zc_nxjc_byc_byf_cementmill01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 20, 110, 1, 10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "eee", "ElectricityConsumpitionAlarmCount", "电耗报警次数", "rawMaterialsPreparation", "生料制备", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "fff", "ProcessDowntimeCount", "工艺故障次数", "94FD551C-9B1B-4DD1-BC74-09D0A034FE50", "1号生料磨", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "ggg", "ProcessDowntimeTime", "工艺故障时间", "94FD551C-9B1B-4DD1-BC74-09D0A034FE50", "1号生料磨", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "hhh", "MechanicalDowntimeTime", "机械故障时间", "94FD551C-9B1B-4DD1-BC74-09D0A034FE50", "1号生料磨", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "iii", "RunningRate", "运转率", "94FD551C-9B1B-4DD1-BC74-09D0A034FE50", "1号生料磨", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "jjj", "FaultRate", "故障率", "94FD551C-9B1B-4DD1-BC74-09D0A034FE50", "1号生料磨", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });
                m_AssessmentItemsTable.Rows.Add(new object[] { "kkk", "OutputPerHour", "台时产量", "94FD551C-9B1B-4DD1-BC74-09D0A034FE50", "1号生料磨", "zc_nxjc_byc_byf_clinker01", "3FFB87B8-EC9F-46CD-8457-29069D5508B5", 10, 0, 2, 110, 1, -10, 200, 0, 1 });

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
            m_WorkingTimeTable.Columns.Add("StartTime", typeof(DateTime));
            m_WorkingTimeTable.Columns.Add("EndTime", typeof(DateTime));

            m_WorkingTimeTable.Rows.Add("2016-10-01 00:00:00", "2016-10-01 08:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-02 08:00:00", "2016-10-02 16:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-03 16:00:00", "2016-10-04 00:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-04 00:00:00", "2016-10-04 08:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-05 08:00:00", "2016-10-05 16:00:00");
            m_WorkingTimeTable.Rows.Add("2016-10-06 16:00:00", "2016-10-07 00:00:00");
            return m_WorkingTimeTable;
        }


    }
}
