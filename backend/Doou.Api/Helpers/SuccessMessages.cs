namespace Doou.Api.Helpers
{
    public class SuccessMessages
    {
        public static class Auth
        {
            public const string UserRegistered = "Usuário registrado com sucesso.";
            public const string UserLoggedIn = "Login realizado com sucesso.";
            public const string PasswordResetEmailSent = "E-mail de redefinição de senha enviado com sucesso.";
            public const string PasswordResetRequested = "Solicitação de redefinição de senha realizada com sucesso.";
        }
        public static class User
        {
            public const string UserCreated = "Usuário criado com sucesso.";
            public const string UserUpdated = "Usuário atualizado com sucesso.";
            public const string UserDeleted = "Usuário deletado com sucesso.";

        }

        public static class Donation
        {
            public const string DonationCreated = "Doação criada com sucesso.";
            public const string DonationUpdated = "Doação atualizada com sucesso.";
            public const string DonationDeleted = "Doação deletada com sucesso.";
        }
    }
}
