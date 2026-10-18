using System.Collections.Generic;

namespace Anastasia423WPF.Model
{
    /// <summary>
    /// Считает неудачные попытки ввода пароля по каждому логину.
    /// Счётчик хранится в памяти на время работы программы и не сбрасывается при
    /// переходе между страницами (поэтому «Назад → Войти» не обнуляет попытки).
    /// После <see cref="MaxAttempts"/> неверных паролей подряд для входа требуется пройти капчу.
    /// </summary>
    public static class LoginAttemptTracker
    {
        /// <summary>Сколько раз можно ошибиться в пароле до показа капчи.</summary>
        public const int MaxAttempts = 3;

        private static readonly Dictionary<string, int> FailedAttempts = new Dictionary<string, int>();

        private static string Normalize(string login)
        {
            return (login ?? string.Empty).Trim().ToLowerInvariant();
        }

        /// <summary>Фиксирует неверный пароль и возвращает текущее число неудачных попыток.</summary>
        public static int RegisterFailure(string login)
        {
            string key = Normalize(login);
            int count;
            FailedAttempts.TryGetValue(key, out count);
            count++;
            FailedAttempts[key] = count;
            return count;
        }

        /// <summary>Нужно ли пройти капчу, чтобы продолжить авторизацию под этим логином.</summary>
        public static bool IsCaptchaRequired(string login)
        {
            int count;
            return FailedAttempts.TryGetValue(Normalize(login), out count) && count >= MaxAttempts;
        }

        /// <summary>Сбрасывает счётчик (после успешного входа или решённой капчи).</summary>
        public static void Reset(string login)
        {
            FailedAttempts.Remove(Normalize(login));
        }
    }
}
