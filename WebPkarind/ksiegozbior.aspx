<html>

<head>
<meta http-equiv="Content-Type" content="text/html; charset=windows-1250">
<meta http-equiv="Content-Language" content="pl">
<meta name="viewport" content="width=device-width, initial-scale=1.0"> 
<title>Biblioteczka</title>
</head>
<%@Language=VBScript
@codepage=1250
%>
<!--#INCLUDE FILE="./base64encdec.inc" -->

<!-- <body bgcolor="#263D66" link="#FF0000" vlink="#0000FF" alink="#FFFF00" topmargin="0" style="color: #FFFFFF; font-family:Arial; font-size:10pt"> -->
<%
 If InStr(Request.ServerVariables("HTTP_USER_AGENT"), "Phone") > 0 Then
	iSize=15
	iIconSize=50
 Else
	iSize=10
	iIconSize=15
 End If
%>
<body bgcolor="#263D66" link="#FFFFFF" vlink="#FFFF00" alink="#FFFFFF" topmargin="0" style="color: #FFFFFF; font-family:Arial; font-size:<%=iSize%>pt">
<h1>Biblioteczka</h1>


<p>
<form action=ksiegozbior.asp>
<table border=1 style="font-size:<%=iSize%>pt">
<tr>
	<th>Name 
	<th>Date
	<th>Len
	<th>Path
</tr>
<tr>
<th><input type="text" name="name" value="<%=Request("name")%>">
<th>&nbsp;
<th><input type="text" name="minLen" value="<%=Request("minLen")%>">
<th><input type="text" name="path" value="<%=Request("path")%>">
</tr>

<tr>
	<td>
	Sort: <select name="ordering">
		<option>name</option>
		<option>path</option>
		<option>date</option>
		<option>len</option>
	</select>
	<td><input type="submit" value="Szukaj!">
	<td>&nbsp;
	<td>&nbsp;
</tr>

<!--<p><a href=buksy.asp>Browser wirtualnych dirow</a></p>-->
<%
 StartConn()

 sQry = ""
 If Len(Request("name")) > 3 Then
	sTmp = Request("name")
	sTmp = ConvertMask(sTmp)
	sTmp = Replace(sTmp,"'","''") 
	sTmp = Replace(sTmp,"*","%") 
	sTmp = Replace(sTmp,"?","_") 

	sQry = "name LIKE '" & sTmp & "' "
 End If	

 If Len(Request("path")) > 1 Then
	sTmp = Request("path")
	sTmp = ConvertMask(sTmp)
	sTmp = Replace(sTmp,"'","''") 
	sTmp = Replace(sTmp,"*","%") 
	sTmp = Replace(sTmp,"?","_") 

	If sQry <> "" Then sQry = sQry & " AND "
	sQry = sQry & "path LIKE '" & sTmp & "' "
 End If	

 sLimit = " AND path LIKE '%\Texts%'"

 iCnt = 0

 If Len(sQry) > 1 Then
	sQry = "SELECT TOP 100 path, name, filedate, len FROM StoreFiles WHERE del=0 AND " & sQry & sLimit
	
	Response.Write "<tr><td colspan=4><small>Running query: " & sQry & "</small></tr>"

	Set oRs = objConn.Execute(sQry)


	Do While Not oRs.EOF
		Response.Write "<tr>"

		sLink=oRs("path")
		sLink=Replace(Mid(oRs("path"),4),"\","/")
		sLink=Replace(sLink," ","%20")
		sName=oRs("name")
		sName=Replace(sName," ","%20")

		If oRs("len") <> "0" Then
			Response.Write "<td><a href=""../store/" & sLink & "/" & sName & """>" & oRs("name") & "</a>"
			Response.Write "</td>"
		Else
			Response.Write "<td>" & oRs("name") & "</td>"
		End If

	        Response.Write "<td>" & oRs("filedate") & "</td>"
	        Response.Write "<td align=right>" & oRs("len") & "</td>"

		sTmp = oRs("path")
		iInd = InStr(sTmp, "\Texts")
		If iInd > 0 Then sTmp = "." & Mid(sTmp,iInd)
	        Response.Write "<td>" & sTmp & "</td>"


	       Response.Write "</tr>" & vbCrLf

		oRs.MoveNext
		iCnt = iCnt + 1
	Loop
	oRs.Close

	If iCnt > 99 Then
%>
<tr><td><font color=red>(i tak dalej)</font><td><td><td></tr>
<%
	End If
 Else
%>
<tr><td><font color=red>(za krotkie)</font><td><td><td></tr>
<%
	
 End If

%> 
</table>

<p>Global books stat:
<%
 Set oRs = objConn.Execute("SELECT COUNT(*) FROM StoreFiles WHERE del=0" & sLimit)
 DrukujBigNum oRs(0)
 Response.Write " files, "
 oRs.Close

 Set oRs = objConn.Execute("SELECT SUM(len) FROM StoreFiles WHERE del=0" & sLimit)
 DrukujBigNum oRs(0)
 Response.Write " bytes"
 oRs.Close

%> 
</body>

</html>