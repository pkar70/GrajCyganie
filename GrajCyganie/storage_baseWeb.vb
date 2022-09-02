Partial Public MustInherit Class Storage_BaseWeb
    Inherits Storage_Base

    Public MustOverride Overrides ReadOnly Property Nazwa As String

    Public MustOverride Overrides Async Function GetMediaSourceFrom(oStoreFile As Vblib.oneStoreFiles) As Task(Of Windows.Media.Core.MediaSource)

End Class