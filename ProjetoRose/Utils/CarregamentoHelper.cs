using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoRose.Utils
{
    internal class CarregamentoHelper
    {
        public static async Task CarregarDadosAsync(
       Panel panelLoading,
       params (Func<DataTable> metodoDados, Action<DataTable> atualizarUI)[] dados)
        {
            try
            {
                // Mostra a tela de carregamento
                panelLoading.Visible = true;

                // Loop através dos métodos que foram passados
                foreach (var dado in dados)
                {
                    // Executa a consulta no banco
                    DataTable dadosCarregados = await Task.Run(dado.metodoDados);

                    // Atualiza a UI com os dados
                    dado.atualizarUI(dadosCarregados);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Esconde a tela de carregamento depois que tudo carregar
                panelLoading.Visible = false;
            }
        }
    }
}
