
Public NotInheritable Class OpenM3u
    Inherits Page

    Private Sub ShowStat()
        If uiGrid.ActualWidth > 500 Then
            uiListItems1Row.ItemsSource = From c In Vblib.App._dekady.GetList
        Else
            uiListItems2Row.ItemsSource = From c In Vblib.App._dekady.GetList
        End If
    End Sub

    Private Sub Page_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        ' jest przed Loaded
        If uiGrid.ActualWidth > 500 Then
            uiScroll1Row.Visibility = Visibility.Visible
            uiScroll2Row.Visibility = Visibility.Collapsed
            uiFreq.Visibility = Visibility.Collapsed
        Else
            uiScroll1Row.Visibility = Visibility.Collapsed
            uiScroll2Row.Visibility = Visibility.Visible
            uiFreq.Visibility = Visibility.Visible
        End If

        ShowStat()
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        ShowStat()
    End Sub

    Private Async Sub uiRefresh_Click(sender As Object, e As RoutedEventArgs)
        If Not Await App.inVb.GetCurrentDb.ReloadDekadyAsync(True) Then Exit Sub

        ShowStat()
    End Sub

    Private Sub uiSave_Click(sender As Object, e As RoutedEventArgs)
        Vblib.App._dekady.Save()
    End Sub

    'Private Sub uiFreqSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles uiFreqSlider.ValueChanged
    '    ' Dim sTxt As String = App.FreqSlider2Text()
    'End Sub ' x:Name="uiFreqSlider"

    Private Sub FreqHideShowAll(bShow As Boolean)
        Dim iMode As Integer = 0
        If bShow Then iMode = 1

        Dim oLV As ListView = uiListItems2Row
        For Each oItem As ListViewItem In oLV.ItemsPanelRoot.Children
            FreqHideShow(oItem.ContentTemplateRoot, iMode)
        Next
    End Sub
    Private Sub uiFreq_Checked(sender As Object, e As RoutedEventArgs) Handles uiFreq.Checked
        FreqHideShowAll(True)
    End Sub

    Private Sub uiFreq_Unchecked(sender As Object, e As RoutedEventArgs) Handles uiFreq.Unchecked
        FreqHideShowAll(False)
    End Sub

    Private Sub FreqHideShow(oGrid As Grid, iMode As Integer)

        Dim iCnt As Integer = oGrid.Children.Count
        If iCnt < 4 Then Exit Sub

        Dim oTxt As TextBlock = TryCast(oGrid.Children(0), TextBlock)
        If oTxt Is Nothing Then Exit Sub
        If oTxt.Text = "Total" Then Exit Sub

        Dim oVis As Visibility
        Select Case iMode
            Case 0
                oVis = Visibility.Collapsed
            Case 1
                oVis = Visibility.Visible
            Case 99
                oVis = oGrid.Children(iCnt - 1).Visibility
                If oVis = Visibility.Collapsed Then
                    oVis = Visibility.Visible
                Else
                    oVis = Visibility.Collapsed
                End If
        End Select

        oGrid.Children(iCnt - 1).Visibility = oVis
        oGrid.Children(iCnt - 2).Visibility = oVis
    End Sub

    Private Sub uiRow_Tapped(sender As Object, e As TappedRoutedEventArgs)
        ' tapniety konkretny rzadek - przełącz widocznosc na nim
        Dim oGrid As Grid = TryCast(sender, Grid)
        If oGrid Is Nothing Then Exit Sub

        FreqHideShow(oGrid, 99) ' 99 oznacza 'przełącz'

    End Sub

    Private Sub uiGoLogin_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Login))
    End Sub

    Private Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
        Dim oSlider As Slider = TryCast(sender, Slider)
        If oSlider Is Nothing Then Exit Sub

        Dim oDekada As Vblib.tDekada = TryCast(oSlider.DataContext, Vblib.tDekada)
        If oDekada Is Nothing Then Exit Sub

        Dim oGrid As Grid = TryCast(oSlider.Parent, Grid)
        For Each oChild As UIElement In oGrid.Children
            Dim oTB As TextBlock = TryCast(oChild, TextBlock)
            If oTB IsNot Nothing AndAlso oTB.Name.StartsWith("uiFreqStr") Then
                oTB.Text = oDekada.GetFreqString
            End If
        Next

    End Sub
End Class
