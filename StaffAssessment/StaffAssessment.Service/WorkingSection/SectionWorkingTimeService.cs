using SqlServerDataAdapter;
using StaffAssessment.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StaffAssessment.Service.WorkingSection
{
    public class SectionWorkingTimeService
    {
        public static DataTable GetQueryDataTable(string mOrganizationID, string mWorkingSectionID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            DataTable table=new DataTable();
            if (mWorkingSectionID.Equals("0"))
            {
                string mySql = @"SELECT A.[ShiftDescriptionID]
                                ,C.Name as OrganizationName
                                ,A.[WorkingSectionID]
	                            ,B.[WorkingSectionType] as [WorkingSectionName]
                                ,A.[Shifts]
                                ,A.[StartTime]
                                ,A.[EndTime]
                                ,A.[Remark]
                      FROM  [dbo].[system_WorkingSectionShiftDescription] A,[dbo].[system_WorkingSectionType] B,system_Organization C
                      where A.[WorkingSectionID]=B.[WorkingSectionID]
                      and B.[OrganizationID]=C.[OrganizationID]
                      and B.[OrganizationID] like @mOrganizationID+'%'
                      order by [WorkingSectionName]";
                SqlParameter[] para = { new SqlParameter("@mOrganizationID", mOrganizationID) };
                table = factory.Query(mySql, para);           
            }
            else {
                string mySql = @"SELECT A.[ShiftDescriptionID]
                                ,C.Name as OrganizationName
                                ,A.[WorkingSectionID]
	                            ,B.[WorkingSectionType] as [WorkingSectionName]
                                ,A.[Shifts]
                                ,A.[StartTime]
                                ,A.[EndTime]
                                ,A.[Remark]
                      FROM  [dbo].[system_WorkingSectionShiftDescription] A,[dbo].[system_WorkingSectionType] B,system_Organization C
                      where A.[WorkingSectionID]=B.[WorkingSectionID]
                      and B.[OrganizationID]=C.[OrganizationID]
                      and B.[OrganizationID] = @mOrganizationID
                      and B.[WorkingSectionID]=@mWorkingSectionID
                      order by [WorkingSectionID],[WorkingSectionName]";
                SqlParameter[] para = { 
                                      new SqlParameter("@mOrganizationID", mOrganizationID) ,
                                      new SqlParameter("@mWorkingSectionID", mWorkingSectionID)
                                  };
                table = factory.Query(mySql, para);                                       
            } 
            return table;
        }
        public static int AddSectionWorkingDefine(string mWorkingSectionID, string mShifts, string mStartTime, string mEndTime, string mRemark) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"INSERT INTO [dbo].[system_WorkingSectionShiftDescription]
                                   ([ShiftDescriptionID]
                                   ,[WorkingSectionID]
                                   ,[Shifts]
                                   ,[StartTime]
                                   ,[EndTime]
                                   ,[Remark])
                             VALUES
                                   (@mShiftDescriptionID
                                   ,@mWorkingSectionID
                                   ,@mShifts
                                   ,@mStartTime
                                   ,@mEndTime
                                   ,@mRemark)";
            SqlParameter[] para = { new SqlParameter("@mShiftDescriptionID",System.Guid.NewGuid().ToString()),
                                    new SqlParameter("@mWorkingSectionID",mWorkingSectionID),
                                    new SqlParameter("@mShifts", mShifts),
                                    new SqlParameter("@mStartTime", mStartTime),
                                    new SqlParameter("@mEndTime", mEndTime),
                                    new SqlParameter("@mRemark", mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;                  
        }
        public static int EditSectionWorking(string mShiftDescriptionID,string mWorkingSectionID, string mShifts, string mStartTime, string mEndTime, string mRemark)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"UPDATE [dbo].[system_WorkingSectionShiftDescription]
                           SET [WorkingSectionID] = @mWorkingSectionID
                              ,[Shifts] =@mShifts
                              ,[StartTime] =@mStartTime
                              ,[EndTime] = @mEndTime
                              ,[Remark] = @mRemark
                         WHERE [ShiftDescriptionID] =@mShiftDescriptionID";
            SqlParameter[] para = { new SqlParameter("@mShiftDescriptionID",mShiftDescriptionID),
                                    new SqlParameter("@mWorkingSectionID",mWorkingSectionID),
                                    new SqlParameter("@mShifts", mShifts),
                                    new SqlParameter("@mStartTime", mStartTime),
                                    new SqlParameter("@mEndTime", mEndTime),
                                    new SqlParameter("@mRemark", mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;     
        
        }
        public static int deleteSectionWorkingDefine(string mShiftDescriptionID) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[system_WorkingSectionShiftDescription]
                         WHERE [ShiftDescriptionID] =@mShiftDescriptionID";

            SqlParameter para = new SqlParameter("@mShiftDescriptionID", mShiftDescriptionID);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;     
        }
    }
}
