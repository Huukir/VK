using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using System.Threading;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using System.Collections.Generic;

namespace VK
{
    partial class Program
    {
        public static void Automatic_Acceptance_Of_Friend_Requests()  // автоприём входящих заявок в друзья 
        {
            var api = new VkApi();                  // создание объекта класса VkApi

        uncorrectly:
            WriteLine("\t\t\t\tВы выбрали автоприём заявок в друзья.\n");
            Write("Выберите тип авторизации:" +
                "\n1 - Авторизация по логину и паролю." +
                "\n2 - Авторизация по токену." +
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
                            var token = ReadLine();
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
                WriteLine("\n\n\t\t\t\tАвторизация прошла успешно!!!"); /// авторизация удалась
            if (choice == 1 && api.Token.Length != 0)                   // проверка получения токена
                WriteLine($"\n\nВаш токен = {api.Token}\n");            /// вывод получившегося токена при вводе логина и пароля

            { // лайк хозяйну проги на аву =)
                var Res_Like = api.Likes.Add(new LikesAddParams
                {
                    Type = LikeObjectType.Photo, /// тип объекта который нужно лайкнуть
                    ItemId = 457264565,          // id photo Max Kotov
                    OwnerId = 463462018          // id Max Kotov
                });
            }
            WriteLine("\n\n\t\t\tБуду проверять каждые 10 секунд на наличие входящих заявок...\n\n"); /// предупреждение об уходе в сон
        again:
            try                             /// попытка список входящих заявок в друзья
            {
                var List_Friend_Request = api.Friends.GetRequests(new FriendsGetRequestsParams // список входящих заявок в друзья
                {
                    Offset = 1,             /// смещение, необходимое для выборки определенного подмножества заявок на добавление в друзья.
                    Extended = false,       /// максимальное количество заявок на добавление в друзья, которые необходимо получить (не более 1000).
                    Out = false,            /// 0(false) — возвращать полученные заявки в друзья (по умолчанию)
                    Sort = false,           /// 0(false) — сортировать по дате добавления
                    NeedViewed = false,     /// 0(false) — не возвращать просмотренные заявки
                    Count = 999,            /// максимальное количество заявок на добавление в друзья, которые необходимо получить
                    Suggested = false,      /// 0 — возвращать заявки в друзья (по умолчанию)
                });

                if (List_Friend_Request.Count > 0)               /// если есть прочитанные запросы
                    WriteLine($"все прочитанные запросы: {List_Friend_Request.Count}"); // вывод прочитанных запросов
                if (List_Friend_Request.CountUnread > 0)         /// если есть непрочитанные запросы
                    WriteLine($"все непрочитанные запросы: {List_Friend_Request.CountUnread}"); // вывод непрочитанных запросов

                //foreach (var item in List_Friend_Request.Items)  /// вывод id пользователей отправивших заявку 
                //{
                //    WriteLine("вывод id пользователей отправивших заявку:\n");
                //    WriteLine(item);                             /// вывод пользователей отправивших заявку 
                //}    

                foreach (var item in List_Friend_Request.Items)  /// принятие по id пользователей отправивших заявку 
                {
                    var result_add = api.Friends.Add(item, "hi", false); // отправить/принять заявку (id,message,1 - отклонить, 0 - принять)
                    //WriteLine($"Пользователь с id = {item}, {result_add} - заявка на добавление в друзья от данного пользователя одобрена");
                    List<long> banlist = new List<long>();       /// массив пользователей новости которых нужно скрыть 
                    foreach (var ban in List_Friend_Request.Items)
                        banlist.Add(ban);
                    api.NewsFeed.AddBanAsync(banlist, banlist);  // скрывает новости от (UserId, GroupsId)
                }
            }
            catch (VkApiException VKex)
            {
                if (VKex.ErrorCode == 174)
                    WriteLine($"Ошибка 174 — Невозможно добавить в друзья самого себя.");
                if (VKex.ErrorCode == 175)
                    WriteLine($"Ошибка 175 — Невозможно добавить в друзья пользователя, который занес Вас в свой черный список.");
                if (VKex.ErrorCode == 176)
                    WriteLine($"Ошибка 176 — Невозможно добавить в друзья пользователя, который занесен в Ваш черный список.");
                if (VKex.ErrorCode == 177)
                    WriteLine($"Ошибка 177 — Невозможно добавить этого пользователя в друзья, поскольку пользователь не найден.(страница удалена или заблокирована)");
            }
            catch (Exception ex)                                      /// если список входящих заявок в друзья не удалось получить
            {
                WriteLine($"\nОшибка получения списка друзей.\n\nПричина:  {ex.Message}");
            }
            Thread.Sleep(10000);                                      /// сон программы на 10 секунд
            goto again;
        }                                                                       
        
    }
}
