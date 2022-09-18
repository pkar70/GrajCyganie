' przeniesienie części funkcjonalności z Mp3Check

Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions

Public NotInheritable Class Mp3Add
    Inherits Page

    Private Shared mListaPlikow As List(Of oneMp3AddFile)

    Private Async Sub uiBrowse_Click(sender As Object, e As RoutedEventArgs)

        ' pick folder
        Dim oFold As Windows.Storage.StorageFolder = Await PickFolder()
        If oFold Is Nothing Then Return
        uiFolder.Text = oFold.Path

        ' get files from folder
        mListaPlikow = Await GetAllFiles(oFold)
        If mListaPlikow Is Nothing Then Return

        ' put into uiListaPlikow
        uiListaPlikow.ItemsSource = mListaPlikow
    End Sub

    Private Async Function GetAllFiles(oFold As Windows.Storage.StorageFolder) As Task(Of List(Of oneMp3AddFile))
        Dim listaPlikow As New List(Of oneMp3AddFile)

        For Each oFile In Await oFold.GetFilesAsync

            If ".m3u.jpg.log.txt.cue".Contains(oFile.FileType) Then Continue For

            Dim oNew As New oneMp3AddFile
            oNew.sFilePathName = IO.Path.Combine(oFile.Path, oFile.Name)

            oNew.oAudioParamDiskFile = Await MainPage.OdczytajMp3InfoAsync(oFile)
            listaPlikow.Add(oNew)
        Next

        Return listaPlikow
    End Function

    Private Async Function PickFolder() As Task(Of Windows.Storage.StorageFolder)
        Dim folderPicker As New Windows.Storage.Pickers.FolderPicker
        folderPicker.FileTypeFilter.Add("*")

        Dim folder As Windows.Storage.StorageFolder = Await folderPicker.PickSingleFolderAsync()
        ' Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder)
        Return folder
    End Function

    Private Async Sub uiSzukaj_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As oneMp3AddFile = TryCast(sender, FrameworkElement).DataContext

        ' niby przeskok do szukarki mógłby być, ale zrobimy "po swojemu"

        Dim oProps As Vblib.oneAudioParam = oItem.oAudioParamDiskFile
        Dim lista As List(Of Vblib.oneAudioParam)
        lista = Await App.inVb.GetCurrentDb.SearchMusicAsync(oProps.artist, oProps.title, "", "")

        If lista Is Nothing Then Return
        oItem.lMatches = lista

        uiListaZnanych.ItemsSource = lista

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        If mListaPlikow IsNot Nothing Then uiListaPlikow.ItemsSource = mListaPlikow
    End Sub
End Class

Public Class oneMp3AddFile
    Public Property oAudioParamDiskFile As Vblib.oneAudioParam
    Public Property sFilePathName As String

    Public Property lMatches As List(Of Vblib.oneAudioParam)

End Class