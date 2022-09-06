
''' <summary>
''' Provides application-specific behavior to supplement the default Application class.
''' </summary>
Partial Public Class App

    'Public Shared Function TimeSecToString(iSec As String) As String
    '    Dim oTS As TimeSpan = TimeSpan.FromSeconds(iSec)
    '    Dim sTxt As String = oTS.ToString("h\:mm\:ss")
    '    If oTS.Days > 0 Then
    '        If oTS.Hours < 10 Then sTxt = "0" & sTxt    ' jesli dajemy dni, to godziny z zerem
    '        sTxt = oTS.Days & "d " & sTxt
    '    End If
    '    Return sTxt
    'End Function

    Private Shared moRandom As Random = New Random
    Public Shared Function MakeRandom(iMax As Integer) As Integer
        ' Random = Windows.Security.Cryptography.CryptographicBuffer.GenerateRandomNumber()
        Return moRandom.Next(iMax + 1)
    End Function

    'Public Shared goDbase As dbase_base = Nothing

    Public Shared _dekady As DekadyList

    Public Shared gaDbs As New List(Of dbase_base)
    Public Sub New(sPath As String)
        _dekady = New DekadyList(sPath)

        gaDbs.Add(New dbase_domekASP)
        gaDbs.Add(New dbase_localASP)
        ' gaDbs.Add(New dbase_sql)
    End Sub

    Public Function GetCurrentDb() As dbase_base
        For Each oItem As dbase_base In gaDbs
            If oItem.Nazwa.ToLower = GetSettingsString("usingDB").ToLower Then
                Return oItem
            End If
        Next

        DumpMessage("WARN: używam bazy domyślnej")
        For Each oItem As dbase_base In gaDbs
            If oItem.Nazwa.ToLower.Contains("local") Then
                Return oItem
            End If
        Next

        DumpMessage("FAIL: nie mam nawet bazy domyślnej!")
        Return Nothing
    End Function
End Class
