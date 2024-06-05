using Microsoft.EntityFrameworkCore;
using _2rkis3.Models;
using Task = _2rkis3.Models.Task;

namespace _2rkis3;

class Program
{
    static PostgresContext db = new PostgresContext();

    static void Main()
    {
        Console.WriteLine();
        Console.WriteLine("Добро пожаловать в ежедневник!");
        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Посмотреть задачи");
            Console.WriteLine("2. Добавить новые задачи");
            Console.WriteLine("3. Удалить задачи");
            Console.WriteLine("4. Пометить задачи выполненными");
            Console.WriteLine("5. Выход");
            Console.WriteLine();

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    viemTasksMenu();
                    break;
                case "2":
                    addNewTasksMenu();
                    break;
                case "3":
                    deleteTask();
                    break;
                case "4":
                    markTaskCompleted();
                    break;
                case "5":
                    Console.WriteLine("До свидания!");
                    return;
                default:
                    Console.WriteLine("Некорректный выбор, попробуйте снова.");
                    break;

            }
        }

// Меню просмотра задач
        static void viemTasksMenu()
        {
            Console.WriteLine("Выберите тип задач:");
            Console.WriteLine("1. Задачи на сегодня");
            Console.WriteLine("2. Задачи на завтра");
            Console.WriteLine("3. Задачи на неделю");
            Console.WriteLine("4. Все задачи");
            Console.WriteLine("5. Предстоящие задачи");
            Console.WriteLine("6. Выполненные задачи");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    viemTasksForDay(DateTime.Today);
                    break;
                case "2":
                    viemTasksForDay(DateTime.Today.AddDays(1));
                    break;
                case "3":
                    viemTasksForWeek();
                    break;
                case "4":
                    viemAllTasks();
                    return;
                case "5":
                    viemUpcomingTasks();
                    return;
                case "6":
                    viemComletedTasks();
                    return;
                default:
                    Console.WriteLine("Некорректный выбор, попробуйте снова.");
                    break;
            }

        }

        // просмотр задач на указаную дату
        static void viemTasksForDay(DateTime date)
        {
            DateOnly dateOnly = new DateOnly(date.Year, date.Month, date.Day);
            
            var tasksForDay = db.Tasks.Where(t => t.Duedata == dateOnly).ToList();

            if (tasksForDay.Count > 0)
            {
                Console.WriteLine($"Список задач на {dateOnly.ToShortDateString()}:");
                foreach (var task in tasksForDay)
                {
                    Console.WriteLine($"Название: {task.Title}");
                    Console.WriteLine($"Описание: {task.Description}");
                    Console.WriteLine($"Дата: {task.Duedata.ToShortDateString()}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("На выбранную дату задач нет.");
            }

            Console.ReadLine();
        }

        // просмотр задач на неделю
        static void viemTasksForWeek()
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            
            DateOnly endOfWeek = currentDate.AddDays(7);
            
            var tasksForWeek = db.Tasks.Where(task => task.Duedata >= currentDate && task.Duedata <= endOfWeek).ToList();

            if (tasksForWeek.Count == 0)
            {
                Console.WriteLine("На следующую неделю задач нет.");
            }
            else
            {
                Console.WriteLine("Список задач на неделю:");
                foreach (var task in tasksForWeek)
                {
                    Console.WriteLine($"{task.Title} - {task.Duedata.ToShortDateString()}");
                }
            }

            Console.ReadLine();
        }


        // просмотр всех задач
        static void viemAllTasks()
        {
            var allTasks = db.Tasks.ToList();
            
            Console.WriteLine("Список всех задач:");
            foreach (var task in allTasks)
            {
                Console.WriteLine($"Название: {task.Title} - Описание: {task.Description} - Дата: {task.Duedata}");
            }
            Console.ReadLine();
        }


        //просмотр предстоящих задач
        static void viemUpcomingTasks()
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            
            var upcomingTasks = db.Tasks.Where(task => task.Duedata >= currentDate).ToList();

            if (upcomingTasks.Count == 0)
            {
                Console.WriteLine("Нет предстоящих задач.");
            }
            else
            {
                Console.WriteLine("Предстоящие задачи:");
                foreach (var task in upcomingTasks)
                {
                    Console.WriteLine($"{task.Title} - {task.Duedata.ToShortDateString()}");
                }
            }

            Console.ReadLine();
        }

