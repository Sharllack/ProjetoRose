using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasseCarregamento
{
    public class Class1
    {
        private static FormCarregamento formCarregamento;
        private static Timer timer;
        private static int progressoAtual;

        public static void IniciarCarregamento()
        {
            if (formCarregamento == null)
            {
                formCarregamento = new FormCarregamento();
                formCarregamento.Show();
            }

            // Configura o Timer para atualizar a barra de progresso
            timer = new Timer();
            timer.Interval = 100; // Atualiza a cada 100ms
            timer.Tick += (sender, e) =>
            {
                if (formCarregamento.progressBar.Value < 100)
                {
                    formCarregamento.progressBar.Value += 1; // Preenche a barra
                }
                else
                {
                    timer.Stop(); // Para o timer quando a barra chegar a 100%
                }
            };
            timer.Start();
        }

        public static void AtualizarProgresso(int valor)
        {
            if (formCarregamento != null)
            {
                formCarregamento.progressBar.Value = valor; // Atualiza o valor da barra diretamente
            }
        }

        public static void FinalizarCarregamento()
        {
            if (formCarregamento != null)
            {
                formCarregamento.Close(); // Fecha a tela de carregamento
                formCarregamento = null; // Limpa a referência
            }
        }
    }
}
