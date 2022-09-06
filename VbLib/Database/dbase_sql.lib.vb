Partial Public Class dbase_sql
    Inherits dbase_base

    Public Overrides ReadOnly Property Nazwa As String = "localSQL"

    Private moConnRO As New System.Data.SqlClient.SqlConnection(sConnString)

#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "BC42356:ReviewUnusedParameters")>
    Public Overrides Async Function GetPermissionAsync(sUser As String) As Task(Of String)
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        mbGranted = True
        Return True
    End Function

#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    Protected Overrides Async Function RetrieveMaxIdAsync() As Task(Of Integer)
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        If Not EnsureOpen() Then Return False

        Dim iRet As Integer = GetDbIntQueryResult("SELECT MAX(ID) FROM audioParam")
        If iRet < 0 Then Return -1

        Return iRet
    End Function

#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    Protected Overrides Async Function DekadyDownloadAsync() As Task(Of List(Of tDekada))
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        Dim mlDekady As New List(Of tDekada)
        If Not EnsureOpen() Then Return mlDekady

        Using oQuery As New System.Data.SqlClient.SqlCommand("SELECT dekada, COUNT(*), SUM(duration) FROM audioParam GROUP BY dekada ORDER BY dekada", moConnRO)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    Dim oNew As New tDekada
                    oNew.sNazwa = oRdr.GetString(0).Trim
                    oNew.iCount = oRdr.GetInt32(1)
                    oNew.iPlayTimeSecs = oRdr.GetInt32(2)
                    mlDekady.Add(oNew)
                End While
            End Using

        End Using

        Return mlDekady

    End Function

    Private Function GetDbIntQueryResult(sQuery As String) As Integer
        DumpCurrMethod()

        Using oQuery As New System.Data.SqlClient.SqlCommand("SELECT MAX(ID) FROM audioParam", moConnRO)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    Return oRdr.GetInt32(0)
                End While
            End Using
        End Using

        Return 0
    End Function
#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    Public Overrides Async Function RetrieveCountsyAsync(oGrany As tGranyUtwor) As Task(Of Boolean)
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        If Not EnsureOpen() Then Return False

        ' ale po co robić "SELECT * FROM audioParam WHERE id=" & iID , skoro to już mamy wyciągnięte do grania?

        Dim iRet As Integer

        iRet = GetDbIntQueryResult("SELECT COUNT(*) FROM audioParam WHERE artist='" &
                                   oGrany.oAudioParam.artist.Replace("'", "''") & "'")
        If iRet > 0 Then oGrany.countArtist = iRet


        iRet = GetDbIntQueryResult("SELECT COUNT(*) FROM audioParam WHERE title='" &
                                   oGrany.oAudioParam.artist.Replace("'", "''") & "'")
        If iRet > 0 Then oGrany.countTitle = iRet

        iRet = GetDbIntQueryResult("SELECT COUNT(*) FROM audioParam WHERE album='" &
                                   oGrany.oAudioParam.album.Replace("'", "''") & "'")
        If iRet > 0 Then oGrany.countAlbum = iRet

        iRet = GetDbIntQueryResult("SELECT COUNT(*) FROM audioParam WHERE year='" &
                                   oGrany.oAudioParam.year.Replace("'", "''") & "'")
        If iRet > 0 Then oGrany.countYear = iRet

        iRet = GetDbIntQueryResult("SELECT COUNT(*) FROM audioParam WHERE dekada='" &
                                   oGrany.oAudioParam.dekada.Replace("'", "''") & "'")
        If iRet > 0 Then oGrany.countDekada = iRet

        Return True
    End Function


