﻿
'Public Class Storage_Local
'    Inherits Storage_Base

'    Public Overrides ReadOnly Property Nazwa As String = "DyskL"

'    Public Overrides Async Function GetMediaSourceFrom(oStoreFile As oneStoreFiles) As Task(Of Windows.Media.Core.MediaSource)

'        Dim sPath As String = oStoreFile.path
'        If sPath.ToLower.StartsWith("u:\") Then sPath = sPath.Substring(3)     ' u:\...
'        sPath = TranslateMatrixPathToLocal(sPath)
'        sPath = sPath.Replace("/", "\")
'        Dim oFile As Windows.Storage.StorageFile = Await Windows.Storage.StorageFile.GetFileFromPathAsync(sPath)

'        Return Windows.Media.Core.MediaSource.CreateFromStorageFile(oFile)

'    End Function

'    Private Function TranslateMatrixPathToLocal(sPath As String) As String
'        ' private\
'        ' pkar\
'        If sPath.ToLower.StartsWith("pkar") Then Return "L:\" & sPath.Substring(5)
'        If sPath.ToLower.StartsWith("public") Then Return "L:\" & sPath.Substring(7)

'        Throw New Exception("FAIL: storage_local.TranslateMatrixPathToLocal unknown prefix in path=" & sPath)

'    End Function
'End Class
