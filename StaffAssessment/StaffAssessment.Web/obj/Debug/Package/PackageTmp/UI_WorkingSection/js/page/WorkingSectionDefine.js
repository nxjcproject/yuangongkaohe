
var mOrganizationId = "";
var IsAdd = true;
var mWorkingSectionItemID = "";
var mWorkingSectionID = "";
var mProductionName = "";  //产线名称
var mSectionType = "";  //岗位类型
var mWorkingSection = ""; //岗位
var mEnabed = "";     //可用标志
var mEditor = "";  //编辑人
var mRemark = "";  //备注

$(document).ready(function () {
    LoadMainDataGrid("first");
    LoadSectionType("first");
    LoadSectionTypeDataGrid("first");
});

function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    mOrganizationId = node.OrganizationId;
    LoadproductionName(mOrganizationId);
    LoadSectionTypeData();
    LoadAllSectionTypeData();
}
function LoadproductionName(mValue) {
    $.ajax({
        type: "POST",
        url: "WorkingSectionDefine.aspx/GetProductionNameList",
        data: " {mOrganizationID:'" + mValue + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#productionName').combotree({
                data: myData,
                valueField: 'OrganizationID',
                textField: 'Name',
                panelHeight: 'auto',
                onSelect: function (record) {
                    mProductionName = record.OrganizationID;
                }
            });
           // $('#productionName').combotree('collapseAll');
        },
        error: function () {
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });

}
//var eWorkingSectionID = "";
function LoadSectionType(type, mydata) {
    if (type == "first") {
        $('#sectionType').combobox({
            valueField: 'id',
            textField: 'WorkingSectionType',
            panelHeight: 'auto',
            data: [],
            onSelect: function (record) {             
                if (record.id == 0) {
                    $('#sectionType').combobox('clear');
                    $('#WorkingSectionType').textbox('setText', "");
                    $('#EnabedMark').combobox('setValue', "");
                    $('#AddSectionType').window('open');
                } else {
                    mWorkingSectionID = record.WorkingSectionID;
                }
            },
            formatter: function (rows) {
                if (rows.id == 0) {
                    return '<span style="color:blue">' + rows.WorkingSectionType + '</span>';
                } else {
                    return rows.WorkingSectionType;
                }
            }
        });
    } else {
        $('#sectionType').combobox('loadData', mydata);
    }
}
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
                    { field: 'OrganizationName', title: '所在产线', width: 100 },
                    { field: 'WorkingSectionName', title: '岗位', width: 100 },
                    { field: 'AssessmentCoefficient', title: '考核系数', width: 80 },                   
                    { field: 'Creator', title: '编辑人', width: 80},
                    { field: 'CreatedTime', title: '编辑时间', width: 120 },
                    {
                        field: 'Enabled', title: '启用标志', width: 80, align: "center", formatter: function (value, row, index) {
                            if (value == "True") { return value = "是"; } else { return value = "否"; }
                        }
                    },
                    { field: 'Remarks', title: '备注', width: 80 },
                    {
                        field: 'edit', title: '编辑', width: 150, formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="editWorkingDefine(true,\'' + row.WorkingSectionItemID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editWorkingDefine(true,\'' + row.WorkingSectionItemID + '\')"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteWorkingDefine(\'' + row.WorkingSectionItemID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteWorkingDefine(\'' + row.WorkingSectionItemID + '\')"/>删除</a>';
                            //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面" onclick="DeletePageFun(\'' + row.id + '\');"/>删除';
                            return str;
                        }
                    }
            ]],
            fit: true,
            toolbar: "#toorBar",
            idField: 'WorkingSectionItemID',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);
    }
}
var myOrganizationId = "";
function LoadSectionTypeData() {
    var mLevel=mOrganizationId.split("_");
    if(mLevel.length<=3){
        $.messager.alert('提示', '请选择产线');
    }
    else {
        myOrganizationId = mLevel[0] + "_" + mLevel[1] + "_" + mLevel[2] + "_" + mLevel[3];
        $.ajax({
            type: "POST",
            url: "WorkingSectionDefine.aspx/GetWorkingSectionType",
            data: "{mOrganizationId:'" + myOrganizationId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = jQuery.parseJSON(msg.d);
               // LoadSectionTypeDataGrid("last",myData);
                var mydata = myData.rows;
                mydata.push({ "id": "0", "WorkingSectionID": "", "WorkingSectionType": "添加岗位类别", "OrganizationID": "", "Enabled": "" });
                mydata.sort(function sortNumber(a, b) {
                    return a.id - b.id
                });
                LoadSectionType("last", mydata);
            },
            error: function () {
                $.messager.alert('提示', '岗位类别加载失败！');
            }
        });
 
    }  
}
function LoadAllSectionTypeData() {
    var mLevel=mOrganizationId.split("_");
    if (mLevel.length <= 3) {
        $.messager.alert('提示', '请选择产线');
    }
    else {
        myOrganizationId = mLevel[0] + "_" + mLevel[1] + "_" + mLevel[2] + "_" + mLevel[3];
        $.ajax({
            type: "POST",
            url: "WorkingSectionDefine.aspx/GetAllWorkingSectionType",
            data: "{mOrganizationId:'" + myOrganizationId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var myData = jQuery.parseJSON(msg.d);
                LoadSectionTypeDataGrid("last", myData);
            },
            error: function () {
                $.messager.alert('提示', '岗位类别加载失败！');
            }
        });
    }
}
var mger = Object;
function Query() {
    if (mOrganizationId == "" || mOrganizationId == undefined) {
        $.messager.alert('提示', '请选择组织机构！');
    }
    $.ajax({
        type: "POST",
        url: "WorkingSectionDefine.aspx/GetQueryData",
        data: " {mOrganizationId:'" + mOrganizationId  + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGrid("last", []);
                $.messager.alert('提示', "没有查询的数据");
            } else {
                LoadMainDataGrid("last", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            mger = $.messager.alert('提示', "加载中...");
        },
        error: function () {
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function refresh() {
    Query();
}
function addFun() {
    editWorkingDefine(false);
}
function save() {
   
        //mProductionName = $('#productionName').combotree('getValue');
        mSectionType = $('#sectionType').combobox('getText');
        mWorkingSection = $('#assessmentCoefficient').numberbox('getText');
        var mWorkingSectionName = $('#workingSection').textbox('getText');
        mEnabed = $('#Enabed').combobox('getValue');
        //mEditor = $('#editor').textbox('getValue');
        mRemark = $('#remark').textbox('getValue');
        if (mProductionName == "" || mSectionType == "" || mWorkingSection == "")
            $.messager.alert('提示', '请填写未填项!');
        else {
            var mUrl = "";
            var mdata = "";
            if (IsAdd) {
                mUrl = "WorkingSectionDefine.aspx/AddWorkingSection";
                mdata = "{mWorkingSectionID:'" + mWorkingSectionID + "',mProductionName:'" + mProductionName + "',mSectionType:'" + mSectionType + "',mWorkingSectionName:'" + mWorkingSectionName + "',mWorkingSection:'" + mWorkingSection + "',mEnabed:'" + mEnabed + "',mRemark:'" + mRemark + "'}";
            } else if (IsAdd == false) {
                mUrl = "WorkingSectionDefine.aspx/EditWorkingSection";
                mdata = "{mWorkingSectionItemID:'" + mWorkingSectionItemID + "',mWorkingSectionID:'" + mWorkingSectionID + "',mProductionName:'" + mProductionName + "',mSectionType:'" + mSectionType + "',mWorkingSectionName:'" + mWorkingSectionName + "',mWorkingSection:'" + mWorkingSection + "',mEnabed:'" + mEnabed + "',mRemark:'" + mRemark + "'}";
            }
            $.ajax({
                type: "POST",
                url: mUrl,
                data: mdata,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 1) {
                        $.messager.alert('提示', '操作成功！');
                        $('#AddandEditor').window('close');
                        refresh();
                    }
                    else {
                        $.messager.alert('提示', '操作失败！');
                        refresh();
                    }
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                    $('#AddandEditor').window('close');
                    refresh();
                }
            });
        }
}
function editWorkingDefine(IsEdit, editContrastId) {
    if (IsEdit) {
        IsAdd = false;
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        //mdata = "{mWorkingSectionID:'" + mWorkingSectionID +
        //    "',mProductionName:'" + mProductionName +
        //    "',mSectionType:'" + mSectionType +
        //    "',mWorkingSection:'" + mWorkingSection +
        //    "',mEnabed:'" + mEnabed +
        //    "',mEditor:'" + mEditor +
        //    "',mRemark:'" + mRemark + "'}";

        mWorkingSectionItemID = data.WorkingSectionItemID;
        $('#productionName').combotree('setText', data.OrganizationName);
        mProductionName = data.OrganizationID
        $('#sectionType').combobox('setText', data.Type);
        mSectionType = data.Type;
        mWorkingSectionID = data.WorkingSectionID;
        $('#workingSection').textbox('setText', data.WorkingSectionName);
        $('#assessmentCoefficient').textbox('setText', data.AssessmentCoefficient);
        mWorkingSection = data.WorkingSectionName;
        $('#Enabed').combobox('setValue', data.Enabled);
        mEnabed=data.Enabled;
        $('#editor').textbox('setText', data.Creator);
        mEditor=data.Creator;
         $('#remark').textbox('setText', data.Remarks);
         mRemark = data.Remarks;
        
    }
    else {
        IsAdd = true;
        //"{mProductionName:'" + mProductionName +
        //"',mSectionType:'" + mSectionType +
        //"',mWorkingSection:'" + mWorkingSection +
        //"',mEnabed:'" + mEnabed +
        //"',mEditor:'" + mEditor +
        //"',mRemark:'" + mRemark + "'}";
        mWorkingSectionItemID = "";
        $('#productionName').combotree('setText', '');
        mProductionName = "";
        $('#sectionType').combobox('clear');
        mSectionType = "";
        mWorkingSectionID = "";
        $('#workingSection').textbox('setText', '');
        $('#assessmentCoefficient').textbox('setText', '');
        mWorkingSection = "";
        $('#Enabed').combobox('setValue', 'True');
        mEnabed = "True";
        $('#editor').textbox('setText', '');
        mEditor = "";
        $('#remark').textbox('setText', '');
        mRemark = "";

        if (mOrganizationId == "" && mOrganizationId == undefined) {
            $.messager.alert('提示', '请选择组织机构！');
        }
    }
    $('#AddandEditor').window('open');
}
function deleteWorkingDefine(deleteContrastId) {

    $('#grid_Main').datagrid('selectRecord', deleteContrastId);
    var data = $('#grid_Main').datagrid('getSelected');

    mWorkingSectionItemID = data.WorkingSectionItemID;
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: "WorkingSectionDefine.aspx/deleteWorkingSection",
                data: "{mWorkingSectionItemID:'" + mWorkingSectionItemID + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 1) {
                        $.messager.alert('提示', '删除成功！');
                        $('#AddandEditor').window('close');
                        refresh();
                    }
                    else {
                        $.messager.alert('提示', '操作失败！');
                        refresh();
                    }
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                    $('#AddandEditor').window('close');
                    refresh();
                }
            });
        }
    }); 
}

