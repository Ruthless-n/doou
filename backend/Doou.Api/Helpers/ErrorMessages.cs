namespace Doou.Api.Helpers
{
    public static class ErrorMessages
    {
        public static class Auth
        {
            public const string InvalidCredentials = "E-mail ou senha inválidos.";
            public const string UserNotFound = "Usuário não encontrado.";
        }

        public static class User
        {
            public const string UserNotFound = "Usuário não encontrado.";
            public const string NameRequired = "O nome é obrigatório.";
            public const string EmailRequired = "O e-mail é obrigatório.";
            public const string EmailInvalid = "Insira um e-mail válido.";
            public const string PasswordRequired = "A senha é obrigatória.";
            public const string PasswordTooShort = "A senha deve ter no mínimo 6 caracteres.";
            public const string EmailAlreadyExists = "Já existe um usuário cadastrado com este e-mail.";
            public const string CPFRequired = "O CPF é obrigatório.";
            public const string CPFInvalid = "Insira um CPF válido.";
            public const string CPFAlreadyExists = "Já existe um usuário cadastrado com este CPF.";
        }

        public static class Donation
        {
            public const string TitleRequired = "O título da doação é obrigatório.";
            public const string CategoryRequired = "A categoria deve ser informada.";
            public const string DescriptionTooLong = "A descrição não pode exceder 500 caracteres.";
        }

        public static class Category
        {
            public const string NotFound = "Categoria não encontrada.";
        }

        public class Search
        {
            public const string QueryTooShort = "A consulta de busca deve ter pelo menos 3 caracteres.";
            public const string NoResults = "Nenhum resultado encontrado para a consulta.";
        }

        public static class General
        {
            public const string InternalServerError = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            public const string NotFound = "O recurso solicitado não foi encontrado.";
            public const string Unauthorized = "Acesso não autorizado.";
        }
    }
}
