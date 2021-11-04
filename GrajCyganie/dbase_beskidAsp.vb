Partial Public Class dbase_beskidAsp
    Inherits dbase_base

    Public Overrides ReadOnly Property Nazwa As String = "BeskidASP"

    Public Overrides Async Function GetPermission(sUser As String) As Task(Of String)
        mbGranted = False
        If sUser = "" Then Return "Zaloguj się"

        Dim sRes As String = Await HttpPageAsync("/cygan-login.asp?user=" & sUser, "login page")
        If sRes.IndexOf("Masz prawo") > -1 Then mbGranted = True

        Return sRes
    End Function

    Protected Overrides Async Function DekadyDownload() As Task(Of Boolean)
        Dim sRes As String = Await HttpPageAsync("/cygan-dekady.asp", "statystyka dekad")

        mlDekady.Clear()

        Dim iTotalCnt As Integer = 0
        Dim iTotalTime As Integer = 0

        Dim aDekady As String() = sRes.Split(vbCrLf)
        Dim oNew As tDekada

        For Each sDekada In aDekady
            Dim aFields As String() = sDekada.Split("|")
            If aFields.GetUpperBound(0) = 2 Then
                oNew = New tDekada
                oNew.sNazwa = aFields(0).Trim
                oNew.iCount = aFields(1)
                iTotalCnt = iTotalCnt + aFields(1)
                oNew.iPlayTime = aFields(2)
                iTotalTime = iTotalTime + aFields(2)
                oNew.sPlayTime = App.TimeSecToString(oNew.iPlayTime)
                mlDekady.Add(oNew)
            End If
        Next

        If mlDekady.Count < 1 Then Return False

        ' total
        oNew = New tDekada
        oNew.sNazwa = "Total"
        oNew.iCount = iTotalCnt
        oNew.iPlayTime = iTotalTime
        oNew.sPlayTime = App.TimeSecToString(iTotalTime)
        mlDekady.Add(oNew)

        Return True
    End Function

    Public Overrides Async Function GetMaxId() As Task(Of Boolean)
        Dim sRes As String = Await HttpPageAsync("/cygan-maxId.asp", "maxno")
        If sRes = "" Then Return False
        SetSettingsInt("maxSoundId", sRes.Trim)

        Return True
    End Function

    Public Overrides Async Function GetCountsy(oGrany As tGranyUtwor) As Task(Of Boolean)
        Dim sPage As String = Await HttpPageAsync("/cygan-counts.asp?id=" & oGrany.oAudioParam.id, "song stats")
        If String.IsNullOrEmpty(sPage) Then Return False

        ExtractCyganCounts(sPage, oGrany) ' uzupełnia count* w oGrany
        Return True
    End Function

    Private Sub ExtractCyganCounts(sPage As String, oGrany As tGranyUtwor)
        Dim aArr As String() = sPage.Split(vbCrLf)
        If aArr.GetUpperBound(0) < 2 Then Return    ' jakiś błąd, ale niby jaki?

        For i As Integer = 0 To aArr.GetUpperBound(0)
            If Not aArr(i).Trim.StartsWith("COUNT") Then Continue For

            Dim aFld As String() = aArr(i).Split("|")
            If aFld.GetUpperBound(0) = 2 Then
                Select Case aFld(1)
                    Case "artist"
                        oGrany.countArtist = aFld(2).Trim
                    Case "title"
                        oGrany.countTitle = aFld(2).Trim
                    Case "album"
                        oGrany.countAlbum = aFld(2).Trim
                    Case "year"
                        oGrany.countYear = aFld(2).Trim
                    Case "dekada"
                        oGrany.countDekada = aFld(2).Trim
                End Select
            End If
        Next

    End Sub

    Public Overrides Async Function GetNextSong(iNextMode As Integer, oGrany As tGranyUtwor) As Task(Of Boolean)
        DebugOut("GetNextSong, miNextMode=" & iNextMode)
        Try
            Dim iCurrId As Integer = 0
            Dim sParams As String = ""

            If App.mtGranyUtwor IsNot Nothing Then iCurrId = oGrany.oAudioParam.id
            Select Case iNextMode
                Case 0
                    iCurrId = GetSettingsInt("maxSoundId")
                    If iCurrId = 0 Then Return False
                    iCurrId = App.MakeRandom(iCurrId)
                    ' random
                    sParams = "id=" & iCurrId & "&mode=random"
                Case 1
                    sParams = "id=" & iCurrId & "&mode=artist"
                Case 2
                    sParams = "id=" & iCurrId & "&mode=title"
                Case 3
                    sParams = "id=" & iCurrId & "&mode=album"
                Case 4
                    sParams = "id=" & iCurrId & "&mode=rok"
                Case 5
                    sParams = "id=" & iCurrId & "&mode=dekada"
            End Select

            ' dane o nastepnym HttpPage -> App.mtGranyUtwor
            Dim sPage As String = Await HttpPageAsync("/cygan-info.asp?" & sParams, "file data")
            If sPage = "" Then
                DialogBox("ERROR get cygan-info empty")
                Return False
            End If
            Dim aArr As String() = sPage.Split(vbCrLf)
            If aArr.GetUpperBound(0) < 2 Then
                DialogBox("ERROR get cygan-info too short, " & vbCrLf &
                    "Request: " & "/cygan-info.asp?" & sParams & vbCrLf &
                    "Returned: " & sPage)
                Return False
            End If
            If aArr(0).Trim <> "OK" Then
                ' moze wygasła sesja, to powtarzamy
                Await App.goDbase.Login(True)
                sPage = Await HttpPageAsync("/cygan-info.asp?" & sParams, "file data")
                If sPage = "" Then
                    DialogBox("ERROR retry get cygan-info empty")
                    Return False
                End If

                aArr = sPage.Split(vbCrLf)
                If aArr.GetUpperBound(0) < 2 Then
                    DialogBox("ERROR retry get cygan-info too short, " & vbCrLf &
                    "Request: " & "/cygan-info.asp?" & sParams & vbCrLf &
                    "Returned: " & sPage)
                    Return False
                End If

                If aArr(0).Trim <> "OK" Then
                    DialogBox("ERROR get cygan-info not OK, aArr(0): " & aArr(0))
                    Return False
                End If
            End If

            App.mtGranyUtwor = New tGranyUtwor

            If Not Await ExtractCyganInfo(aArr) Then Return False

            Return True
        Catch ex As Exception
            CrashMessageAdd("@GetNextSong", ex)
        End Try

        Return False
    End Function

    Private Async Function ExtractCyganInfo(aArr As String()) As Task(Of Boolean)
        ' base64 z linku
        Dim sTxt As String = aArr(1).Trim
        If sTxt.Length Mod 4 > 0 Then sTxt = sTxt & "=" ' dodaj padding
        If sTxt.Length Mod 4 > 0 Then sTxt = sTxt & "="
        If sTxt.Length Mod 4 > 0 Then sTxt = sTxt & "="

        Dim oBuff As Byte() = Nothing
        Dim bError As Boolean = False
        Try
            oBuff = System.Convert.FromBase64String(sTxt)
        Catch ex As Exception
            bError = True
        End Try

        If bError Then
            Await DialogBoxAsync("dodaje minus")
            sTxt = sTxt & "="
            bError = False
            Try
                oBuff = System.Convert.FromBase64String(sTxt)
            Catch ex As Exception
                bError = True
            End Try
        End If

        If bError Then
            Await DialogBoxAsync("dodaje minus2")
            sTxt = sTxt & "="
            bError = False
            Try
                oBuff = System.Convert.FromBase64String(sTxt)
            Catch ex As Exception
                bError = True
            End Try
        End If

        If bError Then
            App.mtGranyUtwor = Nothing
            Return False
        End If

        sTxt = System.Text.Encoding.ASCII.GetString(oBuff)
        ' "U:\Public\MyProduction\OldMusic\DVD\OldMusic.001\OldMusic\70\AlRobertsJr"
        ' "/store/Public/MyProduction/OldMusic/DVD/OldMusic.014/BeeGees/AKickInTheHeadIsWorhtEightInThePants/BeeGees-AKITHIWEITP-03-HomeAgainRivers.Mp3"
        App.mtGranyUtwor.oStoreFile.path = sTxt.Substring(7)
        App.mtGranyUtwor.oStoreFile.name = ""

        For i As Integer = 2 To aArr.GetUpperBound(0)
            Dim aFld As String() = aArr(i).Split("|")
            If aFld.GetUpperBound(0) = 2 Then
                Select Case aFld(1)
                    Case "ID"
                        App.mtGranyUtwor.oAudioParam.id = aFld(2).Trim
                    Case "Artist"
                        App.mtGranyUtwor.oAudioParam.artist = aFld(2).Trim
                    Case "Title"
                        App.mtGranyUtwor.oAudioParam.title = aFld(2).Trim
                    Case "Album"
                        App.mtGranyUtwor.oAudioParam.album = aFld(2).Trim
                    Case "Rok"
                        App.mtGranyUtwor.oAudioParam.year = aFld(2).Trim
                    Case "Dekada"
                        App.mtGranyUtwor.oAudioParam.dekada = aFld(2).Trim
                    Case "Track"
                        App.mtGranyUtwor.oAudioParam.track = aFld(2).Trim
                    Case "Bitrate"
                        App.mtGranyUtwor.oAudioParam.bitrate = aFld(2).Trim
                    Case "Duration"
                        App.mtGranyUtwor.oAudioParam.duration = aFld(2).Trim
                    Case "Channels"
                        App.mtGranyUtwor.oAudioParam.channels = aFld(2).Trim
                    Case "Sample"
                        App.mtGranyUtwor.oAudioParam.sample = aFld(2).Trim
                    Case "fSize"
                        App.mtGranyUtwor.oStoreFile.len = aFld(2).Trim
                    Case Else   ' Comment
                        App.mtGranyUtwor.oAudioParam.comment &= aFld(2).Trim
                End Select

            End If
            '|Artist|The Beatles |Title|Till There Was You |Album|Anthology 1 (disc 2) |Rok|1995 
            '|Dekada|199x |Track| |Bitrate|192 |Duration|174 |channels|Stereo |sample|44100 |fsize|4183064 |Comment|RiBor 
        Next

        Return True

    End Function

    Private Shared moHttp As Windows.Web.Http.HttpClient = New Windows.Web.Http.HttpClient
    Public Shared Async Function HttpPageAsync(sUrl As String, sErrMsg As String, Optional sData As String = "") As Task(Of String)
