
' start: 2021.10.17

' od strony tejże app powinno być:
' filmy         - TAK, filmy oraz aktorzy (oddzielne z main) - dodatkowo może być "z tego filmu (tt) widziałeś..."
' biblioteka    - NIE, bo to jest ograniczone wyszukiwanie, więc oddzielnego nie trzeba
' muzyka        - TAK, bo wyszukiwanie albumów etc., sprawdzanie przy ściaganiu czy warto ściągać
' Graj Cyganie  - NIE, bo od tego jest inna app
' wyszukiwarka plikow - TAK
' browser       - chyba nie korzystam
' browser priv  - chyba nie korzystam
' Tworzenie linkow albumowych   - w ogole z tego nie korzystam, poza tym jest app fotoramka ?
' podglad logow - ewentualnie

Partial Public NotInheritable Class MainPage
    Inherits Page

    Private Sub uiGoLogin_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Loginy))
    End Sub

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        ProgRingInit(True, False)

        ProgRingShow(True)
        Dim sMsg As String = CosmosConnect()
        ProgRingShow(False)
        If Not String.IsNullOrEmpty(sMsg) Then Await DialogBoxAsync(sMsg)

    End Sub

    Private Sub uiGoSearch_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(SzukajPliku))
    End Sub

    Private Sub uiGoAktorzy_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(aktorzy))
    End Sub
End Class
