using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using System.Threading;
using VkNet.Enums.SafetyEnums;
using System.Collections.Generic;
using VkNet.Exception;

namespace VK
{
    partial class Program
    {
        public static void Cheat_Message() // метод накрутки сообщений с помощью создания бесед сообществом
        {
            var api = new VkApi();                  // создание объекта класса VkApi

        uncorrectly:
            WriteLine("\t\t\t\tВы выбрали автонакрутку сообщений с помощью создания бесед.\n");
            Write("Выберите тип авторизации:" +
                "\n1 - Авторизация фейка по логину и паролю." +
                "\n2 - Авторизация фейка по токену." +
                "\n0 - Назад." +
                "\n-> ");
            int choice = int.Parse(ReadLine());     // переменная выбора действия  
            try
            {
                switch (choice)
                {
                    case 1: // кейс входа в вк по логину и паролю
                        {
                            Clear();                            /// очистка консоли
                            Write("-> Введите логин вк: ");
                            var login = ReadLine();             /// считывание логина 

                            Write("\n-> Введите пароль вк: ");
                            var password = ReadLine();          /// считывание пароля

                            api.Authorize(new ApiAuthParams     /// авторизация в вк пол логину и паролю
                            {
                                ApplicationId = 2685278,
                                Login = login,
                                Password = password,
                                Settings = Settings.All,
                                TwoFactorAuthorization = () =>
                                {
                                    Write("\n-> Введите код двухфакторной аутентификации(в лс вк): ");
                                    return ReadLine();
                                }
                            });
                        }
                        break;
                    case 2: // кейс входа в вк по токену 
                        {
                            Clear();    /// очистка консоли
                            Write("\n-> Введите токен вк: ");
                            var token = "92cdb72a92095b77183a81c1db11998fc192d0b8efeef32ed44c1a311e1d498a5628da4f04c30acdd550c";  //ReadLine(); "68f6b135df1b8d5a8c88273c250d1d7bda58452e54185e023648ec79f2492d5d3845bcc9161b60b13e84a";
                            api.Authorize(new ApiAuthParams { AccessToken = token }); /// авторизация в вк по токену
                        }
                        break;
                    case 0: // кейс возврата в главное меню
                        Clear();        /// очистка консоли
                        Main();         /// возврат в главное меню
                        break;
                    default:
                        Clear();        /// очистка консоли
                        WriteLine("\t\t\t\tВведите корректное число!!!\n\n");
                        goto uncorrectly;
                }
            }
            catch (Exception ex)
            {
                Write($"\n\n\n\t\t\t\tОшибка Авторизации!!!\n\nПричина:  {ex.Message}");
            }

            if (api.IsAuthorized)                                       // проверка успешности авторизации
                WriteLine($"\n\n\t\t\t\tАвторизация прошла успешно!!!") ; /// авторизация удалась
            if (choice == 1 && api.Token.Length != 0)                   // проверка получения токена при вводе логина и пароля
                WriteLine($"\n\nВаш токен = {api.Token}\n");            /// вывод получившегося токена при вводе логина и пароля

            { // лайк хозяйну проги на аву =)
                var Res_Like = api.Likes.Add(new LikesAddParams
                {
                    Type = LikeObjectType.Photo, /// тип объекта который нужно лайкнуть
                    ItemId = 457264565,          // id photo Max Kotov
                    OwnerId = 463462018          // id Max Kotov
                });
            }

            try
            {
                List<ulong> list = new List<ulong>(); // создание списка пользователей с которыми необходимо создать беседу
                
                //{
                //    ulong Ids;                          /// id пользователей которым нужно накрутить сообщения
                //    short again = 1;
                //    do
                //    {
                //        Write($"\nВведите id пользователей которым нужно накрутить сообщения: ");
                //        Ids = uint.Parse(ReadLine()); // ввод id пользователей
                //        list.Add(Ids);
                //        WriteLine("\n\t\t\t\tДобавить ещё пользователя?" +
                //            "\n\t\t\t\t1 - Да.\t2 - Нет.");
                //        again = short.Parse(ReadLine()); /// выбор добавления пользователя
                //    }
                //    while (again == 1);
                //}

                list.Add(463462018); // Max Kotov
                list.Add(449965017); // Задрот
                list.Add(526137881); // T T-ЛБ
                int counter = 1;
                do
                {
                    var Id_Chats = api.Messages.CreateChat(list, $"Беседа для накрутки №{counter}"); // создание беседы (userids, название беседы )
                    WriteLine($"Беседа №{counter} с id: {Id_Chats} была успешно создана!!!");
                    counter++;                        /// счётчик бесед прибавляется на 1
                    if (counter <= 50) // после 50 бесед вк ставит ограничение
                    {
                        WriteLine($"Ждём 3 минуты во избежание блокировки...");
                        Thread.Sleep(180000);         // сон 3 минуты
                    }
                }
                while (counter <= 50);                /// создаёт 50 бесед за 2ч 30мин и прекращает работу
                WriteLine("\n\n\t\t\t\tСоздание бесед завершилось!!!\n\n");
            }
            catch (VkApiException VKex)
            {
                if (VKex.ErrorCode == 936)
                    WriteLine($"Ошибка 936 - Contact not found (указанный пользователь не найден)");
            }
            catch (Exception ex)
            {
                WriteLine($"\n\n\n\t\t\t\tОшибка создания беседы!!!\n\nПричина: {ex.Message}");
            }
        }
    }
}
