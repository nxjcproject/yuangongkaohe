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
                                      ,A.[KeyId]
                                      ,A.[TimeStamp]
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

    }
}
