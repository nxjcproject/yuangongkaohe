$(document).ready(function () {
    LoadAssessmentCatalogue();
    LoadMainDataGrid("first");
    
});
function onOrganisationTreeClick(node) {
    mOrganizationId = node.OrganizationId;
    var mLevel = mOrganizationId.split('_');
    if (mLevel.length != 5) {
        $.messager.alert('提示', '请选择产线级别！');

    } else {
        $('#organizationName').textbox('setText', node.text);
        LoadWorkingSection(mOrganizationId);
        //   LoadEditWorkingSection(mOrganizationId);
        LoadProcessLine(mOrganizationId);
    }
}
var myType = "";
var myValueType = "";
var assment = "";
function LoadAssessmentCatalogue() {
    $.ajax({
        type: "POST",
        url: "IndexConfigure.aspx/GetAssessmentCatalogue",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            var cobdata = myData.rows;
            cobdata.sort();
            $('#eAssessmentObject').combobox({
                panelHeight: '200',
                valueField: 'AssessmentId',
                textField: 'Name',
                data: cobdata,
                onSelect: function (record) {
                    myType = record.Type;
                    myValueType = record.ValueType;
                    assment = record.AssessmentId;
                }
            });
        },
        error: function () {
            $.messager.alert('提示', '加载失败！');
        }
    });
}
function LoadMainDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_Main').treegrid({
            columns: [[
                    {
                        field: 'ProductionLineName', title: '考核元素', width: 300,
                        formatter: function (value, row) {
                            if (row.text == '水泥' || row.text == '熟料') {
                                return row.text;
                            }
                           else if (row.ProductionLineName == row.text) {
                                return row.text;
                            }
                            else {                              
                               return row.ProductionLineName + row.text;
                            }
                        }                       
                    },
                    //{ field: 'text', title: '考核元素', width: 150, align: 'left' },
                    {
                        field: 'StandardIndex', title: '标准指标', width: 60, align: 'left',
                        editor: { type: 'numberbox', options: { precision: 2 } }
                    }

            ]],
            fit: true,
            toolbar: "#toorBar",
            rownumbers: true,
            singleSelect: true,
            idField: "ProcessLevelCode",
            treeField: "ProductionLineName",
            striped: true,
            onContextMenu: onContextMenu,
            onClickRow: onClickRow,
            data: []
        });
    }
    else {
        $('#grid_Main').treegrid('loadData', myData);
    }
}
//function Query() {
//    $.ajax({
//        type: "POST",
//        url: "IndexConfigure.aspx/GetAssessmentObjects",
//        data: "{myType:'" + myType + "',myValueType:'" + myValueType + "',myOrganizationId:'" + mOrganizationId + "'}",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            var myData = jQuery.parseJSON(msg.d);
//            if (myData.total == 0) {
//                LoadMainDataGrid("last", []);
//                $.messager.alert('提示', '没有查询到记录！');
//            } else {
//                LoadMainDataGrid("last", myData);
//            }
//        },
//        error: function () {
//            $("#grid_Main").treegrid('loadData', []);
//            $.messager.alert('失败', '加载失败！');
//        }
//    });
//}
function Query() {
    $.ajax({
        type: "POST",
        url: "IndexConfigure.aspx/GetAssessmentObjects",
        data: "{myOrganizationId:'" + mOrganizationId + "',myAssessmentId:'" + assment + "',myType:'" + myType + "',myValueType:'" + myValueType + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = jQuery.parseJSON(msg.d);
            if (myData.total == 0) {
                LoadMainDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                LoadMainDataGrid("last", myData);
            }
        },
        error: function () {
            $("#grid_Main").treegrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    });
}
function onContextMenu(e, row) {
    e.preventDefault();
    $(this).treegrid('select', row.id);
    $('#MenuId').menu('show', {
        left: e.pageX,
        top: e.pageY
    });
}
var editingId;
function onClickRow(clickRow) {
    if (editingId != undefined) {
        var t = $('#grid_Main');
        t.treegrid('endEdit', editingId);
        editingId = clickRow.id;
        $('#grid_Main').treegrid('beginEdit', editingId);
        return;
    }
    var row = $('#grid_Main').treegrid('getSelected');
    if (row) {
        editingId = row.id
        $('#grid_Main').treegrid('beginEdit', editingId);
    }
}
function endEditing() {
    var t = $('#grid_Main');
    if (editingId == undefined) { return true; }
    if ($("#grid_Main").treegrid('validateRow', editingId)) {
        $("#grid_Main").treegrid('endEdit', editingId);
        editingId = undefined;
        return 'true';
    } else {
        return 'false';
    }
}

function Save() {
    endEditing();
    //var windowData = $("#grid_Main").treegrid('getChanges');
    //var rootData = $("#grid_Main").treegrid('getRoots');
    var childrenData = $("#grid_Main").treegrid('getData');
    var assessmentId = $("#eAssessmentObject").combobox('getValue');
    var length = childrenData.length;
    var stra = JSON.stringify(childrenData);
    $.ajax({
        type: "POST",
        url: "IndexConfigure.aspx/SaveIndex",
        data: '{assessmentId:"' + assessmentId + '",json:\'' + stra + '\'}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "success") {
                $.messager.alert('提示', '保存成功！');
            }

            $('#AddandEditor').window('close');
            //refresh();
        }
    });
}