// Просмотр выполненных задач   
        static void viemComletedTasks()
        {
            var completedTasks = db.Tasks.Where(task => task.Iscompleted == true).ToList();

            if (completedTasks.Count == 0)
            {
                Console.WriteLine("Нет выполненных задач.");
            }
            else
            {
                Console.WriteLine("Список выполненных задач:");
                foreach (var task in completedTasks)
                {
                    Console.WriteLine($"{task.Title} - {task.Duedata.ToShortDateString()}");
                }
            }

            Console.ReadLine(); 
        }


        //удаление задач
        static void deleteTask()
        {
            Console.WriteLine("Выберите задачу для удаления: ");
            DisplayTasks();

            int taskId;
            while (true)
            {
                Console.Write("Введите номер задачи для удаления (или 0 для отмены): ");
                if (int.TryParse(Console.ReadLine(), out taskId))
                {
                    var taskToDelete = db.Tasks.FirstOrDefault(t => t.Id == taskId);
                    if (taskToDelete != null)
                    {
                        db.Tasks.Remove(taskToDelete);
                        db.SaveChanges();
                        Console.WriteLine("Задача успешно удалена.");
                    }
                    else if (taskId == 0)
                    {
                        Console.WriteLine("Удаление отменено.");
                    }
                    else
                    {
                        Console.WriteLine("Некорректный номер задачи. Попробуйте снова.");
                    }

                    return;
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                }
            }
        }

        //меню добавления новых задач
        static void addNewTasksMenu()
        {
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine();

            Console.Write("Введите описание задачи: ");
            string description = Console.ReadLine();

            DateOnly inputDate;
            bool validDate;

            do
            {
                Console.WriteLine("Введите дату заверешения задачи (в формате yyyy-WW-dd): ");
                string dateInput = Console.ReadLine();

                validDate = DateOnly.TryParseExact(dateInput, "yyyy-MM-dd", null,
                    System.Globalization.DateTimeStyles.None, out inputDate);

                if (!validDate)
                {
                    Console.WriteLine("Некорректный формат даты. Попробуйте снова.");
                }
            } while (!validDate);


            Models.Task newTask = new Task()
            {
                Title = title,
                Description = description,
                Duedata = inputDate
            };
            db.Tasks.Add(newTask);
            db.SaveChanges();

            Console.WriteLine("Новая задача добавлена.");


            string temp = Console.ReadLine();
            return;
        }

        static void DisplayTasks()
        {
            var allTasks = db.Tasks.ToList();

            if (allTasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            foreach (var task in allTasks)
            {
                Console.WriteLine($"{task.Id}. {task.Title} - {task.Duedata.ToShortDateString()}");
            }
        }


        static void markTaskCompleted()
        {
            Console.WriteLine("Выберите задачу для отметки задачи выполненной:");
            
            DisplayTasks();

            int taskNumber;
            while (true)
            {
                Console.Write("Введите номер задачи для отметки задачи выполненной (или 0 для отмены): ");
                if (int.TryParse(Console.ReadLine(), out taskNumber) && taskNumber >= 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                }
            }

            if (taskNumber == 0)
            {
                Console.WriteLine("Действие отменено.");
            }
            else
            {
                var taskToComplete = db.Tasks.FirstOrDefault(task => task.Id == taskNumber);
                if (taskToComplete != null)
                {
                    taskToComplete.Iscompleted = true; 
                    db.SaveChanges(); 
                    Console.WriteLine("Задача помечена выполненной.");
                }
                else
                {
                    Console.WriteLine("Задача с указанным номером не найдена.");
                }
            }

            Console.WriteLine();
            Console.ReadLine();
        }

    }
}