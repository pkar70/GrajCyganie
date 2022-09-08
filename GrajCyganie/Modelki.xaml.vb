Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions

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
        mSearch = e.Parameter?.ToString
        If mSearch Is Nothing Then mSearch = ""
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        If mSearch <> "" Then uiSearchTerm.Text = mSearch
    End Sub

    Private Sub uiSearchTerm_TextChanged(sender As Object, e As TextChangedEventArgs) Handles uiSearchTerm.TextChanged
        ' *TODO* poszukaj modelek pasujących do stringu
    End Sub

    Private Function NameToPathQuery(sName As String) As String
        ' odpowiednik tworzenia maski do nazwy katalogu
        Dim sWhere As String = Vblib.dbase_baseASP.ConvertQueryParam(sName)
        sWhere = "_:\Public\MyProduction\Models\" & sName.Substring(0, 1) & "\" & sName.Substring(0, 2) & "\" & sWhere
        Return sWhere
    End Function
End Class
