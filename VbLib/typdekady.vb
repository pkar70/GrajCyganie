' wydzielone z typkiapp oraz dbase_base

Public Class tDekada
    Public Property sNazwa As String
    Public Property iCount As Integer
    Public Property iPlayTimeSecs As Integer
    Public Property iFreq As Integer

    '<Newtonsoft.Json.JsonIgnore>
    'Public Property sFreq As String

    Public Function GetCounterValue() As Integer
        Select Case iFreq
            Case 0
                Return 1000
            Case 1
                Return 10
            Case 2
                Return 5
            Case 3
                Return 3
            Case 4
                Return 2
            Case Else
                Return 1
        End Select
    End Function

    Public Function GetFreqString() As String
        Select Case iFreq
            Case 0
                Return "  0 %"
            Case 1
                Return " 10 %"
            Case 2
                Return " 20 %"
            Case 3
                Return " 33 %"
            Case 4
                Return " 50 %"
            Case Else
                Return "100 %"
        End Select
    End Function

End Class

Public Class DekadyList
    Inherits MojaLista(Of tDekada)

    Private Const FILENAME As String = "dekady.json"

    ''' <summary>
    ''' razem z wczytaniem istniejącego cache z dysku
    ''' </summary>
    ''' <param name="sFolder"></param>
    Public Sub New(sFolder As String)
        MyBase.New(sFolder, FILENAME)
        MyBase.Load()
    End Sub

    ''' <summary>
    ''' podmień listę, po zabraniu ze starej Frequency, i zapisz nową wersję
    ''' </summary>
    ''' <param name="nowaLista"></param>
    Public Sub ImportNew(nowaLista As List(Of tDekada))
        ' z przepisaniem wartości

        For Each oItemNew As tDekada In nowaLista
            For Each oItemOld As tDekada In _lista
                If oItemNew.sNazwa = oItemOld.sNazwa Then
                    ' oItemNew.sFreq = oItemOld.sFreq
                    oItemNew.iFreq = oItemOld.iFreq
                    Exit For
                End If
            Next
        Next

        _lista = nowaLista
        _lista.Add(GetTotalItem)
        Save()
    End Sub

    Private Function GetTotalItem() As tDekada
        Dim oNew As New tDekada
        oNew.sNazwa = "Total"
        oNew.iCount = 0
        oNew.iPlayTimeSecs = 0

        For Each oItem As tDekada In _lista
            oNew.iCount += oItem.iCount
            oNew.iPlayTimeSecs += oItem.iPlayTimeSecs
        Next

        Return oNew
    End Function

End Class
