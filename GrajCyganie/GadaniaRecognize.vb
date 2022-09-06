Imports vb14 = Vblib.pkarlibmodule14

Public Module GadaniaRecognize

#Region "Speech control"
    Private moReco As Windows.Media.SpeechRecognition.SpeechRecognizer = Nothing

    Private Function MakeRule(sTag As String, Optional sStr1 As String = "", Optional sStr2 As String = "", Optional sStr3 As String = "", Optional sStr4 As String = "", Optional sStr5 As String = "") As Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint
        Dim oList As List(Of String)
        oList = New List(Of String)
        oList.Clear()
        If sStr1 <> "" Then
            oList.Add(sStr1)
        Else
            oList.Add(sTag)
        End If
        If sStr2 <> "" Then oList.Add(sStr2)
        If sStr3 <> "" Then oList.Add(sStr3)
        If sStr4 <> "" Then oList.Add(sStr4)
        If sStr5 <> "" Then oList.Add(sStr5)

        Return New Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint(oList, sTag)

    End Function

    Private Sub SpeechCommandCreateRules()

        moReco.Constraints.Clear()
        moReco.Constraints.Add(MakeRule("play", "play", "start"))
        moReco.Constraints.Add(MakeRule("stop"))
        moReco.Constraints.Add(MakeRule("pause", "pause", "silence"))
        moReco.Constraints.Add(MakeRule("cont", "continue", "resume"))
        moReco.Constraints.Add(MakeRule("next"))
        moReco.Constraints.Add(MakeRule("prev", "previous", "back"))
        moReco.Constraints.Add(MakeRule("info", "info", "describe"))
        moReco.Constraints.Add(MakeRule("loopArt", "loop artist"))
        moReco.Constraints.Add(MakeRule("loopTitle", "loop title"))
        moReco.Constraints.Add(MakeRule("loopAlbum", "loop album"))
        moReco.Constraints.Add(MakeRule("loopYear", "loop year"))
        moReco.Constraints.Add(MakeRule("loopDecade", "loop decade"))
        moReco.Constraints.Add(MakeRule("loopSong", "loop song"))
        moReco.Constraints.Add(MakeRule("loopNone", "loop none", "no loop"))
        moReco.Constraints.Add(MakeRule("stat", "stats", "how many", "statistic"))
        moReco.Constraints.Add(MakeRule("before"))
        moReco.Constraints.Add(MakeRule("after"))

    End Sub

    Private Sub SpeechCommandSetTimeouts()
        moReco.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.FromDays(60)
        moReco.Timeouts.InitialSilenceTimeout = TimeSpan.FromDays(60)
        moReco.Timeouts.BabbleTimeout = TimeSpan.FromDays(60)
        moReco.Timeouts.EndSilenceTimeout = TimeSpan.FromDays(60)
    End Sub

    Private Function SpeechCommandText2Tag(sTxt As String) As String
        For Each oRule As Windows.Media.SpeechRecognition.SpeechRecognitionListConstraint In moReco.Constraints
            For Each sCmd As String In oRule.Commands
                If sCmd = sTxt Then Return oRule.Tag
            Next
        Next

        Return ""
    End Function

    Public Async Function SpeechOnOffAsync(bStart As Boolean) As Task
        Try

            If bStart Then
                Await moReco.ContinuousRecognitionSession.StartAsync()
            Else
                Await moReco.ContinuousRecognitionSession.StopAsync()
            End If

        Catch ex As Exception
        End Try
    End Function

    Public Async Function SpeechRecoInit(oSpeechCmd As _SpeechCommand) As Task(Of Boolean)
        vb14.DumpCurrMethod()

        SpeechCommand = oSpeechCmd
        If SpeechCommand Is Nothing Then Throw New ArgumentNullException("Ale oSpeechCmd is NUL!")

        Try

            If moReco Is Nothing Then
                moReco = New Windows.Media.SpeechRecognition.SpeechRecognizer()
                SpeechCommandCreateRules()
                Await moReco.CompileConstraintsAsync()
                SpeechCommandSetTimeouts()

                AddHandler moReco.HypothesisGenerated, AddressOf recoNewHypo
                AddHandler moReco.ContinuousRecognitionSession.ResultGenerated, AddressOf recoGetText

            End If

            If moReco.State = Windows.Media.SpeechRecognition.SpeechRecognizerState.Idle Then
                Try
                    Await moReco.ContinuousRecognitionSession.StartAsync()
                Catch ex As Exception

                End Try
            End If
            Return True
        Catch ex As Exception
            vb14.DumpMessage("ERROR connecting to speech recognizer")
            Return False
        End Try
    End Function

    Public Delegate Sub _SpeechCommand(sVoiceCmd As String)
    Private SpeechCommand As _SpeechCommand

    Private Sub recoNewHypo(sender As Windows.Media.SpeechRecognition.SpeechRecognizer, args As Windows.Media.SpeechRecognition.SpeechRecognitionHypothesisGeneratedEventArgs)
        vb14.DumpCurrMethod()
        SpeechCommandCaller("", args.Hypothesis.Text)
    End Sub

    Private Sub recoGetText(sender As Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession, args As Windows.Media.SpeechRecognition.SpeechContinuousRecognitionResultGeneratedEventArgs)
        vb14.DumpCurrMethod()
        Dim sTxt As String
        If args.Result.Constraint IsNot Nothing Then
            sTxt = args.Result.Constraint.Tag
        Else
            sTxt = ""
        End If
        SpeechCommandCaller(sTxt, args.Result.Text)

    End Sub

    Private Sub SpeechCommandCaller(sTag As String, sTxt As String)
        vb14.DumpCurrMethod(sTag & ", " & sTxt)

        Dim sVoiceCmd As String = sTag
        If sTag <> "" Then
            sVoiceCmd = sTag
        Else
            sVoiceCmd = SpeechCommandText2Tag(sTxt)
        End If

        SpeechCommand(sVoiceCmd)

    End Sub
#End Region

End Module
