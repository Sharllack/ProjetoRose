using System;

namespace ClasseDeEntidades
{
    public class ClasseEntidades
    {
        public int codigo { get; set; }
        public int id  { get; set; }
        public string nome { get; set; }
        public string usuario { get; set; }
        public string cpf { get; set; }
        public string celular { get; set; }
        public string veiculo { get; set; }
        public string senha { get; set; }
        public int entregasTotais { get; set; }
        public int documentosTotais { get; set; }
        public decimal salarioBase { get; set; }
        public int fc { get; set; }
        public int carta { get; set; }
        public int caixa { get; set; }
        public int devolucao { get; set; }
        public decimal passagem { get; set; }
        public decimal alimentacao { get; set; }
        public decimal desconto { get; set; }
        public string acao { get; set; }
        public DateTime dataCadastro { get; set; }
        public DateTime dataNascimento { get; set; }
        public DateTime dataEntrega { get; set; }
        public DateTime dataInicio { get; set; }
        public DateTime dataFim { get; set; }
    }
}
