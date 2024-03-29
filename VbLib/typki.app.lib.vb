﻿' 2021.10.27
' zabrane z App.xaml.vb

Public Class tGranyUtwor

    Public Property oStoreFile As New oneStoreFile
    Public Property oAudioParam As New oneAudioParam
    Public Property oAudioParamFile As New oneAudioParam
    Public Property countArtist As Integer = 0
    Public Property countAlbum As Integer = 0
    Public Property countTitle As Integer = 0
    Public Property countYear As Integer = 0
    Public Property countDekada As Integer = 0

End Class

Public Enum eNextMode
    random
    sameArtist
    sameTitle
    sameAlbum
    sameRok
    sameDekada
    loopSong
End Enum

Public Class oneModelSummmary
    Public Property modelDir As String
    Public Property items As Integer
    Public Property total As Long
End Class

Public Class oneAlbumForArtist
    Public Property artist As String
    Public Property album As String
    Public Property tracks As Integer
    Public Property totalTime As Integer
End Class