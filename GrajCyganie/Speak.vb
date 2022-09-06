' 2022.09.05

' ma tu trafić to co dotyczy gadania, z mainpage (zapowiedzi) oraz testowanie (nowa funkcjonalność)

Imports vb14 = Vblib.pkarlibmodule14


Public Module Speak

#Region "rozpoznawanie języków"
    ' ta część może być potem jako Extension do String zrobiona - może się kiedyś przydać

    Private Function StringIsPolish(sString As String) As Boolean

        Dim sT As String = sString.ToLowerInvariant
        If sT.Contains("ą") Then Return True
        If sT.Contains("ć") Then Return True
        If sT.Contains("ę") Then Return True
        If sT.Contains("ł") Then Return True
        If sT.Contains("ń") Then Return True
        If sT.Contains("ó") Then Return True
        If sT.Contains("ś") Then Return True
        If sT.Contains("ż") Then Return True
        If sT.Contains("ź") Then Return True

        Return False
    End Function

    Private Function StringIsFrench(sString As String) As Boolean
        ' https://mylanguages.org/french_alphabet.php

        Dim sT As String = sString.ToUpperInvariant
        If sT.Contains("Ç") Then Return True
        If sT.Contains("Œ") Then Return True
        If sT.Contains("Æ") Then Return True

        If sT.Contains("Â") Then Return True
        If sT.Contains("Ê") Then Return True
        If sT.Contains("Î") Then Return True
        If sT.Contains("Ô") Then Return True
        If sT.Contains("Û") Then Return True

        If sT.Contains("É") Then Return True

        If sT.Contains("À") Then Return True
        If sT.Contains("È") Then Return True
        If sT.Contains("Ì") Then Return True
        If sT.Contains("Ò") Then Return True
        If sT.Contains("Ù") Then Return True

        If sT.Contains("Ë") Then Return True
        If sT.Contains("Ï") Then Return True
        ' If sT.Contains("Ü") Then Return True ' także niemieckie, hiszpanskie


        Return False
    End Function

    Private Function StringIsGerman(sString As String) As Boolean

        If sString.Contains("ß") Then Return True

        Dim sT As String = sString.ToUpperInvariant
        If sT.Contains("Ä") Then Return True
        If sT.Contains("Ö") Then Return True
        ' If sT.Contains("Ü") Then Return True ' także francuskie, hiszpanskie

        Return False
    End Function

    Private Function StringIsSpanish(sString As String) As Boolean

        Dim sT As String = sString.ToUpperInvariant

        If sT.Contains("Ñ") Then Return True
        If sT.Contains("¡") Then Return True
        If sT.Contains("¿") Then Return True

        If sT.Contains("Á") Then Return True
        ' If sT.Contains("É") Then Return True ' także francuskie
        If sT.Contains("Í") Then Return True
        If sT.Contains("Ó") Then Return True
        If sT.Contains("Ú") Then Return True

        ' If sT.Contains("Ü") Then Return True ' także francuskie, niemieckie

        Return False
    End Function

    Private Function RozpoznajJezykStringu(sString As String) As String
        If StringIsPolish(sString) Then Return "PL"
        If StringIsFrench(sString) Then Return "FR"
        If StringIsGerman(sString) Then Return "DE"
        If StringIsSpanish(sString) Then Return "ES"

        Return ""
    End Function

#End Region

    Private Function GetVoiceForLang(sLang As String, bMale As Boolean) As Windows.Media.SpeechSynthesis.VoiceInformation
        vb14.DumpCurrMethod(sLang & ", " & bMale)
        Dim cGender As Windows.Media.SpeechSynthesis.VoiceGender
        If bMale Then
            cGender = Windows.Media.SpeechSynthesis.VoiceGender.Male
        Else
            cGender = Windows.Media.SpeechSynthesis.VoiceGender.Female
        End If

        For Each oVoice As Windows.Media.SpeechSynthesis.VoiceInformation In Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices
            If oVoice.Language.ToUpperInvariant.StartsWith(sLang) AndAlso oVoice.Gender = cGender Then Return oVoice
        Next

        ' nie ma tej płci, no to drugiej płci - ważniejszy przecież język niż płeć
        For Each oVoice As Windows.Media.SpeechSynthesis.VoiceInformation In Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices
            If oVoice.Language.ToUpperInvariant.StartsWith(sLang) Then Return oVoice
        Next

        Return Windows.Media.SpeechSynthesis.SpeechSynthesizer.DefaultVoice

    End Function


    Public Async Function SpeakOdczytajStringAsync(sString As String, sLang As String, bMale As Boolean) As Task
        vb14.DumpCurrMethod(sString & ", " & sLang)

        If App.gbNoSpeak Then Return

        Try

            Dim oSynth As New Windows.Media.SpeechSynthesis.SpeechSynthesizer
            ' 15063 ma oSynth.Options którymi warto byłoby sie pobawić

            oSynth.Voice = GetVoiceForLang(sLang, bMale)

            Dim oStream As Windows.Media.SpeechSynthesis.SpeechSynthesisStream = Await oSynth.SynthesizeTextToStreamAsync(sString)

            Dim oMSource As Windows.Media.Core.MediaSource
            oMSource = Windows.Media.Core.MediaSource.CreateFromStream(oStream, oStream.ContentType)
            Grajek_SetSource(oMSource)

            Grajek_Play()
        Catch ex As Exception
            App.gbNoSpeak = True
        End Try


    End Function

    Public Function RozpoznajJezykUtworu(oGranyUtwor As Vblib.tGranyUtwor) As String
        Dim sLang As String

        sLang = RozpoznajJezykStringu(oGranyUtwor.oAudioParam.artist)
        If sLang <> "" Then Return sLang

        If sLang = "" Then sLang = RozpoznajJezykStringu(oGranyUtwor.oAudioParam.title)
        If sLang <> "" Then Return sLang

        Dim sT As String = oGranyUtwor.oStoreFile.path.ToLowerInvariant
        If sT.IndexOf("/pol/") > -1 Then Return "PL"
        If sT.IndexOf("/polcd/") > -1 Then Return "PL"
        If sT.IndexOf("/winylownia/") > -1 Then Return "PL" ' choć nie tylko polskie tam są
        If sT.IndexOf("/polskie/") > -1 Then Return "PL"

        Return "EN" ' default, *TODO* może być zmienny w jakichś ustawieniach

    End Function

    Public Function OpiszVoice(oVoice As Windows.Media.SpeechSynthesis.VoiceInformation) As String
        Return oVoice.DisplayName & " (" & oVoice.Language & ", " & oVoice.Gender & ")"
    End Function

End Module
