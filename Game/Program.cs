﻿// 1. Мария - Метод создания колоды для игры (с тасовкой).

int[] Deck()
{
    int[] result = new int[52];
    int j = 0;
    for (int i = 0; i < result.Length; i++)
    {
        if (i % 4 == 0) j++;
        result[i] = j;
    }
    return result;
}

void Shuffle(int[] deck, int j)
{
    for (int i = 0; i < deck.Length; i++)
    {
        int temp = deck[i];
        int index = new Random().Next(0, deck.Length);
        deck[i] = deck[index];
        deck[index] = temp;
    }
    j--;
    if (j != 0) Shuffle(deck, j);
}

int[] deck = Deck();
Shuffle(deck, 10);

// 2. Юрий - Метод названия карт.
// 3. Семён - Метод вытягивания карт из колоды (подсчет очков, подсчет использованный карт).
// Раздача карт
void BatchCards(int[] gameCardDeck, List<int> playerCards, int[] gameScore, int numbOfPlayer, int countAddCards)
{
    int batchStop = gameScore[0] + countAddCards;
    if (gameScore[0] > gameCardDeck.Length - 1) gameScore[0] = 0;
    for (; gameScore[0] < batchStop; gameScore[0]++)
    {
        playerCards.Add(gameCardDeck[gameScore[0]]);
    }
}
// Алгоритм набора карт для прикупа дилера
void DilerLogic(int[] gameCardDeck, List<int> dilerCards, int[] gameStatus)
{
    int dilerScore = gameStatus[gameStatus.Length - 1];
    while (dilerScore <= 17)
    {
        BatchCards(gameCardDeck, dilerCards, gameStatus, gameStatus.Length, 1);
        dilerScore = CardTransferToScore(dilerCards);
        ShowCards(dilerCards);
    }
    gameStatus[gameStatus.Length - 1] = dilerScore;
}
// Добавление карт игроку и дилеру
void FillCardsList(int[] gameCardDeck, List<int>[] playersCards, int[] gameStatus)
{
    int countBatchCard = gameStatus[0];
    int countPlayers = gameStatus[1] - 1;
    for (int n = 0; n < countPlayers; n++)
    {
        Console.WriteLine($"Играет {n + 1} игрок");
        ShowCards(playersCards[n]);
        bool addCards = true;
        while (addCards)
        {
            Console.WriteLine("Добавить карту?");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.A:
                    BatchCards(gameCardDeck, playersCards[n], gameStatus, n, 1);
                    gameStatus[n + 2] = CardTransferToScore(playersCards[n]);
                    ShowCards(playersCards[n]);
                    if (gameStatus[n + 2] > 21) addCards = false;
                    break;
                case ConsoleKey.S:
                    gameStatus[n + 2] = CardTransferToScore(playersCards[n]);
                    addCards = false;
                    break;
                default: break;
            }
        }
    }
    DilerLogic(gameCardDeck, playersCards[playersCards.Length - 1], gameStatus);
}
// Подсчет очков
int CardTransferToScore(List<int> playerCards, int scorePlayer = 0)
{
    int countAce = 0;
    for (int c = 0; c < playerCards.Count; c++)
    {
        switch (playerCards[c])
        {
            case 1:
                countAce++;
                break;
            case 10 | 11 | 12 | 13:
                scorePlayer += 10;
                break;
            default:
                scorePlayer += playerCards[c];
                break;
        }
    }
    // Если есть тузы в прикупе и сума очков вместе с ними больше 21 => они превращаются в 1
    // Иначе => в 11
    scorePlayer += countAce > 0 && scorePlayer + 11 * countAce < 21 ? 11 * countAce : countAce;
    return scorePlayer;
}

// Main Process
void GameProcess(int[] gameCardDeck, int[] gameStatus)
{
    // gameStatus будет содержать в себе:
    // [количество карт выданное игрокам;
    //  количество игроков (считая дилера)(на данный момент всегда будет "2");
    //  сумма очков первого игрока (пока единственного);
    //  сумма очков дилера]
    // Создавать можно n игроков, дилер будет всегда последний в этой 
    List<int>[] batchGame = new List<int>[gameStatus[1]];
    for (int i = 0; i < gameStatus[1]; i++) batchGame[i] = new List<int>();
    int startBatch = 2;
    // Для каждого юнита за игральным столом выдаются карты
    for (int n = 0; n < gameStatus[1]; n++)
    {
        BatchCards(gameCardDeck, batchGame[n], gameStatus, n, startBatch);
    }
    // Вывод карт дилера
    ShowCards(batchGame[gameStatus[1] - 1]);
    //Запрос на добавление карты игроку и дилеру
    FillCardsList(gameCardDeck, batchGame, gameStatus);
    System.Console.WriteLine($"Очки игрока {gameStatus[2]}, очки дилера {gameStatus[3]}, выдано карт {gameStatus[0]}");
}
// Шаблон метода для Юрия, какие данные будут входящими
void ShowCards(List<int> playerCards)
{
    foreach (int s in playerCards) System.Console.Write(s);
    System.Console.WriteLine();
}
int[] gameDesk = new int[] { 1, 1, 1, 4, 5, 6, 7, 8, 9 }, game = new int[] { 0, 2, 0, 0 };
GameProcess(gameDesk, game);

// 4. Ольга - Метод сравнения очков (кто победил, ничья).

// Метод подсчета очков 
// Из википедии - При окончательном подсчёте очков в конце раунда карты остальных игроков для вас 
// значения не имеют, игра ведётся только против дилера, то есть сравниваются карты только игрока и дилера, 
// карты и ставки параллельных игроков не учитываются.

// Использовала метод int, который возвращает значения, на случай дальнейшей доработки игры со ставками

// Dealer
// Player

int CalculateWinner(int dealerPoints, int playerPoints)
{
    if (playerPoints > 21) { Console.WriteLine("Увы, Вы проиграли"); return 0; } // перебор, снимается ставка
    else if (dealerPoints == playerPoints) { Console.WriteLine("У вас ничья!"); return 1; }
    else if (playerPoints == 21) { Console.WriteLine("BlackJack! Вы выиграли!"); return 3; } // выплачивается выигрыш 3 к 2
    else if ((dealerPoints > 21) && (dealerPoints != 21) || (playerPoints > dealerPoints))
    { Console.WriteLine("Поздравляем, Вы выиграли!"); return 2; }
    else Console.WriteLine("Увы, Вы проиграли!"); return 0;
}

CalculateWinner(19, 18);

// 5. Евгений - Метод цикла хода игры (сыграть ещё, или выйти).