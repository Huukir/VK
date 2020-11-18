using static System.Console;

namespace VK
{
    partial class Program
    {
        static void Main()
        {
            WriteLine("\n\t\t\t\tДобро пожаловать в прогу  \"HukPiarBot\"!!!"); // приветствие
            uncorrectly:
            Write($"Выберите действие:" +
                $"\n1 - Автоприём входящих заявок в друзья." +
                $"\n2 - Автоотправка заявок в друзья рекомендованный пользователям." +
                $"\n3 - Накрутка сообщений с помощью создания бесед(нужен фейк, который и будет создавать беседы)." +
                $"\n4 - Накрутка комментариев" +
                $"\n5 - Накрутка фото.(в разработке)" +
                $"\n0 - Выход" +
                $"\n-> " +
                $"");

            int choice = int.Parse(ReadLine());     // ввод переменной выбора действия  
            
            switch (choice)
            {
                case 1:
                    {
                        Clear(); // очистка консоли
                        Automatic_Acceptance_Of_Friend_Requests(); // вызов метода на автоприём входящих заявок в друзья 
                    }
                    break;
                case 2:
                    {
                        Clear(); // очистка консоли
                        Automatic_Sending_Of_Requests_To_Friends(); // вызов метода на автоотправку заявок в друзья рекомендованным пользователям
                    }
                    break;
                case 3:
                    {
                        Clear(); // очистка консоли 
                        Cheat_Message(); // вызов метода накрутки сообщений через сообщество 
                    }
                    break;
                case 4:
                    {
                        Clear(); // очистка консоли 
                        Cheat_Comments(); // вызов метода накрутки комментариев 
                    }
                    break;
                case 5:
                    {
                        Clear(); // очистка консоли 

                    }
                    break;
                case 0:
                    {
                        Clear(); // очистка консоли 
                        WriteLine("\t\t\t\tВсего доброго!!!\n\n");
                    }
                    break;
                default: // дефолтный кейс
                    goto uncorrectly;
            }
        }
    }
}
