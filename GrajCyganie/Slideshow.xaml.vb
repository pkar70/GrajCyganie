Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions
Imports Windows.UI.Xaml.Shapes

' tak naprawdę FotoRamka sprzed lat, plus zmiany

Public NotInheritable Class Slideshow
    Inherits Page

    'Private _sUrl As String = ""
    Private _oTimer As DispatcherTimer
    Private _iTickTime As Integer = 5

    Private _Path As String = ""
    Private _Recursive As Boolean = False

    Private _picki As List(Of Vblib.oneStoreFile)

#Region "page events"
    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        If e.Parameter Is Nothing Then Return

        _Recursive = e.Parameter.ToString.Substring(0, 1) = "1"
        _Path = e.Parameter.ToString.Substring(1)

    End Sub

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        _iTickTime = vb14.GetSettingsInt("slideshowTickTime", _iTickTime)
        SetCheckMarks()

        _picki = Await WczytajListePickow(_Path, _Recursive)

    End Sub


#End Region



#Region "bottom menu events"
    ' także z context menu
    Private Async Sub uiStartStop_Click(sender As Object, e As RoutedEventArgs)
        If _oTimer IsNot Nothing Then
            TykanieStop()
            uiStartStop.Label = "Start"
        Else
            Await NextPic()
            TykanieStart()
            uiStartStop.Label = "Stop"
        End If
    End Sub

    Private Sub uiDelay_Click(sender As Object, e As RoutedEventArgs)
        Dim oMI As ToggleMenuFlyoutItem = sender
        Dim sName As String = oMI.Name
        If Not sName.StartsWith("uiDelay") Then Return

        Try
            _iTickTime = sName.ToLower.Replace("uidelay", "")
        Catch ex As Exception
            _iTickTime = 5
        End Try

        vb14.SetSettingsInt("tickTime", _iTickTime)

        If _oTimer Is Nothing Then Return
        ' zmiana ma sens tylko wtedy gdy sobie tykamy

        If _iTickTime > 0 Then
            _oTimer.Interval = TimeSpan.FromSeconds(_iTickTime)
        Else
            _oTimer.Stop()
        End If
    End Sub
#End Region

#Region "pic context menu events"
    Private Sub uiPic_Tapped(sender As Object, e As RoutedEventArgs)
        Dim oResize As Stretch = uiImage.Stretch
        Select Case oResize
            Case Stretch.Uniform
                uiImage.Stretch = Stretch.None
            Case Stretch.None
                uiImage.Stretch = Stretch.Uniform
        End Select
    End Sub

    Private Sub uiGetPath_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As Vblib.oneStoreFile = TryCast(sender, FrameworkElement).DataContext
        Dim sPath As String = IO.Path.Combine(oItem.path, oItem.name)
        vb14.ClipPut(sPath)
    End Sub

    Private Sub uiGoNext_Click(sender As Object, e As RoutedEventArgs)
        Timer_TickUI(Nothing, Nothing)
    End Sub

    Private Sub uiShowInfo_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As Vblib.oneStoreFile = TryCast(sender, FrameworkElement).DataContext
        Dim sInfo As String = oItem.DumpAsText
        vb14.ClipPut(sInfo)
        vb14.DialogBox(sInfo)
    End Sub
#End Region

    Private Async Function WczytajListePickow(sPath As String, bRecursive As Boolean) As Task(Of List(Of Vblib.oneStoreFile))
        ' wycinamy początek (dysk), tak na wszelki wypadek
        If sPath.Length > 1 AndAlso sPath.Substring(1, 1) = ":" Then sPath = sPath.Substring(2)
        If sPath = "" Then sPath = "\"
        Me.ProgRingShow(True)
        Dim lista As List(Of Vblib.oneStoreFile)
        lista = Await App.inVb.GetCurrentDb.GetStorageItemsAsync(sPath & "&recursive=1")    ' dodajemy parametr, może trochę hacking, ale jednak działać powinno
        Me.ProgRingShow(False)

        ' DEL będzie wykorzystywany jako "już pokazany"

    End Function

    Private Async Function NextPic() As Task
        vb14.DumpCurrMethod()

        ' może być losowanie z pełnej listy, albo sekwencyjnie

        ' znajdź aktualny
        For Each oItem As Vblib.oneStoreFile In _picki
            If Not oItem.del Then
                ' pierwszy który nie ma DEL - pierwszy niepokazany
                oItem.del = True
                Dim oFile As Windows.Storage.StorageFile = Await GetFileFromStorageAsync(oItem)
                If oFile Is Nothing Then Return

                Dim oBmp As BitmapImage = New BitmapImage()
                oBmp.SetSource(Await oFile.OpenStreamForReadAsync)
                uiImage.Source = oBmp

            End If
        Next

        ' skończyły się obrazki, można zapętlić

    End Function


    Private Sub TykanieStart()
        Try
            _oTimer = New DispatcherTimer
            _oTimer.Interval = TimeSpan.FromSeconds(_iTickTime)
            AddHandler _oTimer.Tick, AddressOf Timer_TickUI
            _oTimer.Start()
            Return
        Catch ex As Exception
            CrashMessageExit("@InitTykanie", ex.Message)
        End Try
    End Sub

    Private Sub TykanieStop()
        If _oTimer IsNot Nothing Then
            Try
                RemoveHandler _oTimer.Tick, AddressOf Timer_TickUI
                _oTimer.Stop()
                _oTimer = Nothing
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Async Sub Timer_TickUI(sender As Object, e As Object)
        vb14.DumpCurrMethod()

        _oTimer?.Stop()

        Await NextPic()

        _oTimer?.Start()

    End Sub



    Private Sub SetCheckMarks()

        Dim sDelay As String = _iTickTime.ToString("0#")
        Dim bSelected As Boolean = False
        For Each oMFI As ToggleMenuFlyoutItem In uiMenuDelay.Items
            If oMFI.Name.EndsWith(sDelay) Then
                oMFI.IsChecked = True
                bSelected = True
            Else
                oMFI.IsChecked = False
            End If
        Next
        If Not bSelected Then uiDelay05.IsChecked = True

    End Sub

    Private Sub Page_LosingFocus(sender As UIElement, args As LosingFocusEventArgs)
        vb14.DumpCurrMethod()
        ' bo nie zmieniaj obrazków gdy nie jesteś w foreground

        _oTimer?.Stop()
    End Sub

    Private Sub Page_GotFocus(sender As Object, e As RoutedEventArgs)
        _oTimer?.Start()
    End Sub


End Class
