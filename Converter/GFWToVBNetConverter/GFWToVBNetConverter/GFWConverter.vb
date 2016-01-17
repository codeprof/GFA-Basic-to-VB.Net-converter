Module GFWConverter
    Public Class ConvertStringListHelper
        Public Shared Function SplitLine(ByVal line As String) As List(Of String)
            Dim list As List(Of String) = New List(Of String)
            Dim ch, ch2 As String
            Dim inComment As Boolean = False
            Dim inString As Boolean = False
            Dim current As String = ""
            Dim Done As Boolean = False
            For i = 1 To Len(line)
                Done = False
                ch = Mid$(line, i, 1)
                ch2 = Mid$(line, i, 2)

                If ch2 = "//" Or ch = "'" Then
                    If inString = False Then
                        If current <> "" Then
                            list.Add(current)
                        End If
                        current = ""
                        inComment = True 'Kommtar geht immer bis zum Ende
                    End If
                End If

                If inComment = False Then
                    If ch = Chr(34) Then

                        If inString = False Then

                            If (current <> "") Then
                                list.Add(current)
                                current = ""
                            End If
                        End If
                        inString = Not inString
                        If inString = False Then
                            current += ch
                            list.Add(current)
                            current = ""
                            Done = True
                        End If

                    End If
                End If

                If Done = False Then
                    If inString Then
                        current += ch
                    End If
                    If inComment Then
                        current += ch
                    End If

                    If inString = False And inComment = False Then
                        Select Case ch
                            Case " "
                                If Trim(current) = "" And current <> "" Then
                                    current += ch
                                Else
                                    If current <> "" Then
                                        list.Add(current)
                                    End If
                                    current = ch
                                End If
                            Case "(", ")", "+", "-", "*", "/", "=", "<", ">", ",", "~", ";", "@", "^", ":" 'Achtung # ist auch ein Datentyp! # Wird später behandelt
                                If current <> "" Then
                                    list.Add(current)
                                End If
                                list.Add(ch)
                                current = ""
                            Case Else
                                If Trim(current) = "" And current <> "" Then
                                    list.Add(current)
                                    current = ch
                                Else
                                    current += ch
                                End If
                        End Select
                    End If
                End If
            Next
            If current <> "" Then
                list.Add(current)
            End If

            Return list
        End Function
        Public Shared Function AdaptComments(ByVal list As List(Of String)) As List(Of String)
            For i = 0 To list.Count - 1
                If Left(list(i), 2) = "//" Then
                    list(i) = "'" + Right(list(i), Len(list(i)) - 2)
                End If
            Next
            Return list
        End Function
        Public Shared Function AdaptSharpChar(ByVal list As List(Of String)) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)
            For i = 0 To list.Count - 1
                If Left(list(i), 1) = "#" Then
                    newList.Add("#")
                    newList.Add(Right(list(i), Len(list(i)) - 1))
                Else
                    newList.Add(list(i))
                End If
            Next
            Return newList
        End Function
        Public Shared Function RemoveSpaces(ByVal list As List(Of String)) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)
            For i = 0 To list.Count - 1
                If Trim(list(i)) = "" Then 'And list(i) <> "" Then
                    'list(i) = " " ' Durch ein Leerzeichen austauschen
                    'Löschen
                Else
                    newList.Add(list(i))
                End If
            Next
            Return newList
        End Function
        Public Shared Function RemoveComments(ByVal list As List(Of String), ByRef oldComment As String) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)
            oldComment = ""
            For i = 0 To list.Count - 1
                If Left(list(i), 2) = "//" Then
                    oldComment += "'" + Right(list(i), Len(list(i)) - 2)
                ElseIf Left(list(i), 1) = "'" Then
                    oldComment += list(i)
                Else
                    newList.Add(list(i))
                End If
            Next
            Return newList
        End Function
        Public Shared Function RemoveComments(ByVal list As List(Of String)) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)
            For i = 0 To list.Count - 1
                If Left(list(i), 2) = "//" Or Left(list(i), 1) = "'" Then
                    'Nichts tun
                Else
                    newList.Add(list(i))
                End If
            Next
            Return newList
        End Function
        Public Shared Function CombineStrings(ByVal list As List(Of String), ByVal str1 As String, ByVal str2 As String, ByVal ignoreCase As Boolean) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)

            If ignoreCase Then
                str1 = UCase(str1)
                str2 = UCase(str2)
            End If
            For i = 0 To list.Count - 1

                If i + 2 < list.Count Then
                    Dim tmp1, tmp2 As String
                    tmp1 = list(i)
                    tmp2 = list(i + 1)

                    If ignoreCase Then
                        tmp1 = UCase(tmp1)
                        tmp2 = UCase(tmp2)
                    End If

                    If tmp1 = str1 And tmp2 = str2 Then
                        newList.Add(list(i) + list(i + 1))
                        i += 1
                    Else
                        newList.Add(list(i))
                    End If
                Else
                    newList.Add(list(i))
                End If
            Next
            Return newList
        End Function
        Public Shared Function CombineStrings(ByVal list As List(Of String), ByVal str1 As String, ByVal str2 As String, ByVal str3 As String, ByVal ignoreCase As Boolean) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)

            If ignoreCase Then
                str1 = UCase(str1)
                str2 = UCase(str2)
                str3 = UCase(str3)
            End If
            For i = 0 To list.Count - 1
                Dim tmp1, tmp2, tmp3 As String

                If i + 2 < list.Count Then
                    tmp1 = list(i)
                    tmp2 = list(i + 1)
                    tmp3 = list(i + 2)

                    If ignoreCase Then
                        tmp1 = UCase(tmp1)
                        tmp2 = UCase(tmp2)
                        tmp3 = UCase(tmp3)
                    End If

                    If tmp1 = str1 And tmp2 = str2 And tmp3 = str3 Then
                        newList.Add(list(i) + list(i + 1) + list(i + 2))
                        i += 2
                    Else
                        newList.Add(list(i))
                    End If
                Else
                    newList.Add(list(i))
                End If

            Next

            Return newList
        End Function
        'Public Shared Sub ReplaceStringListContent(ByRef list As List(Of String), ByVal searchPrev As String, ByVal search As String, ByVal replace As String, ByVal ignoreCase As Boolean, ByVal IgnorePrev As Boolean)
        '    Dim prev As String = ""
        '    If ignoreCase Then search = UCase(search)
        '    If ignoreCase Then prev = UCase(prev)
        '    For i = 0 To list.Count - 1
        '        Dim tmp As String = list(i)
        '        If ignoreCase Then tmp = UCase(tmp)
        '        If tmp = search And (prev = searchPrev Or IgnorePrev) Then
        '            list(i) = replace
        '        End If
        '        prev = tmp
        '    Next
        'End Sub
        Public Shared Function GetLines(ByVal fileName As String) As List(Of String)
            Dim newList As List(Of String) = New List(Of String)
            Dim lines() As String = IO.File.ReadAllLines(fileName, System.Text.Encoding.Default)
            For Each str As String In lines
                newList.Add(str)
            Next
            Return newList
        End Function
    End Class

    'ZUM TESTEN NOT INSTR() funktioniert nicht korrekt!

    Public Class LanguageException
        Inherits Exception

        Sub New(ByVal text As String)
            MyBase.New(text)
        End Sub
    End Class
    Public Enum LangageObjectType
        OBJTYPE_UNKNOWN = 0
        OBJTYPE_VARIABLE = 1
        OBJTYPE_ARRAY = 2
        OBJTYPE_FUNCTION = 3
        OBJTYPE_PROCEDURE = 4
        OBJTYPE_KEYWORD = 5

        OBJTYPE_STRING = 6
        OBJTYPE_NUMBER = 7
        OBJTYPE_OPERATOR = 8
    End Enum
    Public Class LanguageObject
        Protected m_Name As String
        Protected m_DataType As String
        Protected m_ObjType As LangageObjectType
        Protected m_ReplacementName As String
        Protected m_IsBuildIn As Boolean
        Protected m_Subrutine As String 'todo

        Public Sub New()
            m_Name = ""
            m_DataType = ""
            m_ObjType = LangageObjectType.OBJTYPE_UNKNOWN
            m_ReplacementName = ""
            m_IsBuildIn = False
            m_Subrutine = ""
        End Sub

        Public Sub New(ByVal name As String, ByVal dataType As String, ByVal objType As LangageObjectType)
            m_Name = name
            m_DataType = dataType
            m_ObjType = objType
            m_ReplacementName = ""
            m_IsBuildIn = False
            m_Subrutine = ""
        End Sub

        Public Function GetNewName() As String
            GetNewName = m_ReplacementName
            If GetNewName = "" Then
                GetNewName = m_Name
            End If
        End Function

        Public Function GetNewUName() As String
            GetNewUName = m_ReplacementName.ToUpper
            If GetNewUName = "" Then
                GetNewUName = m_Name.ToUpper
            End If
        End Function

        Public Property IsBuildIn() As Boolean
            Get
                Return m_IsBuildIn
            End Get
            Set(ByVal value As Boolean)
                m_IsBuildIn = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = value
            End Set
        End Property

        Public ReadOnly Property UName() As String
            Get
                Return m_Name.ToUpper
            End Get
        End Property


        Public Property ReplacementName() As String
            Get
                Return m_ReplacementName
            End Get
            Set(ByVal value As String)
                m_ReplacementName = value
            End Set
        End Property

        Public ReadOnly Property UReplacementName() As String
            Get
                Return m_ReplacementName.ToUpper
            End Get
        End Property


        Public Property ObjType() As LangageObjectType
            Get
                Return m_ObjType
            End Get
            Set(ByVal value As LangageObjectType)
                m_ObjType = value
            End Set
        End Property

        Public Property DataType() As String
            Get
                Return m_DataType
            End Get
            Set(ByVal value As String)
                Select Case value
                    Case ""
                        'Kein Typ bzw. Double
                    Case "#"
                        'Dobule
                    Case "%"
                        'Integer
                    Case "&"
                        'Short
                    Case "|"
                        'Byte
                    Case "!"
                        'Boolean
                    Case "$"
                        'String
                    Case Else
                        Debug.WriteLine(Chr(34) + value + Chr(34) + " ist kein gültiger Datentyp")
                End Select
                m_DataType = value
            End Set
        End Property

        Public Property Subrutine() As String
            Get
                Return m_Subrutine
            End Get
            Set(ByVal value As String)
                m_Subrutine = value
            End Set
        End Property

        Public ReadOnly Property USubrutine() As String
            Get
                Return m_Subrutine.ToUpper
            End Get
        End Property

    End Class
    Public Class LanguageKeyword
        Inherits LanguageObject

        Public Sub New(ByVal name As String)
            Me.m_Name = name
            Me.m_DataType = ""
            Me.m_ObjType = LangageObjectType.OBJTYPE_KEYWORD
            Me.m_ReplacementName = ""
            Me.m_IsBuildIn = True
        End Sub

        Public Sub New(ByVal name As String, ByVal replacement As String)
            Me.m_Name = name
            Me.m_DataType = ""
            Me.m_ObjType = LangageObjectType.OBJTYPE_KEYWORD
            Me.m_ReplacementName = replacement
            Me.m_IsBuildIn = True
        End Sub
    End Class
    Public Class LanguageVariable
        Inherits LanguageObject
        'Protected m_Subrutine As String

        Public Sub New()
            Me.m_Name = ""
            Me.m_DataType = ""
            Me.m_Subrutine = ""
            Me.m_ObjType = LangageObjectType.OBJTYPE_VARIABLE
            Me.m_ReplacementName = ""
            Me.m_IsBuildIn = False
        End Sub

        Public Sub New(ByVal name As String, ByVal dataType As String, ByVal subrutine As String)
            Me.m_Name = name
            Me.m_DataType = dataType
            Me.m_Subrutine = subrutine
            Me.m_ObjType = LangageObjectType.OBJTYPE_VARIABLE
            Me.m_ReplacementName = ""
            Me.m_IsBuildIn = False
        End Sub

    End Class
    Public Class LanguageArray
        Inherits LanguageVariable

        Public Sub New(ByVal name As String, ByVal dataType As String, ByVal subrutine As String)
            Me.m_Name = name
            Me.m_DataType = dataType
            Me.m_ObjType = LangageObjectType.OBJTYPE_ARRAY
            Me.m_ReplacementName = ""
            Me.m_Subrutine = subrutine
        End Sub

    End Class
    Public Class LanguageProcedure
        Inherits LanguageObject

        Public Sub New(ByVal name As String, ByVal isBuildIn As Boolean)
            Me.m_Name = name
            Me.m_DataType = ""
            Me.m_ObjType = LangageObjectType.OBJTYPE_PROCEDURE
            Me.m_ReplacementName = ""
            Me.m_IsBuildIn = isBuildIn
        End Sub

        Public Sub New(ByVal name As String, ByVal replacement As String, ByVal isBuildIn As Boolean)
            Me.m_Name = name
            Me.m_DataType = ""
            Me.m_ObjType = LangageObjectType.OBJTYPE_PROCEDURE
            Me.m_ReplacementName = replacement
            Me.m_IsBuildIn = isBuildIn
        End Sub
    End Class
    Public Class LanguageFunction
        Inherits LanguageObject

        Public Sub New(ByVal name As String, ByVal dataType As String)
            Me.m_Name = name
            Me.m_DataType = dataType
            Me.m_ObjType = LangageObjectType.OBJTYPE_FUNCTION
            Me.m_ReplacementName = ""
            Me.m_IsBuildIn = False
        End Sub

        Public Sub New(ByVal name As String, ByVal dataType As String, ByVal isBuildIn As Boolean, ByVal replaceent As String)
            Me.m_Name = name
            Me.m_DataType = dataType
            Me.m_ObjType = LangageObjectType.OBJTYPE_FUNCTION
            Me.m_ReplacementName = replaceent
            Me.m_IsBuildIn = isBuildIn
        End Sub
    End Class
    Public Class LanguageObjects
        Inherits List(Of LanguageObject)


        Public TMPOUT As IO.StreamWriter

        Public Sub AddFunction(ByVal name As String, ByVal replacement As String, ByVal dataType As String, ByVal isBuildIn As Boolean)
            Dim obj As LanguageFunction = New LanguageFunction(name, dataType, isBuildIn, replacement)
            Me.Add(obj)
        End Sub

        Public Sub AddProcedure(ByVal name As String, ByVal isBuildIn As Boolean)
            Dim obj As LanguageProcedure = New LanguageProcedure(name, isBuildIn)
            Me.Add(obj)
        End Sub

        Public Sub AddProcedure(ByVal name As String, ByVal replacement As String, ByVal isBuildIn As Boolean)
            Dim obj As LanguageProcedure = New LanguageProcedure(name, replacement, isBuildIn)
            Me.Add(obj)
        End Sub

        Public Sub AddArray(ByVal name As String, ByVal dataType As String, ByVal subrutine As String)
            Dim obj As LanguageArray = New LanguageArray(name, dataType, subrutine)
            Me.Add(obj)
        End Sub

        Public Sub AddKeyword(ByVal name As String, ByVal replacement As String)
            Dim obj As LanguageKeyword = New LanguageKeyword(name, replacement)
            Me.Add(obj)
        End Sub

        Public Sub AddVariable(ByVal name As String, ByVal dataType As String, ByVal subrutine As String)
            Dim obj As LanguageVariable = New LanguageVariable(name, dataType, subrutine)
            Me.Add(obj)
        End Sub

        Public Sub AddVariable(ByVal name As String, ByVal dataType As String)
            Dim obj As LanguageVariable = New LanguageVariable(name, dataType, "")
            Me.Add(obj)
        End Sub

        Public Sub RemoveBuildIn()
            For i = Me.Count - 1 To 0 Step -1
                If Me(i).IsBuildIn Then
                    Me.RemoveAt(i)
                End If
            Next
        End Sub

        Public Function FindObjType(ByVal name As String, ByVal objType As LangageObjectType)
            name = name.ToUpper
            For Each obj As LanguageObject In Me
                If obj.ObjType = objType Then
                    If obj.UName = name Then
                        Return obj
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Function FindObjType(ByVal name As String, ByVal objType As LangageObjectType, ByVal IgnoreBuildIn As Boolean)
            name = name.ToUpper
            For Each obj As LanguageObject In Me
                If IgnoreBuildIn = False Or obj.IsBuildIn = False Then
                    If obj.ObjType = objType Then
                        If obj.UName = name Then
                            Return obj
                        End If
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Function FindObjType(ByVal name As String, ByVal dataType As String, ByVal objType As LangageObjectType)
            name = name.ToUpper
            For Each obj As LanguageObject In Me
                If obj.ObjType = objType Then
                    If obj.DataType = dataType Then
                        If obj.UName = name Then
                            Return obj
                        End If
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Function FindObjType(ByVal name As String, ByVal dataType As String, ByVal objType As LangageObjectType, ByVal IgnoreBuildIn As Boolean)
            name = name.ToUpper
            For Each obj As LanguageObject In Me
                If IgnoreBuildIn = False Or obj.IsBuildIn = False Then
                    If obj.ObjType = objType Then
                        If obj.DataType = dataType Then
                            If obj.UName = name Then
                                Return obj
                            End If
                        End If
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Function FindObjType(ByVal name As String, ByVal dataType As String, ByVal subrutine As String, ByVal objType As LangageObjectType)
            name = name.ToUpper
            For Each obj As LanguageObject In Me
                If obj.ObjType = objType Then
                    If obj.DataType = dataType Then
                        If obj.UName = name Then
                            If obj.Subrutine = subrutine Then
                                Return obj
                            End If
                        End If
                    End If
                End If
            Next
            Return Nothing
        End Function

        Public Function IsNameConflict(ByVal name As String, ByVal subrutine As String, ByVal isvariable As Boolean)
            name = name.ToUpper
            subrutine = subrutine.ToUpper
            For Each obj As LanguageObject In Me

                'IST DAS GUT SO???

                If isvariable Then

                    If subrutine = obj.USubrutine Or obj.USubrutine = "" Or subrutine = "" Then ' muss auch sein, da sonst nicht unbedingt eideutig!!!
                        If obj.GetNewUName = name Then
                            Return True
                        End If
                    End If

                Else

                    If obj.GetNewUName = name Then
                        Return True
                    End If

                End If

                'If obj.UName = name Then 'TODO: Lokale Variablen, Funktionen!!!
                '    Return True
                'End If
                'If obj.UReplacementName = name Then 'muss auch beachtet werden!
                '    Return True
                'End If
            Next
            Return False
        End Function

        Private Function SearchConflictFreeName(ByVal name As String, ByVal subrutine As String, ByVal isvariable As Boolean, ByVal obj As LanguageObject) As String
            Dim confl As Boolean = False
            Dim orgname As String = name
            Do
                name += "_"
                confl = IsNameConflict(name, subrutine, isvariable)
            Loop Until confl = False

            TMPOUT.WriteLine("rename " + orgname + " to " + name + " (in " + obj.Subrutine + ") Type:" + obj.ToString)
            Return name
        End Function

        Private Function SearchConflictFreeName(ByVal name As String, ByVal obj As LanguageObject) As String
            Dim confl As Boolean = False
            Dim orgname As String = name
            Do
                name += "_"
                confl = IsNameConflict(name, "", False)
            Loop Until confl = False

            TMPOUT.WriteLine("rename " + orgname + " to " + name + " (in " + obj.Subrutine + ") Type:" + obj.ToString)
            Return name
        End Function

        Dim globalVarRenames As Hashtable = New Hashtable


        Public Sub SolveConflict(ByVal obj1 As LanguageObject, ByVal obj2 As LanguageObject)
            Dim var1, var2 As LanguageVariable
            Dim arr1, arr2 As LanguageArray
            Dim func1, func2 As LanguageFunction
            Dim proc1, proc2 As LanguageProcedure
            If obj1 IsNot obj2 Then 'sollte bereits vorher ausgeschlossen sein


                If obj1.ObjType = LangageObjectType.OBJTYPE_PROCEDURE And obj2.ObjType = LangageObjectType.OBJTYPE_PROCEDURE Then
                    proc1 = obj1
                    proc2 = obj2

                    If proc1.IsBuildIn = False Then
                        proc1.ReplacementName = SearchConflictFreeName(proc1.Name, proc1)
                    Else
                        If proc2.IsBuildIn = False Then
                            proc2.ReplacementName = SearchConflictFreeName(proc2.Name, proc2)
                        Else
                            Debug.WriteLine("Conflict! No Procedure of " + proc1.Name + " and " + proc2.Name + " can be renamed!")
                        End If
                    End If
                End If


                If obj1.ObjType = LangageObjectType.OBJTYPE_FUNCTION And obj2.ObjType = LangageObjectType.OBJTYPE_FUNCTION Then
                    func1 = obj1
                    func2 = obj2

                    If func1.IsBuildIn = False Then
                        func1.ReplacementName = SearchConflictFreeName(func1.Name, func1)
                    Else
                        If func2.IsBuildIn = False Then
                            func2.ReplacementName = SearchConflictFreeName(func2.Name, func2)
                        Else
                            Debug.WriteLine("Conflict! No Function of " + func1.Name + " and " + func2.Name + " can be renamed!")
                        End If
                    End If
                End If


                If obj1.ObjType = LangageObjectType.OBJTYPE_FUNCTION And obj2.ObjType = LangageObjectType.OBJTYPE_PROCEDURE Then
                    func1 = obj1
                    proc2 = obj2

                    If func1.IsBuildIn = False Then
                        func1.ReplacementName = SearchConflictFreeName(func1.Name, func1)
                    Else
                        If proc2.IsBuildIn = False Then
                            proc2.ReplacementName = SearchConflictFreeName(proc2.Name, proc2)
                        Else
                            Debug.WriteLine("Conflict! No Function/Procedure of " + func1.Name + " and " + proc2.Name + " can be renamed!")
                        End If
                    End If
                End If

                If obj1.ObjType = LangageObjectType.OBJTYPE_VARIABLE And obj2.ObjType = LangageObjectType.OBJTYPE_VARIABLE Then
                    var1 = obj1
                    var2 = obj2

                    'IST ZU STRENG (lokal und global heissen immer anderst)
                    '(var1.USubrutine <> var2.USubrutine) Then And (var1.USubrutine <> "") And (var2.USubrutine <> "")

                    'NICHT STRENG GENUG
                    '(var1.USubrutine <> var2.USubrutine)

                    'NUR DANN NICHT UMBENENNEN, WENN auch der Typ gleich ist ->Auch nicht viel besser
                    '(var1.USubrutine <> var2.USubrutine) And var1.DataType = var2.DataType

                    If (var1.USubrutine <> var2.USubrutine) Then 'And var1.DataType = var2.DataType Then
                        'Alles ok, da in einem Fall lokal und im anderen Fall globale Variable bzw. lokal aus anderer Unterfunktion

                        Dim varDone As Boolean = False




                        'WEGEN VARIABLE TASTE BENÖTIGT...
                        If var1.USubrutine <> "" And var2.USubrutine = "" Then
                            '2010-10-17 hinz.
                            If var1.ReplacementName = "" Then
                                var1.ReplacementName = SearchConflictFreeName(var1.Name, var1.Subrutine, True, var1)
                                varDone = True
                            Else
                                var2.ReplacementName = SearchConflictFreeName(var2.Name, var2.Subrutine, True, var2)
                                varDone = True
                            End If
                        End If


                        If var1.USubrutine = "" And var2.USubrutine <> "" Then
                            '2010-10-17 hinz.
                            If var2.ReplacementName = "" Then
                                var2.ReplacementName = SearchConflictFreeName(var2.Name, var2.Subrutine, True, var2)
                                varDone = True
                            Else
                                var1.ReplacementName = SearchConflictFreeName(var1.Name, var1.Subrutine, True, var1)
                                varDone = True
                            End If
                        End If
                

                            'If varDone = False Then
                            '    Debug.WriteLine("Cannot rename variables " + var1.Name + "  " + var2.Name)
                            'End If
                        Else

                            If var2.USubrutine <> "" Then

                            If var2.ReplacementName = "" Then
                                var2.ReplacementName = SearchConflictFreeName(var2.Name, var2.Subrutine, True, var2)
                            Else

                                var1.ReplacementName = SearchConflictFreeName(var1.Name, var1.Subrutine, True, var1)
                                globalVarRenames(var1.Name + var1.DataType) = var1.ReplacementName
                            End If
                        Else
                            If var1.ReplacementName = "" Then
                                var1.ReplacementName = SearchConflictFreeName(var1.Name, var1.Subrutine, True, var1)
                                globalVarRenames(var1.Name + var1.DataType) = var1.ReplacementName
                            Else
                                var2.ReplacementName = SearchConflictFreeName(var2.Name, var2.Subrutine, True, var2)
                            End If

                        End If
                    End If
                End If

                If obj1.ObjType = LangageObjectType.OBJTYPE_ARRAY And obj2.ObjType = LangageObjectType.OBJTYPE_VARIABLE Then
                    arr1 = obj1
                    var2 = obj2
                    var2.ReplacementName = SearchConflictFreeName(var2.Name, var2.Subrutine, True, var2)
                End If

                If obj1.ObjType = LangageObjectType.OBJTYPE_ARRAY And obj2.ObjType = LangageObjectType.OBJTYPE_ARRAY Then
                    arr1 = obj1
                    arr2 = obj2
                    arr2.ReplacementName = SearchConflictFreeName(arr2.Name, arr2.Subrutine, True, arr2)
                End If

                If obj1.ObjType = LangageObjectType.OBJTYPE_ARRAY And obj2.ObjType = LangageObjectType.OBJTYPE_FUNCTION Then
                    arr1 = obj1
                    func2 = obj2
                    arr1.ReplacementName = SearchConflictFreeName(arr1.Name, arr1)
                End If


                If obj1.ObjType = LangageObjectType.OBJTYPE_ARRAY And obj2.ObjType = LangageObjectType.OBJTYPE_PROCEDURE Then
                    arr1 = obj1
                    proc2 = obj2
                    arr1.ReplacementName = SearchConflictFreeName(arr1.Name, arr1)
                End If



                If obj1.ObjType = LangageObjectType.OBJTYPE_VARIABLE And obj2.ObjType = LangageObjectType.OBJTYPE_KEYWORD Then
                    var1 = obj1
                    var1.ReplacementName = SearchConflictFreeName(var1.Name, var1)
                End If

                If obj1.ObjType = LangageObjectType.OBJTYPE_ARRAY And obj2.ObjType = LangageObjectType.OBJTYPE_KEYWORD Then
                    arr1 = obj1
                    arr1.ReplacementName = SearchConflictFreeName(arr1.Name, arr1)
                End If


                If obj1.ObjType = LangageObjectType.OBJTYPE_VARIABLE And obj2.ObjType = LangageObjectType.OBJTYPE_FUNCTION Then
                    var1 = obj1
                    func2 = obj2
                    var1.ReplacementName = SearchConflictFreeName(var1.Name, var1)
                End If

                If obj1.ObjType = LangageObjectType.OBJTYPE_VARIABLE And obj2.ObjType = LangageObjectType.OBJTYPE_PROCEDURE Then
                    var1 = obj1
                    proc2 = obj2
                    var1.ReplacementName = SearchConflictFreeName(var1.Name, var1)
                End If
            End If

        End Sub

        'Public Sub ResolveConflict(ByVal obj As LanguageObject)
        '    For Each listObj As LanguageObject In Me
        '        'Gleiches Objekt ignorieren
        '        If listObj IsNot obj Then
        '            If obj.GetNewUName = listObj.GetNewUName Then 'GetNewUName stellt sicher, das der Konflikt nur 1 mal gelöst werden muss
        '                SolveConflict(listObj, obj)
        '            End If
        '        End If
        '    Next
        'End Sub


        Public Sub ResolveConflicts()
            'Zunächst nur Globale Vars...
            For Each listObj As LanguageObject In Me
                If listObj.Subrutine = "" Then
                    For Each obj As LanguageObject In Me
                        'Gleiches Objekt ignorieren
                        If obj.Subrutine = "" Then ' Or obj.ObjType <> listObj.ObjType Then '2010-10-03 bei anderen tYpen! Array-Variable 

                            If listObj.Name <> "" And obj.Name <> "" Then
                                If listObj IsNot obj Then
                                    If obj.GetNewUName = listObj.GetNewUName Then 'GetNewUName stellt sicher, das der Konflikt nur 1 mal gelöst werden muss

                                        If listObj.IsBuildIn = False Or obj.IsBuildIn = False Then
                                            SolveConflict(listObj, obj)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            Next

            'gloeichenamige Lokale Variablen gleich umbenennen...
            For Each listObj As LanguageObject In Me
                If listObj.Subrutine = "" Then
                    For Each obj As LanguageObject In Me
                        'Gleiches Objekt ignorieren
                        If obj.Subrutine <> "" Then
                            If listObj.Name <> "" And obj.Name <> "" Then


                                If listObj IsNot obj Then
                                    If obj.ReplacementName = "" Then 'SIcherstellen, dass nr 1 mal umbenannt wird (nötig???)
                                        If obj.UName = listObj.UName Then '2010-10-16 UName statt GetNewUName ??!!!  'GetNewUName stellt sicher, das der Konflikt nur 1 mal gelöst werden muss
                                            If obj.DataType = listObj.DataType And obj.ObjType = listObj.ObjType Then '2010-10-16 auch ObjType überprüfen???!!!

                                                If listObj.IsBuildIn = False Or obj.IsBuildIn = False Then
                                                    If listObj.ReplacementName <> "" Then
                                                        TMPOUT.WriteLine("replace " + obj.Name + " " + obj.DataType + " by " + listObj.ReplacementName + " (in " + obj.Subrutine + ")")
                                                        obj.ReplacementName = listObj.ReplacementName
                                                        ''obj.IsBuildIn = True 'GANZ GANZ SCHLECHTE LÖSUNG UM ZU VERHINDERN, DAS NAME NOCHMALS GEÄNDERT WIRD!!!!!!!!!!!!!!!!!!!!! 
                                                    Else
                                                        TMPOUT.WriteLine("replace " + obj.Name + " " + obj.DataType + " by " + listObj.Name + " (in " + obj.Subrutine + ")")
                                                        obj.ReplacementName = listObj.Name ' Auch den Fall beachten, wenn die Variable nicht umbenannt werden muss (Problematisch????)
                                                        ''obj.IsBuildIn = True 'GANZ GANZ SCHLECHTE LÖSUNG UM ZU VERHINDERN, DAS NAME NOCHMALS GEÄNDERT WIRD!!!!!!!!!!!!!!!!!!!!! 
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            Next

            'Dim sw2 As IO.StreamWriter = New IO.StreamWriter("D:\GFATEST\out2.txt")
            'For Each listObj As LanguageObject In Me
            'sw2.WriteLine(listObj.GetNewUName + "   " + listObj.DataType + "   " + listObj.Subrutine)
            'Next
            'sw2.Close()

            'Dann alle nicht globalen!
            For Each listObj As LanguageObject In Me
                'If listObj.Subrutine <> "" Then
                For Each obj As LanguageObject In Me
                    'Gleiches Objekt ignorieren
                    If listObj.IsBuildIn = False Or obj.IsBuildIn = False Then
                        If listObj.Name <> "" And obj.Name <> "" Then


                            Dim co As Boolean
                            'WEGEN VARIABLE TASTE BENÖTIGT...
                            co = ((obj.DataType <> listObj.DataType) And ((obj.Subrutine = "" And listObj.Subrutine <> "") Or (obj.Subrutine <> "" And listObj.Subrutine = "")))


                            If (obj.Subrutine = listObj.Subrutine And obj.Subrutine <> "") Or (obj.ObjType <> listObj.ObjType) Or co Then '2010-10-03 bei anderen tYpen! Array-Variable 

                                '  If (obj.Subrutine <> "" And listObj.Subrutine <> "") Or (obj.ObjType <> listObj.ObjType) Or ((obj.DataType <> listObj.DataType) And ((obj.Subrutine = "" And listObj.Subrutine <> "") Or (obj.Subrutine <> "" And listObj.Subrutine = ""))) Then '2010-10-03 bei anderen tYpen! Array-Variable 
                                If listObj IsNot obj Then



                                    If obj.GetNewUName = listObj.GetNewUName Then 'GetNewUName stellt sicher, das der Konflikt nur 1 mal gelöst werden muss

                                        SolveConflict(listObj, obj)

                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
                'End If
            Next

        End Sub


        Public Function IsOperator(ByVal name As String) As Boolean
            Select Case name
                Case "(", ")", "+", "-", "*", "/", "=", "<", ">", ",", "~", ";", "@", "^", ":" 'Achtung # ist auch ein Datentyp! 
                    Return True
                Case Else
                    Return False
            End Select

        End Function

        Public Function IsNumber(ByVal Number As String) As Boolean
            Dim result As Double = 0D
            Number = Number.Replace(".", Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) ' Problem mit .->,
            'Try
            '    Return Convert.ToDouble(numStr)
            'Catch ex As Exception
            '    Return 0D
            'End Try
            Return Double.TryParse(Number, result)
        End Function

        Public Function IsConstString(ByVal Str As String) As Boolean
            Str = Trim(Str)
            If Left(Str, 1) = Chr(34) And Right(Str, 1) = Chr(34) Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ExtractName(ByVal name As String)
            name = Trim(name)
            Dim ending As String = Right(name, 1)
            If ending = "#" Or ending = "%" Or ending = "&" Or ending = "|" Or ending = "!" Or ending = "$" Then
                name = Left(name, Len(name) - 1)
            End If
            Return name
        End Function

        Public Function ExtractDataType(ByVal name As String)
            name = Trim(name)
            Dim ending As String = Right(name, 1)
            If ending = "#" Or ending = "%" Or ending = "&" Or ending = "|" Or ending = "!" Or ending = "$" Then
                'Endung ok
            Else
                ending = ""
            End If
            Return ending
        End Function

        Public Function IsVaraiableNameOk(ByVal name As String) As Boolean
            Dim Ok As Boolean = False
            name = name.ToUpper
            Select Case Left(name, 1)
                Case "A" To "Z"
                    Ok = True
                Case "_"
                    Ok = True
            End Select
            If Ok Then
                For i = 2 To Len(name)
                    If Ok Then
                        Ok = False
                        Select Case Mid(name, i, 1) 'KORR
                            Case "A" To "Z"
                                Ok = True
                            Case "0" To "9"
                                Ok = True
                            Case "_"
                                Ok = True
                        End Select
                    End If
                Next
            End If
            Return Ok
        End Function

        Public Function GetTokenTypeEx(ByVal line As List(Of String), ByVal idx As Integer, ByRef isOwnSubrutine As Boolean, ByVal isInDeclaration As Boolean) As LangageObjectType
            Dim result As LangageObjectType = LangageObjectType.OBJTYPE_UNKNOWN
            Dim token As String = line(idx)
            Dim checkProcedure As Boolean = False
            Dim checkDouble As Boolean = False
            Dim ownSubrutine As Boolean = False
            Dim obj As LanguageObject = Nothing
            Dim ForceFunction As Boolean = False
            Dim CanBeArray As Boolean = False
            Dim CanBeFunctionOrProcedure As Boolean = False
            Dim CanBeVariable As Boolean = True
            'Dim CanBeFunctionOrArray As Boolean = False '2010-10-05 hinz.

            If token = "" Then
                Return LangageObjectType.OBJTYPE_OPERATOR
            End If

            If IsOperator(token) Then
                'result = LangageObjectType.OBJTYPE_OPERATOR
                Return LangageObjectType.OBJTYPE_OPERATOR
            Else

                If IsConstString(token) Then
                    'result = LangageObjectType.OBJTYPE_STRING
                    Return LangageObjectType.OBJTYPE_STRING
                ElseIf IsNumber(token) Then
                    'result = LangageObjectType.OBJTYPE_NUMBER
                    Return LangageObjectType.OBJTYPE_NUMBER
                Else
                    obj = FindObjType(token, LangageObjectType.OBJTYPE_KEYWORD)
                    If obj IsNot Nothing Then
                        'result = LangageObjectType.OBJTYPE_KEYWORD
                        Return LangageObjectType.OBJTYPE_KEYWORD
                    Else
                        'Weitersuchen
                        CanBeVariable = True

                        If idx < (line.Count - 1) Then
                            If line(idx + 1) = "(" Then
                                'CanBeFunctionOrArray = True
                                CanBeArray = True
                                CanBeVariable = False
                            End If
                        End If
                        CanBeFunctionOrProcedure = True
                        If idx < (line.Count - 1) Then
                            'ACHTUNG GEFÄHRLICH, KÖNNTE JA AUCH EIN VERGLEICH SEIN

                            Dim isCompare As Boolean = False

                            If (UCase(line(0)) = "IF") Then
                                isCompare = True
                            End If
                            If idx > 0 Then
                                If (UCase(line(idx - 1)) = "THEN") Then 'Nicht für alle Fälle korrekt!!!
                                    isCompare = False
                                End If
                            End If

                            If line(idx + 1) = "=" And isCompare = False Then
                                'CanBeFunctionOrArray = True
                                CanBeFunctionOrProcedure = False
                            End If
                        End If


                        If line.Count > 0 And idx > 0 Then
                            Dim uLine As String = UCase(Trim(line(idx - 1)))
                            If uLine = "PROCEDURE" Or uLine = "FUNCTION" Or uLine = "SUB" Then
                                CanBeArray = False
                                CanBeVariable = False
                            End If

                        End If

                        If line.Count = 1 Then
                            CanBeVariable = False
                            CanBeArray = False
                        End If

                        '2010-10-05 ---------> TODO!
                        'Für z.B: INKEY$
                        'If idx > 0 Then
                        '    If UCase(line(idx - 1)) = "AND" Or UCase(line(idx - 1)) = "OR" Or UCase(line(idx - 1)) = "IF" Or line(idx - 1) = "(" Or line(idx - 1) = ">" Or line(idx - 1) = "<" Or line(idx - 1) = "=" Or line(idx - 1) = "==" Then
                        '       CanBeFunctionOrArray = True
                        '    End If
                        'End If



                        If CanBeFunctionOrProcedure Then

                            '2010-10-14 WIEDER RAUS!, ZU WAS WAR DAS GUT?
                            'If isInDeclaration = False Then

                            'Wenn checkProcedure=True, dann kann es höchstens eine Prozedur sein...
                            If idx = 0 Then
                                checkProcedure = True
                            Else
                                If line(idx - 1).ToUpper = "THEN" Then
                                    checkProcedure = True
                                End If
                                If idx > 1 Then
                                    If line(idx - 1) = "@" And line(idx - 2) = "THEN" Then
                                        checkProcedure = True
                                        checkDouble = True ' mit @ kann es auch eiene Funktion sein
                                        ownSubrutine = True
                                    End If
                                End If
                            End If
                            If idx = 1 Then
                                If line(0) = "@" Then
                                    checkProcedure = True
                                    ownSubrutine = True
                                    'checkDouble = True ' mit @ kann es auch eiene Funktion sein (Hier aber glaub nicht!)
                                End If
                            End If

                            If idx > 0 Then
                                'Immer Function
                                If line(idx - 1) = "FN" Then
                                    checkProcedure = False
                                    ownSubrutine = True
                                    ForceFunction = True
                                End If
                            End If


                            isOwnSubrutine = ownSubrutine


                            If line.Count > 1 Then
                                If line(1) = "=" Then
                                    If checkProcedure = True Then
                                        checkProcedure = False
                                    End If
                                End If
                            End If

                            If checkProcedure Then
                                obj = FindObjType(token, LangageObjectType.OBJTYPE_PROCEDURE, ownSubrutine)
                                If obj Is Nothing Then
                                    If checkDouble Then
                                        obj = FindObjType(ExtractName(token), ExtractDataType(token), LangageObjectType.OBJTYPE_FUNCTION, ownSubrutine)
                                    End If
                                End If
                            Else

                                '2010-10-03
                                ' If CanBeFunctionOrArray = True Then
                                If ForceFunction Then
                                    obj = FindObjType(ExtractName(token), ExtractDataType(token), LangageObjectType.OBJTYPE_FUNCTION, ownSubrutine)
                                Else


                                    'Variable noch mehr bevorzugen?
                                    If CanBeVariable Then
                                        'Wenn bereits variable deklariert, dann diese Bevorzugen
                                        obj = FindObjType(token, LangageObjectType.OBJTYPE_VARIABLE, ownSubrutine) 'ownSubrutine??? 2010-10-10
                                    End If

                                    'ARRAY BEVORZUGEN WENN KEIN FN VORGESTELLT!
                                    If obj Is Nothing Then
                                        If CanBeArray Then
                                            obj = FindObjType(ExtractName(token), ExtractDataType(token), LangageObjectType.OBJTYPE_ARRAY)
                                        End If
                                    End If
                                    If obj Is Nothing Then
                                        obj = FindObjType(ExtractName(token), ExtractDataType(token), LangageObjectType.OBJTYPE_FUNCTION, ownSubrutine)
                                    End If

                                End If
                                'End If

                                'End If
                            End If
                        End If



                        If obj IsNot Nothing Then
                            result = obj.ObjType
                        Else
                            'Weitersuchen
                            'Arrays müssen deklariert sein...
                            'TODO: (überprüfen?)
                            obj = Nothing
                            If idx < line.Count - 1 Then
                                If line(idx + 1) = "(" Then
                                    obj = FindObjType(ExtractName(token), ExtractDataType(token), LangageObjectType.OBJTYPE_ARRAY) 'ACHTUNG DATENTYP MUSS ANGEGEBEN WERDEN!!!
                                End If
                            End If

                            If obj IsNot Nothing Then
                                result = obj.ObjType
                            Else
                                'Dann kann es nur noch eine Variable sein
                                If IsVaraiableNameOk(ExtractName(token)) Then
                                    result = LangageObjectType.OBJTYPE_VARIABLE
                                Else
                                    'Kein gültiger Variablenname...
                                    result = LangageObjectType.OBJTYPE_UNKNOWN
                                End If

                            End If
                        End If
                    End If
                End If
            End If
            Return result
        End Function



        Public Function GetTokenType(ByVal line As List(Of String), ByVal idx As Integer, ByVal isInDeclaration As Boolean) As LangageObjectType
            Dim dummy As Boolean
            Return GetTokenTypeEx(line, idx, dummy, isInDeclaration)
        End Function


    End Class

    Public Class Converter
        Dim objs As LanguageObjects = New LanguageObjects()
        Dim globalVars As List(Of String) = New List(Of String)

        Dim restorePoints As Hashtable = New Hashtable
        Public Function PrepareLine(ByVal line As String) As String
            line = Trim(line)
            If Left(line, 1) = "$" Then
                line = "' " + line + "' vom Konverter auskommentiert"
            End If
            'Sollte eigentlich erkannt werden, ist aber wohl nicht so...
            If InStr(UCase(line), "PROCEDURE") Then
                If Left(line, 1) = ">" Then
                    line = Trim(Right(line, Len(line) - 1))
                End If
            End If
            If InStr(UCase(line), "FUNCTION") Then
                If Left(line, 1) = ">" Then
                    line = Trim(Right(line, Len(line) - 1))
                End If
            End If

            If InStr(UCase(line), "FONT TO") = 0 Then
                If Left(UCase(Trim(line)), Len("FONT ")) = "FONT " Or Left(UCase(Trim(line)), Len("RFONT ")) = "RFONT " Then
                    line = line.Replace(",WEIGHT ", "," + Chr(34) + "WEIGHT" + Chr(34) + ",")
                    line = line.Replace(",WIDTH ", "," + Chr(34) + "WIDTH" + Chr(34) + ",")
                    line = line.Replace(",HEIGHT ", "," + Chr(34) + "HEIGHT" + Chr(34) + ",")
                    line = line.Replace(",ITALIC ", "," + Chr(34) + "ITALIC" + Chr(34) + ",")
                    line = line.Replace(",STRIKEOUT ", "," + Chr(34) + "STRIKEOUT" + Chr(34) + ",")
                    line = line.Replace(",UNDERLINE ", "," + Chr(34) + "UNDERLINE" + Chr(34) + ",")
                    line = line.Replace(",ESCAPEMENT ", "," + Chr(34) + "ESCAPEMENT" + Chr(34) + ",")
                    line = line.Replace(",FAMILY ", "," + Chr(34) + "FAMILY" + Chr(34) + ",")
                    line = line.Replace(",CHARSET ", "," + Chr(34) + "CHARSET" + Chr(34) + ",")
                    line = line.Replace(",WEIGHT ", "," + Chr(34) + "WEIGHT" + Chr(34) + ",")
                    line = line.Replace(",PITCH ", "," + Chr(34) + "PITCH" + Chr(34) + ",")

                    line = line.Replace(" WEIGHT", " " + Chr(34) + "WEIGHT" + Chr(34) + ",")
                    line = line.Replace(" WIDTH", " " + Chr(34) + "WIDTH" + Chr(34) + ",")
                    line = line.Replace(" HEIGHT", " " + Chr(34) + "HEIGHT" + Chr(34) + ",")
                    line = line.Replace(" ITALIC", " " + Chr(34) + "ITALIC" + Chr(34) + ",")
                    line = line.Replace(" STRIKEOUT", " " + Chr(34) + "STRIKEOUT" + Chr(34) + ",")
                    line = line.Replace(" UNDERLINE ", " " + Chr(34) + "UNDERLINE" + Chr(34) + ",")
                    line = line.Replace(" ESCAPEMENT ", " " + Chr(34) + "ESCAPEMENT" + Chr(34) + ",")
                    line = line.Replace(" FAMILY ", " " + Chr(34) + "FAMILY" + Chr(34) + ",")
                    line = line.Replace(" CHARSET ", " " + Chr(34) + "CHARSET" + Chr(34) + ",")
                    line = line.Replace(" WEIGHT ", " " + Chr(34) + "WEIGHT" + Chr(34) + ",")
                    line = line.Replace(" PITCH ", " " + Chr(34) + "PITCH" + Chr(34) + ",")
                End If
            End If

            Return line
        End Function
        Public Sub InitKeywords()
            objs.AddKeyword("==", "=")

            'Operatoren
            objs.AddKeyword("AND", "And")
            objs.AddKeyword("NOT", "Not")
            objs.AddKeyword("XOR", "XOr")
            objs.AddKeyword("OR", "Or")

            'Bedingugnen
            objs.AddKeyword("IF", "If")
            objs.AddKeyword("ELSE", "Else")
            objs.AddKeyword("ENDIF", "End If")
            objs.AddKeyword("THEN", "Then")
            objs.AddKeyword("EXITIF", "EXITIF") 'Spezialfall


            objs.AddKeyword("SWITCH", "Select Case")
            objs.AddKeyword("SELECT", "Select Case")
            objs.AddKeyword("ENDSELECT", "End Select")
            objs.AddKeyword("ENDSWITCH", "End Select")
            objs.AddKeyword("CASE", "Case")
            objs.AddKeyword("DEFAULT", "Case Else")
            objs.AddKeyword("CONT", "--> CONT 'TODO: Führe auch nächstes Case aus")

            'Schleifen
            objs.AddKeyword("DO", "Do")
            objs.AddKeyword("FOR", "For")
            objs.AddKeyword("NEXT", "Next")
            objs.AddKeyword("TO", "To")
            objs.AddKeyword("STEP", "Step")
            objs.AddKeyword("LOOP", "Loop")
            objs.AddKeyword("UNTIL", "Until")
            objs.AddKeyword("GOTO", "Goto")
            objs.AddKeyword("WHILE", "While")
            objs.AddKeyword("WEND", "End While") '"010-10-05 End While statt WEND
            objs.AddKeyword("REPEAT", "Do") ' Es gibt kein Repeat in VB.Net

            'Konstanten
            objs.AddKeyword("PI", "PI")
            objs.AddKeyword("E", "E")
            objs.AddKeyword("TRUE", "True")
            objs.AddKeyword("FALSE", "False")

            'Kompilereinstellungen
            objs.AddKeyword("$ABIG", "' $ABIG") 'Auskommentieren

            'Variablendeklaration
            objs.AddKeyword("DIM", "Redim")
            objs.AddKeyword("LOCAL", "Dim")

            'Proceduren/Funktionen
            objs.AddKeyword("GOSUB", "Gosub")
            objs.AddKeyword("PROCEDURE", "Public Sub")
            objs.AddKeyword("FUNCTION", "Public Function")
            objs.AddKeyword("RETURN", "Return")
            objs.AddKeyword("ENDFUNC", "End Function")
            objs.AddKeyword("VAR", "ByRef")
            objs.AddKeyword("FN", "")
            objs.AddKeyword("EXPROC", "[EXPROCReturn]")

            objs.AddKeyword("LET", "")

            'Fehlerbehandlung
            objs.AddKeyword("TRY", "Try")
            objs.AddKeyword("CATCH", "Catch")

            'Anderes
            objs.AddKeyword("END", "_END")
            objs.AddKeyword("LET", "Let")
            objs.AddKeyword("V:", "") 'Adressoperator entfernen!
            objs.AddKeyword("AS", "AS")
            objs.AddKeyword("USING", "USING")

            objs.AddKeyword("ON", "ON")
            'objs.AddKeyword("MENU", "MENU") 'Evtl. Problematisch?
            objs.AddKeyword("MESSAGE", "MESSAGE")


            objs.AddKeyword("DIM?", "DIM?")
            objs.AddKeyword("DECL", "DECL")
            objs.AddKeyword("OUTPUT", "OUTPUT")
            objs.AddKeyword("DLL", "DLL")
            objs.AddKeyword("ENDDLL", "ENDDLL")
            objs.AddKeyword("ERASE", "ERASE")
            objs.AddKeyword("VOID", "VOID")
            objs.AddKeyword("ENDTYPE", "ENDTYPE")
            objs.AddKeyword("VARPTR", "VARPTR")
            objs.AddKeyword("_HUGE", "_HUGE")

            objs.AddKeyword("OFFSET", ",")
            'objs.AddKeyword("END", "_END")

            objs.AddKeyword("OPERATOR", "OPERATOR") ' Von VB.Net reserviertes Wort
            objs.AddKeyword("NEW", "NEW") ' Von VB.Net reserviertes Wort
            objs.AddKeyword("BYTE", "BYTE") ' Von VB.Net reserviertes Wort
            objs.AddKeyword("WORD", "WORD") ' Von VB.Net reserviertes Wort
            objs.AddKeyword("BITMAP", "BITMAP") ' Von VB.Net reserviertes Wort
        End Sub
        Public Shared Function CombineProcedures(ByVal list As List(Of String)) As List(Of String)
            Dim newList As List(Of String) = ConvertStringListHelper.CombineStrings(list, "SEEK", "#", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "LINE", "INPUT", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "LINEINPUT", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "FORM", "INPUT", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "CLOSE", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "INPUT", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "WRITE", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPENW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "PARENTW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "CHILDW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "TOPW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "CLOSEW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "WIN", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "TITLEW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "MOVEW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "SIZEW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "GETWINDRECT", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "SHOWW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "FULLW", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "SETCAPTURE", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "WINDGET", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "FILESELECT", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "PRINT", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "PRINT", "AT", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "PRINT", "USING", True)

            'Functions:
            newList = ConvertStringListHelper.CombineStrings(newList, "EOF", "(", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "LOF", "(", "#", True)
            'newList = ConvertStringListHelper.CombineStrings(newList, "WIN", "#", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "DLG", "COLOR", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "DLG", "PRINT", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "DLG", "FONT", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "FONT", "TO", True)

            'Spezialfälle:
            newList = ConvertStringListHelper.CombineStrings(newList, "WIN", "(", ")", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN", Chr(34) + "i" + Chr(34), ",", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN", Chr(34) + "o" + Chr(34), ",", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN", Chr(34) + "a" + Chr(34), ",", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN", Chr(34) + "r" + Chr(34), ",", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN" + Chr(34) + "i" + Chr(34) + ",", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN" + Chr(34) + "o" + Chr(34) + ",", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN" + Chr(34) + "a" + Chr(34) + ",", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "OPEN" + Chr(34) + "r" + Chr(34) + ",", "#", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "EXIT", "IF", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "V", ":", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "ON", "MENU", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "MESSAGE", "GOSUB", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "KEY", "GOSUB", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "ONMENU", "MESSAGEGOSUB", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "ONMENU", "KEYGOSUB", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "ONMENU", "GOSUB", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "CLIP", "OFF", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "NEW", "FRAME", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "BGET", "#", True)
            newList = ConvertStringListHelper.CombineStrings(newList, "BPUT", "#", True)

            newList = ConvertStringListHelper.CombineStrings(newList, "=", "=", True)

            Return newList
        End Function
        Public hashProcedures As Hashtable = New Hashtable
        Public ArrayDimensions As Hashtable = New Hashtable
        Public Sub InitProcedures()
            objs.AddProcedure("FORMINPUT", "_FORMINPUT", True)

            objs.AddProcedure("KILL", "_KILL", True)
            objs.AddProcedure("MKDIR", "_MKDIR", True)
            'objs.AddProcedure("OPEN", "_OPEN", True)
            objs.AddProcedure("CLOSE", "_CLOSE", True)
            objs.AddProcedure("SEEK#", "_SEEK", True)
            objs.AddProcedure("CLOSE#", "_CLOSE", True) ' Für Close mit und ohne #, da es Close auch ohne Parameter gibt
            objs.AddProcedure("INPUT#", "_INPUT", True)
            'objs.AddProcedure("PRINT", "_PRINT", True)
            objs.AddProcedure("WRITE#", "_WRITE", True)
            objs.AddProcedure("ARRAYFILL", "_ARRAYFILL", True)
            objs.AddProcedure("DELETE", "_DELETE", True)
            objs.AddProcedure("RANDOMIZE", "_RANDOMIZE", True)
            objs.AddProcedure("SWAP", "_SWAP", True)
            objs.AddProcedure("INC", "_INC", True)
            objs.AddProcedure("INC", "_INC", True)
            objs.AddProcedure("DEC", "__DEC", True)
            objs.AddProcedure("MUL", "_MUL", True)
            objs.AddProcedure("DIV", "_DIV", True)
            objs.AddProcedure("ADD", "_ADD", True)
            objs.AddProcedure("SUB", "_SUB", True)
            objs.AddProcedure("OPENW#", "_OPENW", True)
            objs.AddProcedure("PARENTW#", "_PARENTW", True)
            objs.AddProcedure("CHILDW#", "_CHILDW", True)
            objs.AddProcedure("TOPW#", "_TOPW", True)
            objs.AddProcedure("CLOSEW#", "_CLOSEW", True)
            objs.AddProcedure("WIN#", "_WIN", True)
            objs.AddProcedure("TITLEW#", "_TITLEW", True)
            objs.AddProcedure("MOVEW#", "_MOVEW", True)
            objs.AddProcedure("SIZEW#", "_SIZEW", True)
            objs.AddProcedure("GETWINDRECT#", "_GETWINDRECT", True)
            objs.AddProcedure("SHOWW#", "_SHOWW", True)
            objs.AddProcedure("FULLW#", "_FULLW", True)
            objs.AddProcedure("SETCAPTURE#", "_SETCAPTURE", True)
            objs.AddProcedure("RELEASECAPTURE", "_RELEASECAPTURE", True)
            objs.AddProcedure("WINDGET#", "_WINDGET", True)
            objs.AddProcedure("SETFONT", "_SETFONT", True)
            objs.AddProcedure("SETDC", "_SETDC", True)
            objs.AddProcedure("RGBCOLOR", "_RGBCOLOR", True)
            objs.AddProcedure("CIRCLE", "_CIRCLE", True)
            objs.AddProcedure("ELLIPSE", "_ELLIPSE", True)
            objs.AddProcedure("DEFLINE", "_DEFLINE", True)
            objs.AddProcedure("DEFFILL", "_DEFFILL", True)
            objs.AddProcedure("FILL", "_FILL", True)
            objs.AddProcedure("PLOT", "_PLOT", True)
            objs.AddProcedure("BOX", "_BOX", True)
            objs.AddProcedure("PBOX", "_PBOX", True)
            objs.AddProcedure("CLS", "_CLS", True)
            objs.AddProcedure("TEXT", "_TEXT", True)
            objs.AddProcedure("PCIRCLE", "_PCIRCLE", True)
            objs.AddProcedure("GRAPHMODE", "_GRAPHMODE", True)
            objs.AddProcedure("FREEBMP", "_FREEBMP", True)
            objs.AddProcedure("FREEDC", "FREEDC", True)
            objs.AddProcedure("POLYLINE", "_POLYLINE", True)
            objs.AddProcedure("POLYFILL", "_POLYFILL", True)
            objs.AddProcedure("GET", "_GET", True)
            objs.AddProcedure("PUT", "_PUT", True)
            objs.AddProcedure("STRETCH", "_STRETCH", True)
            objs.AddProcedure("LOCATE", "_LOCATE", True)
            objs.AddProcedure("CREATEMETA", "_CREATEMETA", True)
            objs.AddProcedure("CLOSEMETA", "_CLOSEMETA", True)
            objs.AddProcedure("PLAYMETA", "_PLAYMETA", True)
            objs.AddProcedure("FREEFONT", "_FREEFONT", True)
            objs.AddProcedure("PAUSE", "_PAUSE", True)
            objs.AddProcedure("DELAY", "_DELAY", True)
            objs.AddProcedure("SHOWM", "_SHOWM", True)
            objs.AddProcedure("HIDEM", "_HIDEM", True)
            objs.AddProcedure("DEFMOUSE", "_DEFMOUSE", True)
            objs.AddProcedure("SETMOUSE", "_SETMOUSE", True)
            objs.AddProcedure("MOUSE", "_MOUSE", True)
            objs.AddProcedure("CHAIN", "_CHAIN", True)
            objs.AddProcedure("KEYGET", "_KEYGET", True)
            objs.AddProcedure("PROMPT", "_PROMPT", True)
            objs.AddProcedure("FILESELECT#", "_FILESELECT", True) 'Mit und ohne # 
            objs.AddProcedure("FILESELECT", "_FILESELECT", True)
            objs.AddProcedure("ALERT", "_ALERT", True)
            objs.AddProcedure("MENU", "_MENU", True)
            objs.AddProcedure("PEEKEVENT", "_PEEKEVENT", True)
            objs.AddProcedure("GETEVENT", "_GETEVENT", True)
            objs.AddProcedure("SLEEP", "_SLEEP", True)
            objs.AddProcedure("CLRCLIP", "_CLRCLIP", True)
            objs.AddProcedure("CLIPFORMAT", "_CLIPFORMAT", True)
            objs.AddProcedure("CLIPCOPY", "_CLIPCOPY", True)
            objs.AddProcedure("READ", "_READ", True)
            objs.AddProcedure("CLR", "_CLR", True)

            objs.AddProcedure("DLGCOLOR", "_DLG_COLOR", True)
            objs.AddProcedure("DLGPRINT", "_DLG_PRINT", True)
            objs.AddProcedure("DLGFONT", "_DLG_FONT", True)
            objs.AddProcedure("RFONT", "_RFONT", True)

            objs.AddProcedure("PRINT#", "_PRINT", True)
            objs.AddProcedure("PRINT", "_PRINTTEXT", True)
            objs.AddProcedure("LPRINT", "_PRINTTEXT", True) ' Ist LPRINT das gleiche wie PRINT???
            objs.AddProcedure("PRINTAT", "_PRINT_AT", True)
            objs.AddProcedure("PRINTUSING", "_PRINT_USING", True)
            objs.AddProcedure("LINEINPUT", "_LINE_INPUT", True)
            objs.AddProcedure("LINE", "_LINE", True)
            objs.AddProcedure("FONTTO", "_FONT_TO", True)
            objs.AddProcedure("FONT", "_FONT", True)
            objs.AddProcedure("ONMENU", "_ON_MENU", True)

            objs.AddProcedure("RESTORE", "_RESTORE", True)
            objs.AddProcedure("DATA", "_DATA", True)
            objs.AddProcedure("RENAME", "_RENAME", True)
            objs.AddProcedure("WINDGET", "_WINDGET", True)

            objs.AddProcedure("BGET#", "_BGET", True)
            objs.AddProcedure("BPUT#", "_BPUT", True)
            objs.AddProcedure("COLOR", "_COLOR", True)
            objs.AddProcedure("SETCAPTURE", "_SETCAPTURE", True)
            objs.AddProcedure("STARTDOC", "_STARTDOC", True)
            objs.AddProcedure("NEWFRAME", "_NEWFRAME", True)
            objs.AddProcedure("ENDDOC", "_ENDDOC", True)
            objs.AddProcedure("WINDSET", "_WINDSET", True)
            objs.AddProcedure("DRAWTEXT", "_DRAWTEXT", True)
            objs.AddProcedure("SETWINDOWSTYLE", "_SETWINDOWSTYLE", True)

            objs.AddProcedure("CLIPOFF", "_CLIPOFF", True)
            objs.AddProcedure("CLIP", "_CLIP", True)
            objs.AddProcedure("FREEDC", "_FREEDC", True)
            objs.AddProcedure("FREEFONT", "_FREEFONT", True)
            objs.AddProcedure("FREEBMP", "_FREEBMP", True)

            objs.AddProcedure("COLOR", "_COLOR", True)

            objs.AddProcedure("SWAP", "_SWAP", True)

            objs.AddProcedure("OPEN" + Chr(34) + "i" + Chr(34) + ",#", "_OPEN " + Chr(34) + "i" + Chr(34) + " ,", True)
            objs.AddProcedure("OPEN" + Chr(34) + "o" + Chr(34) + ",#", "_OPEN " + Chr(34) + "o" + Chr(34) + " ,", True)
            objs.AddProcedure("OPEN" + Chr(34) + "a" + Chr(34) + ",#", "_OPEN " + Chr(34) + "a" + Chr(34) + " ,", True)
            objs.AddProcedure("OPEN" + Chr(34) + "r" + Chr(34) + ",#", "_OPEN " + Chr(34) + "r" + Chr(34) + " ,", True)

            objs.AddProcedure("BringWindowToTop", "BringWindowToTop", True)
            objs.AddProcedure("SetWindowPos", "SetWindowPos", True)
            objs.AddProcedure("GlobalCompact", "GlobalCompact", True)
            objs.AddProcedure("SystemHeapInfo", "SystemHeapInfo", True)

            'Wird als Prozedur verwendet
            objs.AddProcedure("EXEC", "_EXEC", True)

            'Von VB.Net verwendet!
            objs.AddProcedure("PRINT", True)


            '  Dim t As Integer
            ' For t = 0 To objs.Count - 1
            'If objs(t).ObjType = LangageObjectType.OBJTYPE_PROCEDURE Then
            'hashProcedures(objs(t).UName) = True
            ' End If
            ' Next

        End Sub
        Public Sub AddConst(ByVal name As String)
            objs.AddKeyword(name, name)
        End Sub
        Public Sub InitConstants()

            AddConst("WS_OVERLAPPED")
            AddConst("WS_CLIPSIBLINGS")
            AddConst("WS_SYSMENU")
            AddConst("WS_VISIBLE")
            AddConst("WS_THICKFRAME")
            AddConst("WS_MAXIMIZEBOX")
            AddConst("WS_MINIMIZEBOX")

            AddConst("VK_LBUTTON") ' 1
            AddConst("VK_RBUTTON") ' 2
            AddConst("VK_MBUTTON") ' 4

            AddConst("IDI_ERROR") ' 32513
            AddConst("IDI_WARNING") ' 32515
            AddConst("IDI_QUESTION") ' 32514

            AddConst("SW_ERASE") ' 4
            AddConst("SW_HIDE") ' 0
            AddConst("SW_INVALIDATE") ' 2
            AddConst("SW_MAX") ' 10
            AddConst("SW_MAXIMIZE") ' 3
            AddConst("SW_MINIMIZE") ' 6
            AddConst("SW_NORMAL") ' 1
            AddConst("SW_OTHERUNZOOM") ' 4
            AddConst("SW_OTHERZOOM") ' 2
            AddConst("SW_PARENTCLOSING") ' 1
            AddConst("SW_PARENTOPENING") ' 3
            AddConst("SW_RESTORE") ' 9
            AddConst("SW_SCROLLCHILDREN") ' 1
            AddConst("SW_SHOW") ' 5
            AddConst("SW_SHOWDEFAULT") ' 10
            AddConst("SW_SHOWMAXIMIZED") ' 3
            AddConst("SW_SHOWMINIMIZED") ' 2
            AddConst("SW_SHOWMINNOACTIVE") ' 7
            AddConst("SW_SHOWNA") ' 8
            AddConst("SW_SHOWNOACTIVATE") ' 4
            AddConst("SW_SHOWNORMAL") ' 1

            AddConst("MB_ICONASTERISK") ' 64
            AddConst("MB_ICONHAND") ' 16
            AddConst("MB_OK") ' 0
            AddConst("MB_ICONQUESTION") ' 32
            AddConst("MB_ICONEXCLAMATION") ' 48

            AddConst("WM_CLOSE") ' 16
            AddConst("WM_PAINT") ' 15
            AddConst("WM_CHAR") ' 258
            AddConst("WM_NULL") ' 0
            AddConst("WM_LBUTTONDOWN") ' 513
            AddConst("WM_LBUTTONUP") ' 514
            AddConst("WM_LBUTTONDBLCLK") ' 515
            AddConst("WM_RBUTTONDBLCLK") ' 518
            AddConst("WM_RBUTTONDOWN") ' 516
            AddConst("WM_RBUTTONUP") ' 517
            AddConst("WM_COMMAND") ' 273
            AddConst("WM_SIZE") ' 5

            AddConst("SIZE_MAXHIDE") ' 4
            AddConst("SIZE_MAXIMIZED") ' 2
            AddConst("SIZE_MAXSHOW") ' 3
            AddConst("SIZE_MINIMIZED") ' 1
            AddConst("SIZE_RESTORED") ' 0

            AddConst("SIZEICONIC") ' 1
            AddConst("SIZEFULLSCREEN") ' 2

            'Konstanten für _DLG_COLOR
            AddConst("CC_RGBINIT") ' 1
            AddConst("CC_FULLOPEN") ' 2
            AddConst("CC_ANYCOLOR") ' 256
            AddConst("CC_PREVENTFULLOPEN") ' 4
            AddConst("CC_SOLIDCOLOR") ' 128
            AddConst("CC_SHOWHELP") ' 8

            AddConst("SWP_ASYNCWINDOWPOS") ' 16384
            AddConst("SWP_DEFERERASE") ' 8192
            AddConst("SWP_DRAWFRAME") ' 32
            AddConst("SWP_FRAMECHANGED") ' 32
            AddConst("SWP_HIDEWINDOW") ' 128
            AddConst("SWP_NOACTIVATE") ' 16
            AddConst("SWP_NOCOPYBITS") ' 256
            AddConst("SWP_NOMOVE") ' 2
            AddConst("SWP_NOOWNERZORDER") ' 512
            AddConst("SWP_NOREDRAW") ' 8
            AddConst("SWP_NOREPOSITION") ' 512
            AddConst("SWP_NOSENDCHANGING") ' 1024
            AddConst("SWP_NOSIZE") ' 1
            AddConst("SWP_NOZORDER") ' 4

            AddConst("SB_BOTH") ' 3
            AddConst("SB_BOTTOM") ' 7
            AddConst("SB_CTL") ' 2
            AddConst("SB_HORZ") ' 0
            AddConst("SB_VERT") ' 1

            'Zwischenablageformate
            AddConst("CF_BITMAP") ' 2
            AddConst("CF_TEXT") ' 1

            'Menü
            AddConst("MF_MENUBARBREAK") ' 32
            AddConst("MF_CHECKED") ' 8

            'Konstanten für Druckerdialog
            AddConst("PD_ALLPAGES") ' 0
            AddConst("PD_COLLATE") ' 16
            AddConst("PD_DISABLEPRINTTOFILE") ' 524288
            AddConst("PD_ENABLEPRINTHOOK") ' 4096
            AddConst("PD_ENABLEPRINTTEMPLATE") ' 16384
            AddConst("PD_ENABLEPRINTTEMPLATEHANDLE") ' 65536
            AddConst("PD_ENABLESETUPHOOK") ' 8192
            AddConst("PD_ENABLESETUPTEMPLATE") ' 32768
            AddConst("PD_ENABLESETUPTEMPLATEHANDLE") ' 131072
            AddConst("PD_HIDEPRINTTOFILE") ' 1048576
            AddConst("PD_NOPAGENUMS") ' 8
            AddConst("PD_NOSELECTION") ' 4
            AddConst("PD_NOWARNING") ' 128
            AddConst("PD_PAGENUMS") ' 2
            AddConst("PD_PRINTSETUP") ' 64
            AddConst("PD_PRINTTOFILE") ' 32
            AddConst("PD_RETURNDC") ' 256
            AddConst("PD_RETURNDEFAULT") ' 1024
            AddConst("PD_RETURNIC") ' 512
            AddConst("PD_SELECTION") ' 1
            AddConst("PD_SHOWHELP") ' 2048
            AddConst("PD_USEDEVMODECOPIES") ' 262144




            'Konstanten für GetDeviceCaps
            AddConst("HORZRES") ' 8
            AddConst("VERTRES") ' 10
            AddConst("VERTSIZE") ' 6
            AddConst("HORZSIZE") ' 4
            AddConst("NUMCOLORS") ' 24

            'Konstanten für GetStockObject
            AddConst("BLACK_BRUSH") ' 4
            AddConst("BLACK_PEN") ' 7
            AddConst("WHITE_BRUSH") ' 0
            AddConst("WHITE_PEN") ' 6
            AddConst("DKGRAY_BRUSH") ' 3
            AddConst("GRAY_BRUSH") ' 2
            AddConst("HOLLOW_BRUSH") ' 5
            AddConst("NULL_PEN") ' 8
            AddConst("LTGRAY_BRUSH") ' 1
            AddConst("DEFAULT_PALETTE") ' 15
            AddConst("NULL_BRUSH") ' 5
            AddConst("SYSTEM_FONT") ' 13
            AddConst("SYSTEM_FIXED_FONT") ' 16
            AddConst("ANSI_FIXED_FONT") ' 11
            AddConst("ANSI_VAR_FONT") ' 12
            AddConst("DEVICE_DEFAULT_FONT") ' 14
            AddConst("OEM_FIXED_FONT") ' 10
            AddConst("DEFAULT_GUI_FONT") ' 17

            'Konstanten für GetObjectType
            AddConst("OBJ_BITMAP") ' 7
            AddConst("OBJ_BRUSH") ' 2
            AddConst("OBJ_DC") ' 3
            AddConst("OBJ_ENHMETADC") ' 12
            AddConst("OBJ_ENHMETAFILE") ' 13
            AddConst("OBJ_EXTPEN") ' 11
            AddConst("OBJ_FONT") ' 6
            AddConst("OBJ_MEMDC") ' 10
            AddConst("OBJ_METADC") ' 4
            AddConst("OBJ_METAFILE") ' 9
            AddConst("OBJ_PAL") ' 5
            AddConst("OBJ_PEN") ' 1
            AddConst("OBJ_REGION") ' 8

            'Konstanten für SetBkMode
            AddConst("TRANSPARENT") ' 1
            AddConst("OPAQUE") ' 2

            'Konstanten für SetROP2
            AddConst("R2_BLACK") ' 1
            AddConst("R2_COPYPEN") ' 13
            AddConst("R2_LAST") ' 16
            AddConst("R2_MASKNOTPEN") ' 3
            AddConst("R2_MASKPEN") ' 9
            AddConst("R2_MASKPENNOT") ' 5
            AddConst("R2_MERGENOTPEN") ' 12
            AddConst("R2_MERGEPEN") ' 15
            AddConst("R2_MERGEPENNOT") ' 14
            AddConst("R2_NOP") ' 11
            AddConst("R2_NOT") ' 6
            AddConst("R2_NOTCOPYPEN") ' 4
            AddConst("R2_NOTMASKPEN") ' 8
            AddConst("R2_NOTMERGEPEN") ' 2
            AddConst("R2_NOTXORPEN") ' 10
            AddConst("R2_WHITE") ' 16
            AddConst("R2_XORPEN") ' 7

            'Konstanten für LoadImage
            AddConst("IMAGE_BITMAP") ' 0
            AddConst("IMAGE_CURSOR") ' 2
            AddConst("IMAGE_ICON") ' 1
            AddConst("LR_COLOR") ' 2
            AddConst("LR_COPYDELETEORG") ' 8
            AddConst("LR_COPYFROMRESOURCE") ' 16384
            AddConst("LR_COPYRETURNORG") ' 4
            AddConst("LR_CREATEDIBSECTION") ' 8192
            AddConst("LR_DEFAULTCOLOR") ' 0
            AddConst("LR_DEFAULTSIZE") ' 64
            AddConst("LR_LOADFROMFILE") ' 16
            AddConst("LR_LOADMAP3DCOLORS") ' 4096
            AddConst("LR_LOADTRANSPARENT") ' 32
            AddConst("LR_MONOCHROME") ' 1
            AddConst("LR_SHARED") ' 32768
            AddConst("LR_VGACOLOR") ' 128


            'Public Const PS_ALTERNATE") ' 8
            'Public Const PS_COSMETIC") ' 0
            AddConst("PS_DASH") ' 1
            AddConst("PS_DASHDOT") ' 3
            AddConst("PS_DASHDOTDOT") ' 4
            AddConst("PS_DOT") ' 2
            AddConst("PS_NULL") ' 5
            AddConst("PS_SOLID") ' 0

            'Konstanten für BitBlt, StretchBlt
            AddConst("BLACKNESS") ' 66
            AddConst("WHITENESS") ' 16711778
            AddConst("SRCAND") ' 8913094
            AddConst("SRCCOPY") ' 13369376
            AddConst("SRCERASE") ' 4457256
            AddConst("SRCINVERT") ' 6684742
            AddConst("SRCPAINT") ' 15597702
            AddConst("DSTINVERT") ' 5570569
            AddConst("MERGECOPY") ' 12583114
            AddConst("MERGEPAINT") ' 12255782
            AddConst("NOTSRCCOPY") ' 3342344
            AddConst("NOTSRCERASE") ' 1114278
            AddConst("PATCOPY") ' 15728673
            AddConst("PATINVERT") ' 5898313
            AddConst("PATPAINT") ' 16452105

            'Konstanten für SetStretchBltMode
            AddConst("BLACKONWHITE") ' 1
            AddConst("COLORONCOLOR") ' 3
            AddConst("HALFTONE") ' 4

            'Konstanten für CreateFont
            AddConst("FW_BLACK") ' 900
            AddConst("FW_BOLD") ' 700
            AddConst("FW_DEMIBOLD") ' 600
            AddConst("FW_DONTCARE") ' 0
            AddConst("FW_EXTRABOLD") ' 800
            AddConst("FW_EXTRALIGHT") ' 200
            AddConst("FW_HEAVY") ' 900
            AddConst("FW_LIGHT") ' 300
            AddConst("FW_MEDIUM") ' 500
            AddConst("FW_NORMAL") ' 400
            AddConst("FW_REGULAR") ' 400
            AddConst("FW_SEMIBOLD") ' 600
            AddConst("FW_THIN") ' 100
            AddConst("FW_ULTRABOLD") ' 800
            AddConst("FW_ULTRALIGHT") ' 200

            AddConst("ANSI_CHARSET") ' 0
            AddConst("BALTIC_CHARSET") ' 186
            AddConst("CHINESEBIG5_CHARSET") ' 136
            AddConst("DEFAULT_CHARSET") ' 1
            AddConst("EASTEUROPE_CHARSET") ' 238
            AddConst("GB2312_CHARSET") ' 134
            AddConst("GREEK_CHARSET") ' 161
            AddConst("MAC_CHARSET") ' 77
            AddConst("OEM_CHARSET") ' 255
            AddConst("RUSSIAN_CHARSET") ' 204
            AddConst("SHIFTJIS_CHARSET") ' 128
            AddConst("SYMBOL_CHARSET") ' 2
            AddConst("TURKISH_CHARSET") ' 162
            AddConst("VIETNAMESE_CHARSET") ' 163

            AddConst("DEFAULT_PITCH") ' 0
            AddConst("VARIABLE_PITCH") ' 2
            AddConst("FIXED_PITCH") ' 1

            'Konstanten für DrawText
            AddConst("DT_BOTTOM") ' 8
            AddConst("DT_CALCRECT") ' 1024
            AddConst("DT_CENTER") ' 1
            AddConst("DT_EDITCONTROL") ' 8192
            AddConst("DT_END_ELLIPSIS") ' 32768
            AddConst("DT_EXPANDTABS") ' 64
            AddConst("DT_EXTERNALLEADING") ' 512
            AddConst("DT_HIDEPREFIX") ' 1048576
            AddConst("DT_INTERNAL") ' 4096
            AddConst("DT_LEFT") ' 0
            AddConst("DT_MODIFYSTRING") ' 65536
            AddConst("DT_NOCLIP") ' 256
            AddConst("DT_NOFULLWIDTHCHARBREAK") ' 524288
            AddConst("DT_NOPREFIX") ' 2048
            AddConst("DT_PATH_ELLIPSIS") ' 16384
            AddConst("DT_PREFIXONLY") ' 2097152
            AddConst("DT_RIGHT") ' 2
            AddConst("DT_RTLREADING") ' 131072
            AddConst("DT_SINGLELINE") ' 32
            AddConst("DT_TABSTOP") ' 128
            AddConst("DT_TOP") ' 0
            AddConst("DT_VCENTER") ' 4
            AddConst("DT_WORD_ELLIPSIS") ' 262144
            AddConst("DT_WORDBREAK") ' 16

            'Konstante für SetDIBitsToDevice
            AddConst("DIB_RGB_COLORS") ' 0
            AddConst("DIB_PAL_COLORS") ' 1

            AddConst("BI_bitfields") ' 3
            AddConst("BI_JPEG") ' 4
            AddConst("BI_PNG") ' 5
            AddConst("BI_RGB") ' 0
            AddConst("BI_RLE4") ' 2
            AddConst("BI_RLE8") ' 1


        End Sub
        Public Sub InitFunctions()
            objs.AddFunction("FRE", "_FRE", "", True)
            objs.AddFunction("LOWER", "_LOWER", "$", True)
            objs.AddFunction("UPPER", "_UPPER", "$", True)
            objs.AddFunction("INSTR", "_INSTR", "", True)
            objs.AddFunction("SHR", "_SHR", "", True)
            objs.AddFunction("SHL", "_SHL", "", True)
            objs.AddFunction("EXIST", "_EXIST", "", True)
            objs.AddFunction("EOF(#", "_EOF(", "", True)
            objs.AddFunction("LOF(#", "_LOF(", "", True)
            objs.AddFunction("ROUND", "_ROUND", "", True)
            objs.AddFunction("MAX", "_MAX", "", True)
            objs.AddFunction("MIN", "_MIN", "", True)
            objs.AddFunction("TAN", "_TAN", "", True)
            objs.AddFunction("ACOS", "_ACOS", "", True)
            objs.AddFunction("ASIN", "_ASIN", "", True)
            objs.AddFunction("COS", "_COS", "", True)
            objs.AddFunction("SIN", "_SIN", "", True)
            objs.AddFunction("ATN", "_ATN", "", True)
            objs.AddFunction("ATAN", "_ATAN", "", True)
            objs.AddFunction("LOG10", "_LOG10", "", True)
            objs.AddFunction("LOG2", "_LOG2", "", True)
            objs.AddFunction("LOG", "_LOG", "", True)
            objs.AddFunction("ABS", "_ABS", "", True)
            objs.AddFunction("FIX", "_FIX", "", True)
            objs.AddFunction("INT", "_INT", "", True)
            objs.AddFunction("FLOOR", "_FLOOR", "", True)
            objs.AddFunction("FACT", "_FACT", "", True)
            objs.AddFunction("EXP", "_EXP", "", True)
            objs.AddFunction("EQV", "_EQV", "", True)
            objs.AddFunction("PRED", "_PRED", "", True)
            objs.AddFunction("SUCC", "_SUCC", "", True)
            objs.AddFunction("RAD", "_RAD", "", True)
            objs.AddFunction("DEG", "_DEG", "", True)
            objs.AddFunction("TRUNC", "_TRUNC", "", True)
            objs.AddFunction("RAND", "_RAND", "", True)
            objs.AddFunction("ODD", "_ODD", "", True)
            objs.AddFunction("EVEN", "_EVEN", "", True)
            objs.AddFunction("SGN", "_SGN", "", True)
            objs.AddFunction("UWORD", "_UWORD", "", True)
            objs.AddFunction("RANDOM", "_RANDOM", "", True)
            objs.AddFunction("XOR", "_XOR", "", True)
            objs.AddFunction("MOD", "_MOD", "", True)
            objs.AddFunction("SHORT", "_SHORT", "", True)
            objs.AddFunction("USHORT", "_USHORT", "", True)
            objs.AddFunction("BYT", "_BYTE", "", True)
            objs.AddFunction("BIN", "_BIN", "$", True)
            objs.AddFunction("HEX", "_HEX", "$", True)
            objs.AddFunction("OCT", "_OCT", "$", True)
            objs.AddFunction("DEC", "_DEC", "$", True)
            objs.AddFunction("STRING", "_STRING", "$", True)
            objs.AddFunction("SPACE", "_SPACE", "$", True)
            objs.AddFunction("MIRROR", "_MIRROR", "$", True)
            objs.AddFunction("UPPER", "_UPPER", "$", True)
            objs.AddFunction("LOWER", "_LOWER", "$", True)
            objs.AddFunction("RINSTR", "_RINSTR", "", True)
            objs.AddFunction("VAL", "_VAL", "", True)
            objs.AddFunction("_DC", "__DC", "", True)
            objs.AddFunction("ZOOMED?", "_ZOOMED", "", True)
            objs.AddFunction("ICONIC?", "_ICONIC", "", True)
            objs.AddFunction("GETDEVCAPS", "_GETDEVCAPS", "", True)
            objs.AddFunction("LOADBMP", "_LOADBMP", "", True)
            objs.AddFunction("TXTLEN", "_TXTLEN", "", True)
            objs.AddFunction("TAB", "_TAB", "", True)
            objs.AddFunction("CRSLIN", "_CRSLIN", "", True)
            objs.AddFunction("CRSCOL", "_CRSCOL", "", True)
            objs.AddFunction("DATE", "_DATE$", "$", True)
            objs.AddFunction("TIME", "_TIME", "", True)
            objs.AddFunction("TIMER", "_TIMER", "", True)
            objs.AddFunction("MOUSESX", "_MOUSESX", "", True)
            objs.AddFunction("MOUSESY", "_MOUSESY", "", True)
            objs.AddFunction("MOUSEK", "_MOUSEK", "", True)
            objs.AddFunction("MOUSEX", "_MOUSEX", "", True)
            objs.AddFunction("MOUSEY", "_MOUSEY", "", True)
            objs.AddFunction("_X", "__X", "", True)
            objs.AddFunction("_Y", "__Y", "", True)
            'EXEC ist eine Prozedure
            'objs.AddFunction("EXEC", "_EXEC","", True)
            objs.AddFunction("INKEY", "_INKEY", "$", True)
            objs.AddFunction("_DOSCMD", "__DOSCMD", "$", True)
            objs.AddFunction("MENU", "_MENU", "", True)

            'objs.AddFunction "WIN", "_WIN"
            objs.AddFunction("WIN", "_WIN_HANDLE", "", True)
            objs.AddFunction("WIN()", "_WIN()", "", True)

            objs.AddFunction("ASC", "Asc", "", True) 'Wird zwar nicht konvertier muss aber bekannt sein!
            objs.AddFunction("GetPixel", "GetPixel", "", True)
            objs.AddFunction("StretchDIBits", "StretchDIBits", "", True)
            objs.AddFunction("GetKeyState", "GetKeyState", "", True)

            objs.AddFunction("MFREE", "_MFREE", "", True)
            objs.AddFunction("LOCARD", "_LOCARD", "", True)
            objs.AddFunction("_ECX", "__ECX", "", True)

            objs.AddFunction("PRINTERDC", "_PRINTERDC", "", True)

            objs.AddFunction("SINH", "_SINH", "", True)
            objs.AddFunction("COSH", "_COSH", "", True)
            objs.AddFunction("TANH", "_TANH", "", True)
            objs.AddFunction("SQR", "_SQR", "", True)

            objs.AddFunction("MALLOC", "_MALLOC", "", True)

            objs.AddFunction("LEFT", "Left", "$", True)
            objs.AddFunction("RIGHT", "Right", "$", True)
            objs.AddFunction("MID", "Mid", "$", True)
            objs.AddFunction("TRIM", "Trim", "$", True)
            objs.AddFunction("CHR", "Chr", "$", True)
            objs.AddFunction("STR", "_STR", "$", True)
            'objs.AddFunction("TRIM", "Trim", "", True)
            objs.AddFunction("LEN", "Len", "", True)

            objs.AddFunction("MKL", "_MKL", "", True)
            objs.AddFunction("CVI", "_CVI", "", True)

            'Sehr hässlicher fall
            objs.AddFunction("CF_SCREENFONTS|CF_EFFECTS|CF_TTONLY", "CF_SCREENFONTS or CF_EFFECTS or CF_TTONLY", "", True)

            objs.AddFunction("DestroyMenu", "DestroyMenu", "", True)
            objs.AddFunction("GetMenu", "GetGFAMenu", "", True)




        End Sub
        Public Sub Init()
            InitKeywords()
            InitFunctions()
            InitProcedures()
            InitConstants()
        End Sub
        Public Sub AnalyzeRestores(ByVal lines As List(Of String))

            For Each line As String In lines
                line = line.Replace(":", "")
                line = UCase(Trim(line))

                If Left(line, Len("RESTORE ")) = "RESTORE " Then
                    restorePoints.Add(Trim(Right(line, Len(line) - Len("RESTORE "))), True)
                End If
            Next
        End Sub
        Public Sub AnalyzeProcedures(ByVal lines As List(Of String))
            Dim line As String = ""
            Dim splitted As List(Of String)
            Dim oldComment As String = ""

            For i = 0 To lines.Count - 1
                line = lines(i)

                splitted = CombineProcedures(ConvertStringListHelper.AdaptSharpChar(ConvertStringListHelper.RemoveComments(ConvertStringListHelper.RemoveSpaces(ConvertStringListHelper.SplitLine(PrepareLine(line))))))

                If splitted.Count > 1 Then
                    If splitted(0) = ">" And ((splitted(1).ToUpper = "PROCEDURE") Or (splitted(1).ToUpper = "FUNCTION")) Then
                        splitted.RemoveAt(0) 'Einrücksymbol entfernen
                    End If
                End If

                If splitted.Count >= 2 Then
                    If UCase(splitted(0)) = "PROCEDURE" Then
                        objs.AddProcedure(splitted(1), False)
                    End If
                    If UCase(splitted(0)) = "FUNCTION" Then
                        objs.AddFunction(objs.ExtractName(splitted(1)), "", objs.ExtractDataType(splitted(1)), False)
                    End If
                End If
            Next
        End Sub
        'Schlecht gelöst....
        Public last_dimensions As Integer
        Public last_doAddDimensions As Boolean = False
        'Liefert "varname()" bei array
        'VAR im Parameter wird nicht zurückgegeben!
        Public Function GetParameterNames(ByVal names As List(Of String), ByVal start As Integer, ByVal bracketIndex As Integer) As List(Of String)
            Dim result As List(Of String) = New List(Of String)
            Dim bracket As Integer = 0
            Dim hasBracket As Boolean = False
            Dim name As String = ""
            Dim done As Boolean = False
            last_dimensions = 1

            For i = start To names.Count - 1
                done = False
                If names(i) = "(" Then
                    bracket += 1
                    hasBracket = True
                    done = True
                End If
                If names(i) = ")" Then
                    'hasBracket = True 'Es dürfen nur aufgehende Klammern gewertet werden (Letzter Param bei Procedure abc(...))
                    bracket -= 1
                    done = True
                End If

                If names(i) <> "," And UCase(names(i)) <> "VAR" Then '2010-10-10 TES VAR als "," interpretieren
                    If bracket = bracketIndex And done = False Then
                        name = names(i)
                        hasBracket = False 'Muss hier auf False gesetzt werden (Probl. erster Param bei Prozedur...)
                    End If
                Else
                    last_dimensions += 1
                    If bracket = bracketIndex Then
                        If hasBracket Then name += "()"
                        If last_doAddDimensions Then
                            ArrayDimensions(name) = last_dimensions
                        End If
                        last_dimensions = 1

                        result.Add(name)
                        hasBracket = False
                    End If
                End If
            Next
            If hasBracket Then name += "()"
            result.Add(name)
            If last_doAddDimensions Then
                ArrayDimensions(UCase(name)) = last_dimensions
            End If
            Return result
        End Function
        Public Sub AnalyzeDeclarations(ByVal lines As List(Of String))
            Dim line As String = ""
            Dim splitted As List(Of String)
            Dim oldComment As String = ""
            Dim InFunction As Boolean = False
            Dim Subrutine As String = ""

            For i = 0 To lines.Count - 1
                line = lines(i)

                splitted = CombineProcedures(ConvertStringListHelper.AdaptSharpChar(ConvertStringListHelper.RemoveComments(ConvertStringListHelper.RemoveSpaces(ConvertStringListHelper.SplitLine(PrepareLine(line))))))

                If splitted.Count > 1 Then
                    If splitted(0) = ">" And ((splitted(1).ToUpper = "PROCEDURE") Or (splitted(1).ToUpper = "FUNCTION")) Then
                        splitted.RemoveAt(0) 'Einrücksymbol entfernen
                    End If
                End If

                If splitted.Count >= 2 Then

                    If UCase(splitted(0)) = "PROCEDURE" Then
                        InFunction = False
                        Subrutine = splitted(1)

                        Dim vars As List(Of String) = GetParameterNames(splitted, 2, 1)
                        For Each var As String In vars

                            If InStr(var, "()") Then
                                var = var.Replace("()", "")
                                objs.AddArray(objs.ExtractName(var), objs.ExtractDataType(var), Subrutine)
                            Else
                                objs.AddVariable(objs.ExtractName(var), objs.ExtractDataType(var), Subrutine)
                            End If
                        Next
                    End If
                    If UCase(splitted(0)) = "FUNCTION" Then
                        InFunction = True
                        Subrutine = splitted(1)
                        Dim vars As List(Of String) = GetParameterNames(splitted, 2, 1)
                        For Each var As String In vars

                            If InStr(var, "()") Then
                                var = var.Replace("()", "")
                                objs.AddArray(objs.ExtractName(var), objs.ExtractDataType(var), Subrutine)
                            Else
                                objs.AddVariable(objs.ExtractName(var), objs.ExtractDataType(var), Subrutine)
                            End If
                        Next
                    End If
                End If

                If splitted.Count > 1 Then

                    If UCase(splitted(0)) = "DIM" Then
                        last_doAddDimensions = True
                        Dim vars As List(Of String) = GetParameterNames(splitted, 1, 0)
                        last_doAddDimensions = False
                        For Each var As String In vars
                            If InStr(var, "()") = 0 Then
                                Debug.WriteLine("bei DIM sollte es keine Variablen geben!")
                            End If
                            var = var.Replace("()", "")

                            objs.AddArray(objs.ExtractName(var), objs.ExtractDataType(var), "") 'Immer global!
                        Next
                    End If

                    If UCase(splitted(0)) = "LOCAL" Then
                        Dim vars As List(Of String) = GetParameterNames(splitted, 1, 0)
                        For Each var As String In vars
                            If InStr(var, "()") Then
                                Debug.WriteLine("bei LOCAL sollte es keine Array geben!")
                            End If
                            objs.AddVariable(objs.ExtractName(var), objs.ExtractDataType(var), Subrutine)
                        Next
                    End If
                End If
            Next
        End Sub
        Public Sub AnalyzeUndeclared(ByVal lines As List(Of String))
            Dim line As String = ""
            Dim splitted As List(Of String)
            Dim oldComment As String = ""
            Dim tokenType As LangageObjectType
            Dim Done As Boolean = False
            Dim InFunction As Boolean = False
            Dim Subrutine As String = ""
            Dim InType As Boolean = False

            For i = 0 To lines.Count - 1
                line = lines(i)


                splitted = CombineProcedures(ConvertStringListHelper.AdaptSharpChar(ConvertStringListHelper.RemoveComments(ConvertStringListHelper.RemoveSpaces(ConvertStringListHelper.SplitLine(PrepareLine(line))))))
                Done = False
                If InType Then
                    Done = True
                End If

                If splitted.Count > 1 Then
                    If splitted(splitted.Count - 1) = ":" Then
                        'ES ist ein Label!
                        Done = True
                    End If
                    If splitted(0) = "DATA" Then
                        'ES ist ein Databereich!
                        Done = True
                    End If
                    If splitted(0) = "RESTORE" Then
                        'Keine Variable!
                        Done = True
                    End If

                    If splitted(0) = "TYPE" Then
                        Done = True
                        InType = True
                    End If

                    If splitted(0) = "DLL" Then
                        Done = True
                        InType = True
                    End If


                    If splitted(0) = "ONMENUGOSUB" Or splitted(0) = "ONMENUMESSAGEGOSUB" Or splitted(0) = "ONMENUKEYGOSUB" Then
                        Done = True
                    End If

                End If
                If splitted.Count > 0 Then
                    If splitted(0) = "ENDTYPE" Then
                        Done = True
                        InType = False
                    End If

                    If splitted(0) = "ENDDLL" Then
                        Done = True
                        InType = False
                    End If
                End If


                If splitted.Count >= 2 Then
                    If UCase(splitted(0)) = "PROCEDURE" Then
                        InFunction = False
                        Subrutine = splitted(1)
                        Done = True
                    End If
                    If UCase(splitted(0)) = "FUNCTION" Then
                        InFunction = True
                        Subrutine = splitted(1)
                        Done = True
                    End If
                End If

                If splitted.Count > 1 Then
                    If UCase(splitted(0)) = "DIM" Then
                        Done = True
                    End If
                    If UCase(splitted(0)) = "LOCAL" Then
                        Done = True
                    End If
                End If

                If Done = False Then
                    For j = 0 To splitted.Count - 1
                        tokenType = objs.GetTokenType(splitted, j, False)
                        If tokenType = LangageObjectType.OBJTYPE_VARIABLE Then
                            If objs.FindObjType(objs.ExtractName(splitted(j)), objs.ExtractDataType(splitted(j)), Subrutine, LangageObjectType.OBJTYPE_VARIABLE) Is Nothing And objs.FindObjType(objs.ExtractName(splitted(j)), objs.ExtractDataType(splitted(j)), "", LangageObjectType.OBJTYPE_VARIABLE) Is Nothing Then
                                Me.objs.TMPOUT.WriteLine("Declare " + objs.ExtractName(splitted(j)) + "  type: " + objs.ExtractDataType(splitted(j)))
                                If Trim(objs.ExtractName(splitted(j))) <> "" Then
                                    objs.AddVariable(objs.ExtractName(splitted(j)), objs.ExtractDataType(splitted(j)), "") ' Immer nur als Global deklarieren
                                End If
                            End If
                        End If
                    Next
                End If

            Next
        End Sub
        Public Sub ResolveConflicts()
            objs.ResolveConflicts()
        End Sub
        Public Sub RemoveBuildIn()
            objs.RemoveBuildIn()
        End Sub
        Public Function GetGlobalDeclarations() As List(Of String)
            Dim lines As List(Of String) = New List(Of String)
            Dim ext As String


            lines.Add("'================================================")
            lines.Add("'GLOBAL VARIABLES")
            lines.Add("'================================================")


            For i = 0 To Me.objs.Count - 1

                If objs(i).ObjType = LangageObjectType.OBJTYPE_VARIABLE And objs(i).USubrutine = "" Then
                    ext = GetDeclaration(objs(i).DataType)

                    'Evtl. nicht alle Bitmaps, Fonts fehlen!
                    If Left(UCase(objs(i).GetNewName()), Len("BITMAP")) = "BITMAP" And objs(i).DataType = "&" Then
                        ext = " As IntPtr"
                    End If

                    Dim line As String = "Public " + GetFinalVarName(objs(i).GetNewName(), objs(i).DataType)
                    If objs(i).DataType = "$" Then
                    Else
                        line += ext
                    End If


                    If InStr(objs(i).GetNewName(), ".") Or InStr(objs(i).GetNewName(), "{") Then
                        line = "' " + line + "' ERROR: Ungültiger Variablenname"
                    End If

                    If Len(line) < 50 Then
                        lines.Add(LSet(line, 50) + "' Org: " + objs(i).Name + objs(i).DataType)

                    Else
                        lines.Add(line + "    ' Org: " + objs(i).Name + objs(i).DataType)

                    End If
                End If

            Next

            lines.Add("'================================================")
            lines.Add("'GLOBAL ARRAYS")
            lines.Add("'================================================")

            For i = 0 To Me.objs.Count - 1

                If objs(i).ObjType = LangageObjectType.OBJTYPE_ARRAY And objs(i).USubrutine = "" Then
                    ext = GetDeclaration(objs(i).DataType)
                    'Evtl. nicht alle Bitmaps, Fonts fehlen!
                    If Left(UCase(objs(i).GetNewName()), Len("BITMAP")) = "BITMAP" And objs(i).DataType = "&" Then
                        ext = " As IntPtr"
                    End If


                    Dim dim_text As String = "()"
                    If ArrayDimensions.Contains(objs(i).Name + objs(i).DataType) Then
                        Dim dims As Integer = ArrayDimensions(UCase(objs(i).Name) + objs(i).DataType + "()")
                        If dims > 0 Then
                            dim_text = "(" + New String(","c, dims - 1) + ")"
                        End If
                    End If


                    Dim line As String = "Public " + GetFinalVarName(objs(i).GetNewName(), objs(i).DataType) + dim_text ' + ext
                    If objs(i).DataType = "$" Then
                    Else
                        line += ext
                    End If

                    If InStr(objs(i).GetNewName(), ".") Or InStr(objs(i).GetNewName(), "{") Then
                        line = "' " + line + "' ERROR: Ungültiger Arrayname"
                    End If

                    If Len(line) < 50 Then
                        lines.Add(LSet(line, 50) + "' Org: " + objs(i).Name + objs(i).DataType + dim_text)
                    Else
                        lines.Add(line + "' Org: " + objs(i).Name + objs(i).DataType + dim_text)
                    End If

                End If

            Next
            lines.Add("'================================================")
            lines.Add("'ENDE DER DEKLARATION")
            lines.Add("'================================================")

            lines.Add("")
            lines.Add("")
            lines.Add("Public Sub Main()")

            Return lines
        End Function
        Public Function GetDeclarationWithoutStr(ByVal DataType As String) As String
            Dim ext As String

            Select Case DataType
                Case ""
                    'Kein Typ bzw. Double
                    ext = " As Double"
                Case "#"
                    'Dobule
                    ext = " As Double"
                Case "%"
                    'Integer
                    ext = " As Integer"
                Case "&"
                    'Short
                    ext = " As Short"
                Case "|"
                    'Byte
                    ext = " As Byte"
                Case "!"
                    'Boolean
                    ext = " As Boolean"
                Case "$"
                    'String
                    ext = "" '  2010-10-05 nicht verwenden?!  " As String"
                Case Else
                    ext = "ERROR !!!"
            End Select

            Return ext
        End Function
        Public Function GetDeclaration(ByVal DataType As String) As String
            Dim ext As String

            Select Case DataType
                Case ""
                    'Kein Typ bzw. Double
                    ext = " As Double"
                Case "#"
                    'Dobule
                    ext = " As Double"
                Case "%"
                    'Integer
                    ext = " As Integer"
                Case "&"
                    'Short
                    ext = " As Short"
                Case "|"
                    'Byte
                    ext = " As Byte"
                Case "!"
                    'Boolean
                    ext = " As Boolean"
                Case "$"
                    'String
                    ext = " As String"
                Case Else
                    ext = "ERROR !!!"
            End Select

            Return ext
        End Function
        Function GetFinalVarName(ByVal name As String, ByVal dataType As String) As String
            If dataType = "$" Then
                Return name + "$"
            Else
                Return name
            End If
        End Function
        Public Function Convert(ByVal lines As List(Of String)) As List(Of String)
            Dim newLines As List(Of String) = GetGlobalDeclarations()
            Dim line As String = ""
            Dim splitted As List(Of String)
            Dim oldComment As String = ""
            Dim tokenType As LangageObjectType
            Dim VarMustBeLocal As Boolean = False
            Dim AddVarDeclaration As Boolean = False
            Dim Done As Boolean = False
            Dim InFunction As Boolean = False
            Dim Subrutine As String = ""
            Dim newLine As String = ""
            Dim isOwnProcedure As Boolean
            Dim LastExitStrings As List(Of String) = New List(Of String)
            Dim IsCatch As Boolean = False
            Dim LastVarDeclaration As String
            Dim LastVarDeclIsArray As Boolean
            Dim FunctionReturnType As String
            Dim IsByRef As Boolean
            Dim InFunctionOrProcedure As Boolean
            Dim AddBracketForProcedure As Boolean = False
            Dim AddArrayDeclarationDataType As Boolean = False
            Dim BracketCount As Integer = 0
            Dim NoDoublePointReplace As Boolean = False
            Dim isInDeclaration As Boolean = False
            Dim InCase As Boolean = False
            Dim InArrayDeclaration As Boolean = False
            Dim endOfMain As Boolean = False
            Dim newLineStringDeclarationLine As String = ""
            Dim newLineStringDeclarationComma As Boolean = False
            Dim IsLocalDeclaration As Boolean = False
            Dim CanRemoveBrackets As Boolean = False
            newLines.Add("")
            newLines.Add("")
            newLines.Add("")
            newLines.Add("")

            For i = 0 To lines.Count - 1
                line = lines(i)
                InCase = False
                InArrayDeclaration = False
                isInDeclaration = False
                BracketCount = 0
                NoDoublePointReplace = False

                AddArrayDeclarationDataType = False
                newLines.Add("'>>=  " + line)


                If Left(Trim(UCase(line)), Len("GOSUB ")) = "GOSUB " Then
                    line = line.Replace("GOSUB ", "")
                End If

                newLine = ""

                splitted = CombineProcedures(ConvertStringListHelper.AdaptSharpChar(ConvertStringListHelper.RemoveComments(ConvertStringListHelper.RemoveSpaces(ConvertStringListHelper.SplitLine(PrepareLine(line))), oldComment)))
                VarMustBeLocal = False
                AddVarDeclaration = False
                Done = False

                LastVarDeclaration = ""
                LastVarDeclIsArray = False
                FunctionReturnType = ""
                IsByRef = False
                InFunctionOrProcedure = False
                newLineStringDeclarationLine = ""
                newLineStringDeclarationComma = False
                IsLocalDeclaration = False
                CanRemoveBrackets = True
                If splitted.Count > 0 Then

                    Dim UFisrt As String = UCase(Trim(splitted(0)))

                    If UFisrt = "DIM" Then
                        NoDoublePointReplace = True
                        AddVarDeclaration = True
                        isInDeclaration = True
                        InArrayDeclaration = True
                        CanRemoveBrackets = False
                    End If
                    If UFisrt = "LOCAL" Then
                        NoDoublePointReplace = True
                        AddVarDeclaration = True
                        isInDeclaration = True
                        InArrayDeclaration = True
                    End If

                    If UFisrt = "PROCEDURE" Then
                        InFunctionOrProcedure = True
                        LastExitStrings.Add("SUB")
                        AddVarDeclaration = True
                        isInDeclaration = True
                        InArrayDeclaration = True
                        If endOfMain = False Then
                            newLines.Add("End Sub")
                            newLines.Add("")
                            endOfMain = True
                        End If
                        CanRemoveBrackets = False
                    End If
                    If UFisrt = "FUNCTION" Then
                        InFunctionOrProcedure = True
                        InFunction = True
                        LastExitStrings.Add("FUNCTION")
                        AddVarDeclaration = True
                        isInDeclaration = True
                        InArrayDeclaration = True
                        If endOfMain = False Then
                            newLines.Add("End Sub")
                            newLines.Add("")
                            endOfMain = True
                        End If
                        CanRemoveBrackets = False
                    End If
                    If UFisrt = "DO" Then
                        LastExitStrings.Add("DO")
                    End If
                    If UFisrt = "REPEAT" Then
                        LastExitStrings.Add("DO")
                    End If
                    If UFisrt = "WHILE" Then
                        LastExitStrings.Add("WHILE")
                    End If
                    If UFisrt = "FOR" Then
                        LastExitStrings.Add("FOR")
                    End If
                    If UFisrt = "WEND" Then
                        If LastExitStrings.Count > 0 Then
                            If LastExitStrings(LastExitStrings.Count - 1) <> "WHILE" Then
                                Debug.WriteLine("ERROR: Wend without while")
                            Else
                                LastExitStrings.RemoveAt(LastExitStrings.Count - 1)
                            End If
                        Else
                            Debug.WriteLine("ERROR: No LastExitString!")
                        End If
                    End If
                    If UFisrt = "ENDWHILE" Or UFisrt = "END WHILE" Then
                        If LastExitStrings.Count > 0 Then
                            If LastExitStrings(LastExitStrings.Count - 1) <> "WHILE" Then
                                Debug.WriteLine("ERROR: Wend without while")
                            Else
                                LastExitStrings.RemoveAt(LastExitStrings.Count - 1)
                            End If
                        Else
                            Debug.WriteLine("ERROR: No LastExitString!")
                        End If
                    End If
                    If UFisrt = "LOOP" Or UFisrt = "UNTIL" Then '2010-10-02 UNTIL hinz.
                        If LastExitStrings.Count > 0 Then
                            If LastExitStrings(LastExitStrings.Count - 1) <> "DO" Then
                                Debug.WriteLine("ERROR: Loop without Do")
                            Else
                                LastExitStrings.RemoveAt(LastExitStrings.Count - 1)
                            End If
                        Else
                            Debug.WriteLine("ERROR: No LastExitString!")
                        End If
                    End If
                    If UFisrt = "ENDFUNC" Then
                        InFunction = False
                        If LastExitStrings.Count > 0 Then
                            If LastExitStrings(LastExitStrings.Count - 1) <> "FUNCTION" Then
                                Debug.WriteLine("ERROR: END FUNCTION without FUNCTION")
                            Else
                                LastExitStrings.RemoveAt(LastExitStrings.Count - 1)
                            End If
                        Else
                            Debug.WriteLine("ERROR: No LastExitString!")
                        End If
                    End If

                    If UFisrt = "RETURN" And InFunction = False Then
                        If LastExitStrings.Count > 0 Then
                            If LastExitStrings(LastExitStrings.Count - 1) <> "SUB" Then
                                Debug.WriteLine("ERROR: RETURN without PROCEDURE")
                            Else
                                LastExitStrings.RemoveAt(LastExitStrings.Count - 1)
                            End If
                        Else
                            Debug.WriteLine("ERROR: No LastExitString!")
                        End If
                    End If

                    If UFisrt = "NEXT" Then
                        If LastExitStrings.Count > 0 Then
                            If LastExitStrings(LastExitStrings.Count - 1) <> "FOR" Then
                                Debug.WriteLine("ERROR: NEXT without FOR")
                            Else
                                LastExitStrings.RemoveAt(LastExitStrings.Count - 1)
                            End If
                        Else
                            Debug.WriteLine("ERROR: No LastExitString!")
                        End If
                    End If

                    'If splitted(0) = "RETURN" Then
                    '    If InFunction = False Then
                    '        splitted(0) = "End Sub" ' Return -> End Sub
                    '    End If
                    'End If
                End If

                If splitted.Count = 2 Then
                    If splitted(splitted.Count - 1) = ":" Then
                        NoDoublePointReplace = True
                        'ES ist ein Label!
                        newLine = splitted(0) + ":"
                        If restorePoints.Contains(UCase(Trim(splitted(0)))) Then
                            newLines.Add("_DATA_LABEL(" + Chr(34) + Trim(splitted(0)) + Chr(34) + ")")
                        End If
                        Done = True
                    End If
                End If

                If splitted.Count > 1 Then
                    If UCase(splitted(0)) = "DATA" Then
                        NoDoublePointReplace = True
                        'ES ist ein Databereich!
                        Dim tmpLine As String = Trim(line)
                        tmpLine = Right(tmpLine, Len(tmpLine) - Len("DATA "))
                        tmpLine = tmpLine.Replace(",", Chr(34) + "," + Chr(34))
                        newLine = "_DATA(" + Chr(34) + tmpLine + Chr(34) + ")"
                        Done = True
                    End If

                    If UCase(splitted(0)) = "RESTORE" Then
                        newLine = Trim(line)
                        Done = True
                    End If
                End If

                If splitted.Count >= 2 Then
                    If UCase(splitted(0)) = "PROCEDURE" Then
                        NoDoublePointReplace = True
                        InFunction = False
                        Subrutine = splitted(1)
                        VarMustBeLocal = True
                        'Sehr hässlich!
                        splitted(0) = "@"
                        newLine = "PROCEDURE"
                        AddArrayDeclarationDataType = True
                    End If
                    If UCase(splitted(0)) = "FUNCTION" Then
                        NoDoublePointReplace = True
                        InFunction = True
                        Subrutine = splitted(1)
                        VarMustBeLocal = True
                        ''''Sehr hässlich!
                        splitted(0) = "FN"
                        newLine = "FUNCTION"
                        AddArrayDeclarationDataType = True
                    End If
                End If

                If splitted.Count > 1 Then
                    If UCase(splitted(0)) = "DIM" Then
                        'VarMustBeLocal = True 'DARF NICHT AUF TRUE STEHEN!
                    End If
                    If UCase(splitted(0)) = "LOCAL" Then
                        VarMustBeLocal = True
                        IsLocalDeclaration = True
                    End If

                    If UCase(splitted(0)) = "ONMENUGOSUB" Then
                        Done = True
                        newLine = line.Replace("ON MENU GOSUB", "ONMENUGOSUB")
                    End If
                    If UCase(splitted(0)) = "ONMENUMESSAGEGOSUB" Then
                        Done = True
                        newLine = line.Replace("ON MENU MESSAGE GOSUB", "ONMENUMESSAGEGOSUB")
                    End If
                    If UCase(splitted(0)) = "ONMENUKEYGOSUB" Then
                        Done = True
                        newLine = line.Replace("ON MENU KEY GOSUB", "ONMENUKEYGOSUB")
                    End If
                    If UCase(splitted(0)) = "CASE" Then
                        InCase = True
                    End If
                End If

                AddBracketForProcedure = False
                If Done = False Then

                    For j = 0 To splitted.Count - 1

                        Dim obj As LanguageObject
                        isOwnProcedure = False

                        If AddVarDeclaration And InFunctionOrProcedure Then
                            If UCase(splitted(j)) = "VAR" Then
                                IsByRef = True
                                splitted(j) = ""
                            End If
                        End If

                        Dim token As String = splitted(j)

                        Dim prefix As String = ""
                        If IsByRef And AddVarDeclaration And InFunctionOrProcedure Then
                            prefix = "ByRef "
                        End If

                        tokenType = objs.GetTokenTypeEx(splitted, j, isOwnProcedure, isInDeclaration)

                        If token = "(" Then BracketCount += 1
                        If token = ")" Then BracketCount -= 1



                        If token <> "" Then
                            Select Case tokenType
                                Case LangageObjectType.OBJTYPE_ARRAY

                                    If VarMustBeLocal Then
                                        obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), Subrutine, LangageObjectType.OBJTYPE_ARRAY)
                                    Else
                                        obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), Subrutine, LangageObjectType.OBJTYPE_ARRAY)
                                        If obj Is Nothing Then
                                            'Global ausprobieren
                                            obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), "", LangageObjectType.OBJTYPE_ARRAY)
                                        End If
                                    End If

                                    '2010-10-10 evtl. gefährlich
                                    If InArrayDeclaration = False Then
                                        If i + 2 < splitted.Count Then
                                            If splitted(i + 1) = "(" And splitted(i + 2) = ")" Then
                                                splitted(i + 1) = "" : splitted(i + 2) = ""
                                            End If
                                        End If
                                    End If

                                    If obj IsNot Nothing Then

                                        If AddVarDeclaration Then
                                            LastVarDeclaration = "" ' Kein TYP, da redim! GetDeclaration(obj.DataType)
                                            LastVarDeclIsArray = True
                                        End If
                                        newLine += prefix + GetFinalVarName(obj.GetNewName(), obj.DataType)
                                        If AddArrayDeclarationDataType Then
                                            newLine += GetDeclaration(obj.DataType)
                                        End If

                                    Else
                                        newLine += prefix + token
                                        oldComment += "'ERROR ARRAY " + token + " NOT FOUND"
                                        Debug.WriteLine("ERROR ARRAY " + token + " NOT FOUND")
                                    End If

                                Case LangageObjectType.OBJTYPE_FUNCTION

                                    obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), LangageObjectType.OBJTYPE_FUNCTION, isOwnProcedure)
                                    If obj IsNot Nothing Then

                                        If AddVarDeclaration Then
                                            FunctionReturnType = GetDeclaration(obj.DataType)
                                        End If
                                        newLine += obj.GetNewName()
                                    Else
                                        newLine += token
                                        oldComment += "'ERROR FUNCTION " + token + " NOT FOUND"
                                        Debug.WriteLine("ERROR FUNCTION " + token + " NOT FOUND")
                                    End If

                                Case LangageObjectType.OBJTYPE_KEYWORD
                                    obj = objs.FindObjType(token, LangageObjectType.OBJTYPE_KEYWORD)
                                    If obj IsNot Nothing Then
                                        newLine += obj.GetNewName()
                                    Else
                                        newLine += token
                                        oldComment += "'ERROR KEYWORD " + token + " NOT FOUND"
                                        Debug.WriteLine("ERROR KEYWORD " + token + " NOT FOUND")
                                    End If

                                Case LangageObjectType.OBJTYPE_NUMBER
                                    newLine += token

                                Case LangageObjectType.OBJTYPE_PROCEDURE
                                    NoDoublePointReplace = True
                                    obj = objs.FindObjType(token, LangageObjectType.OBJTYPE_PROCEDURE, isOwnProcedure)
                                    If obj IsNot Nothing Then
                                        If obj.IsBuildIn Then
                                            newLine += obj.GetNewName() + "("
                                            AddBracketForProcedure = True
                                        Else
                                            newLine += obj.GetNewName()
                                        End If
                                    Else
                                        newLine += token
                                        oldComment += "'ERROR PROCEDURE " + token + " NOT FOUND"
                                        Debug.WriteLine("ERROR PROCEDURE " + token + " NOT FOUND")
                                    End If

                                Case LangageObjectType.OBJTYPE_VARIABLE

                                    If VarMustBeLocal Then
                                        obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), Subrutine, LangageObjectType.OBJTYPE_VARIABLE)
                                    Else
                                        obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), Subrutine, LangageObjectType.OBJTYPE_VARIABLE)
                                        If obj Is Nothing Then
                                            'Global ausprobieren
                                            obj = objs.FindObjType(objs.ExtractName(token), objs.ExtractDataType(token), "", LangageObjectType.OBJTYPE_VARIABLE)
                                        End If
                                    End If
                                    If obj IsNot Nothing Then

                                        Dim suffix As String = ""
                                        'Darf bei Dim abc(xyz) nicht für xyz ansprechen
                                        If AddVarDeclaration And LastVarDeclIsArray = False Then
                                            suffix = " " + GetDeclarationWithoutStr(obj.DataType)

                                        End If
                                        Dim newLineStr As String
                                        newLineStr = GetFinalVarName(obj.GetNewName(), obj.DataType) + suffix
                                        If Right(GetFinalVarName(obj.GetNewName(), obj.DataType), 1) = "$" And IsLocalDeclaration Then
                                            newLineStringDeclarationLine += prefix + newLineStr
                                            newLineStringDeclarationComma = True
                                        Else
                                            newLine += prefix + newLineStr
                                            newLineStringDeclarationComma = False
                                        End If

                                    Else
                                        newLine += prefix + token
                                        oldComment += "'ERROR VARIABLE " + token + " NOT FOUND"
                                        Debug.WriteLine("ERROR VARIABLE " + token + " NOT FOUND")
                                    End If

                                Case LangageObjectType.OBJTYPE_OPERATOR
                                    If InCase = False Then

                                        If newLineStringDeclarationComma And token = "," Then
                                            newLineStringDeclarationLine += ","
                                            newLineStringDeclarationComma = False
                                            token = " "
                                        End If
                                        If token = "," And BracketCount = 0 And NoDoublePointReplace = False Then
                                            'Zuweisung muss vorkommen, ist allerding immernoch keine 100% Lösung dann...
                                            If InStr(line, "=") Then
                                                token = ":"
                                            End If
                                        End If
                                    End If
                                    newLine += token
                                Case LangageObjectType.OBJTYPE_STRING
                                    newLine += token
                                Case LangageObjectType.OBJTYPE_UNKNOWN
                                    newLine += token
                            End Select
                        End If



                        If splitted(j) = ")" Then
                            If AddVarDeclaration Then
                                LastVarDeclIsArray = False
                                newLine += " " + LastVarDeclaration + " "
                            End If
                        End If

                        newLine += " "
                    Next

                    If InFunction Then '2010-10-03
                        If AddVarDeclaration And FunctionReturnType <> "" Then
                            newLine += " " + FunctionReturnType
                        End If
                    End If

                    If AddBracketForProcedure Then
                        newLine += " )"
                    End If

                End If


                If newLineStringDeclarationLine <> "" Then

                    If Trim(UCase(newLine)) = "DIM" Then
                        newLine = ""
                    End If
                    If Right(newLineStringDeclarationLine, 1) = "," Then
                        newLineStringDeclarationLine = Left(newLineStringDeclarationLine, Len(newLineStringDeclarationLine) - 1)
                    End If
                    newLine += ": Dim " + newLineStringDeclarationLine
                    newLine = newLine.Replace(" ,  : Dim ", " : Dim ")
                    newLineStringDeclarationLine = ""

                End If


                'Nachbearbeitung
                If InFunction = False Then
                    If UCase(Trim(newLine)) = "RETURN" Then ' RETURN IST NOCH NICHT UMBENANNT
                        If IsCatch Then
                            IsCatch = False
                            newLines.Add("End Try")
                        End If
                        newLine = "End Sub"
                    End If
                End If

                If Left(UCase(Trim(newLine)), Len("CATCH")) = "CATCH" Then
                    IsCatch = True
                End If

                If Left(newLine, Len("EXITIF")) = "EXITIF" Then
                    newLine = "IF" + Right(newLine, Len(newLine) - Len("EXITIF")) + " THEN EXIT " + LastExitStrings(LastExitStrings.Count - 1)
                End If

                If Left(newLine, 1) = "@" Then
                    newLine = Right(newLine, Len(newLine) - 1)
                End If
                newLine = newLine.Replace("PROCEDURE@", "Public Sub")
                newLine = newLine.Replace("FUNCTIONFN", "Public Function")
                newLine = newLine.Replace("THEN @", "THEN")


                If InStr(newLine, "ONMENUGOSUB") Then
                    newLine = newLine.Replace("ONMENUGOSUB", "_ON_MENU_GOSUB = New _ON_MENU_GOSUB_DELEGATE(AddressOf ") + ")"
                End If
                If InStr(newLine, "ONMENUMESSAGEGOSUB") Then
                    newLine = newLine.Replace("ONMENUMESSAGEGOSUB", "_ON_MESSAGE_GOSUB = New _ON_MESSAGE_GOSUB_DELEGATE(AddressOf ") + ")"
                End If

                If InStr(newLine, "ONMENUKEYGOSUB") Then
                    newLine = newLine.Replace("ONMENUKEYGOSUB", "_ON_KEY_GOSUB = New _ON_KEY_GOSUB_DELEGATE(AddressOf ") + ")"
                End If
                If Left(Trim(newLine), 1) = "~" Then
                    newLine = newLine.Replace("~ SetWindowPos (", "SetWindowPos(")
                    newLine = newLine.Replace("~ BringWindowToTop (", "BringWindowToTop(")
                    newLine = newLine.Replace("~ GlobalCompact (", "GlobalCompact(")
                    newLine = newLine.Replace("~ DestroyMenu (", "DestroyGFAMenu(")
                    newLine = newLine.Replace("~ DestroyMenu (", "DestroyGFAMenu(")
                    newLine = newLine.Replace("~ GlobalCompact_ (", "'~ GlobalCompact_ (")
                    newLine = newLine.Replace("~ ^ SystemHeapInfo_", "'~ ^ SystemHeapInfo_ (")

                End If

                If InStr(UCase(newLine), "FOR OUTPUT AS #") Then
                    newLine = newLine.Replace("OPEN ", "OPEN " + Chr(34) + "o" + Chr(34))
                    newLine = newLine.Replace("FOR OUTPUT AS #", ",")
                    newLine = newLine + " -->TODO"
                End If

                If InStr(newLine, "_RESTORE ") Then
                    newLine = newLine.Replace("_RESTORE ", "_RESTORE(" + Chr(34))
                    newLine = Trim(newLine)
                    newLine += Chr(34) + ")"
                End If

                If InStr(newLine, "_GRAPHMODE ,") Then
                    newLine = newLine.Replace("_GRAPHMODE ,", "_GRAPHMODE 1 ,")
                    newLine += "   ' Konverter: evtl nicht korrekt übersetzt"
                End If

                If InStr(newLine, "CC_FULLOPEN | CC_RGBINIT") Then
                    newLine = newLine.Replace("CC_FULLOPEN | CC_RGBINIT", "CC_FULLOPEN or CC_RGBINIT")
                End If
                If InStr(newLine, "PD_PRINTSETUP | PD_USEDEVMODECOPIES") Then
                    newLine = newLine.Replace("PD_PRINTSETUP | PD_USEDEVMODECOPIES", "PD_PRINTSETUP Or PD_USEDEVMODECOPIES")
                End If

                If Left(Trim(newLine), Len("NEW FRAME")) = "NEW FRAME" Then
                    newLine = newLine.Replace("NEW FRAME", "_NEWFRAME")
                End If
                If Left(Trim(newLine), Len("RESTORE")) = "RESTORE" Then
                    newLine = newLine.Replace("RESTORE", "_RESTORE(" + Chr(34)) + Chr(34) + ")"
                End If

                If Left(Trim(UCase(newLine)), Len("UNTIL")) = "UNTIL" Then
                    newLine = "LOOP " + newLine
                End If

                'If Left(Trim(UCase(newLine)), Len("fenster =")) = "FENSTER =" Then
                '    newLine = newLine.Replace("fenster =", "fenster_ =")
                'End If

                If Left(Trim(UCase(newLine)), Len("_ARRAYFILL")) = "_ARRAYFILL" Then
                    newLine = newLine.Replace(" ( )", "")
                End If

                If Left(Trim(UCase(newLine)), Len("LET ")) = "LET " Then
                    newLine = newLine.Replace("LET ", "")
                    newLine = newLine.Replace("Let ", "")
                End If

                If Left(Trim(UCase(newLine)), Len("If FN")) = "IF FN" Then
                    newLine = newLine.Replace("IF FN", "If ")
                    newLine = newLine.Replace("If FN", "If ")
                End If


                If Left(Trim(UCase(newLine)), Len("WEND")) = "WEND" Then
                    newLine = newLine.Replace("WEND", "END WHILE")
                End If


                If Left(Trim(UCase(newLine)), Len("_PRINTTEXT")) = "_PRINTTEXT" Or Left(Trim(UCase(newLine)), Len("_PRINT_AT")) = "_PRINT_AT" Then
                    newLine = newLine.Replace(" ;  )", ")")
                    newLine = newLine.Replace(" ; ", " + ")
                End If

                If Left(Trim(UCase(newLine)), Len("_LINE( ( ")) = "_LINE( ( " Then
                    newLine = Trim(newLine)
                    newLine = newLine.Replace("_LINE( ( ", "_LINE(")
                    If Right(newLine, 1) = ")" Then
                        newLine = Left(newLine, Len(newLine) - 1)
                    End If
                End If

                newLine = newLine.Replace("Then LET", "Then ")
                newLine = newLine.Replace(" And @ ", " And ")

                'Evtl. verschleierung!

                newLine = newLine.Replace(" < > @ ", " <> ")
                newLine = newLine.Replace("Then @", "Then ")
                newLine = newLine.Replace("Then  @", "Then ")
                newLine = newLine.Replace("THEN @", "THEN ")
                newLine = newLine.Replace("THEN  @", "THEN ")

                newLine = newLine.Replace("Then GoSub", "Then ")
                newLine = newLine.Replace("THEN GOSUB", "Then ")
                newLine = newLine.Replace("Then Gosub", "Then ")

                newLine = newLine.Replace("1.0E - 06", "1.0*10.0 ^ - 06")

                If CanRemoveBrackets Then
                    If InStr(newLine, " ( ) ") < Len(newLine) - 5 Then
                        newLine = newLine.Replace(" ( ) ", "")
                    End If
                End If

                If InStr(newLine, "_FORMINPUT") Then
                    newLine = newLine.Replace(" AS ", " , ")
                End If

                If InStr(newLine, "_COLOR 0") Then
                    newLine = newLine.Replace("_COLOR 0", "_RGBCOLOR 0")
                End If

                If InStr(newLine, "DIM?") Then
                    newLine = newLine.Replace("DIM?", "_ARRAYSIZE")
                End If
                If InStr(newLine, " = FN ") Then
                    newLine = newLine.Replace(" = FN ", " = ")
                End If
                If InStr(newLine, "CLIP OFF") Then
                    newLine = newLine.Replace("CLIP OFF", "_CLIPOFF")
                End If
                If InStr(newLine, "ON _MENU") Then
                    newLine = newLine.Replace("ON _MENU", "_ON_MENU")
                End If
                If InStr(newLine, "[EXPROCReturn]") Then
                    newLine = newLine.Replace("[EXPROCReturn]", "Return")
                End If

                newLine += oldComment

                If InStr(newLine, "Public Sub") Then
                    newLines.Add("")
                End If

                newLines.Add(newLine)
            Next
            If endOfMain = False Then
                newLines.Add("End Sub")
                newLines.Add("")
                endOfMain = True
            End If

            Return newLines
        End Function
        Public Sub DoConvertFile(Byval inputFile as String, Byval outputFile as String)

            Try
                Dim lines As List(Of String) = ConvertStringListHelper.GetLines(inputFile)

                Me.Init()
                Me.objs.TMPOUT = New IO.StreamWriter(outputFile + ".log")

                Me.AnalyzeRestores(lines)
                Me.AnalyzeProcedures(lines)
                Me.AnalyzeDeclarations(lines)
                Me.AnalyzeUndeclared(lines)
                Me.ResolveConflicts()
                'Me.RemoveBuildIn()

                'lines = GetGlobalDeclarations()

                IO.File.WriteAllLines(outputFile, Me.Convert(lines).ToArray(), System.Text.Encoding.Default)
                Me.objs.TMPOUT.Close()
                'MsgBox("ende")
                Me.objs = Nothing
            Catch ex As Exception
                MsgBox(ex.Message + " " + ex.ToString)
            End Try

        End Sub

    End Class



End Module
