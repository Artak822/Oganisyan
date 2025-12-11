using System;

class Program
{
    static bool ValidateBattlefield(int[,] field)
    {
        var ships = new System.Collections.Generic.Dictionary<int, int>
        {
            { 1, 4 },
            { 2, 3 },
            { 3, 2 },
            { 4, 1 }
        };

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (field[i, j] == 1)
                {
                    if ((i > 0 && j > 0 && field[i - 1, j - 1] != 0) ||
                        (i > 0 && j < 9 && field[i - 1, j + 1] != 0) ||
                        (i < 9 && j > 0 && field[i + 1, j - 1] != 0) ||
                        (i < 9 && j < 9 && field[i + 1, j + 1] != 0))
                    {
                        return false;
                    }

                    int sumCross = 0;
                    if (i > 0) sumCross += field[i - 1, j];
                    if (i < 9) sumCross += field[i + 1, j];
                    if (j > 0) sumCross += field[i, j - 1];
                    if (j < 9) sumCross += field[i, j + 1];

                    if (sumCross == 0)
                    {
                        ships[1]--;
                        field[i, j] = 2;
                        continue;
                    }
                    else if (sumCross == 1)
                    {
                        field[i, j] = 2;
                        int count = 1;
                        int currentI = i;
                        int currentJ = j;

                        if (j < 9 && field[i, j + 1] != 0)
                        {
                            currentJ = j + 1;
                            while (currentJ < 10 && field[i, currentJ] != 0)
                            {
                                count++;
                                field[i, currentJ] = 2;
                                currentJ++;
                            }
                        }
                        else if (i < 9 && field[i + 1, j] != 0)
                        {
                            currentI = i + 1;
                            while (currentI < 10 && field[currentI, j] != 0)
                            {
                                count++;
                                field[currentI, j] = 2;
                                currentI++;
                            }
                        }

                        if (count > 4)
                        {
                            return false;
                        }
                        ships[count]--;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        foreach (var ship in ships.Values)
        {
            if (ship != 0)
            {
                return false;
            }
        }

        return true;
    }

    static int[,] CreateField(int[,] data)
    {
        int[,] field = new int[10, 10];
        for (int i = 0; i < 10 && i < data.GetLength(0); i++)
        {
            for (int j = 0; j < 10 && j < data.GetLength(1); j++)
            {
                field[i, j] = data[i, j];
            }
        }
        return field;
    }

    static void Assert(bool condition, string testName)
    {
        if (condition)
        {
            Console.WriteLine($"✓ {testName} - PASSED");
        }
        else
        {
            Console.WriteLine($"✗ {testName} - FAILED");
        }
    }

    static void RunTests()
    {
        Console.WriteLine("Запуск тестов...\n");

        int[,] validField = CreateField(new int[,] {
            {1,0,0,0,0,1,1,0,0,0},
            {1,0,1,0,0,0,0,0,1,0},
            {1,0,1,0,1,1,1,0,1,0},
            {1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,0},
            {0,0,0,0,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,0,1,0},
            {0,0,0,1,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        });
        Assert(ValidateBattlefield(validField), "Валидное поле");

        int[,] emptyField = new int[10, 10];
        Assert(!ValidateBattlefield(emptyField), "Пустое поле (должно быть невалидным)");

        int[,] diagonalContactField = CreateField(new int[,] {
            {1,0,0,0,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        });
        Assert(!ValidateBattlefield(diagonalContactField), "Корабли по диагонали (должно быть невалидным)");

        int[,] tooManyShipsField = CreateField(new int[,] {
            {1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        });
        Assert(!ValidateBattlefield(tooManyShipsField), "Слишком большой корабль (должно быть невалидным)");

        int[,] correctShipsField = CreateField(new int[,] {
            {1,0,0,0,0,1,1,0,0,0},
            {1,0,1,0,0,0,0,0,1,0},
            {1,0,1,0,1,1,1,0,1,0},
            {1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,0},
            {0,0,0,0,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,0,1,0},
            {0,0,0,1,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,1,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        });
        Assert(ValidateBattlefield(correctShipsField), "Правильное расположение кораблей");

        int[,] wrongCountField = CreateField(new int[,] {
            {1,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        });
        Assert(!ValidateBattlefield(wrongCountField), "Неправильное количество кораблей (должно быть невалидным)");

        Console.WriteLine("\nТесты завершены.");
    }

    static void Main(string[] args)
    {
        RunTests();
    }
}

