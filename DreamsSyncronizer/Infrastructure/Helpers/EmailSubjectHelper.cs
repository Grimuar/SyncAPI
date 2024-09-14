using DreamsSyncronizer.Models.Const;

namespace DreamsSyncronizer.Infrastructure.Helpers;

public static class EmailSubjectHelper
{
    public static (string Subject, string Body) GetRegistrationText(string locale)
    {
        string subject;
        string body;
        
        if (locale == LocaleConst.RusLocale)
        {
            subject = "Успешная регистрация.";
            body = "Учетная запись создана. Ваш пароль для входа:";
        }
        else if (locale == LocaleConst.UkrLocale)
        {
            subject = "Успішна регістрація.";
            body = "Обліковий запис створено. Ваш пароль для входу:";
        }
        else
        {
            subject = "Successful registration.";
            body = "Your account has been created. Your login password:";
        }
        
        return (subject, body);
    }

    public static (string Subject, string Body) GetChangePasswordText(string locale)
    {
        string subject;
        string body;

        if (locale == LocaleConst.RusLocale)
        {
            subject = "Изменение пароля.";
            body = "Новый пароль для доступа к учетной записи:";
        }
        else if (locale == LocaleConst.UkrLocale)
        {
            subject = "Зміна пароля.";
            body = "Новий пароль для доступу до облікового запису:";
        }
        else
        {
            subject = "Change Password.";
            body = "New password to access your account:";
        }
        
        return (subject, body);
    }
    
    public static (string Subject, string Body) GetResetPasswordText(string locale)
    {
        string subject;
        string body;

        if (locale == LocaleConst.RusLocale)
        {
            subject = "Сброс пароля.";
            body = "Ваш пароль был сброшен. Используйте новый пароль для входа:";
        }
        else if (locale == LocaleConst.UkrLocale)
        {
            subject = "Скидання пароля.";
            body = "Ваш пароль було скинуто. Використовуйте новий пароль для входу:";
        }
        else
        {
            subject = "Password reset.";
            body = "Your password has been reset. Use the new password to login:";
        }

        return (subject, body);
    }
}