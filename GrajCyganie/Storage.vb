﻿Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions


Public Module Storage


    Private Async Function GetFileFromPath(sPath As String) As Task(Of Windows.Storage.StorageFile)
        vb14.DumpCurrMethod("sPath=" & sPath)

        Dim oFile As Windows.Storage.StorageFile
        Dim sMsg As String = ""
        Try
            oFile = Await Windows.Storage.StorageFile.GetFileFromPathAsync(sPath)
            Return oFile
        Catch ex As UnauthorizedAccessException
            sMsg = "PermissionDenied, więc może Włącz permissiony: Apps » Settings » Privacy » File System"
        Catch ex As FileNotFoundException
            sMsg = ""
        Catch ex As Exception
            sMsg = ex.Message
        End Try

        If sMsg = "" Then Return Nothing

        Await vb14.DialogBoxAsync(sMsg)
        Return Nothing

    End Function



    Public Async Function GetMediaSourceFrom(oStoreFile As Vblib.oneStoreFiles) As Task(Of Windows.Media.Core.MediaSource)
        vb14.DumpCurrMethod()

        Dim sPath As String = oStoreFile.path
        If sPath.ToLower.StartsWith("u:") Then sPath = sPath.Substring(3)     ' u:\...
        ' teraz sPath zaczyna sie od pkar badz od public

        If Not sPath.ToLower.StartsWith("pkar") And Not sPath.ToLower.StartsWith("public") Then
            Await vb14.DialogBoxAsync("Uknown prefix in path? " & vbCrLf & sPath)
            Return Nothing
        End If

        sPath = sPath.Replace("/", IO.Path.DirectorySeparatorChar)

        Dim sPath1 As String = ""
        Dim oFile As Windows.Storage.StorageFile

        ' najpierw plik lokalnie jak jest
        If vb14.GetSettingsString("uiLocalPath") <> "" Then
            ' moje L:\, bez podziału na priv/public
            If sPath.ToLower.StartsWith("pkar") Then sPath1 = sPath.Substring(5)
            If sPath.ToLower.StartsWith("public") Then sPath1 = sPath.Substring(7)
            sPath1 = vb14.GetSettingsString("uiLocalPath") & sPath1
            oFile = Await GetFileFromPath(sPath1)
            If oFile IsNot Nothing Then Return Windows.Media.Core.MediaSource.CreateFromStorageFile(oFile)

            ' lokalnie (co może być dysk external!), z podziałem na priv/public
            sPath1 = vb14.GetSettingsString("uiLocalPath") & sPath
            oFile = Await GetFileFromPath(sPath1)
            If oFile IsNot Nothing Then Return Windows.Media.Core.MediaSource.CreateFromStorageFile(oFile)
        End If

        ' potem wedle pliku z lokalnego cache OneDrive - ale tylko wtedy gdy jest sieć, inaczej nie ma sensu :)
        If vb14.GetSettingsString("uiLocalODPath") <> "" Then
            If NetIsIPavailable() Then
                sPath1 = vb14.GetSettingsString("uiLocalODPath") & sPath
                oFile = Await GetFileFromPath(sPath1)
                If oFile IsNot Nothing Then Return Windows.Media.Core.MediaSource.CreateFromStorageFile(oFile)
            Else
                Await vb14.DialogBoxAsync("Nie ma lokalnie, a do OD potrzebuję sieci")
                Return Nothing
            End If
        End If

        ' a potem się poddajemy
        Await vb14.DialogBoxAsync("Ale sorry, nie mogę znaleźć pliku")
        Return Nothing

    End Function

End Module