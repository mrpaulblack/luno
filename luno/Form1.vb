Public Class Luno

    Dim player_num, player_cards_num As Integer         ' number of player
    Dim deck_cards(53), deck_cards_avail(53) As Integer ' specific cards and their avail.
    Dim deck_cards_color(53) As Color                   ' color of cards: 1.red, 2.yellow, 3.green, 4. blue, 5.black (special cards)
    Dim player_deck(0, 0) As Integer                    ' index of player deck, each x for 1 player
    Dim player_deck_avail() As Integer                  ' number of cards each player has left
    Dim deck_open, player_current As Integer            ' index of open card, current player
    Dim turn_dir As Boolean                             ' direction of turns
    Dim player_ui_listview()                            ' btn array for content
    Dim player_lvi_items(0, 0)                          ' listview items sorted in twodimentional array
    Dim ok                                              ' ok btn
    Dim deck_open_lbl                                   ' label shows current open deck
    Dim Deck_open_color_ov As Color                     ' override deck color if deck-open is black card


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load    ' init, form create event
        Generate_deck()
        Generate_player_deck(User_input())
        Generate_defaults()
        Generate_form()

        Turn_reset_cards()  ' first turn on init
        Turn_close_cards()
        Turn_open_cards()
    End Sub

    Private Sub Generate_deck()    ' generate deck: 11 = aussetzten, 12= change dir, 13= +2, 14 = wünschen, 15= +4
        Dim index As Integer    ' var for loops
        For index = 0 To 12  ' generate numbers for card deck: red cards
            deck_cards(index) = index
            If index = 0 Then  ' card 0 only exists once
                deck_cards_avail(index) = 1
            Else
                deck_cards_avail(index) = 2
            End If
            deck_cards_color(index) = Color.Red
        Next
        For index = 13 To 25  ' generate numbers for card deck: yellow cards
            deck_cards(index) = index - 13
            If index = 13 Then  ' card 0 only exists once
                deck_cards_avail(index) = 1
            Else
                deck_cards_avail(index) = 2
            End If
            deck_cards_color(index) = Color.Yellow
        Next
        For index = 26 To 38  ' generate numbers for card deck: green cards
            deck_cards(index) = index - 26
            If index = 26 Then  ' card 0 only exists once
                deck_cards_avail(index) = 1
            Else
                deck_cards_avail(index) = 2
            End If
            deck_cards_color(index) = Color.Green
        Next
        For index = 39 To 52  ' generate numbers for card deck: blue cards
            deck_cards(index) = index - 39
            If index = 39 Then  ' card 0 only exists once
                deck_cards_avail(index) = 1
            Else
                deck_cards_avail(index) = 2
            End If
            deck_cards_color(index) = Color.Blue
        Next
        For index = 52 To 53    ' special cards: wish and +4
            deck_cards(index) = index - 39
            deck_cards_avail(index) = 4
            deck_cards_color(index) = Color.Black
        Next
    End Sub

    Function User_input()
        player_num = InputBox("Anzahl der Spieler?", "Spieleranzahl", 4) ' userinput amount of player
        player_cards_num = InputBox("Anzahl der Karten pro Spieler?", "Kartenanzahl", 8)   ' userinput cards per playerdeck default = 8
        Return player_cards_num
    End Function

    Private Sub Generate_player_deck(player_num_cards As Integer)   ' generate cards of player
        Dim n, i As Integer ' var for counting
        Dim r As New Random, randm As Integer ' var for randomize
        ReDim player_deck(player_num_cards - 1, player_num - 1) ' size of 2dimentional array

        For i = 0 To player_num - 1
            For n = 0 To (player_num_cards - 1) ' content randm player 2dimentional array
                randm = r.Next(0, deck_cards.Length)
                If deck_cards_avail(randm) > 0 Then
                    player_deck(n, i) = randm
                    deck_cards_avail(randm) = deck_cards_avail(randm) - 1
                Else
                    n = n - 1
                End If
            Next
        Next
    End Sub

    Private Sub Generate_form()
        ReDim player_ui_listview(player_num - 1) ' 1 ListView per player
        Dim n, x As Integer    ' for the for-loop
        Dim listbox_x, listbox_y As Integer     ' x and y size of Listboxes

        x = 0   ' properties, setback to default
        listbox_x = 200
        listbox_y = 400

        For n = 0 To player_num - 1
            player_ui_listview(n) = New ListView()

            With player_ui_listview(n)
                .Location = New Point(x, 0)
                .Name = "listview" & CStr(n)
                .Size = New Size(listbox_x, listbox_y)
                .BackColor = Color.White
                .ForeColor = Color.Black
                '.FormattingEnabled = True
                .TabIndex = n
                .Parent = Me
            End With
            Controls.Add(player_ui_listview(n))
            x = x + listbox_x
        Next

        deck_open_lbl = New Label ' current open deck
        With deck_open_lbl
            .Location = New Point(listbox_x * player_num, 0)
            .Parent = Me
            .Parent.Controls.Add(deck_open_lbl)
            .Text = "Offene Karte: "
            .Name = "deck_open_lbl1"
            .Size = New Point(listbox_x, 50)
        End With

        ok = New Button ' btn for onpress event to kick of each turn
        With ok
            .Parent = Me
            .Parent.Controls.Add(ok)
            .Text = "Zug beenden"
            .Name = "ok1"
            .TabIndex = 0
            .Location = New Point(listbox_x * player_num, listbox_y - 50)
            .Size = New Size(listbox_x - 15, 50)
        End With

        Me.Size = New Point((listbox_x * player_num) + listbox_x, listbox_y + 38) ' set size of forms

        Dim btn As Button = Nothing 'onclick event btn
        For Each ctrl As Control In Me.Controls
            If TypeOf ctrl Is Button Then
                btn = DirectCast(ctrl, Button)
                AddHandler btn.Click, AddressOf Me.Button_Click
            End If
        Next
    End Sub

    Private Sub Generate_defaults()   ' generate first card
        Dim randm, n As Integer
        Dim r As New Random

