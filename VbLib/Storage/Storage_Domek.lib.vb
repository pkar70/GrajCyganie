'Public Class Storage_Domek
'    Inherits Storage_BaseWeb

'    Public Overrides ReadOnly Property Nazwa As String = "DomekWeb"

'#Disable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
'    Public Overrides Async Function GetMediaSourceFrom(oStoreFile As oneStoreFiles) As Task(Of Windows.Media.Core.MediaSource)
'#Enable Warning BC42356 ' This async method lacks 'Await' operators and so will run synchronously
'        Dim sUri As String = oStoreFile.path.Substring(3) ' u:\...
'        If Not String.IsNullOrEmpty(oStoreFile.name) Then sUri = sUri & "\" & oStoreFile.name
'        sUri = sUri.Replace("#", "%23")
'        Dim oUri As Uri = New Uri(msBaseUri & sUri)
'        Return Windows.Media.Core.MediaSource.CreateFromUri(oUri)
'    End Function

'End Class
