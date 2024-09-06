namespace DesafioProjetoHospedagem.Models;
using System.Text.RegularExpressions;

public class Pessoa
{
     private string _cpf;
    public Pessoa() { }

    public Pessoa(string nome)
    {
        Nome = nome;
    }
    public Pessoa(string nome, string cpf)
    {
        Nome = nome;
        Cpf = cpf;
    }

    public Pessoa(string nome, string sobrenome, string cpf)
    {
        Nome = nome;
        Sobrenome = sobrenome;
        Cpf = cpf;
    }

    public string Nome { get; set; }
    public string Sobrenome { get; set; }
     public string Cpf
        {
            get {return _cpf;}
            set { _cpf = FormatCpf(value); }
        }
    public string NomeCompleto => $"{Nome} {Sobrenome}".ToUpper();

     private string FormatCpf(string cpf)
    {
         if (string.IsNullOrEmpty(cpf))
        {
            throw new ArgumentNullException(nameof(cpf), "CPF não pode ser nulo ou vazio.");
        }
        // Remove todos os caracteres não numéricos
        cpf = Regex.Replace(cpf, @"[^\d]", "");

        //Verifica se o CPF possui o comprimento correto
        if (cpf.Length != 11)
        {
            throw new ArgumentException("CPF deve conter 11 dígitos.");
        }

        // Formata o CPF no padrão xxx.xxx.xxx-xx
        return string.Format("{0}.{1}.{2}-{3}",
            cpf.Substring(0, 3),
            cpf.Substring(3, 3),
            cpf.Substring(6, 3),
            cpf.Substring(9, 2));
    }
}