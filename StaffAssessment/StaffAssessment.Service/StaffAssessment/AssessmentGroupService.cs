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
    public class AssessmentGroupService
    {
        public static DataTable GetQueryDataTable()
        {
             string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT [GroupId]
                                        ,[Name]
                                        ,[StatisticalCycle] 
                                        ,[Remark]
                                        ,[Creator]
                                        ,[CreateTime]
                                FROM [dbo].[assessment_ ShiftAssessmentResultGroup] order by CreateTime desc";
            DataTable dt = factory.Query(mySql);
            return dt;
        }

        public static int InsertWorkingSection(string mName, string mStatisticalcycle, string mCreator, string mRemark)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"INSERT INTO [dbo].[assessment_ ShiftAssessmentResultGroup]
                            ([GroupId]
                             ,[Name]
                             ,[StatisticalCycle]
                             ,[Remark]
                             ,[Creator]
                             ,[CreateTime])
                        VALUES
                            (
                                    @mGroupId
                                    ,@mName
                                    ,@mStatisticalcycle
                                    ,@mRemark
                                    ,@mCreator
                                    ,@mCreattime

                              )";
            SqlParameter[] para = { new SqlParameter("@mGroupId",System.Guid.NewGuid().ToString()),
                                    new SqlParameter("@mName", mName),
                                    new SqlParameter("@mStatisticalcycle", mStatisticalcycle),
                                    new SqlParameter("@mCreator", mCreator),
                                    new SqlParameter("@mCreattime", DateTime.Now.ToString()),
                                    new SqlParameter("@mRemark", mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }

        public static int deleteWorkingSection(string mCreateTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[assessment_ ShiftAssessmentResultGroup]
                         WHERE CreateTime =@mCreateTime";
            SqlParameter para = new SqlParameter("@mCreateTime", mCreateTime);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;
        }

        public static int EditWorkingSections(string mName, string mStatisticalcycle, string mCreator, string mRemark, string mGroupId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"UPDATE [dbo].[assessment_ ShiftAssessmentResultGroup]
                               SET [Name]=@mName
                              ,[StatisticalCycle]=@mStatisticalcycle
                              ,[Remark]=@mRemark
                              ,[Creator]=@mCreator
                              ,[CreateTime]=@mCreatedTime
                       WHERE [GroupId] =@mGroupId";
            SqlParameter[] para = { new SqlParameter("@mName", mName),
                                    new SqlParameter("@mStatisticalcycle",mStatisticalcycle),
                                    new SqlParameter("@mCreator", mCreator),
                                    new SqlParameter("@mRemark", mRemark),
                                    new SqlParameter("@mCreatedTime",DateTime.Now.ToString()),
                                    new SqlParameter("@mGroupId", mGroupId) };
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;

        }

    }
}
