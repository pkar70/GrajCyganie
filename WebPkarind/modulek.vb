Module modulek

#Region "konwersja MASK"

    'Function ConvertMask(sMask As String) As String
    '    sMask = sMask.Replace(".", "\.")

    '    ' SQL
    '    sMask = sMask.Replace("_", ".")
    '    sMask = sMask.Replace("%", "*")

    '    ' DOS
    '    sMask = sMask.Replace("?", ".")

    '    Return sMask
    'End Function

    'Function ConvertMask(sMask As String) As String
    '    Dim sTmp As String = sMask
    '    sTmp = ConvertMaskPrefix(sTmp)
    '    sTmp = ConvertMaskSufix(sTmp)
    '    Return sTmp
    'End Function

    'Function ConvertMaskPrefix(sMask As String) As String
    '    Dim sTmp As String = sMask
    '    If sMask.Length > 1 Then
    '        If Left(sTmp, 1) <> "*" Then
    '            If Left(sTmp, 1) = "^" Then
    '                sTmp = Mid(sTmp, 2)
    '            Else
    '                sTmp = "*" & sTmp
    '            End If
    '        End If
    '    End If
    '    Return sTmp
    'End Function

    'Function ConvertMaskSufix(sMask As String) As String
    '    Dim sTmp As String = sMask
    '    If sMask.Length > 1 Then
    '        If Right(sTmp, 1) <> "*" Then
    '            If Right(sTmp, 1) = "$" Then
    '                sTmp = Left(sTmp, Len(sTmp) - 1)
    '            Else
    '                sTmp = sTmp & "*"
    '            End If
    '        End If
    '    End If
    '    Return sTmp
    'End Function
#End Region

    Function StorageFileToHtmlRow(oItem As oneFile, bNoLinks As Boolean, Optional sPathPrefixStrip As String = "") As HtmlTableRow

        Dim oNewRow As New HtmlTableRow

        ' nazwa pliku, ewentualnie LINK
        Dim oNewCell As New HtmlTableCell()
        If bNoLinks OrElse oItem.len < 1 Then
            oNewCell.InnerText = oItem.name
        Else
            'Dim oLink As New HtmlLink
            'oLink.Href = ""
            Dim sLink As String = oItem.path.Substring(3) & "/" & oItem.name
            sLink = sLink.Replace("\", "/")
            sLink = sLink.Replace(" ", "%20")

            oNewCell.InnerHtml = "<a href='" & gmStorageUri & sLink & "'>" & oItem.name & "</a>"
        End If
        oNewRow.Cells.Add(oNewCell)

        ' czas pliku
        oNewCell = New HtmlTableCell()
        oNewCell.InnerText = oItem.filedate
        oNewRow.Cells.Add(oNewCell)

        ' len
        oNewCell = New HtmlTableCell()
        oNewCell.InnerText = Int2BigString(oItem.len)
        oNewCell.Align = "right"
        oNewRow.Cells.Add(oNewCell)

        ' path
        oNewCell = New HtmlTableCell()
        If sPathPrefixStrip <> "" Then
            Dim iInd As Integer = oItem.path.ToLower.IndexOf(sPathPrefixStrip.ToLower)
            If iInd < 5 AndAlso iInd > -1 Then
                oNewCell.InnerText = ".\" & oItem.path.Substring(iInd + sPathPrefixStrip.Length)
            Else
                oNewCell.InnerText = oItem.path
            End If
        Else
            oNewCell.InnerText = oItem.path
        End If
        oNewRow.Cells.Add(oNewCell)

        Return oNewRow
    End Function

    Function Int2BigString(iNum As Integer) As String

        Dim sNum As String = iNum

        ' 1234
        If sNum.Length < 5 Then Return sNum

        sNum = sNum.Substring(0, sNum.Length - 3) & " " & sNum.Substring(sNum.Length - 3)
        ' 123 456
        If sNum.Length < 8 Then Return sNum

        sNum = sNum.Substring(0, sNum.Length - 7) & " " & sNum.Substring(sNum.Length - 7)
        ' 123 456 789
        If sNum.Length < 11 Then Return sNum

        sNum = sNum.Substring(0, sNum.Length - 11) & " " & sNum.Substring(sNum.Length - 11)
        If sNum.Length < 15 Then Return sNum

        sNum = sNum.Substring(0, sNum.Length - 15) & " " & sNum.Substring(sNum.Length - 15)

        Return sNum
    End Function

End Module
