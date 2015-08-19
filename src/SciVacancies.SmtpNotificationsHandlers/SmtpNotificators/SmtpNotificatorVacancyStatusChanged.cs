﻿using System.Net.Mail;
using SciVacancies.ReadModel;
using SciVacancies.ReadModel.Core;

namespace SciVacancies.SmtpNotificationsHandlers.SmtpNotificators
{
    public class SmtpNotificatorVacancyStatusChanged : SmtpNotificator
    {
        public void Send(Vacancy vacancy, Researcher researcher)
        {
            var body = $@"
<div style=''>
    Уважаемый(-ая), {researcher.secondname} {researcher.firstname}, сообщаем, что вакансия
    <a target='_blank' href='http://{Domain}/vacancies/card/{vacancy.guid}'>{vacancy.fullname}</a> получила новый статус: {vacancy.status.GetDescriptionByResearcher()}
</div>

<br/>
<br/>
<hr/>

<div style='color: lightgray; font-size: smaller;'>
    Это письмо создано автоматически с 
    <a target='_blank' href='http://{Domain}'>Портала вакансий</a>.
    Чтобы не получать такие уведомления отключите их или смените email в 
    <a target='_blank' href='http://{Domain}/researchers/account/'>личном кабинете</a>.
</div>
";
            var mailMessage = new MailMessage(@from: "mailer@alt-lan.com", to: researcher.email, body: body, subject: "Уведомление с портала вакансий")
            {
                IsBodyHtml = true
            };

            Send(mailMessage);
        }
    }
}
