using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using static System.Console;
using System.Threading;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;

namespace VK
{
    partial class Program
    {
        public static void Cheat_Comments() // метод для накрутки комментариев
        {
            var api = new VkApi();                  // создание объекта класса VkApi

        uncorrectly:
            WriteLine("\t\t\t\tВы выбрали накрутку комментариев к записи.\n");
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
                WriteLine($"\n\n\t\t\t\tАвторизация прошла успешно!!!"); /// авторизация удалась
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

            try
            {
                long owner_id, post_id; /// id пользователе и его записи, где надо оставить комментраий
                string Text; /// текст комментария
                short count = 1;
                Write($"\n\nВведите id пользователя, на записи которого нужно оставить комментарий: ");
                owner_id = long.Parse(ReadLine());
                Write($"\nВведите id записи: ");
                post_id = long.Parse(ReadLine());
                Write($"\nВведите сообщение комментария: ");
                Text = ReadLine();
                Write($"\nВведите кол-во комментариев (желательно не более 500): ");
                count = short.Parse(ReadLine());
                for (int i = 1; i <= count; i++)
                {
                    api.Wall.CreateComment(new WallCreateCommentParams // метод создание комментария
                    {
                        OwnerId = owner_id,
                        PostId = post_id,
                        Message = Text,
                    });

                    if (i <= count)
                    {
                        WriteLine($"\nКомментарий №{i} успешно отправлен.\nЖдём 20 сек во избежание блокировки...");
                        Thread.Sleep(20000); /// сон 20 сек
                    }
                }
            }
            catch(VkApiException VKex)
            {
                if (VKex.ErrorCode == 212)
                    WriteLine($"Ошибка 212 - Access to post comments denied (Доступ к публикации комментариев запрещен.)");
                if (VKex.ErrorCode == 213)
                    WriteLine($"Ошибка 213 - Нет доступа к комментированию записи.");
                if (VKex.ErrorCode == 222)
                    WriteLine($"Ошибка 222 - Запрещено размещение ссылок в комментариях.");
                if (VKex.ErrorCode == 223) 
                    WriteLine($"Ошибка 223 - Превышен лимит комментариев на стене.");
            }
            catch (Exception ex)
            {
                WriteLine($"\n\n\t\t\t\tОшибка создания комментария. Причина:  {ex.Message}");
            }
            
        }
    }
}
