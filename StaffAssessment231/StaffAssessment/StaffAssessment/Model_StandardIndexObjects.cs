using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
namespace StaffAssessment
{
    public class Model_StandardIndexObjects
    {
        private string _StructureName;      //表示结构,tree结构还是grid结构
        private string _StructureType;      //表结构的类型,不同目录对应相同的计算对象集,相同对象的表结构归为一类
        private DataTable _StandardIndexObjectsTable;   //对象集表
        public Model_StandardIndexObjects()
        {
            _StructureName = "";
            _StructureType = "";
            _StandardIndexObjectsTable = new DataTable();
        }
        public string StructureName
        {
            get
            {
                return _StructureName;
            }
            set
            {
                _StructureName = value;
            }
        }
        public string StructureType
        {
            get
            {
                return _StructureType;
            }
            set
            {
                _StructureType = value;
            }
        }
        public DataTable StandardIndexObjectsTable
        {
            get
            {
                return _StandardIndexObjectsTable;
            }
            set
            {
                _StandardIndexObjectsTable = value;
            }
        }
    }
    public class Table_StandardIndexObjects
    {
        public static Model_StandardIndexObjects GetStandardIndexObjects(string myOrganizationId, string myAssessmentId, string myType, string ValueType,  SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            Model_StandardIndexObjects m_StandardIndexObjects = new Model_StandardIndexObjects();
            if (myType == "MaterialWeight")
            {
                m_StandardIndexObjects.StructureName = "grid";
                m_StandardIndexObjects.StructureType = "MaterialDetail";
                m_StandardIndexObjects.StandardIndexObjectsTable = GetMaterialDetail(myOrganizationId, myAssessmentId, myDataFactory);
            }
            else if (myType == "Energy")
            {
                m_StandardIndexObjects.StructureName = "tree";
                m_StandardIndexObjects.StructureType = "FormulaDetail";
                m_StandardIndexObjects.StandardIndexObjectsTable = GetProductionProcess(myOrganizationId, myAssessmentId, myDataFactory);
            }
            else if (myType == "Comprehensive")
            {
                m_StandardIndexObjects.StructureName = "grid";
                m_StandardIndexObjects.StructureType = "ComprehensiveInternal";
                m_StandardIndexObjects.StandardIndexObjectsTable = GetComprehensiveDetail(myOrganizationId, myAssessmentId, myDataFactory);
                //    new DataTable();
                //m_StandardIndexObjects.StandardIndexObjectsTable.Columns.Add("VariableId", typeof(string));
                //m_StandardIndexObjects.StandardIndexObjectsTable.Columns.Add("Name", typeof(string));
                //m_StandardIndexObjects.StandardIndexObjectsTable.Rows.Add("clinker", "熟料");
                //m_StandardIndexObjects.StandardIndexObjectsTable.Rows.Add("cementmill", "水泥");
            }
            else if (myType == "Production")
            {
                m_StandardIndexObjects.StructureName = "grid";
                m_StandardIndexObjects.StructureType = "FormulaDetail";
                m_StandardIndexObjects.StandardIndexObjectsTable = GetEquipmentDetail(myOrganizationId, myAssessmentId, myDataFactory);
            }
            return m_StandardIndexObjects;
        }
        private static DataTable GetMaterialDetail(string myOrganizationId, string myAssessmentId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select M.*, '{1}' as AssessmentId, N.StandardIndex from 
                                 (SELECT B.VariableId as VariableId
                                                              ,C.Name as ProductionLineName
	                                                          ,B.Name as Name
	                                                          ,A.OrganizationID
                                                              ,C.LevelCode as ProductionLineLevelCode
	                                                          ,B.Unit
								                              ,C.Type
                                                          FROM tz_Material A, material_MaterialDetail B, system_Organization C, system_Organization D
                                                          where A.Type = 2
                                                          and A.Enable = 1
                                                          and A.State = 0
                                                          and A.KeyID = B.KeyID
                                                          and A.OrganizationID = C.OrganizationID
                                                          and D.OrganizationID = '{0}'
                                                          and C.LevelCode like D.LevelCode + '%') M
                            left join assessment_StandardIndex N on N.AssessmentId = '{1}' and M.OrganizationID = N.OrganizationID and M.VariableId = N.ObjectId
                            order by M.Type, M.OrganizationID, M.ProductionLineLevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId, myAssessmentId);
            try
            {
                DataTable m_Result = myDataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }

        }
        private static DataTable GetProductionProcess(string myOrganizationId, string myAssessmentId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select M.*, '{1}' as AssessmentId, N.StandardIndex from 
                                (SELECT B.VariableId as VariableId
                                    ,C.Name as ProductionLineName
	                                ,B.Name as Name
	                                ,A.OrganizationID
	                                ,C.LevelCode as ProductionLineLevelCode
	                                ,B.LevelCode as ProcessLevelCode
                                    ,C.Type
                                    FROM tz_Formula A, formula_FormulaDetail B, system_Organization C, system_Organization D
                                    where A.Type = 2
                                    and A.Enable = 1
                                    and A.State = 0
                                    and A.KeyID = B.KeyID
                                    and A.OrganizationID = C.OrganizationID
                                    and D.OrganizationID = '{0}'
                                    and C.LevelCode like D.LevelCode + '%') M
	                            left join assessment_StandardIndex N on N.AssessmentId = '{1}' and M.OrganizationID = N.OrganizationID and M.VariableId = N.ObjectId
                                order by M.Type, M.OrganizationID, M.ProcessLevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId, myAssessmentId);
            try
            {
                DataTable m_Result = myDataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetComprehensiveDetail(string myOrganizationId, string myAssessmentId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select C.*, '{1}' as AssessmentId, D.StandardIndex from 
	                            (Select (case when A.[Type] = '熟料' then 'clinker' else 'cementmill' end) as VariableId
									    ,A.Name as ProductionLineName
									    ,(case when A.[Type] = '熟料' then '熟料' else '水泥' end) as Name
									    ,A.OrganizationID
									    ,A.LevelCode as ProductionLineLevelCode
	                            from system_Organization A, system_Organization B
	                            where B.OrganizationID = '{0}'
	                            and A.LevelCode like B.LevelCode + '%' 
	                            and A.LevelType = 'ProductionLine'
	                            and A.[Type] in ('熟料','水泥磨')) C
                            left join assessment_StandardIndex D on D.AssessmentId = '{1}' and C.OrganizationID = D.OrganizationID and C.VariableId = D.ObjectId
                            order by C.ProductionLineLevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId, myAssessmentId);
            try
            {
                DataTable m_Result = myDataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetEquipmentDetail(string myOrganizationId, string myAssessmentId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select M.*, '{1}' as AssessmentId, N.StandardIndex from 
                                (SELECT A.EquipmentId as VariableId
                                        ,D.Name as ProductionLineName
                                        ,A.EquipmentName as Name
	                                    ,C.OrganizationID
	                                    ,D.LevelCode as ProductionLineLevelCode
								        ,D.Type
                                    FROM equipment_EquipmentDetail A, equipment_EquipmentCommonInfo B, system_MasterMachineDescription C, system_Organization D, system_Organization E
                                    where A.Enabled = 1
                                    and A.EquipmentCommonId = B.EquipmentCommonId
                                    and A.EquipmentId = C.id
                                    and C.OrganizationID = D.OrganizationID
                                    and E.OrganizationID = '{0}'
                                    and D.LevelCode like E.LevelCode + '%') M
                                left join assessment_StandardIndex N on N.AssessmentId = '{1}' and M.OrganizationID = N.OrganizationID and M.VariableId = N.ObjectId
                                    order by M.Type, M.OrganizationID, M.ProductionLineLevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId, myAssessmentId);
            try
            {
                DataTable m_Result = myDataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
    }
}
