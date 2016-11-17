using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StaffAssessment.Service.StaffAssessment
{
    public class StaffAssessmentResultDetialService
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
            string mySql = @"select G.StaffName,G.Time,G.AssessmentCoefficient,G.KeyId,G.Value,row_number()over(order by G.Value desc)as RowNo from(
                     select   C.[Name] as StaffName
		         ,A.StartTime as Time
				 ,A.AssessmentCoefficient
				 ,A.KeyId
				  ,sum(B.[WeightedAverageCredit]) as Value
	              from [NXJC].[dbo].[tz_ShiftAssessmentResult] A ,[NXJC].[dbo].[system_StaffInfo] C 
				  ,[NXJC].[dbo].[assessment_ ShiftAssessmentResultGroup] D
				  ,[NXJC].[dbo].[assessment_ShiftAssessmentResultDetail] B
				  , [NXJC].[dbo].[system_WorkingSection] E
				  where  A.[OrganizationID]='zc_nxjc_byc_byf'
				         and C.[StaffInfoItemId]=A.[StaffID]
                         and A.[WorkingSectionID]= E.WorkingSectionItemID
					     and E.WorkingSectionID =@mWorkingSectionID
                         and A.[GroupId]=@mGroupId
                        and A.[StartTime]>=convert(datetime,@mStartTime)
                         and A.[EndTime]<=convert(datetime,@mEndTime)
                         and B.[KeyId]=A.[KeyId]
                         and D.[GroupId]=A.[GroupId]
						 group by A.KeyId,C.[Name],A.StartTime,A.AssessmentCoefficient
                 ) as G
                                        ";
            SqlParameter[] para = { new SqlParameter("@mProductionID", mProductionID) ,
                                        new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                        new SqlParameter("@mGroupId", mGroupId),
                                        new SqlParameter("@mStartTime", starTime),
                                        new SqlParameter("@mEndTime", endTime)
                                     };
                                 
            DataTable dt = factory.Query(mySql, para);
            foreach (DataRow dr in dt.Rows)
            {
                dr[4] = Convert.ToInt32(dr[4]) * Convert.ToInt32( dr[2]);
            }
            return dt;     
        }
        public static DataTable GetAssessmentResultdetailTable(string mAssessmentId) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT
                           A.[ObjectName]
                           ,A.[AssessmentName]
                          ,A.[WeightedValue]
                          ,A.[BestValue]
                          ,A.[WorstValue]
                          ,A.[AssessmenScore]
                          ,A.[WeightedAverageCredit]
                          ,B.[AssessmentCoefficient]
                          from [dbo].[assessment_ShiftAssessmentResultDetail] A,[dbo].[tz_ShiftAssessmentResult] B
                         where A.[KeyId]=@mAssessmentId
                          and B.[KeyId]=@mAssessmentId
                           ";
            SqlParameter para = new SqlParameter("@mAssessmentId", mAssessmentId);
            DataTable dt = factory.Query(mySql, para);

            return dt;
        }

    }
}
