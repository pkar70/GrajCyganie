Public Class oneLogin
    Public Property id As String
    Public Property sUserName As String
    Public Property sLimitPath As String
    Public Property bAlsoPriv As Boolean
    Public Property bAlsoTajne As Boolean
    Public Property bNoLinks As Boolean = False

End Class

Public Class oneFile
    Public Property id As String
    Public Property name As String
    Public Property path As String
    Public Property isDir As Boolean
    Public Property filedate As String
    Public Property len As Long
    Public Property del As Boolean
    '"_rid": "ncweAL39oCgCAAAAAAAAAA==",
    '"_self": "dbs/ncweAA==/colls/ncweAL39oCg=/docs/ncweAL39oCgCAAAAAAAAAA==/",
    '"_etag": "\"090076f3-0000-0e00-0000-616d72e10000\"",
    '"_attachments": "attachments/",
    'Public Property _ts As Long    '1634562785
End Class

Public Class oneAudio

    Public Property id As String    '" "1",
    Public Property fileID As Long    '": 1296117,
    Public Property artist As String    '": "Intimate orchestra",
    Public Property title As String    '": "Song sung blue",
    Public Property album As String    '": "Various Artists",
    Public Property comment As String    '": "",
    Public Property duration As Integer ' ": 229,
    Public Property dekada As String    '": "200x    ",
    Public Property bitrate As Integer ' ": 167,
    Public Property channels As String    '": "                                ",
    Public Property sample As Integer ' ": 0,
    Public Property vbr As Integer ' ": 0,
    Public Property year As String    '": "2002      ",
    Public Property track As String    '": "12              ",
    '"_rid": "ncweAMVBuK4BAAAAAAAAAA==",
    '"_self": "dbs/ncweAA==/colls/ncweAMVBuK4=/docs/ncweAMVBuK4BAAAAAAAAAA==/",
    '"_etag": "\"2300e3d1-0000-0e00-0000-616dbc290000\"",
    '"_attachments": "attachments/",
    '"_ts": 1634581545
End Class