Generate_randm:
        randm = r.Next(0, deck_cards.Length)    ' generate first card, only numbers, no special card
        If deck_cards_avail(randm) <= 0 Or deck_cards(randm) > 9 Then
            GoTo Generate_randm
        End If
        deck_open = randm

        turn_dir = True     ' set turn direction to default
        player_current = 0  ' set first player as startpoint

        ReDim player_deck_avail(player_num - 1)   ' set number of cards for each player
        For n = 0 To player_num - 1
            player_deck_avail(n) = player_cards_num - 1
        Next
    End Sub



    Private Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)  ' the actual game, wait for btn press
        Turn(player_ui_listview(player_current).FocusedItem.Index)  ' actual turn, query listview_item_index

        If Win() = True Then    ' if win true -> then reset, else nothing
            Reset()
        End If

        Turn_next() ' prepare cards for next player
        Turn_reset_cards()
        Turn_close_cards()
        Turn_open_cards()
    End Sub


    Private Sub Turn(current_card As Integer)   ' ruleset of game
        If deck_cards(player_deck(current_card, player_current)) = 10 And Deck_open_color() = deck_cards_color(player_deck(current_card, player_current)) Then    ' miss a turn, same color
            Card_normal(current_card)
            Card_skip_next()
        ElseIf deck_cards(player_deck(current_card, player_current)) = 10 And deck_cards(deck_open) = 12 Then    ' miss a turn, and open_deck = miss a turn
            Card_normal(current_card)
            Card_skip_next()
        ElseIf deck_cards(player_deck(current_card, player_current)) = 11 And Deck_open_color() = deck_cards_color(player_deck(current_card, player_current)) Then  ' change dir, same color
            Card_normal(current_card)
            Card_turndir()
        ElseIf deck_cards(player_deck(current_card, player_current)) = 11 And deck_cards(deck_open) = 11 Then    ' change dir, change dir = deck_open
            Card_normal(current_card)
            Card_turndir()
        ElseIf deck_cards(player_deck(current_card, player_current)) = 12 And Deck_open_color() = deck_cards_color(player_deck(current_card, player_current)) Then  ' +2 and same color
            ' +2
            MsgBox("+2")
        ElseIf deck_cards(player_deck(current_card, player_current)) = 12 And deck_cards(deck_open) = 12 Then    ' +2 and +2 = open_deck
            ' +2
            MsgBox("+2")
        ElseIf deck_cards(player_deck(current_card, player_current)) = 13 Then  ' wish next color
            Card_wish_color()
            Card_normal(current_card)
        ElseIf deck_cards(player_deck(current_card, player_current)) = 14 Then  ' +4
            ' Card_wish_color()
            MsgBox("+4")
        ElseIf deck_cards(player_deck(current_card, player_current)) = deck_cards(deck_open) Then   ' same num
            Card_normal(current_card)
        ElseIf Deck_open_color() = deck_cards_color(player_deck(current_card, player_current)) Then   ' same color
            Card_normal(current_card)
        Else
            MsgBox("Zug nicht möglich")
        End If
    End Sub

    Private Sub Card_wish_color()   ' wish next color prompt
        Dim result As Integer

        result = ColorPicker.Show(
                    {"Rot", "Gelb", "Grün", "Blau"},
                    "Wähle die nächste Farbe:",
                    "Farbauswahl")

        If result = 0 Then  ' setze farbe nach auswahl
            Deck_open_color_ov = Color.Red
        ElseIf result = 1 Then
            Deck_open_color_ov = Color.Yellow
        ElseIf result = 2 Then
            Deck_open_color_ov = Color.Green
        ElseIf result = 3 Then
            Deck_open_color_ov = Color.Blue
        End If
    End Sub

    Private Sub Card_turndir()  ' change turndir to oposite
        If turn_dir = True Then
            turn_dir = False
        Else
            turn_dir = True
        End If
    End Sub

    Private Sub Card_skip_next()    ' skip next player
        If turn_dir = True Then
            player_current = player_current + 1
        Else
            player_current = player_current - 1
        End If
    End Sub

    Private Sub Card_normal(index As Integer)  ' standard add card to open_deck
        Dim n As Integer    ' numvar for loop

        deck_cards_avail(deck_open) = deck_cards_avail(deck_open) + 1   ' set new deck card and put the one before back to deck
        deck_open = player_deck(index, player_current)

        For n = index To player_deck_avail(player_current)  ' remove card and put alle following cards one index -1
            If n = player_deck_avail(player_current) Then
                ' do nothing
            Else
                player_deck(n, player_current) = player_deck(n + 1, player_current)
            End If
        Next

        player_deck(player_deck_avail(player_current), player_current) = Nothing
        player_deck_avail(player_current) = player_deck_avail(player_current) - 1   ' set avail of cards per player after every card_add
    End Sub

    Private Sub Card_add(cards_num As Integer)  ' add cards to playerdeck cards_num -> amount of cards
        Dim n As Integer ' var for counting
        Dim r As New Random, randm As Integer ' var for randomize
        ReDim player_deck(player_deck_avail.Max() + cards_num - 1, player_num - 1) ' size of 2dimentional array redim after card_add
        MsgBox(CStr(player_deck.Length))

        For n = player_deck_avail(player_current) + 1 To player_deck_avail(player_current) + cards_num + 1 ' content randm player 2dimentional array
            randm = r.Next(0, deck_cards.Length)
            If deck_cards_avail(randm) > 0 Then
                player_deck(n, player_current) = randm
                MsgBox(CStr(deck_cards_avail.Length) & ", " & CStr(randm))
                deck_cards_avail(randm) = deck_cards_avail(randm) - 1
            Else
                n = n - 1
            End If
        Next

        player_deck_avail(player_current) = player_deck_avail(player_current) + cards_num
    End Sub


    Private Sub Turn_reset_cards()    ' remove listview items -> reset cards for update after each turn and set  open_card_lbl to open_card
        Dim n As Integer
        Array.Clear(player_lvi_items, 0, player_lvi_items.Length)

        For n = 0 To player_num - 1 ' clear listview
            player_ui_listview(n).Items.Clear
        Next

        With deck_open_lbl  ' set open_card_lbl to open_card query
            .BackColor = Deck_open_color()
            .ForeColor = Query_color_open()
            .Text = Query_card_open()
            .Font = New Font(New FontFamily("Arial"), 12, FontStyle.Bold)
        End With
    End Sub

    Private Sub Turn_close_cards()  ' close all cards -> set black site for all player != player_current
        Dim i, n As Integer
        ReDim player_lvi_items(player_deck_avail.Max(), player_num - 1)  ' 2dimentional listview array (items of listview)

        For i = 0 To player_num - 1
            If i = player_current Then
                ' do nothing
            Else
                For n = 0 To player_deck_avail(i)
                    player_lvi_items(n, i) = New ListViewItem
                    With player_lvi_items(n, i)
                        .Text = "...."
                        .BackColor = Color.Black
                        .ForeColor = Color.Black
                        .Font = New Font(New FontFamily("Arial"), 16, FontStyle.Bold)
                    End With
                    player_ui_listview(i).Items.Add(player_lvi_items(n, i))
                Next
            End If
        Next
    End Sub

    Private Sub Turn_open_cards()   ' open cards of current player
        Dim n As Integer

        For n = 0 To player_deck_avail(player_current)
            player_lvi_items(n, player_current) = New ListViewItem
            With player_lvi_items(n, player_current)
                .Text = Query_card(n, player_current)
                .BackColor = deck_cards_color(player_deck(n, player_current))
                .ForeColor = Query_color(n, player_current)
                .Font = New Font(New FontFamily("Arial"), 12, FontStyle.Regular)
            End With
            player_ui_listview(player_current).Items.Add(player_lvi_items(n, player_current))
        Next
    End Sub

    Private Sub Turn_next() ' determine next player
        If turn_dir = True Then
            If player_current + 1 > player_num - 1 Then
                player_current = 0
            Else
                player_current = player_current + 1
            End If
        ElseIf turn_dir = False Then
            If player_current - 1 < 0 Then
                player_current = player_num - 1
            Else
                player_current = player_current - 1
            End If
        End If
    End Sub


    Function Win() ' after every turn test if win = true
        Return If(player_deck_avail(player_current) < 0, True, False)
    End Function

    Function Reset()    ' close or reset
        MsgBox("Spieler " & player_current + 1 & " hat gewonnen!")
        Close()
        Return True
    End Function


    Function Query_card(index As Integer, player As Integer) ' input index and player -> return card
        If deck_cards(player_deck(index, player)) > 9 Then
            If deck_cards(player_deck(index, player)) = 10 Then
                Return "⌧"
            ElseIf deck_cards(player_deck(index, player)) = 11 Then
                Return "↻"
            ElseIf deck_cards(player_deck(index, player)) = 12 Then
                Return "+2"
            ElseIf deck_cards(player_deck(index, player)) = 13 Then
                Return "W"
            ElseIf deck_cards(player_deck(index, player)) = 14 Then
                Return "+4"
            Else
                Return False   ' impossible code path
            End If
        Else
            Return CStr(deck_cards(player_deck(index, player)))
        End If
    End Function

    Function Query_card_open() ' call function -> return char
        If deck_cards(deck_open) > 9 Then
            If deck_cards(deck_open) = 10 Then
                Return "Ausetzen"
            ElseIf deck_cards(deck_open) = 11 Then
                Return "Richtungswechseler"
            ElseIf deck_cards(deck_open) = 12 Then
                Return "plus 2"
            ElseIf deck_cards(deck_open) = 13 Then
                Return "Wuenscher"
            ElseIf deck_cards(deck_open) = 14 Then
                Return "plus 4"
            Else
                Return False   ' impossible code path
            End If
        Else
            Return CStr(deck_cards(deck_open))
        End If
    End Function

    Function Query_color(index As Integer, player As Integer)  ' input index and player -> return contrast foreground-color
        If deck_cards_color(player_deck(index, player)) = Color.Red Or deck_cards_color(player_deck(index, player)) = Color.Yellow Then
            Return Color.Black
        ElseIf deck_cards_color(player_deck(index, player)) = Color.Green Or deck_cards_color(player_deck(index, player)) = Color.Blue Then
            Return Color.White
        Else
            Return Color.White   ' default
        End If
    End Function

    Function Query_color_open()  ' call func -> return contrast foreground-color for deck_open
        If Deck_open_color() = Color.Red Or Deck_open_color() = Color.Yellow Then
            Return Color.Black
        ElseIf Deck_open_color() = Color.Green Or Deck_open_color() = Color.Blue Then
            Return Color.White
        Else
            Return Color.White   ' default
        End If
    End Function

    Private Function Deck_open_color()  ' set background color of deck_open
        If deck_cards(deck_open) = 13 Then
            Return Deck_open_color_ov
        Else
            Return deck_cards_color(deck_open)
        End If
    End Function
End Class
