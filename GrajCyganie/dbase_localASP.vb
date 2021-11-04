Public Class dbase_localASP
    Inherits dbase_base

    Public Overrides ReadOnly Property Nazwa As String = "LocalASP"

    Public Overrides Async Function GetPermission(sUser As String) As Task(Of String)
        mbGranted = True
        Return "MAM"
    End Function

    Public Overrides Async Function GetMaxId() As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function

    Protected Overrides Async Function DekadyDownload() As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function
    Public Overrides Async Function GetCountsy(oGrany As tGranyUtwor) As Task(Of Boolean)

    End Function

    Public Overrides Function GetNextSong(iNextMode As Integer, oGrany As tGranyUtwor) As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function
End Class
