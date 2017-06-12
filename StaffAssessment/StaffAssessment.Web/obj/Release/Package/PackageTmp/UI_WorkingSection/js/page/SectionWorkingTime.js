
var mOrganizationId = "";
var mShiftDescriptionID = "";
var mWorkingSectionID = "";
var mProductionName = "";  //产线名称

var mWorkingSection = "";
var mShifts = "";
var mStartTime = "";
var mEndTime = "";
var mRemark = "";  //备注

$(document).ready(function () {
    LoadMainDataGrid("first");
    //LoadWorkingSection();
});

function onOrganisationTreeClick(node) {
    $('#organizationName').textbox('setText', node.text);
    mOrganizationId = node.OrganizationId;
    LoadWorkingSection(mOrganizationId);
    LoadEditWorkingSection(mOrganizationId);
}
var WorkingSectionID = "";
function LoadWorkingSection(mValue) {
    $.ajax({
        type: "POST",
        url: "SectionWorkingTime.aspx/GetWorkingSection",
        data: " {mOrganizationID:'" + mValue + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            var comboboxData = new Array();
            comboboxData[0]={ "WorkingSectionID": "0", "WorkingSectionName": "全部" };
            for(i=1;i<myData.rows.length+1;i++){
                comboboxData[i]=myData.rows[i-1];            
            }
            $('#section').combobox({
                valueField: 'WorkingSectionID',
                textField: 'WorkingSectionName',
                panelHeight: '300',
                data: comboboxData,              
                onSelect: function (record) {
                    WorkingSectionID = record.WorkingSectionID;
                }
            });         
        },
        error: function () {
            $.messager.alert('失败', '加载失败！');
        }
    });
}
function LoadEditWorkingSection(mValue){
    $.ajax({
        type: "POST",
        url: "SectionWorkingTime.aspx/GetWorkingSection",
        data: " {mOrganizationID:'" + mValue + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            $('#workingSection').combobox({
                valueField: 'WorkingSectionID',
                textField: 'WorkingSectionName',
                panelHeight: '300',
                data: myData.rows,
                onSelect: function (record) {
                    mWorkingSectionID = record.WorkingSectionID;
                }
            });         
        },
        error: function () {
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });
}
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
                    { field: 'WorkingSectionName', title: '岗位名称', width: 120 },
                   // { field: 'OrganizationName', title: '所在产线', width: 100 },
                    { field: 'Shifts', title: '班次', width: 60 },
                    { field: 'StartTime', title: '开始时间', width: 80 },
                    { field: 'EndTime', title: '结束时间', width: 80 },
                    { field: 'Remark', title: '备注', width: 140 },
                    {
                        field: 'edit', title: '编辑', width: 150, formatter: function (value, row, index) {
                            var str = "";
                            str = '<a href="#" onclick="editFun(true,\'' + row.ShiftDescriptionID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" style="border:none;" title="编辑页面" onclick="editFun(true,\'' + row.ShiftDescriptionID + '\')"/>编辑</a>';
                            str = str + '<a href="#" onclick="deleteFun(\'' + row.ShiftDescriptionID + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" style="border:none;" title="删除页面"  onclick="deleteFun(\'' + row.ShiftDescriptionID + '\')"/>删除</a>';
                            //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_deleteFun.png" title="删除页面" onclick="deleteFunPageFun(\'' + row.id + '\');"/>删除';
                            return str;
                        }
                    }
            ]],
            fit: true,
            toolbar: "#toorBar",
            idField: 'ShiftDescriptionID',
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

function Query() {
    if (mOrganizationId == "" || mOrganizationId == undefined) {
        $.messager.alert('提示', '请选择组织机构！');
    }
    workingSectionName = $('#section').combobox('getText');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "SectionWorkingTime.aspx/GetQueryData",
        data: " {mOrganizationId:'" + mOrganizationId + "',mWorkingSectionID:'" + WorkingSectionID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGrid("last", []);
                $.messager.alert('提示', "没有查询的数据");
            } else {
                LoadMainDataGrid("last", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function refresh() {
    Query();
}
function addFun() {
    editFun(false);
}

function save()
{
    mWorkingSection = $('#workingSection').combobox('getText');
    //mWorkingSectionID = $('#workingSection').combobox('getValue');
    mShifts = $('#shift').combobox('getText');
    mStartTime = $('#beginTime').timespinner('getValue');
    mEndTime = $('#endTime').timespinner('getValue');
    mRemark = $('#remark').textbox('getText');
    if (mWorkingSection == ""  || mShifts == ""  || mStartTime == "" || mEndTime == "")
    {
        $.messager.alert('提示', '请填写未填项!');
    } 
    else {
        var mUrl = "";
        var mdata = "";
        if (IsAdd) {
            mUrl = "SectionWorkingTime.aspx/AddSectionWorkingDefine";
            mdata = "{mWorkingSectionID:'" + mWorkingSectionID + "',mShifts:'" + mShifts + "',mStartTime:'" + mStartTime + "',mEndTime:'" + mEndTime + "',mRemark:'" + mRemark + "'}";
        } else if (IsAdd == false) {
            mUrl = "SectionWorkingTime.aspx/EditSectionWorkingDefine";
            mdata = "{mShiftDescriptionID:'" + mShiftDescriptionID + "',mWorkingSectionID:'" + mWorkingSectionID + "',mShifts:'" + mShifts + "',mStartTime:'" + mStartTime + "',mEndTime:'" + mEndTime + "',mRemark:'" + mRemark + "'}";
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
                refresh();
            }
        });
    }
}
function editFun(IsEdit, editContrastId) {
    if (IsEdit) {
        IsAdd = false;
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        $('#workingSection').combobox('clear');
        $('#shift').combobox('clear');
        $('#shift').combobox('setText', '');
        $('#workingSection').combogrid('setText', '');
        //$('#workingSection').combobox('clear');
        //$('#shift').combobox('clear');
        $('#workingSection').combobox('setValue', data.WorkingSectionID);
        //$('#productionName').textbox('setText', data.OrganizationName);
        
        $('#shift').combobox('setText', data.Shifts);
        $('#beginTime').timespinner('setValue', data.StartTime);
        $('#endTime').timespinner('setValue', data.EndTime);
        $('#remark').textbox('setText', data.Remark);
      
        mShiftDescriptionID = data.ShiftDescriptionID;
        mWorkingSectionID = data.WorkingSectionID;

    }
    else {
        IsAdd = true;
        $('#workingSection').combobox('clear');
        $('#shift').combobox('clear');

        $('#workingSection').combogrid('setText', '');
        $('#productionName').textbox('setText', '');

        $('#shift').combobox('setText', '');
        $('#beginTime').timespinner('setValue', '00:00');
        $('#endTime').timespinner('setValue','12:00');
        $('#remark').textbox('setText','');

        if (mOrganizationId == "" && mOrganizationId == undefined) {
            $.messager.alert('提示', '请选择组织机构！');
        }
    }
    $('#AddandEditor').window('open');
}

function deleteFun(deleteFunContrastId) {
    $.messager.confirm('提示', '确定要删除吗？', function (r) {
        if (r) {
            $('#grid_Main').datagrid('selectRecord', deleteFunContrastId);
            var data = $('#grid_Main').datagrid('getSelected');

            mShiftDescriptionID = data.ShiftDescriptionID;
    
            $.ajax({
                type: "POST",
                url: "SectionWorkingTime.aspx/deleteFunSectionWorkingDefine",
                data: "{mShiftDescriptionID:'" + mShiftDescriptionID + "'}",
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
        }})
}