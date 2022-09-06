Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions

' pokazywanie wszystkich plików i katalogów w ramach danego katalogu
' może być jakieś "łączenie", np. models.0*, texts.0* i tak dalej w jeden duży wirtualny (ale to później)
' wyszukiwanie path/name

' pamiętać o del=false, i konwerterach LEN, DATE. Hide te pola dla isdir, i bold dla IsDir. OrderBy isdir, name

Public NotInheritable Class Browser
    Inherits Page

    Private _currDir As String = ""
    Private _lista As List(Of Vblib.oneStoreFile) = Nothing

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        _currDir = e.Parameter?.ToString
        If _currDir Is Nothing Then _currDir = ""
    End Sub

    Private Function CreateUpDirItem(currDir As String) As Vblib.oneStoreFile
        Dim oNew As New Vblib.oneStoreFile
        oNew.isDir = True
        oNew.name = ".."
        oNew.path = currDir
        Return oNew
    End Function

    Private Async Function WczytajDirectory(sPath As String) As Task(Of List(Of Vblib.oneStoreFile))
        vb14.DumpCurrMethod(sPath)

        ' wycinamy początek (dysk), tak na wszelki wypadek
        If sPath.Length > 1 AndAlso sPath.Substring(1, 1) = ":" Then sPath = sPath.Substring(2)
        If sPath = "" Then sPath = "\"

        uiCurrentDir.Text = "." & sPath

        Me.ProgRingShow(True)
        Dim lista As List(Of Vblib.oneStoreFile)
        lista = Await App.inVb.GetCurrentDb.GetStorageItemsAsync(sPath)
        Me.ProgRingShow(False)

        lista?.Add(CreateUpDirItem(sPath))
        Return lista
    End Function

    Private Sub PokazDirectory(lista As List(Of Vblib.oneStoreFile))
        If lista Is Nothing Then Return

        uiLista.ItemsSource = From c In lista Where Not c.del Order By Not c.isDir, c.name
    End Sub

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Me.ProgRingInit(True, False)
        If _lista Is Nothing Then _lista = Await WczytajDirectory(_currDir)
        PokazDirectory(_lista)
    End Sub

    Private Async Sub uiItem_DoubleTapped(sender As Object, e As DoubleTappedRoutedEventArgs)
        Dim oItem As Vblib.oneStoreFile = TryCast(sender, FrameworkElement).DataContext
        If Not oItem.isDir Then Return

        Dim sTempDir As String = oItem.path

        If oItem.name = ".." Then
            ' katalog wyżej przechodzimy
            Dim iInd As Integer = sTempDir.LastIndexOf("\")
            If iInd < 2 Then Return

            sTempDir = sTempDir.Substring(0, iInd)
        Else
            sTempDir = sTempDir & "\" & oItem.name
            sTempDir = sTempDir.Replace("\\", "\")
        End If

        ' *TODO* może być potem jako me.navigate(gettype(me), sTempDir) - ale wtedy Back może być trudny (cofnięcie się do grajka)
        _currDir = sTempDir
        _lista = Await WczytajDirectory(_currDir)
        PokazDirectory(_lista)

    End Sub

    Private Sub uiContextCopyPath(sender As Object, e As RoutedEventArgs)
        Dim oItem As Vblib.oneStoreFile = TryCast(sender, FrameworkElement).DataContext

        If oItem.name = ".." Then Return
        Dim sTempDir As String = oItem.path & "\" & oItem.name
        sTempDir = sTempDir.Replace("\\", "\")

        vb14.ClipPut(sTempDir)
    End Sub

    Private Async Sub uiContextShowDu(sender As Object, e As RoutedEventArgs)
        Dim oItem As Vblib.oneStoreFile = TryCast(sender, FrameworkElement).DataContext

        Dim iId As Integer = oItem.id

        Me.ProgRingShow(True)
        Dim dirsize As Long = Await App.inVb.GetCurrentDb.GetDirSize(iId)
        Me.ProgRingShow(False)

        vb14.DialogBox("Len: " & dirsize.ToStringISOsufix("Bytes"))

        oItem.len = dirsize
        PokazDirectory(_lista)
    End Sub

    Private Sub uiContextOpenPath(sender As Object, e As RoutedEventArgs)
        uiItem_DoubleTapped(sender, Nothing)
    End Sub
End Class

Public Class KonwersjaBoolToBold
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As Boolean = CType(value, Boolean)
        If temp Then Return Windows.UI.Text.FontWeights.Bold
        Return Windows.UI.Text.FontWeights.Normal

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaFileLen
        Implements IValueConverter

        Public Function Convert(ByVal value As Object,
        ByVal targetType As Type, ByVal parameter As Object,
        ByVal language As System.String) As Object _
        Implements IValueConverter.Convert

            Dim temp As Long = CType(value, Long)
            Return temp.ToStringISOsufix("Bytes")

        End Function


        ' ConvertBack is not implemented for a OneWay binding.
        Public Function ConvertBack(ByVal value As Object,
        ByVal targetType As Type, ByVal parameter As Object,
        ByVal language As System.String) As Object _
        Implements IValueConverter.ConvertBack

            Throw New NotImplementedException

        End Function
    End Class