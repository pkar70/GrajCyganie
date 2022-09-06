Imports vb14 = Vblib.pkarlibmodule14

Public NotInheritable Class Search
    Inherits Page

    Public Shared mPageWidth As Integer = -1

    Private Sub uiCurrent_Click(sender As Object, e As RoutedEventArgs)
        If App.mtGranyUtwor Is Nothing Then Return
        uiArtist.Text = App.mtGranyUtwor.oAudioParam.artist
        uiTitle.Text = App.mtGranyUtwor.oAudioParam.title
        uiAlbum.Text = App.mtGranyUtwor.oAudioParam.album
        uiRok.Text = App.mtGranyUtwor.oAudioParam.year
    End Sub

    Private Sub uiSearch_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub uiPage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        mPageWidth = Me.ActualWidth
    End Sub
End Class

Public Class KonwersjaZnikaniePol
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        ' ignorujemy binding jako taki, bierzemy tylko jego parametr
        If Search.mPageWidth > 0 AndAlso Search.mPageWidth < 500 Then Return 0

        If parameter Is Nothing Then Return 0
        Dim sParam As String = CType(parameter, String)
        Return Integer.Parse(sParam)

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class
