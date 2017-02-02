<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Team7ADProjectMVC.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link rel="stylesheet" href="~/Resources/mystyle.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <style type="text/css">
        body{
             background-image:url("Resources/bbb.jpg");
               background-size: 100% 160%;
        }
    
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="t7-right">
    
        <br />
        <br />
        <br />
        <br />
        <h1 style="font-size:60px" class="w3-text-shadow t7-white t7-text-indigo t7-center">&nbsp;Logic University <br />SSIS</h1><br />
        <br />
         <br />
        <br />
        <br />
          <br />
        <br />
         
         <br />
        <br />              
            <asp:Login ID="Login1" runat="server"  DestinationPageUrl="Auth" CssClass="t7-white" Height="286px" Width="579px">
            <LayoutTemplate>
                <center>
                <table cellpadding="1" cellspacing="0" style="border-collapse:collapse;">
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="1">
                               
                                <tr>
                                    <td align="Left" class="auto-style1">
                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"  CssClass="t7-text-black t7-large" Width="122px">User ID:</asp:Label>
                                    </td>
                                    <td>

                                        <asp:TextBox ID="UserName" runat="server" Class="form-control" Width="400px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="Left" class="auto-style1">
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="t7-large">Password:</asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="Password" runat="server" Class="form-control" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                      
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2" style="color:Red;">
                                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" colspan="2">
                                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" CssClass="t7-btn  t7-blue" ValidationGroup="Login1" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </LayoutTemplate>
        </asp:Login>
          

        </center>
    </div>
    </form>

</body>
</html>
