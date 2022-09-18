Imports Vblib.Extensions

Public NotInheritable Class SearchBooks
    Inherits Page

    Private Shared mLista As List(Of Vblib.oneStoreFile)

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Me.ProgRingInit(True, False)
        If mLista IsNot Nothing Then uiLista.ItemsSource = mLista
    End Sub

    Private Async Sub uiSzukaj_Click(sender As Object, e As RoutedEventArgs)

        Dim sPathMask As String = uiPath.Text
        Dim sFileMask As String = uiBook.Text


        Me.ProgRingShow(True)
        mLista = Await App.inVb.GetCurrentDb.SearchFilesAsync(sPathMask, sFileMask)
        Me.ProgRingShow(False)

        uiLista.ItemsSource = mLista

    End Sub


End Class

Public Class KonwersjaPath
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As String = CType(value, String)

        Dim iInd As Integer = temp.IndexOf("\Texts\Texts.")
        If iInd > 5 Then temp = "." & temp.Substring(iInd + "\Texts".Length)
        iInd = temp.IndexOf("\MyProduction")
        If iInd > 5 Then temp = "." & temp.Substring(iInd + "\MyProduction".Length)

        Return temp

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaDaty
    Implements IValueConverter

    ' teoretycznie będzie pokazywał datę, bez godziny

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As String = CType(value, String)
        Dim iInd As Integer = temp.IndexOf(" ")
        If iInd < 5 Then Return temp
        Return temp.Substring(0, iInd).Trim

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaLen
    Implements IValueConverter

    ' teoretycznie będzie pokazywał datę, bez godziny

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As String = CType(value, Long)

        Dim tempint As Integer = temp
        Return tempint.ToStringWithSpaces

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class