using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Collections;

namespace StaffAssessment.Service.StaffAssessment
{
    public class StaffAssessmentResultService
    {
        public static DataTable GetWorkingSectionList(string mOrganizationID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT [WorkingSectionID]
                                     ,[WorkingSectionType]
                                     ,[OrganizationID]
                                     ,[DisplayIndex]
                                     ,[Enabled]
                              FROM [dbo].[system_WorkingSectionType]
                              where [Enabled]=1 and
                              [OrganizationID] like @mOrganizationID+'%'
                              order by [WorkingSectionType] ";
            SqlParameter para = new SqlParameter("@mOrganizationID", mOrganizationID);
            DataTable dt = factory.Query(mySql, para);
            return dt;
        }
        public static DataTable GetAllAssessmentResultTable(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            DataTable table = new DataTable();
            string starTime = "";
            string endTime = "";
            if (mStatisticalCycle.Equals("month"))
            {
                starTime = Convert.ToDateTime(mStartTime + "-01 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
                endTime = Convert.ToDateTime(mEndTime + "-01 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (mStatisticalCycle.Equals("year"))
            {
                starTime = Convert.ToDateTime(mStartTime + "-01-01 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
                endTime = Convert.ToDateTime(mStartTime + "-01-01 23:59:59").ToString("yyyy-MM-dd HH:mm:ss");

            }
            else if (mStatisticalCycle.Equals("day"))
            {
                starTime = Convert.ToDateTime(mStartTime + " 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
                endTime = Convert.ToDateTime(mStartTime + "  23:59:59").ToString("yyyy-MM-dd HH:mm:ss");
            }
            string mySql = @"SELECT C.[Name] as StaffName
	                                  ,D.[StatisticalCycle]
                                      ,A.[WorkingSectionID]
                                      ,A.[AssessmentCoefficient]
	                                  ,case D.[StatisticalCycle] 
	                                  when 'day' then  convert(char(10),A.StartTime,120) 
	                                  when 'month' then  convert(char(7),A.StartTime,120) 
	                                  when 'year' then  convert(char(4),A.StartTime,120) 
	                                  else '' end  as [Time]
                                    ,E.[AssessmentName]
                                    ,E.[ObjectName]  
                                    ,G.[Name] as[OrganizationName]
                                    ,E.[KeyId]
                                  ,E.[WeightedAverageCredit]
                                  FROM [dbo].[tz_ShiftAssessmentResult] A,[dbo].[system_StaffInfo] C,[dbo].[assessment_ ShiftAssessmentResultGroup] D,[dbo].[assessment_ShiftAssessmentResultDetail] E,[dbo].[system_Organization] G,[dbo].[system_WorkingSection] F
                                 where
                                 A.[OrganizationID]=@mProductionID
                                 and A.[GroupId]=@mGroupId
                                 and A.[StartTime]>=convert(datetime,@mStartTime)
                                 and A.[EndTime]<=convert(datetime,@mEndTime)
                                 and F.[WorkingSectionID]=@mWorkingSectionID
                                 and A.[WorkingSectionID]=F.[WorkingSectionItemID]
                                 and E.[KeyId]=A.[KeyId]
                                 and A.[StaffID]=C.[StaffInfoItemId]
                                 and A.[GroupId]=D.[GroupId]
                                 and G.[OrganizationID]=E.[OrganizationID]
                                 order by Time desc ,StaffName,KeyId,OrganizationName,ObjectName,AssessmentName desc
                                                                 ";
            SqlParameter[] para = { new SqlParameter("@mProductionID", mProductionID) ,
                                        new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                        new SqlParameter("@mGroupId", mGroupId),
                                        new SqlParameter("@mStartTime", starTime),
                                        new SqlParameter("@mEndTime", endTime)
                                     };
                                 
            DataTable dt = factory.Query(mySql, para);
            return dt;
        }

        public static DataTable GetAssessmentResultdetailTable(string mAssessmentId, string mOrganizationID) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.[Id]
                        ,D.[Name] as StaffName
                        ,A.[AssessmentId]
                        ,C.[AssessmentName]
                        ,C.[ObjectName]
                        ,A.[ObjectId]
                        ,A.[OrganizationID]
                        ,A.[KeyId]
                        ,A.[WeightedValue]
                        ,A.[BestValue]
                        ,A.[WorstValue]
                        ,A.[AssessmenScore]
                        ,A.[WeightedAverageCredit]
                FROM  [dbo].[assessment_ShiftAssessmentResultDetail] A,[dbo].[tz_ShiftAssessmentResult] B
                    ,[dbo].[assessment_AssessmentDetail] C,[dbo].[system_StaffInfo] D
                where B.[KeyId]=@mAssessmentId              
                and A.[KeyId]=B.[KeyId]
                and C.[AssessmentId]=A.[AssessmentId]        
                and C.[OrganizationID] like @mOrganizationID+'%'
                and B.[StaffID]=D.[StaffInfoItemId]";
            SqlParameter[] para ={
                                  new SqlParameter("@mAssessmentId", mAssessmentId),
                                  new SqlParameter("@mOrganizationID", mOrganizationID)
                                 };
            DataTable dt = factory.Query(mySql, para);
            return dt;

        }
        public static DataTable GetAssessmentResultCloumnName(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            DataTable table = GetAllAssessmentResultTable(mProductionID, mWorkingSectionID, mGroupId, mStartTime, mEndTime, mStatisticalCycle);
            DataTable cloumnName=new DataTable();
           string name="";
           string keyId =Convert.ToString( table.Rows[0][8]);
            cloumnName.Columns.Add("Name", Type.GetType("System.String"));
            foreach (DataRow dr in table.Rows)
            {
                if (name != Convert.ToString(dr[0]))
                {
                    DataRow mycloumn = cloumnName.NewRow();
                    mycloumn["Name"] = Convert.ToString(dr[0]);
                    name = Convert.ToString(dr[0]);
                    cloumnName.Rows.Add(mycloumn);
                    keyId = Convert.ToString(dr[8]);
                }
                if (keyId != Convert.ToString(dr[8]))
                {
                    DataRow mycloumn = cloumnName.NewRow();
                    mycloumn["Name"] = Convert.ToString(dr[0]);
                    keyId = Convert.ToString(dr[8]);
                    cloumnName.Rows.Add(mycloumn);
                }
                
                
            }
            return cloumnName;
        }
        public static DataTable GetAssessmentTime(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            DataTable table = GetAllAssessmentResultTable(mProductionID, mWorkingSectionID, mGroupId, mStartTime, mEndTime, mStatisticalCycle);
            DataTable cloumnTime = new DataTable();
            string name = "";
            string keyId = "";
            cloumnTime.Columns.Add("Time", Type.GetType("System.String"));
            foreach (DataRow dr in table.Rows)
            {
                if (name != Convert.ToString(dr[0]))
                {
                    DataRow mycloumn = cloumnTime.NewRow();
                    mycloumn["Time"] = Convert.ToString(dr[4]);
                    name = Convert.ToString(dr[0]);
                    cloumnTime.Rows.Add(mycloumn);
                    keyId = Convert.ToString(dr[8]);
                }
                if (keyId != Convert.ToString(dr[8]))
                {
                    DataRow mycloumn = cloumnTime.NewRow();
                    mycloumn["Time"] = Convert.ToString(dr[4]);
                    keyId = Convert.ToString(dr[8]);
                    cloumnTime.Rows.Add(mycloumn);
                }
                
            }
            return cloumnTime;
        }
        public static string GetCloumnString(DataTable cloumnName)
        {
            StringBuilder cloumnJson = new StringBuilder("{\"frozenColumns\":[");
            cloumnJson.Append("{\"field\":\"产线\",\"title\":\"产线\",\"width\":80,\"align\":\"center\"},{\"field\": \"考核元素\", \"title\":\"考核元素\", \"width\":120, \"align\":\"center\" },{\"field\": \"考核项\", \"title\":\"考核项\", \"width\":120, \"align\":\"center\" }]");
            cloumnJson.Append(",\"columns\":[");
            for (int i = 0; i < cloumnName.Rows.Count; i++)
            {
                string name = Convert.ToString(cloumnName.Rows[i][0]);
                cloumnJson.Append("{\"field\":\"" + i + "\",\"title\":\"" + name + "\",\"width\":100,\"align\":\"center\"},");
            }
            cloumnJson.Remove(cloumnJson.Length - 1, 1); 
            cloumnJson.Append("]}");
            return cloumnJson.ToString();
        }
        //{\"field\":\"edit\",\"title\":\"详表\"，\"width\":80,\"formatter\":\"\"}
        //public static DataTable GetAssessmentResultTable(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        //{ 

        public static string Getjson(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            DataTable table = GetAllAssessmentResultTable(mProductionID, mWorkingSectionID, mGroupId, mStartTime, mEndTime, mStatisticalCycle);
            DataTable timeTable = GetAssessmentTime(mProductionID, mWorkingSectionID, mGroupId, mStartTime, mEndTime, mStatisticalCycle);
            DataTable nameTable = GetAssessmentResultCloumnName(mProductionID, mWorkingSectionID, mGroupId, mStartTime, mEndTime, mStatisticalCycle);
            string keyId = Convert.ToString(table.Rows[0][8]);
            int socre = 0;//每项分数
            int k = 0;
            int j = 0;
            int i = 0;
            int l = 0;
            int m = 0;
            StringBuilder dataJson = new StringBuilder("{\"total\":1,\"rows\":[");
            //foreach (DataRow dr_name in nameTable.Rows)
            //{
            //    string name = Convert.ToString(dr_name["name"]);
            //    string time = Convert.ToString(timeTable.Rows[i][0]);
            //    dataJson.Append(",\"" + i + "\":\"" + time + "\"");
            //    i++;
            //}
            //dataJson.Append("},");
            foreach (DataRow dr in table.Rows)
            {
                string line = Convert.ToString(table.Rows[k][7]);//生产线
                string element = Convert.ToString(table.Rows[k][6]);//考核元素
                string item = Convert.ToString(table.Rows[k][5]);//考核项
                if (Convert.ToString(table.Rows[k][8]) == keyId)
                {
                    dataJson.Append("{\"产线\":\"" + line + "\",\"考核元素\":\"" + element + "\",\"考核项\":\"" + item + "\"");

                    foreach (DataRow m_dr in table.Rows)
                    {

                        if (m_dr["OrganizationName"].ToString() == line && m_dr["ObjectName"].ToString() == element && m_dr["AssessmentName"].ToString() == item)
                        {

                            string name = Convert.ToString(nameTable.Rows[j][0]);
                            socre = Convert.ToInt32(m_dr["WeightedAverageCredit"]);
                            dataJson.Append(",\"" + j + "\":\"" + socre + "\"");
                            j++;
                        }
                    }
                }
                else
                {
                    break;
                }
                dataJson.Append("},");
                k++;
                j = 0;
                socre = 0;

            }
            keyId = "";
            int coefficient = 0;
            dataJson.Append("{\"产线\":\"\",\"考核元素\":\"\",\"考核项\":\"考核系数\"");

            {
                foreach (DataRow dr in table.Rows)
                {
                    if(dr["KeyId"].ToString()!=keyId)
                    {
                        coefficient = Convert.ToInt32(dr["AssessmentCoefficient"]);
                        dataJson.Append(",\"" + m + "\":\"" + coefficient + "\"");
                        keyId = Convert.ToString(dr["KeyId"]);
                        m++;
                    }
 
                }

            }
            dataJson.Append("},");
            m = 0;
            keyId = Convert.ToString(table.Rows[0][8]);
            coefficient = 0;
            int sum = 0;
            dataJson.Append("{\"产线\":\"\",\"考核元素\":\"\",\"考核项\":\"总分\"");
            foreach (DataRow dr in table.Rows)
            {
                if (Convert.ToString(dr["KeyId"]) == keyId)
                {
                    socre = Convert.ToInt32(dr["WeightedAverageCredit"]);
                    sum += socre;
                    coefficient = Convert.ToInt32(dr["AssessmentCoefficient"]);
                }
                else
                {
                    socre = sum * Convert.ToInt32(dr["AssessmentCoefficient"]);
                    dataJson.Append(",\"" + m + "\":\"" + socre + "\"");
                    keyId = Convert.ToString(dr["KeyId"]);
                    sum = Convert.ToInt32(dr["WeightedAverageCredit"]);
                    m++;
                }
            }
            socre = sum * coefficient;
            dataJson.Append(",\"" + m + "\":\"" + socre + "\"");
            dataJson.Append("}");
            //dataJson.Remove(dataJson.Length - 1, 1);
            dataJson.Append("]}");
            return dataJson.ToString();
        }
        
    }
}
