<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="spisek.aspx.vb" Inherits="WebPkarind.spisek" Async="True" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Indeksy własnego storage</title>
</head>
<body bgcolor="#263D66" link="#FFFFFF" vlink="#FFFF00" alink="#FFFFFF" topmargin="0" style="color: #FFFFFF;">
    <h1>Indeksy własnego storage</h1>

    <div runat="server" id="loggedOK">
        <p>Seems like logged-in</p>
    </div>

    <div runat="server" id="loggedError">
        <p runat="server" id="loginMessage">Seems like logged-in</p>
    </div>

    <div runat="server" id="askForLogin">
        <form action="spisek.aspx">
            <table border="1">
                <tr>
                    <td>UserName:</td>
                    <td>
                        <input type="text" name="user" /></td>
                    <td>
                        <input type="submit" value="login!" /></td>
                </tr>
            </table>
        </form>
    </div>

</body>
</html>