#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    Public Overrides Async Function GetNextSongAsync(iNextMode As eNextMode, oGrany As tGranyUtwor) As Task(Of tGranyUtwor)
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        DebugOut("GetNextSong, miNextMode=" & iNextMode)
        If Not EnsureOpen() Then Return Nothing

        Dim sQuery As String = "SELECT TOP 1 " &
            "id, fileID, artist, title, album, comment, duration, dekada, bitrate, channels, sample, vbr, year, track" &
            " FROM audioParam WHERE "
        ' wymieniam kolumny, żeby znac ich układ (kolejność) w oResult

        Select Case iNextMode
            Case eNextMode.random
                sQuery &= " 1=1 "
            Case eNextMode.sameArtist
                sQuery &= " artist='" & oGrany.oAudioParam.artist.Replace("'", "''") & "'"
            Case eNextMode.sameTitle
                sQuery &= " title='" & oGrany.oAudioParam.title.Replace("'", "''") & "'"
            Case eNextMode.sameAlbum
                sQuery &= " album='" & oGrany.oAudioParam.album.Replace("'", "''") & "'"
            Case eNextMode.sameRok
                sQuery &= " year='" & oGrany.oAudioParam.year.Replace("'", "''") & "'"
            Case eNextMode.sameDekada
                sQuery &= " dekada='" & oGrany.oAudioParam.dekada.Replace("'", "''") & "'"
        End Select


        Dim iCurrId As Integer = 0
        If oGrany IsNot Nothing Then iCurrId = oGrany.oAudioParam.id
        If iNextMode = 0 Then
            iCurrId = GetSettingsInt("maxSoundId")
            If iCurrId = 0 Then Return Nothing
            iCurrId = App.MakeRandom(iCurrId)
        End If

        sQuery &= " AND ID > " & iCurrId & " ORDER BY id"

        Dim bEmpty As Boolean = True

        Dim oNew As New tGranyUtwor

        Using oQuery As New System.Data.SqlClient.SqlCommand(sQuery, moConnRO)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    oNew.oAudioParam.id = oRdr.GetInt32(0)
                    oNew.oAudioParam.fileID = oRdr.GetInt32(1)
                    oNew.oAudioParam.artist = oRdr.GetString(2)
                    oNew.oAudioParam.title = oRdr.GetString(3)
                    oNew.oAudioParam.album = oRdr.GetString(4)
                    oNew.oAudioParam.comment = oRdr.GetString(5)
                    oNew.oAudioParam.duration = oRdr.GetInt16(6)
                    oNew.oAudioParam.dekada = oRdr.GetString(7).Trim    ' bo jest (n)char, a nie (n)VARchar
                    oNew.oAudioParam.bitrate = oRdr.GetInt32(8)
                    oNew.oAudioParam.channels = oRdr.GetString(9).Trim
                    oNew.oAudioParam.sample = oRdr.GetInt32(10)
                    oNew.oAudioParam.vbr = oRdr.GetInt32(11)
                    oNew.oAudioParam.year = oRdr.GetString(12).Trim
                    oNew.oAudioParam.track = oRdr.GetString(13).Trim
                    bEmpty = False
                End While
            End Using
        End Using

        If bEmpty Then Return Nothing

        bEmpty = True
        sQuery = "SELECT name, path, filedate, len, del FROM StoreFiles WHERE id=" & oNew.oAudioParam.fileID
        Using oQuery As New System.Data.SqlClient.SqlCommand(sQuery, moConnRO)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    oNew.oStoreFile.id = oNew.oAudioParam.fileID
                    oNew.oStoreFile.name = oRdr.GetString(1)
                    oNew.oStoreFile.path = oRdr.GetString(2)
                    oNew.oStoreFile.filedate = oRdr.GetString(3).Trim
                    oNew.oStoreFile.len = oRdr.GetInt64(4)
                    oNew.oStoreFile.del = oRdr.GetBoolean(5)
                    bEmpty = False
                End While
            End Using
        End Using


        If bEmpty Then Return Nothing

        Return oNew

    End Function

    Private Function EnsureOpen() As Boolean
        DumpCurrMethod()

        If moConnRO.State = System.Data.ConnectionState.Open Then Return True
        ' tak, stanów może być dużo, ale zakładam że się nie 'zagnieżdżę' w Retrieving itp.

        'Exception thrown 'System.IO.FileNotFoundException' in System.Data.SqlClient.dll
        'Could Not load file Or assembly 'System.Threading.Thread, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'. The system cannot find the file specified.
        Dim iError As Integer
        Try
            moConnRO.Open()
            If moConnRO.State = System.Data.ConnectionState.Open Then Return True
        Catch ex As Exception
            iError = ex.HResult
        End Try

        If iError = -2147024894 Then
            DialogBox("FAIL opening, maybe too low MinVersion (doesn't work for 15063)")
        Else
            DialogBox("FAIL opening, maybe SQL has no enabled TCP/IP (see ComputerManagement»Services»SQL»SQL server network»Protocols)")
        End If
        Return False
    End Function

    Public Shared Function ConvertQueryParam(sMask As String) As String
        If String.IsNullOrWhiteSpace(sMask) Then Return ""
        sMask = sMask.Replace("'", "''")
        sMask = sMask.Replace("*", "%")
        sMask = sMask.Replace("?", "_")
        If sMask.Contains("%") OrElse sMask.Contains("_") Then Return sMask

        If Not sMask.StartsWith("^") Then
            sMask = "%" & sMask
        Else
            sMask = sMask.Substring(1)
        End If

        If Not sMask.EndsWith("$") Then
            sMask = sMask & "%"
        Else
            sMask = sMask.Substring(0, sMask.Length - 1)
        End If

        Return sMask
    End Function


    Private Function AddQueryParam(sFieldName As String, sMask As String) As String
        If String.IsNullOrWhiteSpace(sMask) Then Return ""

        sMask = ConvertQueryParam(sMask)
        Return $" AND {sFieldName} LIKE '{sMask}' "
    End Function

#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    Public Overrides Async Function SearchAsync(sArtist As String, sTitle As String, sAlbum As String, sRok As String) As Task(Of List(Of oneAudioParam))
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        If Not EnsureOpen() Then Return Nothing
        If (sArtist & sTitle & sAlbum & sRok).Length < 3 Then Return Nothing

        Dim sQry As String = "SELECT TOP 200 * FROM audioParam WHERE 1=1 "
        sQry = sQry & AddQueryParam("artist", sArtist)
        sQry = sQry & AddQueryParam("title", sTitle)
        sQry = sQry & AddQueryParam("album", sAlbum)
        sQry = sQry & AddQueryParam("rok", sRok)
        sQry = sQry & " FOR JSON AUTO"

        Using oQuery As New System.Data.SqlClient.SqlCommand(sQry, moConnRO)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    Dim sJson As String = oRdr.GetString(0)
                    ' import JSON
                    Dim oLista As New List(Of oneAudioParam)
                    oLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sJson, GetType(List(Of oneAudioParam)))
                    Return oLista
                End While
            End Using
        End Using

        Return Nothing
    End Function

#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
    Public Overrides Async Function GetStoreFileAsync(id As Integer) As Task(Of oneStoreFiles)
#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
        DumpCurrMethod()

        If Not EnsureOpen() Then Return Nothing

        Dim sQry As String = "SELECT * FROM storeFiles WHERE id=" & id

        Using oQuery As New System.Data.SqlClient.SqlCommand(sQry, moConnRO)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    Dim sJson As String = oRdr.GetString(0)
                    ' import JSON
                    Dim oItem As oneStoreFiles
                    oItem = Newtonsoft.Json.JsonConvert.DeserializeObject(sJson, GetType(oneStoreFiles))
                    Return oItem
                End While
            End Using
        End Using

        Return Nothing

    End Function
End Class