///////////
//myOrganizationId
var myWorkingSectionID = "";
var myWorkingSectionType="";
var myEnabedMark="";
function saveSectionType() {
    //myOrganizationId
    myWorkingSectionType = $('#WorkingSectionType').textbox('getText');
    myEnabedMark = $('#EnabedMark').combobox('getValue');
    if(myOrganizationId==""){
        $.messager.alert('提示', '请选择产线');
    }else{
        if (myWorkingSectionType == "" || myEnabedMark == "") {
            $.messager.alert('提示', '请填写岗位类别名称');
        } else {
            $.ajax({
                type: "POST",
                url: "WorkingSectionDefine.aspx/InsertWorkingSectionType",
                data: "{mOrganizationId:'" + myOrganizationId + "',mWorkingSectionType:'" + myWorkingSectionType + "',mEnabedMark:'" + myEnabedMark + "',mWorkingSectionID:'" + myWorkingSectionID + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 00) {
                        $.messager.alert('提示', '添加失败！');
                    } else if (myData == 01) {
                        $.messager.alert('提示', '添加成功！');
                    }
                    else if (myData == 10) {
                        $.messager.alert('提示', '更新失败！');
                    } else if (myData == 11) {
                        $.messager.alert('提示', '更新成功！');
                    }
                    LoadAllSectionTypeData();
                    LoadSectionTypeData();
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                }
            });
        }
    }
}
function LoadSectionTypeDataGrid(type,myData) {
    if (type == "first") {
        $('#grid_SectionType').datagrid({
            columns: [[
                    { field: 'WorkingSectionType', title: '岗位类型', width: 120 },
                    {
                        field: 'Enabled', title: '启用标志', width: 80, align: "center", formatter: function (value, row, index) {
                            if (value == "True") { return value = "是"; } else { return value = "否"; }
                        }
                    },
                    {
                        field: 'edit', title: '编辑', width: 110, formatter: function (value, row, index) {
                            var str = "";
                          //  str = '<a href="#" onclick="editSectionType(true,\'' + row.id + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editeditSectionType(true,\'' + row.id + '\')"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteeditSectionType(\'' + row.WorkingSectionID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteeditSectionType(\'' + row.WorkingSectionID + '\')"/>删除</a>';
                            //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面" onclick="DeletePageFun(\'' + row.id + '\');"/>删除';
                            return str;
                        }
                    }
            ]],
            fit: true,
            idField: 'WorkingSectionID',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: [],
            onClickRow: function (index, row) {
                myWorkingSectionID = row.WorkingSectionID;
                $('#WorkingSectionType').textbox('setText', row.WorkingSectionType);
                $('#EnabedMark').combobox('setValue', row.Enabled);
            }
        });
    }
    else {
        $('#grid_SectionType').datagrid('loadData', myData);
    }
}
function deleteeditSectionType(deleteContrastId) {
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: "WorkingSectionDefine.aspx/deleteWorkingSectionType",
                data: "{mWorkingSectionID:'" + deleteContrastId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myData = msg.d;
                    if (myData == 1) {
                        $.messager.alert('提示', '删除成功！');
                    }
                    else {
                        $.messager.alert('提示', '删除失败！');
                    }
                    LoadAllSectionTypeData();
                    LoadSectionTypeData();
                },
                error: function () {
                    $.messager.alert('提示', '操作失败！');
                }
            });
        }
    })
}