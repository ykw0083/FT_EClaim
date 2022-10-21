<%@ Page Language="C#" AutoEventWireup="true" Inherits="Default" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Default.aspx.cs" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v21.2, Version=21.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.ExpressApp.Web.Templates" TagPrefix="cc3" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v21.2, Version=21.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Main Page</title>
    <meta http-equiv="Expires" content="0" />

<%--     <style>  
        .Layout {
            font-size: 12px !important;
            font-family: 'Times New Roman' !important;
        }        
        .dxgvTable_XafTheme  
        {  
            font-size: 12px !important;  /* font size tables */  
            font-family: 'Times New Roman' !important;
        }
        .xafAlignCenter {  
            width: auto !important;  
            padding-left: 1px !important;  
            padding-right: 1px !important;  
        }  
            .xafContent {  
            width: 200px !important;  /* Width of the navbar content*/  
            margin-right:3px !important;             
            }  
        .sizeLimit {  
            max-width: 1240px;  /*set the width whole page*/                

        }  
    </style>--%>

</head>
<body class="VerticalTemplate">
    <form id="form2" runat="server">
    <cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
    <div runat="server" id="Content" />
   </form>

</body>
</html>
