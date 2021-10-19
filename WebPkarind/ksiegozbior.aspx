<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ksiegozbior.aspx.vb" Inherits="WebPkarind.ksiegozbior" Async="True" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Biblioteczka</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <!--<meta name="viewport" content="width=device-width, initial-scale=1.0"> -->
</head>
<body bgcolor="#263D66" link="#FFFFFF" vlink="#FFFF00" alink="#FFFFFF" topmargin="0" style="color: #FFFFFF; font-family:Arial; font-size:10pt">
    <h1>Biblioteczka</h1>


    <div runat="server" id="loggedOK">

        <form action="ksiegozbior.aspx">
            <table runat="server" border="1" id="uiResults">
                <tr>
                    <th>Name </th>
                    <th>Date</th>
                    <th>Len</th>
                    <th>Path</th>
                </tr>
                <tr>
                    <th>
                        <input type="text" name="name" value="<%=Request("name")%>" /></th>
                    <th>&nbsp;</th>
                    <th>&nbsp;</th>
                    <th>
                        <input type="text" name="path" value="<%=Request("path")%>" /></th>
                </tr>

                <tr>
                    <td>Sort:
                        <select name="ordering">
                            <option>name</option>
                            <option>path</option>
                            <option>date</option>
                            <option>len</option>
                        </select></td>
                    <td>
                        <input type="submit" value="Szukaj!" /></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </form>
        <!--<p>Global books stat <span runat="server" id="uiTotal">x</span></p>-->
    </div>

    <div runat="server" id="loggedError">
        <p runat="server" id="loginMessage">Seems like logging error</p>
    </div>

    <div runat="server" id="askForLogin">
        <form action="ksiegozbior.aspx">
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

