<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexConfigure.aspx.cs" Inherits="StaffAssessment.Web.UI_StaffAssessment.IndexConfigure" %>
<%@ Register Src="/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>标准指标配置</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css"/>
	<link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css"/>
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css"/>

	<script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
	<script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script> 
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script> 

    <script type="text/javascript" src="js/page/IndexConfigure.js" charset="utf-8"></script>
</head>
<body>
    <div id="cc" class="easyui-layout"data-options="fit:true,border:false" >    
         <div data-options="region:'west',split:true" style="width: 230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
          <div id="toorBar" title="" style="height:56px;padding:10px;">
            <div>
                <table>
                    <tr>
                         <td>组织机构:</td>
                        <td >                               
                            <input id="organizationName" class="easyui-textbox" readonly="readonly"style="width:80px" />
                            <input id="organizationId" readonly="readonly" style="display: none;" />               
                       </td>                               
                         <td>考核项:</td>
                        <td >                               
                            <input id="eAssessmentObject" class="easyui-combobox" style="width:180px" />               
                        </td>   
                        <%--<td>产线：</td> 
	    			    <td>
                            <input class="easyui-combobox" type="text" id="eProcessLine" style="width:200px" />                  
                        </td>  --%>         
                         <td>
                            <a id="btn" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="Query()">查询</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="Save()">保存</a>
                        </td>
                    </tr>                   
                </table>      
            </div>
	    </div> 
         <div data-options="region:'center'" style="padding:5px;background:#eee;">
             <table id="grid_Main"class="easyui-treegrid"></table>
         </div>
    </div>
 	  
</body>
</html>