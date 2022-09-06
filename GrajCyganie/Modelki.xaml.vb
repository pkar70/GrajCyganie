Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions

' proste wyszukiwanie - pole tekstowe, ale z maskami
' pokazywanie znalezionych modelek
' browser plików w ramach modelki?
' slideshow w ramach modelki
' później może byc slideshow, przerywany w momencie końca grania muzyki (czyli piosenka ilustrowana zdjęciami)


Public NotInheritable Class Modelki
    Inherits Page

    Private mSearch As String = ""

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        mSearch = e.Parameter?.ToString
        If mSearch Is Nothing Then mSearch = ""
    End Sub
End Class
