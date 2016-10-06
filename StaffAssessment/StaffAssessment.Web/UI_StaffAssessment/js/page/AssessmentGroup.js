//var mOrganizationId = "";
var IsAdd = true;
var mName = "";
var mStatisticalcycle = "";
var mCreator = "";
var mRemark = "";
var mNameMader = "";
var mGroupId = "";
$(document).ready(function () {
    LoadMainDataGrid("first");
    Query();
});
//$(function () {
//    LoadMainDataGrid("first");
//});
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').datagrid({
            columns: [[
             //   { field: 'GroupId', title: '标识符 ', width: '10%' },
                { field: 'Name', title: '分组名称', width: '100' },
                {
                    field: 'StatisticalCycle', title: '统计周期', width: '75', formatter: function (value, row, index) {
                        if (value=='day') {
                            return value = '日';
                        } else if (value == 'month') {
                            return value = '月';
                        } else if (value == 'year') {
                            return value = '年';
                        }
                    }
                },
                { field: 'Remark', title: '备注', width: '120' },
                { field: 'Creator', title: '编辑人', width: '60' },
                { field: 'CreateTime', title: '编辑时间', width: '120' },
                {
                    field: 'edit', title: '编辑', width: '65', formatter: function (value, row, index) {
                        var str = "";
                        str = '<a href="#" onclick="editWorkingDefine(true,\'' + row.GroupId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_edit.png" title="编辑页面" onclick="editWorkingDefine(true,\'' + row.GroupId + '\')"/>编辑</a>';
                        // str = str + '<a href="#" onclick="deleteWorkingDefine(\'' + row.GroupId + '\')"><img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面"  onclick="deleteWorkingDefine(\'' + row.GroupId + '\')"/>删除</a>';
                        //str = str + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除页面" onclick="DeletePageFun(\'' + row.id + '\');"/>删除';
                        return str;
                    }
                }
            ]],
            fit: true,
            toolbar: "#toorBar",
            idField: 'GroupId',
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        });
    }
    else {
        $('#grid_Main').datagrid('loadData', myData);//loadData:加载本地数据 旧的数据会被替代
    }
}
function Query() {
    $.ajax({
        type: "POST",
        url: "AssessmentGroup.aspx/GetQueryData",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            //mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGrid("last", []);
                $.messager.alert('提示', "没有查询的数据");
            } else {
                LoadMainDataGrid("last", myData);
            }
        },
        error: function () {
            $("#grid_Main").datagrid('loadData', []);
            $.messager.alert('失败', '获取数据失败');
        }
    });
}
function addFun() {     //增加
    editWorkingDefine(false);
}
function refresh() {     //刷新
    Query();
}
function deleteFun() {     //删除
    deleteItem();
}

function save() {

    mName = $('#name').textbox('getText');
    mStatisticalcycle = $('#statisticalcycle').combobox('getValue');
   // mCreator = $('#creator').textbox('getText');
    mRemark = $('#remark').textbox('getText');
    if (mName == "" || mStatisticalcycle == "")
        $.messager.alert('提示', '请填写未填项!');
    else {
        var mUrl = "";
        var mdata = "";
        if (IsAdd == true) {
            mUrl = "AssessmentGroup.aspx/AddWorkingSection";
            mdata = "{mName:'" + mName + "',mStatisticalcycle:'" + mStatisticalcycle  + "',mRemark:'" + mRemark + "'}";
        }
        else {
            mUrl = "AssessmentGroup.aspx/EditWorkingSection";
            mdata = "{mName:'" + mName + "',mStatisticalcycle:'" + mStatisticalcycle  + "',mRemark:'" + mRemark + "',mGroupId:'" + mGroupId + "'}";
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
                    if (IsAdd) {
                        $.messager.alert('提示', '添加成功！');
                    } else {
                        $.messager.alert('提示', '修改成功！');
                    }                 
                    $('#AddandEditor').window('close');
                    refresh()
                }
                else {
                    $.messager.alert('提示', '添加失败！');
                    refresh()
                }
            }
        });
    }
}
function editWorkingDefine(IsEdit, editContrastId) {

    if (IsEdit) {
        IsAdd = false;
        $('#grid_Main').datagrid('selectRecord', editContrastId);
        var data = $('#grid_Main').datagrid('getSelected');
        $('#name').textbox('setText', data.Name);
        mName = data.Name;
        $('#statisticalcycle').combobox('setValue', data.StatisticalCycle);
        mStatisticalcycle = data.StatisticalCycle;
        $('#creator').textbox('setText', data.Creator);
        mCreator = data.Creator;
        $('#remark').textbox('setText', data.Remark);
        mRemark = data.Remark;
        mGroupId = data.GroupId;
    }
    else {
        IsAdd = true;
        $('#name').textbox('setValue', '');
        // mName = "";
        $('#statisticalcycle').combobox('setValue', '');
        // mStatisticalcycle = "";
        $('#creator').textbox('setValue', '');
        //mCreator = "";
        //$('#creattime').textbox('setValue', '');
        $('#remark').textbox('setValue', '');
        //mRemark = "";
    }
    $('#AddandEditor').window('open');
}

function deleteItem() {
    var row = $("#grid_Main").datagrid('getSelected');
    if (row == null) {
        alert('请选中一行数据！');
    }
    else {
        var index = $("#grid_Main").datagrid('getRowIndex', row);
        //$.messager.defaults = { ok: "是", cancel: "否" };
        $.messager.confirm('提示', '确定要删除选中行？', function (r) {
            if (r) {
                $('#grid_Main').datagrid('deleteRow', index);
                deleteWorkingDefine(row['CreateTime']);
            }
        });
    }
}
function deleteWorkingDefine(time) {

    $.ajax({
        type: "POST",
        url: "AssessmentGroup.aspx/deleteWorkingSection",
        data: "{mCreateTime:'" + time + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (msg) {
            var myData = msg.d;
            if (msg.d == '1') {
                $.messager.alert('提示',"删除成功！");
            }
            else {
                $.messager.alert('提示',"删除失败！");
            }
        }
    });
}
