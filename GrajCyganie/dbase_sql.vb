Partial Public Class dbase_sql
    Inherits dbase_base

    Public Overrides ReadOnly Property Nazwa As String = "localSQL"

    Private moConnRO As New System.Data.SqlClient.SqlConnection(sConnString)

    Public Overrides Async Function GetPermission(sUser As String) As Task(Of String)
        mbGranted = True
        Return True
    End Function

    Public Overrides Async Function GetMaxId() As Task(Of Boolean)
        If Not EnsureOpen() Then Return False

        Dim iRet As Integer = GetDbIntQueryResult("SELECT MAX(ID) FROM audioParam")
        If iRet < 0 Then Return False

        SetSettingsInt("maxSoundId", iRet)
        Return True

    End Function

    Protected Overrides Async Function DekadyDownload() As Task(Of Boolean)
        If Not EnsureOpen() Then Return False

        mlDekady.Clear()
        Dim iTotalCnt As Integer = 0
        Dim iTotalTime As Integer = 0

        Using oQuery As New System.Data.SqlClient.SqlCommand("SELECT dekada, COUNT(*), SUM(duration) FROM audioParam GROUP BY dekada ORDER BY dekada")
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    Dim oNew As New tDekada
                    oNew.sNazwa = oRdr.GetString(0).Trim
                    oNew.iCount = oRdr.GetInt32(1)
                    iTotalCnt = iTotalCnt + oNew.iCount
                    oNew.iPlayTime = oRdr.GetInt32(2)
                    iTotalTime = iTotalTime + oNew.iPlayTime
                    oNew.sPlayTime = App.TimeSecToString(oNew.iPlayTime)
                    mlDekady.Add(oNew)
                End While
            End Using

        End Using

        If mlDekady.Count < 1 Then Return False

        ' total
        Dim oTotal As New tDekada
        oTotal.sNazwa = "Total"
        oTotal.iCount = iTotalCnt
        oTotal.iPlayTime = iTotalTime
        oTotal.sPlayTime = App.TimeSecToString(iTotalTime)
        mlDekady.Add(oTotal)

        Return True

    End Function

    Private Function GetDbIntQueryResult(sQuery As String) As Integer
        Using oQuery As New System.Data.SqlClient.SqlCommand("SELECT MAX(ID) FROM audioParam")
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    Return oRdr.GetInt32(0)
                End While
            End Using
        End Using

        Return 0
    End Function
    Public Overrides Async Function GetCountsy(oGrany As tGranyUtwor) As Task(Of Boolean)
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


    Public Overrides Async Function GetNextSong(iNextMode As Integer, oGrany As tGranyUtwor) As Task(Of Boolean)
        DebugOut("GetNextSong, miNextMode=" & iNextMode)
        If Not EnsureOpen() Then Return False

        If App.mtGranyUtwor Is Nothing Then App.mtGranyUtwor = New tGranyUtwor

        Dim sQuery As String = "SELECT TOP 1 " &
            "id, fileID, artist, title, album, comment, duration, dekada, bitrate, channels, sample, vbr, year, track" &
            " FROM audioParam WHERE "
        ' wymieniam kolumny, żeby znac ich układ (kolejność) w oResult

        Select Case iNextMode
            Case 0
                sQuery &= " 1=1 "
            Case 1
                sQuery &= " artist='" & oGrany.oAudioParam.artist.Replace("'", "''") & "'"
            Case 2
                sQuery &= " title='" & oGrany.oAudioParam.title.Replace("'", "''") & "'"
            Case 3
                sQuery &= " album='" & oGrany.oAudioParam.album.Replace("'", "''") & "'"
            Case 4
                sQuery &= " year='" & oGrany.oAudioParam.year.Replace("'", "''") & "'"
            Case 5
                sQuery &= " dekada='" & oGrany.oAudioParam.dekada.Replace("'", "''") & "'"
        End Select


        Dim iCurrId As Integer = 0
        If App.mtGranyUtwor IsNot Nothing Then iCurrId = App.mtGranyUtwor.oAudioParam.id
        If iNextMode = 0 Then
            iCurrId = GetSettingsInt("maxSoundId")
            If iCurrId = 0 Then Return False
            iCurrId = App.MakeRandom(iCurrId)
        End If

        sQuery &= " AND ID > " & iCurrId & " ORDER BY id"

        Dim bEmpty As Boolean = True

        Using oQuery As New System.Data.SqlClient.SqlCommand(sQuery)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    App.mtGranyUtwor.oAudioParam.id = oRdr.GetInt32(0)
                    App.mtGranyUtwor.oAudioParam.fileID = oRdr.GetInt32(1)
                    App.mtGranyUtwor.oAudioParam.artist = oRdr.GetString(2)
                    App.mtGranyUtwor.oAudioParam.title = oRdr.GetString(3)
                    App.mtGranyUtwor.oAudioParam.album = oRdr.GetString(4)
                    App.mtGranyUtwor.oAudioParam.comment = oRdr.GetString(5)
                    App.mtGranyUtwor.oAudioParam.duration = oRdr.GetInt16(6)
                    App.mtGranyUtwor.oAudioParam.dekada = oRdr.GetString(7).Trim    ' bo jest (n)char, a nie (n)VARchar
                    App.mtGranyUtwor.oAudioParam.bitrate = oRdr.GetInt32(8)
                    App.mtGranyUtwor.oAudioParam.channels = oRdr.GetString(9).Trim
                    App.mtGranyUtwor.oAudioParam.sample = oRdr.GetInt32(10)
                    App.mtGranyUtwor.oAudioParam.vbr = oRdr.GetInt32(11)
                    App.mtGranyUtwor.oAudioParam.year = oRdr.GetString(12).Trim
                    App.mtGranyUtwor.oAudioParam.track = oRdr.GetString(13).Trim
                    bEmpty = False
                End While
            End Using
        End Using

        If bEmpty Then Return False

        bEmpty = True
        sQuery = "SELECT name, path, filedate, len, del FROM StoreFiles WHERE id=" & App.mtGranyUtwor.oAudioParam.fileID
        Using oQuery As New System.Data.SqlClient.SqlCommand(sQuery)
            Using oRdr As System.Data.SqlClient.SqlDataReader = oQuery.ExecuteReader()
                While oRdr.Read
                    App.mtGranyUtwor.oStoreFile.id = App.mtGranyUtwor.oAudioParam.fileID
                    App.mtGranyUtwor.oStoreFile.name = oRdr.GetString(1)
                    App.mtGranyUtwor.oStoreFile.path = oRdr.GetString(2)
                    App.mtGranyUtwor.oStoreFile.filedate = oRdr.GetString(3).Trim
                    App.mtGranyUtwor.oStoreFile.len = oRdr.GetInt64(4)
                    App.mtGranyUtwor.oStoreFile.del = oRdr.GetBoolean(5)
                    bEmpty = False
                End While
            End Using
        End Using


        If bEmpty Then Return False

        Return True

    End Function

    Private Function EnsureOpen() As Boolean
        If moConnRO.State = System.Data.ConnectionState.Open Then Return True
        ' tak, stanów może być dużo, ale zakładam że się nie 'zagnieżdżę' w Retrieving itp.
        moConnRO.Open()
        If moConnRO.State = System.Data.ConnectionState.Open Then Return True
        Return False
    End Function

End Class
