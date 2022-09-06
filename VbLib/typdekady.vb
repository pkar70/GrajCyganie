Imports vb14 = Vblib.pkarlibmodule14

' wydzielone z typkiapp oraz dbase_base

Public Class tDekada
    Public Property sNazwa As String
    Public Property iCount As Integer
    Public Property iPlayTimeSecs As Integer
    Public Property iFreq As Integer

    <Newtonsoft.Json.JsonIgnore>
    Public Property iCounterToNext As Integer = -1

    '<Newtonsoft.Json.JsonIgnore>
    'Public Property sFreq As String

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

#Region "obsługa listy"
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
#End Region

#Region "liczniki - sprawdzanie"

    Private Function GetCounterValue(iFreq As Integer) As Integer
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

    ''' <summary>
    ''' czy można zagrać utwór z dekady; aktualizuje liczniki
    ''' </summary>
    ''' <param name="sDekada"></param>
    ''' <returns></returns>
    Public Function CanPlay(sDekada As String)
        vb14.DumpCurrMethod(sDekada)

        Dim oItem As tDekada = Find(Function(x) x.sNazwa = sDekada)
        If oItem Is Nothing Then Return True ' się nie powinno zdarzyć, ale kto go tam wie :) plik dekad może być starszy i nie uwzględniac jakiejś nowej dekady

        If oItem.iCounterToNext = -1 Then
            oItem.iCounterToNext = GetCounterValue(oItem.iFreq) ' reset, bo po wczytaniu są same -1 tutaj
            vb14.DumpMessage("resetting counter for dekada to: " & oItem.iCounterToNext)
        End If

        oItem.iCounterToNext -= 1
        If oItem.iCounterToNext > 0 Then
            vb14.DumpMessage($"counter for dekada {sDekada}: {oItem.iCounterToNext}")
            Return False
        End If

        oItem.iCounterToNext = GetCounterValue(oItem.iFreq)
        Return False
    End Function

#End Region

End Class