#If CONFIG = "Debug" Then
        ' próba wylapywania errorów gdy nic innego tego nie złapie
        Dim sDebugCatch As String = ""
        Try
#End If
            If Not NetIsIPavailable(True) Then Return ""
            If sUrl = "" Then Return ""

            If sUrl.Substring(0, 4) <> "http" Then sUrl = BaseUri & "/p/dysk" & sUrl

            If moHttp Is Nothing Then
                moHttp = New Windows.Web.Http.HttpClient
                moHttp.DefaultRequestHeaders.UserAgent.TryParseAdd("GrajCyganie")
            End If

            Dim sError = ""
            Dim oResp As Windows.Web.Http.HttpResponseMessage = Nothing

            Try
                If sData <> "" Then
                    Dim oHttpCont = New Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded")
                    oResp = Await moHttp.PostAsync(New Uri(sUrl), oHttpCont)
                Else
                    oResp = Await moHttp.GetAsync(New Uri(sUrl))
                End If
            Catch ex As Exception
                sError = ex.Message
            End Try

            If sError <> "" Then
                Await DialogBoxAsync("error " & sError & " at " & sErrMsg & " page")
                Return ""
            End If

            If oResp.StatusCode = 303 Or oResp.StatusCode = 302 Or oResp.StatusCode = 301 Then
                ' redirect
                sUrl = oResp.Headers.Location.ToString
                'If sUrl.ToLower.Substring(0, 4) <> "http" Then
                '    sUrl = "https://sympatia.onet.pl/" & sUrl   ' potrzebne przy szukaniu
                'End If

                If sData <> "" Then
                    ' Dim oHttpCont = New HttpStringContent(sData, Text.Encoding.UTF8, "application/x-www-form-urlencoded")
                    Dim oHttpCont = New Windows.Web.Http.HttpStringContent(sData, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded")
                    oResp = Await moHttp.PostAsync(New Uri(sUrl), oHttpCont)
                Else
                    oResp = Await moHttp.GetAsync(New Uri(sUrl))
                End If
            End If

            If oResp.StatusCode > 290 Then
                Await DialogBoxAsync("ERROR " & oResp.StatusCode & " getting " & sErrMsg & " page")
                Return ""
            End If

            Dim sResp As String = ""
            Try
                sResp = Await oResp.Content.ReadAsStringAsync
            Catch ex As Exception
                sError = ex.Message
            End Try

            If sError <> "" Then
                Await DialogBoxAsync("error " & sError & " at ReadAsStringAsync " & sErrMsg & " page")
                Return ""
            End If

            Return sResp

#If CONFIG = "Debug" Then
        Catch ex As Exception
            sDebugCatch = ex.Message
        End Try

        If sDebugCatch <> "" Then
#Disable Warning BC42358
            DialogBox("DebugCatch in HttpPageAsync:" & vbCrLf & sDebugCatch)
#Enable Warning BC42358
        End If
        Return ""
#End If
    End Function

End Class
