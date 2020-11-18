using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using System.Threading;
using VkNet.Enums.SafetyEnums;
using VkNet.Utils;
using VkNet.Exception;
using System.Collections.Generic;

namespace VK
{
    partial class Program
    {
        public static void Automatic_Sending_Of_Requests_To_Friends() // автоотправка заявок в друзья рекомендованным пользователям 
        {
            var api = new VkApi();                  // создание объекта класса VkApi

        uncorrectly:
            WriteLine("\t\t\t\tВы выбрали автоотправку заявок в друзья.\n");
            WriteLine("Выберите тип авторизации:" +
                "\n1 - Авторизация по логину и паролю." +
                "\n2 - Авторизация по токену." +
                "\n0 - Назад." +
                "\n-> ");
            int choice = int.Parse(ReadLine());     /// переменная выбора действия  
            try
            {
                switch (choice)
                {
                    case 1:// кейс входа в вк по логину и паролю
                        {
                            Clear();                        /// очистка консоли
                            Write("-> Введите логин вк: ");
                            var login = ReadLine();         /// считывание логина 

                            Write("\n-> Введите пароль вк: ");
                            var password = ReadLine();      /// считывание пароля

                            api.Authorize(new ApiAuthParams /// авторизация в вк пол логину и паролю
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
                            Clear();                                                  /// очистка консоли
                            Write("\n-> Введите токен вк: ");
                            var token = ReadLine();
                            api.Authorize(new ApiAuthParams { AccessToken = token }); /// авторизация в вк по токену
                        }
                        break;
                    case 0: // кейс возврата в главное меню
                        Clear();
                        Main();                                                       /// возврат в главное меню
                        break;
                    default:
                        Clear();
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
            if (choice == 1 && api.Token.Length != 0)                   // проверка получения токена при вводе логина и пароля
                WriteLine($"\n\nВаш токен = {api.Token}\n");            /// вывод получившегося токена при вводе логина и пароля
            
            { // лайк хозяйну проги на аву =)
                var Res_Like = api.Likes.Add(new LikesAddParams
                {
                    Type = LikeObjectType.Photo, /// тип объекта который нужно лайкнуть
                    ItemId = 457264565, // id photo Max Kotov
                    OwnerId = 463462018 // id Max Kotov
                });
            }

            try // попытка получить список рекомендованных пользователей
            {
                var List_Of_Recommended_People = api.Friends.GetSuggestions(); // список (500)профилей, которые рекомендованны текущему пользователю.
                WriteLine("Вывод id пользователей, которые рекомендованны текущему пользователю:");
                int counter = 1;

                foreach (var user in List_Of_Recommended_People)
                {
                    if (user.IsFriend.Equals(false) && !(user.IsDeactivated))         ///если профиль ещё не добавлен и не заблокирован
                    {
                        Write($"{counter})\t\tid: {user.Id},\t\tИмя: {user.FirstName},\t\tФамилия: {user.LastName}\n");
                        counter++;
                    }
                }

                counter = 1;

                foreach (var user in List_Of_Recommended_People) // цикл отправки заявок рекомендованных пользователям
                {
                    bool friend = true;
                    if (user.IsFriend == false && !user.IsDeactivated)         ///если профиль ещё не добавлен и не заблокирован
                    {
                        var AreFriends = api.Friends.Search(new FriendsSearchParams { UserId = 463462018 }); // список моих текущих друзей
                        foreach (var item in AreFriends) // цикл на проверку людей которым уже отправлена заявка
                        {
                            if (item.Id == user.Id)
                                friend = false;
                        }
                        if (friend && !user.Blacklisted) /// если пользователь ущё не в друзьях и не в черном списке
                        {
                            try
                            {
                                var result_add = api.Friends.Add(user.Id, "", false); // отправить/принять заявку (id,message,1 - отклонить, 0 - отправить/принять)
                                WriteLine($"\nПользователю с id: {user.Id},\t\tИмя: {user.FirstName},\t\tФамилия: {user.LastName}" +
                                    $"\nУспешно отправлена заявка на добавление в друзья");
                                
                                /*
                                    List<long> banlist = new List<long>();
                                    foreach (var ban in List_Of_Recommended_People)
                                        banlist.Add(ban.Id);

                                    api.NewsFeed.AddBanAsync(banlist, banlist); // скрывает новости от (UserId, GroupsId)
                                */
                                counter++;
                                WriteLine($"Отправлено {counter} заявок(ки). Ждём 3 минуты во избежание блокировки...");
                                Thread.Sleep(180000);                                  /// сон программы на 3 минуты
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
                            catch (Exception ex)
                            {
                                WriteLine($"Ошибка отправки заявки в друзья.\n\nПричина:  {ex.Message}");
                            }
                        }
                        else
                            WriteLine($"\nПользователь с id: {user.Id},\t\tИмя: {user.FirstName},\t\tФамилия: {user.LastName}" +
                                $"\nУже находится в друзьях, поэтому был(a) пропущен(a)");
                    } 
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Ошибка получения списка профилей. \nПричина:  {ex.Message}");
            }
            
        }
    }
}
