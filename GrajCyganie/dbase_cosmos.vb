Public Class dbase_cosmos
    Inherits dbase_base

    'Private gmCosmosClient As Microsoft. = Nothing

    Public Overrides ReadOnly Property Nazwa As String = "Cosmos"
    Public Overrides Function GetPermission(sUser As String) As Task(Of String)
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetMaxId() As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetCountsy(oGrany As tGranyUtwor) As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetNextSong(iNextMode As Integer, oGrany As tGranyUtwor) As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function DekadyDownload() As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function
End Class
