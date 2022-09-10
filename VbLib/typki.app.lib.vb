' 2021.10.27
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
