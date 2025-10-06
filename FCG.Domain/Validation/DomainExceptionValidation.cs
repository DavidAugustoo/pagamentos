namespace FCG.Domain.Validation
{
    #region DomainExceptionValidation
    //A classe DomainExceptionValidation é utilizada para validar condições e lançar exceções se essas condições não forem atendidas. Isso é usado em todas as validações para garantir que o objeto Cliente esteja sempre em um estado válido.
    //As validações de domínio são regras de negócio que garantem que a entidade esteja sempre em um estado válido.Essas regras são aplicadas diretamente na entidade para verificar e garantir que os dados atendam aos requisitos do negócio.No meu exemplo, as validações de domínio são feitas usando a classe DomainExceptionValidation.
    //Exemplo de Validação de Domínio: Na minha classe Cliente, você tem o método ValidateDomain que realiza validações para garantir que o CPF tenha 11 caracteres, o nome tenha no máximo 200 caracteres, e o telefone celular não seja vazio.
    #endregion
    public class DomainExceptionValidation : Exception
    {
        public DomainExceptionValidation(string error):base(error) { }
        public static void When(bool hasError, string error)
        {
            if (hasError){
                throw new DomainExceptionValidation(error);
            }
        }
    }
}
