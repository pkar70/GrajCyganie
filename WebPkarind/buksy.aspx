<html>

<head>
<meta http-equiv="Content-Type" content="text/html; charset=windows-1250">
<meta http-equiv="Content-Language" content="pl">
<meta name="viewport" content="width=device-width, initial-scale=1.0"> 
<title>Szukanie wlasnych plikow</title>
</head>
<%@Language=VBScript
@codepage=1250
%>
<!--#INCLUDE FILE="./base64encdec.inc" -->

<body bgcolor="#263D66" link="#FFFFFF" vlink="#FFFF00" alink="#FFFFFF" topmargin="0" style="color: #FFFFFF; font-family:Arial; font-size:<%=iSize%>pt">
<h1>Browser</h1>

<%
 sRoot = "u:\public\myproduction\texts\texts.0[0-9][0-9]%"
 sPath = Request("dir")
 sLike = "path LIKE '" & sRoot & sPath & "' AND path NOT LIKE '" & sRoot & sPath & "\%'" 

%>
<table>
<%
 StartConn()

 sQry = "SELECT DISTINCT name FROM StoreFiles WHERE isDir=1 AND del=0 AND " & sLike & " ORDER BY name"

' Response.Write "<tr><td colspan=3>sDir=" & sDir & "</tr>"
' Response.Write "<tr><td colspan=3>sQry=" & sQry & "</tr>"
 Set oRs = objConn.Execute(sQry)

' If sBase <> "" Then Response.Write "<tr><td><b><a href=buksy.asp?dir=.>..</a></b><td><td></tr>" & vbCrLf

 Do While Not oRs.EOF
	Response.Write "<tr><td><b><a href=""buksy.asp?dir=" & sPath & "\" & oRs("name") & """>" & oRs("name") & "</a></b><td><td>"
	Response.Write "</tr>" & vbCrLf
	oRs.MoveNext
 Loop

 oRs.Close

 Response.Write "<tr><td colspan=3><hr></tr>"
 sQry = "SELECT name,filedate,len,path FROM StoreFiles WHERE isDir=0 AND del=0 AND " & sLike & " ORDER BY name"
' Response.Write "<tr><td colspan=3>sDir=" & sDir & "</tr>"
' Response.Write "<tr><td colspan=3>sQry=" & sQry & "</tr>"

 Set oRs = objConn.Execute(sQry)

 Do While Not oRs.EOF
	If oRs("name") <> "webdir.htm" Then
		Response.Write "<tr><td><a href='/p/store/" & Replace(Mid(oRs("path"),4),"\","/") & "/" & oRs("name") & "'>" & oRs("name") & "</a>"
		Response.Write "<td align=right>"
		DrukujBigNum oRs("len")
		Response.Write " bytes </td>"
        	Response.Write "<td align=right width=150>"
		iInd = InStrRev(oRs("filedate"),":")
		If iInd > 1 Then Response.Write Left(oRs("filedate"),iInd-1) & "</td>"
		Response.Write "</tr>" & vbCrLf
	End If
	oRs.MoveNext
 Loop

 oRs.Close

%>
</table>
</body>

</html>