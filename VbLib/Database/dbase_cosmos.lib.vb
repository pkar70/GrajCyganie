﻿Public Class dbase_cosmos
    Inherits dbase_base

    'Private gmCosmosClient As Microsoft. = Nothing

    Public Overrides ReadOnly Property Nazwa As String = "Cosmos"
    Public Overrides Function GetPermissionAsync(sUser As String) As Task(Of String)
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function RetrieveMaxIdAsync() As Task(Of Integer)
        Throw New NotImplementedException()
    End Function

    Public Overrides Function RetrieveCountsyAsync(oGrany As tGranyUtwor) As Task(Of Boolean)
        Throw New NotImplementedException()
    End Function

    Public Overrides Function GetNextSongAsync(iNextMode As Integer, oGrany As tGranyUtwor) As Task(Of tGranyUtwor)
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function DekadyDownloadAsync() As Task(Of List(Of tDekada))
        Throw New NotImplementedException()
    End Function
End Class