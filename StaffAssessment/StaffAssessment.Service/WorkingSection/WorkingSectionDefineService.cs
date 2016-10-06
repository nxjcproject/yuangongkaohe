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
    public class WorkingSectionDefineService
    {
        public static DataTable GetQueryDataTable(string mOrganizationID) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT A.[WorkingSectionItemID]
                          ,A.[WorkingSectionID]
	                      ,B.[LevelCode]
                          ,A.[WorkingSectionName]
                          ,A.[Type]
                          ,A.[OrganizationID]
	                      ,B.Name as OrganizationName
                          ,A.[DisplayIndex]
                          ,A.[ElectricityQuantityId]
                          ,A.[OutputId]
                          ,A.[CoalWeightId]
                          ,A.[Creator]
                          ,A.[CreatedTime]
                          ,A.[Enabled]
                          ,A.[Remarks]
                      FROM [dbo].[system_WorkingSection] A,[dbo].[system_Organization] B,dbo.system_WorkingSectionType C
                      where A.[OrganizationID]=B.[OrganizationID]
                      and  A.[WorkingSectionID]=C.[WorkingSectionID]
                      and  A.[OrganizationID] like @mOrganizationID+'%' 
                      order by LevelCode,Type";
            SqlParameter para = new SqlParameter("@mOrganizationID", mOrganizationID);
            DataTable dt = factory.Query(mySql, para);
            return dt;          
        }
        public static int InsertWorkingSection(string mWorkingSectionID,string mProductionName, string mSectionType, string mWorkingSection, string mEnabed, string mEditor, string mRemark)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"INSERT INTO [dbo].[system_WorkingSection]
                            ([WorkingSectionItemID]
                            ,[WorkingSectionID]
                            ,[WorkingSectionName]
                            ,[Type]
                            ,[OrganizationID]
                            ,[DisplayIndex]
                            ,[ElectricityQuantityId]
                            ,[OutputId]
                            ,[CoalWeightId]
                            ,[Creator]
                            ,[CreatedTime]
                            ,[Enabled]
                            ,[Remarks])
                        VALUES
                            (@mWorkingSectionItemID
                            ,@mWorkingSectionID
                            ,@mWorkingSectionName
                            ,@mType
                            ,@mOrganizationID
                            ,null
                            ,null
                            ,null
                            ,null
                            ,@mCreator
                            ,@mCreatedTime
                            ,@mEnabled
                            ,@mRemarks)";
            SqlParameter[] para = { new SqlParameter("@mWorkingSectionItemID",System.Guid.NewGuid().ToString()),
                                    new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                    new SqlParameter("@mWorkingSectionName", mWorkingSection),
                                    new SqlParameter("@mType", mSectionType),
                                    new SqlParameter("@mOrganizationID", mProductionName),
                                    new SqlParameter("@mCreator", mEditor),
                                    new SqlParameter("@mCreatedTime",DateTime.Now.ToString()),
                                    new SqlParameter("@mEnabled", mEnabed),
                                    new SqlParameter("@mRemarks", mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;              
        }
        public static int EditWorkingSection(string mWorkingSectionItemID,string mWorkingSectionID, string mProductionName, string mSectionType, string mWorkingSection, string mEnabed, string mEditor, string mRemark) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"UPDATE [dbo].[system_WorkingSection]
                               SET [WorkingSectionID] =@mWorkingSectionID
                                  ,[WorkingSectionName] = @mWorkingSectionName
                                  ,[Type] = @mType
                                  ,[OrganizationID] = @mOrganizationID                                 
                                  ,[Creator] = @mCreator
                                  ,[CreatedTime] = @mCreatedTime
                                  ,[Enabled] = @mEnabled
                                  ,[Remarks] = @mRemarks
                         WHERE [WorkingSectionItemID]=@mWorkingSectionItemID";
            SqlParameter[] para = { 
                                    new SqlParameter("@mWorkingSectionItemID", mWorkingSectionItemID),
                                    new SqlParameter("@mWorkingSectionID", mWorkingSectionID),
                                    new SqlParameter("@mWorkingSectionName", mWorkingSection),
                                    new SqlParameter("@mType", mSectionType),
                                    new SqlParameter("@mOrganizationID", mProductionName),
                                    new SqlParameter("@mCreator", mEditor),
                                    new SqlParameter("@mCreatedTime",DateTime.Now.ToString()),
                                    new SqlParameter("@mEnabled", mEnabed),
                                    new SqlParameter("@mRemarks", mRemark)};
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;     
        }
        public static int deleteWorkingSection(string mWorkingSectionItemID) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[system_WorkingSection]
                         WHERE [WorkingSectionItemID]=@mWorkingSectionItemID";
            SqlParameter para = new SqlParameter("@mWorkingSectionItemID", mWorkingSectionItemID);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;     
        }
        public static DataTable GetWorkingSectionTypeList(string mOrganizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT  ROW_NUMBER() OVER (ORDER BY [WorkingSectionID]) as id,[WorkingSectionID]
                              ,[WorkingSectionType]
                              ,[OrganizationID]
                              ,[Enabled]
                          FROM [dbo].[system_WorkingSectionType]
                          where [OrganizationID]=@mOrganizationId
                          and [Enabled]=1";
            SqlParameter para = new SqlParameter("@mOrganizationId", mOrganizationId);
            DataTable dt = factory.Query(mySql, para);
            return dt;            
        }
        public static DataTable GetAllWorkingSectionTypeList(string mOrganizationId) 
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT  ROW_NUMBER() OVER (ORDER BY [WorkingSectionID]) as id,[WorkingSectionID]
                              ,[WorkingSectionType]
                              ,[OrganizationID]
                              ,[Enabled]
                          FROM [dbo].[system_WorkingSectionType]
                          where [OrganizationID]=@mOrganizationId";
            SqlParameter para = new SqlParameter("@mOrganizationId", mOrganizationId);
            DataTable dt = factory.Query(mySql, para);
            return dt;            
        }
        public static string InsertWorkingSectionTypeList(string mOrganizationId, string mWorkingSectionType, string mEnabedMark, string mWorkingSectionID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            //先进行查询匹配，如果存在进行更新操作，如果不存在进行插入操作
            string selectSql = @"select * from [dbo].[system_WorkingSectionType]
                                    where [OrganizationID]=@mOrganizationId
                                    and [WorkingSectionType]=@mWorkingSectionType";
            SqlParameter[] selectpara = { 
                                    new SqlParameter("@mOrganizationId", mOrganizationId),
                                    new SqlParameter("@mWorkingSectionType", mWorkingSectionType)};
           // int mHave = factory.ExecuteSQL(selectSql, selectpara);
            DataTable resultTable = factory.Query(selectSql, selectpara);
            int mHave = resultTable.Rows.Count;
            string result = mHave.ToString();
            if (mHave >= 1)   //进行更新操作
            {
                string updateSql = @"UPDATE [dbo].[system_WorkingSectionType]
                                    SET [WorkingSectionType] = @mWorkingSectionType
                                        ,[OrganizationID] = @mOrganizationId
                                        ,[Enabled] = @mEnabedMark
                                    WHERE [WorkingSectionID] = @mWorkingSectionID";
                SqlParameter[] updatepara = { new SqlParameter("@mWorkingSectionID",mWorkingSectionID),
                                    new SqlParameter("@mWorkingSectionType", mWorkingSectionType),
                                    new SqlParameter("@mOrganizationId", mOrganizationId),
                                    new SqlParameter("@mEnabedMark", mEnabedMark)};
                int updateResult = factory.ExecuteSQL(updateSql, updatepara);
                result = result + updateResult.ToString();  //10  更新失败    11 更新成功           

            }
            else {      //进行插入操作
                string insertSql = @"INSERT INTO [dbo].[system_WorkingSectionType]
                                   ([WorkingSectionID]
                                   ,[WorkingSectionType]
                                   ,[OrganizationID]
                                   ,[Enabled])
                                 VALUES
                                       (@mWorkingSectionID
                                       ,@mWorkingSectionType
                                       ,@mOrganizationId
                                       ,@mEnabedMark)";
                SqlParameter[] insertpara = { new SqlParameter("@mWorkingSectionID",System.Guid.NewGuid().ToString()),
                                    new SqlParameter("@mWorkingSectionType", mWorkingSectionType),
                                    new SqlParameter("@mOrganizationId", mOrganizationId),
                                    new SqlParameter("@mEnabedMark", mEnabedMark)};
                int insertResult = factory.ExecuteSQL(insertSql, insertpara);
                result = result + insertResult.ToString();  //00  插入失败    01 插入成功           
            }

            return result;               
        }
        public static int deleteWorkingSectionTypeList(string mWorkingSectionID)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);

            string mySql = @"delete from [dbo].[system_WorkingSectionType]
                         WHERE [WorkingSectionID] =@mWorkingSectionID";
            SqlParameter para = new SqlParameter("@mWorkingSectionID", mWorkingSectionID);
            int dt = factory.ExecuteSQL(mySql, para);
            return dt;             
        }
    }
}
