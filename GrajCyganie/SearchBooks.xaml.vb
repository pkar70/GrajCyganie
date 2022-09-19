Imports Vblib.Extensions
Imports vb14 = Vblib.pkarlibmodule14


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

    Private Sub uiExport_Click(sender As Object, e As RoutedEventArgs)
        If mLista Is Nothing Then Return

        Dim sText As String = ""
        For Each oItem As Vblib.oneStoreFile In mLista
            sText = sText & vbCrLf & oItem.path & "\" & oItem.name
        Next

        vb14.ClipPut(sText)
    End Sub

    Private Sub uiLubimyCzytac_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As Vblib.oneStoreFile = TryCast(sender, FrameworkElement).DataContext
        If oItem Is Nothing Then Return

        Dim oUri As New Uri("https://lubimyczytac.pl/szukaj/ksiazki?phrase=" & oItem.name.Replace(" ", "%20"))
        oUri.OpenBrowser()
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