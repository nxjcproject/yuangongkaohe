using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
namespace StaffAssessment
{
    public class Model_CalculateObjects
    {
        private string _StructureName;      //表示结构,tree结构还是grid结构
        private string _StructureType;      //表结构的类型,不同目录对应相同的计算对象集,相同对象的表结构归为一类
        private DataTable _CalculateObjectsTable;   //对象集表
        public Model_CalculateObjects()
        {
            _StructureName = "";
            _StructureType = "";
            _CalculateObjectsTable = new DataTable();
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
        public DataTable CalculateObjectsTable
        {
            get
            {
                return _CalculateObjectsTable;
            }
            set
            {
                _CalculateObjectsTable = value;
            }
        }
    }
    public class Table_CalculateObjects
    {
        public static Model_CalculateObjects GetCalculateObjects(string myType, string myValueType, string myOrganizationId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            Model_CalculateObjects m_CalculateObjects = new Model_CalculateObjects();
            if (myType == "MaterialWeight")
            {
                m_CalculateObjects.StructureName = "grid";
                m_CalculateObjects.StructureType = "MaterialDetail";
                m_CalculateObjects.CalculateObjectsTable = GetMaterialDetail(myOrganizationId, myDataFactory);
            }
            else if (myType == "Energy")
            {
                m_CalculateObjects.StructureName = "tree";
                m_CalculateObjects.StructureType = "FormulaDetail";
                m_CalculateObjects.CalculateObjectsTable = GetProductionProcess(myOrganizationId, myDataFactory);
            }
            else if (myType == "Comprehensive")
            {
                m_CalculateObjects.StructureName = "grid";
                m_CalculateObjects.StructureType = "ComprehensiveInternal";
                m_CalculateObjects.CalculateObjectsTable = GetComprehensiveDetail(myOrganizationId, myDataFactory);
                    //new DataTable();
                //m_CalculateObjects.CalculateObjectsTable.Columns.Add("VariableId", typeof(string));
                //m_CalculateObjects.CalculateObjectsTable.Columns.Add("Name", typeof(string));
                //m_CalculateObjects.CalculateObjectsTable.Rows.Add("clinker","熟料");
                //m_CalculateObjects.CalculateObjectsTable.Rows.Add("cementmill", "水泥");
            }
            else if (myType == "Production")
            {
                m_CalculateObjects.StructureName = "grid";
                m_CalculateObjects.StructureType = "FormulaDetail";
                m_CalculateObjects.CalculateObjectsTable = GetEquipmentDetail(myOrganizationId, myDataFactory);
            }
            return m_CalculateObjects;
        }
        private static DataTable GetMaterialDetail(string myOrganizationId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT B.VariableId as VariableId
                                  ,C.Name as ProductionLineName
	                              ,B.Name as Name
	                              ,A.OrganizationID
                                  ,C.LevelCode as ProductionLineLevelCode
	                              ,B.Unit
                              FROM tz_Material A, material_MaterialDetail B, system_Organization C, system_Organization D
                              where A.Type = 2
                              and A.Enable = 1
                              and A.State = 0
                              and A.KeyID = B.KeyID
                              and A.OrganizationID = C.OrganizationID
                              and D.OrganizationID = '{0}'
                              and C.LevelCode like D.LevelCode + '%' 
                              order by C.Type, A.OrganizationID";
            m_Sql = string.Format(m_Sql, myOrganizationId);
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
        private static DataTable GetProductionProcess(string myOrganizationId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT B.VariableId as VariableId
                                ,C.Name as ProductionLineName
	                            ,B.Name as Name
	                            ,A.OrganizationID
	                            ,C.LevelCode as ProductionLineLevelCode
	                            ,B.LevelCode as ProcessLevelCode
                              FROM tz_Formula A, formula_FormulaDetail B, system_Organization C, system_Organization D
                              where A.Type = 2
                              and A.Enable = 1
                              and A.State = 0
                              and A.KeyID = B.KeyID
                              and A.OrganizationID = C.OrganizationID
                              and D.OrganizationID = '{0}'
                              and C.LevelCode like D.LevelCode + '%' 
                              order by C.Type, A.OrganizationID, B.levelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId);
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
        private static DataTable GetComprehensiveDetail(string myOrganizationId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"Select (case when A.[Type] = '熟料' then 'clinker' else 'cementmill' end) as VariableId
                                  ,A.Name as ProductionLineName
                                  ,(case when A.[Type] = '熟料' then '熟料' else '水泥' end) as Name
	                              ,A.OrganizationID
	                              ,A.LevelCode as ProductionLineLevelCode
                            from system_Organization A, system_Organization B
                            where B.OrganizationID = '{0}'
                            and A.LevelCode like B.LevelCode + '%' 
                            and A.LevelType = 'ProductionLine'
                            and A.[Type] in ('熟料','水泥磨')
                            order by A.LevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId);
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
        private static DataTable GetEquipmentDetail(string myOrganizationId, SqlServerDataAdapter.ISqlServerDataFactory myDataFactory)
        {
            string m_Sql = @"SELECT A.EquipmentId as VariableId
                                  ,D.Name as ProductionLineName
                                  ,A.EquipmentName as Name
	                              ,C.OrganizationID
	                              ,D.LevelCode as ProductionLineLevelCode
                              FROM equipment_EquipmentDetail A, equipment_EquipmentCommonInfo B, system_MasterMachineDescription C, system_Organization D, system_Organization E
                              where A.Enabled = 1
                              and A.EquipmentCommonId = B.EquipmentCommonId
                              and A.EquipmentId = C.id
                              and C.OrganizationID = D.OrganizationID
                              and E.OrganizationID = '{0}'
                              and D.LevelCode like E.LevelCode + '%'
                              order by B.DisplayIndex, A.DisplayIndex";
            m_Sql = string.Format(m_Sql, myOrganizationId);
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
