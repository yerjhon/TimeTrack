using Mvc.Mailer;
using MemberShipMVC.Mailers.Models;

namespace MemberShipMVC.Mailers
{ 
    public interface IPasswordResetMailer
    {
			MvcMailMessage PasswordReset(MailerModel model);
	}
}