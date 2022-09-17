Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions
Imports Vblib

' proste wyszukiwanie - pole tekstowe, ale z maskami
' pokazywanie znalezionych modelek
' browser plików w ramach modelki?
' slideshow w ramach modelki
' później może byc slideshow, przerywany w momencie końca grania muzyki (czyli piosenka ilustrowana zdjęciami)
' może też być OpenFolder, i z niego pokazywanie slideshow - przełącznik "with subfolders"
' slideshow: full screen, fit (z boku widać to co pod spodem), full window
' zabrać trochę z FotoRamka
' U:\Public\MyProduction\Models\U\Ul\UllaPetersson\Models.006
' combo z listą trafień, disabled gdy nie ma trafień

Public NotInheritable Class Modelki
    Inherits Page

    Private mSearch As String = ""

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        mSearch = e.Parameter?.ToString.Trim
        If mSearch Is Nothing Then mSearch = ""
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Me.ProgRingInit(True, False)
        If mSearch <> "" Then uiSearchTerm.Text = mSearch
    End Sub

    Private Async Sub uiSearchTerm_TextChanged(sender As Object, e As TextChangedEventArgs) Handles uiSearchTerm.TextChanged

        Dim sModelName As String = uiSearchTerm.Text
        If sModelName.Length < 3 Then Return

        Me.ProgRingShow(True)
        Dim lista As List(Of oneModelSummmary) = Await App.inVb.GetCurrentDb.GetModelsSummaryAsync(sModelName)
        Me.ProgRingShow(False)

        If lista Is Nothing Then Return

        uiListaTrafien.ItemsSource = lista

        ' pokaż listę modelek

        ' do każdego daj guzik <slideshow> - zakładamy że raczej nie jest to lokalnie

        ' PicMdlCheck robi Enable na guzikach browsera, gdy znajdzie takową w katalogu newDVD, archDVD, sorted

    End Sub

    Private Sub uiDoSlideshow_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dim oItem As Vblib.oneModelSummmary = TryCast(sender, FrameworkElement)?.DataContext
        If oItem Is Nothing Then Return

        Frame.Navigate(GetType(Slideshow), "1" & oItem.modelDir) ' z rekursją
    End Sub
End Class

Public Class KonwersjaBigNum
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As Integer = CType(value, Integer)

        Return temp.BigNumFormat

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaIsoPrefix
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As Long = CType(value, Integer)
        Dim sRet As String = temp.ToStringISOsufix("")
        If parameter IsNot Nothing Then
            Dim sParam As String = CType(parameter, String)
            sRet &= sParam
        End If
        Return sRet

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

' z PictMdlCheck
' wpisywanie modelki - jeśli < 3 znaki, nic nie robi
' wyszukuje trafienia w pliku z modelkami (czyli ze spacjami!), wersja zwykła oraz pozbawiona akcentów - do listbox (sorted, scrollowalny)
' podmiany akcentów - podmiana znaków wedle pliku/sekcji Accents
