using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ExpenseManager
{
    
    class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }

    class Program
    {
        static List<Expense> expenses = new List<Expense>();
        static int nextId = 1;

        

        static void Main()
        {

            if (File.Exists("expenses.json"))
            {
                string json = File.ReadAllText("expenses.json");

                List<Expense>? loadedExpenses = JsonSerializer.Deserialize<List<Expense>>(json);

                expenses = loadedExpenses ?? new List<Expense>();

                nextId = expenses.Count > 0 ? expenses.Max(expense => expense.Id) + 1 : 1;

                //TODO подумати ч и можна винести в окремий метод
            }

            while (true)
            {
                ShowMenu();

                Console.Write("Оберіть дію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddExpense();
                        break;

                    case "2":
                        FindeExpens();
                        break;

                    case "3":
                        ShowExpenses();
                        break;

                    case "4":
                        ShowTotal();
                        break;

                    case "5":
                        FindByCategory(expenses);

                        break;

                    case "6":
                        DeleteExpense();
                        break;

                    case "7":
                        ShowStatistics();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невідома команда.");
                        break;
                }

                Console.WriteLine("\n Натисніть Enter...");
                Console.ReadLine();
                Console.Clear();

            }
        }

        static void FindeExpens()
        {
            
                GlobalFindeManu();

            Console.Write("Оберіть критерій пошуку: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        // пошук за назвою
                        break;

                    case "2":
                        // пошук за сумою
                        break;

                    case "3":
                    FindByCategory(expenses);
                    break;

                    case "0":
                        return;

                default:
                        Console.WriteLine("Невідома команда.");
                        break;
                }
            Console.WriteLine("\n Натисніть Enter...");
            Console.ReadLine();
                Console.Clear();
                
            }


        static void ShowMenu()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("        МЕНЕДЖЕР ВИТРАТ");
            Console.WriteLine("=================================");
            Console.WriteLine();
            Console.WriteLine("1. Додати витрату");
            Console.WriteLine("2. Глобальний пошук витрат");
            Console.WriteLine("3. Показати всі витрати");
            Console.WriteLine("4. Показати загальну суму");
            Console.WriteLine("5. Показати витрати за категорією");
            Console.WriteLine("6. Видалити витрату");
            Console.WriteLine("7. Статистика");
            Console.WriteLine("0. Вихід");
            Console.WriteLine();
        }

        static void GlobalFindeManu()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("  ГЛОБАЛЬНИЙ ПОШУК ПО ВИТРАТАМ");
            Console.WriteLine("=================================");
            Console.WriteLine();
            Console.WriteLine("1. Пошук за назвою");
            Console.WriteLine("2. Пошук за сумою");
            Console.WriteLine("3. Пошук за категорією");
            Console.WriteLine("0. Вихід");
            Console.WriteLine();
        }

        static string? GetDataFromConsole(string prompt)
        {
            Console.WriteLine(prompt);
            string answer = Console.ReadLine() ?? "";

            if(string.Equals(answer, "exit", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            else if (string.IsNullOrWhiteSpace(answer))
            {
                Console.WriteLine("Введене значення не може бути порожнім. Спробуйте ще раз:");
                return GetDataFromConsole(prompt);
            }
            return answer;
        }

        static string? GetExpenseName()
        {
            string? name = GetDataFromConsole("Введіть назву витрати: ");
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            return null;
        }

        static decimal? GetExpenseAmount()
        {
            decimal amount;

            while (true)
            {
                string? value = GetDataFromConsole("Введіть суму витрати: ");

                if (value == null)
                {
                    return null;
                }

                if (!decimal.TryParse(value, out amount) || amount <= 0)
                {
                    Console.WriteLine("Введене значення не є дійсним числом або менше або дорівнює нулю. Спробуйте ще раз:");
                    continue;
                }

                    return amount;
            }

        }

        static string? GetExpenseCategory()
        {
            string? category = GetDataFromConsole("Введіть назву категорії: ");
            if (!string.IsNullOrWhiteSpace(category))
            {
                return category;
            }
            return null;
        }
        

        static void AddExpense()
        {
            Console.WriteLine("Введіть необхідні данні. Напишіть exit для повернення в меню на будь якому єтапі внесення данних.");
            string? name = GetExpenseName();
            if (name == null)
            {
                return;
            }

            decimal? amount = GetExpenseAmount();
            if (amount == null)
            {
                return;
            }

            string? category = GetExpenseCategory();
            if (category == null)
            {
                return;
            }

            expenses.Add(new Expense { Id = nextId++, Name = name, Amount = amount.Value, Category = category });
            SaveExpenses();

            Console.WriteLine("Витрату додано успішно !");
        }


        static void ShowExpenses()
        {

            foreach (Expense expense in expenses) 
            {
              Console.WriteLine($"ID: {expense.Id}, Назва: {expense.Name}, Сума: {expense.Amount}, Категорія: {expense.Category}"); 
            }
        }

        static void ShowTotal()
        {
            decimal allAmounts = 0;

            foreach (Expense expense in expenses)
            {
                allAmounts += expense.Amount;

            }
            Console.WriteLine($"Загальна cумма всіх витрат: {allAmounts} ");
          
        }


         static List<Expense> Search(List<Expense> expenses, Func<Expense, bool> condition)
    
            {
                return expenses
                    .Where(condition)
                    .ToList();
            }

        

        static void FindByCategory(List<Expense> expenses)
        {
            while (true)
            {
                string? category = GetDataFromConsole("Введіть назву категорії: ");
                if (category == null)
                {
                    return;
                }

                bool found = false;
                foreach (Expense expense in expenses)
                {
                    if (string.Equals(expense.Category, category, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        Console.WriteLine($"ID: {expense.Id}| Назва: {expense.Name}| Сума: {expense.Amount}| Категорія: {expense.Category}");
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Категорії не знайдено ! Спробуй ще раз..");

                }
            }
        }      


        static void DeleteExpense()
        {
            if (expenses.Count == 0)
            {
                Console.WriteLine("Список витрат порожній.");
                return;
            }

            string? input = GetDataFromConsole("Введіть назву, категорію, суму або ID для пошуку (exit — скасувати): ");
            if (input == null)
            {
                return;
            }

            string query = input.Trim();
            bool isAmount = decimal.TryParse(query, out decimal searchedAmount);
            bool isId = int.TryParse(query, out int searchedId);

            List<Expense> foundResults = Search(expenses, expense =>
                expense.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(expense.Category, query, StringComparison.OrdinalIgnoreCase) ||
                (isAmount && expense.Amount == searchedAmount) ||
                (isId && expense.Id == searchedId));

            if (foundResults.Count == 0)
            {
                Console.WriteLine("Витрат за цим запитом не знайдено.");
                return;
            }

            Console.WriteLine("Знайдені витрати:");
            foreach (Expense expense in foundResults)
            {
                Console.WriteLine($"ID: {expense.Id}| Назва: {expense.Name}| Сума: {expense.Amount}| Категорія: {expense.Category}");
            }



            while (true)
            {
                string? idInput = GetDataFromConsole("Введіть ID витрати для видалення (exit — скасувати): ");
                if (idInput == null)
                {
                    return;
                }

                if (!int.TryParse(idInput, out int selectedId))
                {
                    Console.WriteLine("ID має бути цілим числом. Спробуйте ще раз.");
                    continue;
                }

                Expense? selectedExpense = foundResults.FirstOrDefault(expense => expense.Id == selectedId);
                if (selectedExpense == null)
                {
                    Console.WriteLine("Витрати з таким ID немає серед знайдених. Спробуйте ще раз.");
                    continue;
                }

                Console.WriteLine($"Вибрано: ID: {selectedExpense.Id}| Назва: {selectedExpense.Name}| Сума: {selectedExpense.Amount}| Категорія: {selectedExpense.Category}");
                string? confirmation = GetDataFromConsole("Підтвердити видалення? Введіть так або y (інша відповідь — скасувати): ");
                if (!string.Equals(confirmation, "так", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Видалення скасовано.");
                    return;
                }

                if (!expenses.Remove(selectedExpense))
                {
                    Console.WriteLine("Не вдалося видалити витрату.");
                    return;
                }

                SaveExpenses();
                Console.WriteLine("Витрату видалено.");
                return;
            }

        }

        static void ShowStatistics()
        {
            // TODO:
            // Кількість витрат
            // Загальна сума
            // Середня витрата
            // Мінімальна витрата
            // Максимальна витрата
            // Сума за кожною категорією
        }

        static void SaveExpenses()
        {
            string json = JsonSerializer.Serialize(expenses);
            File.WriteAllText("expenses.json", json);
        }
    }
}
