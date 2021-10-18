<%

Function CyganLogin(sUser)

 If sUser = "" Then sUser = Session("CyganUser") 

 Session("CyganUser") = sUser
 Session("CyganPriv") = 0
 Session("CyganAllow") = 0
 Session("CyganLimit") = 0

 Select Case sUser
	Case "jamcito"
		Session("CyganPriv") = 1
		Session("CyganAllow") = 1
	Case "ptaszek"
		Session("CyganPriv") = 1
		Session("CyganAllow") = 1
	Case "muzykameza"
		Session("CyganAllow") = 1
	Case "janstaw"
		Session("CyganAllow") = 1
	Case "ka-gru"
		Session("CyganAllow") = 1
	Case "ms-test"
		Session("CyganAllow") = 1
		Session("CyganLimit") = 1
 End Select

 If Session("CyganAllow") = 0 Then
	sRes = "Brak uprawnien"
 Else
	sRes = "Masz prawo"
	If Session("CyganPriv") = 1 Then
		sRes = sRes & " (wraz z rodzinnymi)"
	End If
 End If

 CyganLogin = sRes
End Function

%>