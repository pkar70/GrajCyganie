' 2021.10.27
' wszystko związane z dostępem do plików
' - wydzielone, by było łatwo zmieniać między... lokalnym storage a ...
' pozostaje też wersja Beskid, tak na wszelki wypadek

Public MustInherit Class Storage_Base

    Public MustOverride Async Function GetMediaSourceFrom(oStoreFile As Vblib.oneStoreFiles) As Task(Of Windows.Media.Core.MediaSource)
    Public MustOverride ReadOnly Property Nazwa As String
End Class
