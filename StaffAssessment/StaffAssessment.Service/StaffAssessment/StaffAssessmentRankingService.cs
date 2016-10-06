using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StaffAssessment.Service.StaffAssessment
{
    public class StaffAssessmentRankingService
    {
        public static DataTable GetAssessmentResultTable(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime,  string mStatisticalCycle)
        {
            DataTable resultTable = generationTableTemplate(mStartTime, mEndTime, mStatisticalCycle);
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
                endTime = Convert.ToDateTime(mEndTime + "  00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
            }
                string mySql = @"SELECT C.[Name] as StaffName
	                                  ,D.[StatisticalCycle]
                                      ,A.[OrganizationID]
                                      ,A.[WorkingSectionID]
	                                  ,case D.[StatisticalCycle] 
	                                  when 'day' then  convert(char(10),A.StartTime,120) 
	                                  when 'month' then  convert(char(7),A.StartTime,120) 
	                                  when 'year' then  convert(char(4),A.StartTime,120) 
	                                  else '' end  as [Time]
                                      ,A.[GroupId]
	                                  ,B.[AssessmenScore]
                                  FROM [dbo].[tz_ShiftAssessmentResult] A,(select KeyId,sum([AssessmenScore]) as [AssessmenScore] from [dbo].[assessment_ShiftAssessmentResultDetail]
                                        where KeyId in( select KeyId from [dbo].[tz_ShiftAssessmentResult]
                                        where [OrganizationID]=@mProductionID
                                        and [WorkingSectionID]=@mWorkingSectionID
                                        and [GroupId]=@mGroupId
                                        and [StartTime]>=convert(datetime,@mStartTime)
                                        and [StartTime]<=convert(datetime,@mEndTime)
                                        )
                                        group by KeyId,[AssessmenScore]) B,[dbo].[system_StaffInfo] C,[dbo].[assessment_ ShiftAssessmentResultGroup] D
                                 where A.[KeyId]=B.[KeyId]
                                 and A.[StaffID]=C.[StaffInfoItemId]
                                 and A.[GroupId]=D.[GroupId]
                                 order by C.[StaffInfoID],Time asc";
                SqlParameter[] para = { new SqlParameter("@mProductionID", mProductionID),
                                        new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                        new SqlParameter("@mGroupId", mGroupId),
                                        new SqlParameter("@mStartTime", starTime),
                                        new SqlParameter("@mEndTime", endTime)
                                     };
                 table = factory.Query(mySql, para);           
            int rowNum = 0;
            if (table.Rows.Count > 0)
            {
                resultTable.Rows.Add(table.Rows[0]["StaffName"].ToString().Trim());
                resultTable.Rows[rowNum][table.Rows[0]["Time"].ToString().Trim()] = table.Rows[0]["AssessmenScore"];
                decimal averageValue = Convert.ToDecimal(table.Rows[0]["AssessmenScore"]);
                resultTable.Rows[rowNum]["总分"] = averageValue;
                for (int i = 0; i < table.Rows.Count - 1; i++)
                {
                    if (table.Rows[i + 1]["StaffName"].ToString().Trim().Equals(table.Rows[i]["StaffName"].ToString().Trim()))
                    {
                        resultTable.Rows[rowNum][table.Rows[i + 1]["Time"].ToString().Trim()] = table.Rows[i + 1]["AssessmenScore"];
                        averageValue = averageValue + Convert.ToDecimal(table.Rows[i + 1]["AssessmenScore"]);
                        resultTable.Rows[rowNum]["总分"] = averageValue;

                    }
                    else
                    {
                        ++rowNum;
                        resultTable.Rows.Add(table.Rows[i + 1]["StaffName"].ToString().Trim());
                        resultTable.Rows[rowNum][table.Rows[i + 1]["Time"].ToString().Trim()] = table.Rows[i + 1]["AssessmenScore"];
                        averageValue = Convert.ToDecimal(table.Rows[i + 1]["AssessmenScore"]);
                        resultTable.Rows[rowNum]["总分"] = averageValue;
                    }
                }
                DataView dv = resultTable.DefaultView;
                dv.Sort = "总分 desc";
                resultTable = dv.ToTable();
                for (int i = 0; i < resultTable.Rows.Count; i++)
                {
                    resultTable.Rows[i]["排名"] = i + 1;
                }
            }       
            return resultTable;
        }
        public static string GetGenerationTableTemplateColumnsJson(string mProductionID, string mWorkingSectionID, string mGroupId, string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            DataTable resultTable = generationTableTemplate(mStartTime, mEndTime, mStatisticalCycle);
            StringBuilder jsonBuilder = new StringBuilder("{\"columns\":[");
            jsonBuilder.Append("{\"field\":\"员工姓名\",\"title\":\"员工姓名\",\"width\":80,\"align\":\"left\"},");
            for(int i=1;i<resultTable.Columns.Count-2;i++)
            {               
                string columnName = resultTable.Columns[i].ColumnName;
                int columnLength = 0;
                if (Regex.IsMatch(columnName, @"^[\u4e00-\u9fa5]+$"))
                {
                    columnLength = columnName.Length * 15;
                }
                else {
                    if (columnName.Length > 4)
                    {
                        columnLength = columnName.Length * 8;
                    }
                    else {
                        columnLength = 60;
                    }                       
                }
                jsonBuilder.Append("{\"field\":\"" + columnName + "\",\"title\":\"" + columnName + "\",\"width\":" + columnLength + ",\"align\":\"center\"},");
            }
            jsonBuilder.Append("{\"field\":\"总分\",\"title\":\"总分\",\"width\":80,\"align\":\"center\"},");
            jsonBuilder.Append("{\"field\":\"排名\",\"title\":\"排名\",\"width\":80,\"align\":\"center\"}]}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 生成表头
        /// </summary>
        /// <param name="mStartTime"></param>
        /// <param name="mEndTime"></param>
        /// <param name="mStatisticalCycle"></param>
        /// <returns></returns>
        private static DataTable generationTableTemplate(string mStartTime, string mEndTime, string mStatisticalCycle)
        {
            //int year = 0;
            //int month = 0;
            //int day = 0;
            string[] sdate = mStartTime.Split('-');
            string[] edate = mEndTime.Split('-');
            DataTable tableTemplate = new DataTable();
            tableTemplate.Columns.Add("员工姓名", typeof(string));
            if (mStatisticalCycle.Equals("month"))
            { 
                if (Convert.ToInt16(sdate[0]) == Convert.ToInt16(edate[0]))
                {
                    for (int i = Convert.ToInt16(sdate[1]); i <= Convert.ToInt16(edate[1]);i++ )
                    {
                        tableTemplate.Columns.Add(Convert.ToInt16(sdate[0]).ToString() + "-" + i.ToString("00"), typeof(double));
                    }
                }
                else {
                    for (int i = Convert.ToInt16(sdate[1]); i <= 12;i++ )
                    {
                        tableTemplate.Columns.Add(Convert.ToInt16(sdate[0]).ToString() + "-" + i.ToString("00"), typeof(double));
                    }
                    for (int i = Convert.ToInt16(sdate[0])+1; i < Convert.ToInt16(edate[0]); i++) //遍历年
                    {
                        for (int j = 1; j <= 12;j++ )
                        {
                            tableTemplate.Columns.Add((Convert.ToInt16(sdate[0]) + 1).ToString() + "-" + j.ToString("00"), typeof(double));
                        }
                    }
                    for (int i = 1; i <= Convert.ToInt16(edate[1]); i++)
                    {
                        tableTemplate.Columns.Add(Convert.ToInt16(edate[0]).ToString() + "-" + i.ToString("00"), typeof(double));                  
                    }             
                }
            }
            else if (mStatisticalCycle.Equals("day"))  //默认是在相同年份
            {
                if (Convert.ToInt16(sdate[1]) == Convert.ToInt16(edate[1]))  ////同月
                {
                    for (int i = Convert.ToInt16(sdate[2]); i <= Convert.ToInt16(edate[2]); i++)
                    {
                        tableTemplate.Columns.Add(Convert.ToInt16(sdate[0]).ToString() + "-" + Convert.ToInt16(sdate[1]).ToString("00") + "-" + i.ToString("00"), typeof(double));
                    }
                }
                else
                {  ////不同月
                    int myear = Convert.ToInt16(sdate[0]);
                    for (int i = Convert.ToInt16(sdate[2]); i <= DateTime.DaysInMonth(myear, Convert.ToInt16(sdate[1])); i++)
                    {
                        tableTemplate.Columns.Add(Convert.ToInt16(sdate[0]).ToString() + "-" + Convert.ToInt16(sdate[1]).ToString("00") + "-" + i.ToString("00"), typeof(double));
                    }
                    for (int i = Convert.ToInt16(sdate[1]) + 1; i < Convert.ToInt16(edate[1]); i++) //遍历月
                    {
                        for (int j = 1; j <= DateTime.DaysInMonth(myear,i); j++)
                        {
                            tableTemplate.Columns.Add(Convert.ToInt16(sdate[0]).ToString() + "-" + i.ToString("00") + "-" + j.ToString("00"), typeof(double));
                        }
                    }
                    for (int i = 1; i <= Convert.ToInt16(edate[2]); i++)
                    {
                        tableTemplate.Columns.Add(Convert.ToInt16(sdate[0]).ToString() + "-" + Convert.ToInt16(edate[1]).ToString("00") + "-" + i.ToString("00"), typeof(double));
                    }             
                }
            }
            else if (mStatisticalCycle.Equals("year")){
                tableTemplate.Columns.Add(mStartTime, typeof(double));
            }
            tableTemplate.Columns.Add("总分", typeof(double));
            tableTemplate.Columns.Add("排名", typeof(string));
            return tableTemplate;
        }
    }
}
