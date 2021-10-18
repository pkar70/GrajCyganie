<%
Dim objConn
Dim objConnRW

Function base64_decode( strIn )
	For n = 1 To Len( strIn ) Step 4
		w1 = mimedecode( Mid( strIn, n, 1 ) )
		w2 = mimedecode( Mid( strIn, n + 1, 1 ) )
		w3 = mimedecode( Mid( strIn, n + 2, 1 ) )
		w4 = mimedecode( Mid( strIn, n + 3, 1 ) )
		If w2 >= 0 Then _
			strOut = strOut + _
				Chr( ( ( w1 * 4 + Int( w2 / 16 ) ) And 255 ) )
		If w3 >= 0 Then _
			strOut = strOut + _
				Chr( ( ( w2 * 16 + Int( w3 / 4 ) ) And 255 ) )
		If w4 >= 0 Then _
			strOut = strOut + _
				Chr( ( ( w3 * 64 + w4 ) And 255 ) )
	Next
	base64_decode = strOut
End Function

Function mimedecode( strIn )
	Base64Chars =	"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
	If Len( strIn ) = 0 Then 
		mimedecode = -1 : Exit Function
	Else
		mimedecode = InStr( Base64Chars, strIn ) - 1
	End If
End Function

Function base64_encode( strIn )
	For n = 1 To Len( strIn ) Step 3
		c1 = Asc( Mid( strIn, n, 1 ) )
		c2 = Asc( Mid( strIn, n + 1, 1 ) + Chr(0) )
		c3 = Asc( Mid( strIn, n + 2, 1 ) + Chr(0) )
		w1 = Int( c1 / 4 ) : w2 = ( c1 And 3 ) * 16 + Int( c2 / 16 )
		If Len( strIn ) >= n + 1 Then 
			w3 = ( c2 And 15 ) * 4 + Int( c3 / 64 ) 
		Else 
			w3 = -1
		End If
		If Len( strIn ) >= n + 2 Then 
			w4 = c3 And 63 
		Else 
			w4 = -1
		End If
		strOut = strOut + mimeencode( w1 ) + mimeencode( w2 ) + mimeencode( w3 ) + mimeencode( w4 )
	Next
	base64_encode = strOut
End Function

Function mimeencode( intIn )
	Base64Chars =	"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
	If intIn >= 0 Then 
		mimeencode = Mid( Base64Chars, intIn + 1, 1 ) 
	Else 
		mimeencode = ""
	End If
End Function	

Sub DrukujBigNum(iNum)
 If iNum = "" Then
	Response.Write "0"
	Exit Sub
 End If

 sNum = CStr(iNum)
 If Len(sNum) > 4 Then
	sNum = Left(sNum,Len(sNum)-3) & " " & Right(sNum,3)
 End If
 If Len(sNum) > 7 Then
	sNum = Left(sNum,Len(sNum)-7) & " " & Right(sNum,7)
 End If

 If Len(sNum) > 10 Then
	sNum = Left(sNum,Len(sNum)-11) & " " & Right(sNum,11)
 End If

 If Len(sNum) > 14 Then
	sNum = Left(sNum,Len(sNum)-15) & " " & Right(sNum,15)
 End If


 Response.Write sNum
End Sub

Function ConvertMask(sMask)
 sTmp = sMask
 sTmp = ConvertMaskPrefix(sTmp)
 response.write "<!-- mask: " & sTmp & " -->"
 sTmp = ConvertMaskSufix(sTmp)
 response.write "<!-- mask: " & sTmp & " -->"
 ConvertMask = sTmp  
End Function

Function ConvertMaskPrefix(sMask)
 sTmp = sMask
 If Len(sMask) > 1 Then
 If Left(sTmp,1) <> "*" Then
	If Left(sTmp,1) = "^" Then
		sTmp = Mid(sTmp,2)
	Else
		sTmp = "*" & sTmp
	End If
 End If
 End If
 ConvertMaskPrefix = sTmp  
End Function

Function ConvertMaskSufix(sMask)
 sTmp = sMask
 If Len(sMask) > 1 Then
 If Right(sTmp,1) <> "*" Then
	If Right(sTmp,1) = "$" Then
		sTmp = Left(sTmp,Len(sTmp)-1)
	Else
		sTmp = sTmp & "*"
	End If
 End If
 End If
 ConvertMaskSufix = sTmp  
End Function


Sub StartConn()

  If IsObject(Application("objConnFiles")) And Request("resetconn") <> 1 Then
  	Set objConn = Application("objConnFiles")
  Else
  	Set objConn = Server.CreateObject("ADODB.Connection")
  	Set Application("objConnFiles") = objConn
  End If

 If objConn.State = 0 Then
	Server.ScriptTimeout = 180
	objConn.CommandTimeout = 180
'	objConn.Open "dsn=PKARweb;uid='PKARweb';pwd='DajPrzeczytac!'"
'	objConn.Open "Provider=SQLOLEDB;Data Source=gorc;Initial Catalog=PKARweb;uid='PKARweb';pwd='DajPrzeczytac!'"
	objConn.Open "Provider=SQLOLEDB.1;Data Source=zawrat;Initial Catalog=PKARweb;user id ='PKARweb';password='DajPrzeczytac!'"
 End If
End Sub

Sub StartConnRW()
  If IsObject(Application("objConnFilesRW")) Then
  	Set objConnRW = Application("objConnFilesRW")
  Else
  	Set objConnRW = Server.CreateObject("ADODB.Connection")
  	Set Application("objConnFilesRW") = objConnRW
  End If

 If objConnRW.State = 0 Then
	Server.ScriptTimeout = 180
	objConnRW.CommandTimeout = 180
	objConnRW.Open "Provider=SQLOLEDB.1;Data Source=zawrat;Initial Catalog=PKARweb;user id ='PKARwebRW';password='DajPopisac!'"
 End If
End Sub

Function DePolitek(sTxtIn)
' dla fotosy.asp, tam jest najpierw LCase()
 sKto = sTxtIn
 sKto = Replace(sKto,"ą","a")
 sKto = Replace(sKto,"ć","c")
 sKto = Replace(sKto,"ę","e")
 sKto = Replace(sKto,"ł","l")
 sKto = Replace(sKto,"ń","n")
 sKto = Replace(sKto,"ó","o")
 sKto = Replace(sKto,"ś","s")
 sKto = Replace(sKto,"ż","z")
 sKto = Replace(sKto,"ź","z")
 DePolitek = sKto
End Function


Function Sec2PlayTime(iSec)
 sTmp = ""
 If iSec > 60*60*24 Then
	sTmp = sTmp & iSec\(60*60*24) & "d "
	iSec = iSec Mod (60*60*24)
 End If
 If iSec > 60*60 Then
	sTmp = sTmp & iSec\(60*60) & ":"
	iSec = iSec Mod (60*60)
 End If
 If iSec\60 < 10 And Len(sTmp) > 1 Then sTmp = sTmp & "0"
 sTmp = sTmp & iSec\60 & ":"
 iSec = iSec Mod 60
 If iSec < 10 And Len(sTmp) > 1 Then sTmp = sTmp & "0"
 Sec2PlayTime = sTmp & iSec
End Function

%>