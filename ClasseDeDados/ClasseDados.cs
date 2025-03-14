using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using ClasseDeEntidades;
using System;
using System.Windows;

namespace ClasseDeDados
{
    public class ClasseDados
    {
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);

        // MÉTODO DE LOGIN
        public DataTable Dlogin(ClasseEntidades objeto)
        {
            string query = "SELECT user, pass " +
                           "FROM host WHERE user = @usuario AND pass = @senha";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@usuario", objeto.usuario);
            cmd.Parameters.AddWithValue("@senha", objeto.senha);

            // Criando o adaptador de dados para preencher o DataTable com os resultados da consulta
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                // Executando a consulta e preenchendo o DataTable com os dados
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                // Em caso de erro, logamos o erro no arquivo
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao executar a consulta: " + ex.Message);
                throw new Exception("Erro ao executar a consulta: " + ex.Message);
            }
            finally
            {
                // Fechando a conexão com o banco de dados
                conn.Close();
            }
            // Retornando o DataTable com os dados ou vazio caso não tenha resultado
            return dt;
        }

        // MÉTODO DE CADASTRO DE ENTREGA
        public DataTable DlistarEntregas()
        {
            string query = "" +
                "SELECT entregas.id, nome, veiculo, data_entrega, fc, carta, caixa, quantidade_entregas, quantidade_devolucoes " +
                "FROM entregas " +
                "JOIN funcionarios ON entregas.id_funcionario = funcionarios.id";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();


            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar entregas: " + ex.Message);
                throw new Exception("Erro ao listar entregas: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarFuncionarios()
        {
            string query = "" +
                "SELECT id, nome " +
                "FROM funcionarios ";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();


            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar funcionários: " + ex.Message);
                throw new Exception("Erro ao listar funcionários: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DbuscarEntregas(ClasseEntidades objeto)
        {
            string query = "" +
                "SELECT entregas.id, nome, veiculo, data_entrega, fc, carta, caixa, quantidade_entregas, quantidade_devolucoes " + // Adicionado tipo_nome
                "FROM entregas " +
                "JOIN funcionarios ON entregas.id_funcionario = funcionarios.id " +
                "WHERE nome LIKE @nome " +
                "OR cpf LIKE @cpf " +
                "OR veiculo LIKE @veiculo"; // Mantido o LIKE correto

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nome", objeto.nome + "%"); // Garante que sempre tenha o %
            cmd.Parameters.AddWithValue("@cpf", objeto.cpf + "%"); // Garante que sempre tenha o %
            cmd.Parameters.AddWithValue("@veiculo", objeto.veiculo + "%"); // Garante que sempre tenha o %

            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\\temp\\log.txt", "Erro ao listar usuários: " + ex.Message);
                throw new Exception("Erro ao listar usuários: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public string DmanutencaoEntregas(ClasseEntidades objeto)
        {
            string mensagem = "";
            try
            {
                conn.Open();

                // Verifica se o usuário já está cadastrado
                if (objeto.acao == "1") // Cadastro
                {
                    string checkQuery = "" +
                        "SELECT COUNT(*) FROM entregas " +
                        "JOIN funcionarios ON entregas.id_funcionario = funcionarios.id " +
                        "WHERE funcionarios.id = @id " +
                        "AND data_entrega = @data";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", objeto.id);
                    checkCmd.Parameters.AddWithValue("@data", objeto.dataEntrega);
                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        return "Já existe uma entrega cadastrada para esse funcionário no dia selecionado!";
                    }
                }

                // Define a query de acordo com a ação
                string query = "";
                if (objeto.acao == "1") // Inserir
                {
                    query = "INSERT INTO entregas (id_funcionario, data_entrega, fc, carta, caixa, quantidade_entregas, quantidade_devolucoes) VALUES (@id, @data_entrega, @fc, @carta, @caixa, @quantidade_entregas, @quantidade_devolucoes)";
                }
                else if (objeto.acao == "2") // Atualizar
                {
                    query = "UPDATE entregas SET id_funcionario = @id, data_entrega = @data_entrega, fc = @fc, carta = @carta, quantidade_entregas = @quantidade_entregas, quantidade_devolucoes = @quantidade_devolucoes WHERE id = @codigo";
                }
                else if (objeto.acao == "3") // Deletar
                {
                    query = "DELETE FROM entregas WHERE id = @codigo";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", objeto.id);
                cmd.Parameters.AddWithValue("@data_entrega", objeto.dataEntrega);
                cmd.Parameters.AddWithValue("@fc", objeto.fc);
                cmd.Parameters.AddWithValue("@carta", objeto.carta);
                cmd.Parameters.AddWithValue("@caixa", objeto.caixa);
                cmd.Parameters.AddWithValue("@quantidade_entregas", objeto.entregasTotais);
                cmd.Parameters.AddWithValue("@quantidade_devolucoes", objeto.devolucao);
                if (objeto.acao != "1") // Somente atualização e exclusão precisam do ID
                {
                    cmd.Parameters.AddWithValue("@codigo", objeto.codigo);
                }

                int linhasAfetadas = cmd.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                {
                    mensagem = objeto.acao == "1" ? "Entrega cadastrada com sucesso!"
                              : objeto.acao == "2" ? "Entrega atualizada com sucesso!"
                              : "Entrega excluída com sucesso!";
                }
                else
                {
                    mensagem = "Nenhuma alteração realizada.";
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro: " + ex.Message);
                throw new Exception("Erro: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return mensagem;
        }

        // MÉTODOS FUNCIONÁRIOS
        public DataTable DlistarInfosFuncionarios()
        {
            string query = "" +
            "SELECT id, nome, cpf, celular, data_nascimento, veiculos.veiculo, salario_base, data_cadastro " +
            "FROM funcionarios " +
            "JOIN veiculos ON funcionarios.veiculo = veiculos.id_veiculo";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();


            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar funcionarios: " + ex.Message);
                throw new Exception("Erro ao listar funcionarios: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarVeiculos()
        {
            string query = "" +
            "SELECT * " +
            "FROM veiculos ";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();


            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar veiculos: " + ex.Message);
                throw new Exception("Erro ao listar veiculos: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DbuscarFuncionarios(ClasseEntidades objeto)
        {
            string query = "" +
                "SELECT id, nome, cpf, celular, data_nascimento, veiculos.veiculo, salario_base, data_cadastro " +
                "FROM funcionarios " +
                "JOIN veiculos ON funcionarios.veiculo = veiculos.id_veiculo " +
                "WHERE nome LIKE @nome " +
                "OR cpf LIKE @cpf " +
                "OR id_veiculo LIKE @veiculo " +
                "OR celular LIKE @celular";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nome", objeto.nome + "%"); // Garante que sempre tenha o %
            cmd.Parameters.AddWithValue("@cpf", objeto.cpf + "%"); // Garante que sempre tenha o %
            cmd.Parameters.AddWithValue("@veiculo", objeto.veiculo + "%"); // Garante que sempre tenha o %
            cmd.Parameters.AddWithValue("@celular", objeto.celular + "%"); // Garante que sempre tenha o %

            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\\temp\\log.txt", "Erro ao listar funcionários: " + ex.Message);
                throw new Exception("Erro ao listar funcionários: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public string DmanutencaoFuncionarios(ClasseEntidades objeto)
        {
            string mensagem = "";
            try
            {
                conn.Open(); // Abre a conexão antes de executar o comando

                // Verifica se o usuário já está cadastrado
                if (objeto.acao == "1") // Cadastro
                {
                    string checkQuery = "" +
                        "SELECT COUNT(*) FROM funcionarios " +
                        "WHERE nome = @nome " +
                        "AND cpf = @cpf";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@nome", objeto.nome);
                    checkCmd.Parameters.AddWithValue("@cpf", objeto.cpf);
                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        return "Já existe um funcionário cadastrado com o mesmo nome e CPF!";
                    }
                }

                // Define a query de acordo com a ação
                string query = "";
                if (objeto.acao == "1") // Inserir
                {
                    query = "INSERT INTO funcionarios (nome, cpf, data_nascimento, veiculo, salario_base, celular) VALUES (@nome, @cpf, @data_nascimento, @veiculo, @salario_base, @celular)";
                }
                else if (objeto.acao == "2") // Atualizar
                {
                    query = "UPDATE funcionarios SET nome = @nome, cpf = @cpf, data_nascimento = @data_nascimento, veiculo = @veiculo, salario_base = @salario_base, celular = @celular WHERE id = @codigo";
                }
                else if (objeto.acao == "3") // Deletar
                {
                    query = "DELETE FROM funcionarios WHERE id = @codigo";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nome", objeto.nome);
                cmd.Parameters.AddWithValue("@cpf", objeto.cpf);
                cmd.Parameters.AddWithValue("@data_nascimento", objeto.dataNascimento);
                cmd.Parameters.AddWithValue("@veiculo", objeto.veiculo);
                cmd.Parameters.AddWithValue("@salario_base", objeto.salarioBase);
                cmd.Parameters.AddWithValue("@celular", objeto.celular);
                if (objeto.acao != "1") // Somente atualização e exclusão precisam do ID
                {
                    cmd.Parameters.AddWithValue("@codigo", objeto.codigo);
                }

                int linhasAfetadas = cmd.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                {
                    mensagem = objeto.acao == "1" ? "Funcionário cadastrado com sucesso!"
                              : objeto.acao == "2" ? "Funcionário atualizado com sucesso!"
                              : "Funcionário excluído com sucesso!";
                }
                else
                {
                    mensagem = "Nenhuma alteração realizada.";
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro: " + ex.Message);
                throw new Exception("Erro: " + ex.Message);
            }
            finally
            {
                conn.Close(); // Fecha a conexão no final
            }

            return mensagem;
        }

        // MÉTODOS ESTATÍSTICAS
        public DataTable DbuscarDadosEntregas(ClasseEntidades objeto)
        {
            string query = @"
            SELECT funcionarios.nome, funcionarios.id, 
                   SUM(entregas.quantidade_entregas) AS total_entregas 
            FROM entregas 
            JOIN funcionarios ON entregas.id_funcionario = funcionarios.id 
            WHERE data_entrega BETWEEN @data_inicio AND @data_fim 
            GROUP BY funcionarios.id 
            ORDER BY total_entregas DESC";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@data_inicio", objeto.dataInicio);
                cmd.Parameters.AddWithValue("@data_fim", objeto.dataFim);

                using (MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dtTotalEntregas = new DataTable();

                    try
                    {
                        sqlDataAdapter.Fill(dtTotalEntregas);
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao executar a consulta: " + ex.Message);
                        throw new Exception("Erro ao executar a consulta: " + ex.Message);
                    }

                    return dtTotalEntregas;
                }
            }
        }

        public DataTable DbuscarDadosEntregasDetalhadas(ClasseEntidades objeto)
        {
            string query = @"
            SELECT funcionarios.nome, funcionarios.id, entregas.quantidade_entregas, entregas.data_entrega 
            FROM entregas 
            JOIN funcionarios ON entregas.id_funcionario = funcionarios.id 
            WHERE data_entrega BETWEEN @data_inicio AND @data_fim
            GROUP BY entregas.id";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@data_inicio", objeto.dataInicio);
                cmd.Parameters.AddWithValue("@data_fim", objeto.dataFim);

                using (MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dtEntregasDetalhadas = new DataTable();

                    try
                    {
                        sqlDataAdapter.Fill(dtEntregasDetalhadas);
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao executar a consulta: " + ex.Message);
                        throw new Exception("Erro ao executar a consulta: " + ex.Message);
                    }

                    return dtEntregasDetalhadas;
                }
            }
        }

        // MÉTODO ADC/DESC
        public DataTable DlistarAdcDesc()
        {
            string query = "" +
               "SELECT adc_desc.id, data_inicio, data_fim, nome, passagem_combustivel, alimentacao, desconto " +
               "FROM adc_desc " +
               "JOIN funcionarios ON adc_desc.id_funcionario = funcionarios.id";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();


            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public string DmanutencaoAdcdesc(ClasseEntidades objeto)
        {
            string mensagem = "";
            try
            {
                conn.Open();

                // Verifica se o usuário já está cadastrado
                if (objeto.acao == "1") // Cadastro
                {
                    string checkQuery = "SELECT COUNT(*) FROM adc_desc " +
                        "WHERE id_funcionario = @id " +
                        "AND (" +
                        "    (data_inicio BETWEEN @dataInicio AND @dataFim) " +
                        "    OR (data_fim BETWEEN @dataInicio AND @dataFim) " +
                        "    OR (@dataInicio BETWEEN data_inicio AND data_fim) " +
                        "    OR (@dataFim BETWEEN data_inicio AND data_fim)" +
                        ")";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", objeto.id);
                    checkCmd.Parameters.AddWithValue("@dataInicio", objeto.dataInicio);
                    checkCmd.Parameters.AddWithValue("@dataFim", objeto.dataFim);
                    int existe = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        return "Já existe um registro cadastrada para esse funcionário no período selecionado!";
                    }
                }

                // Define a query de acordo com a ação
                string query = "";
                if (objeto.acao == "1") // Inserir
                {
                    query = "INSERT INTO adc_desc (data_inicio, data_fim, id_funcionario, passagem_combustivel, alimentacao, desconto) VALUES (@dataInicio, @dataFim, @id, @pass_comb, @alimentacao, @desconto)";
                }
                else if (objeto.acao == "2") // Atualizar
                {
                    query = "UPDATE adc_desc SET data_inicio = @dataInicio, data_fim = @dataFim, id_funcionario = @id, passagem_combustivel = @pass_comb, alimentacao = @alimentacao, desconto = @desconto WHERE id = @codigo";
                }
                else if (objeto.acao == "3") // Deletar
                {
                    query = "DELETE FROM adc_desc WHERE id = @codigo";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@dataInicio", objeto.dataInicio);
                cmd.Parameters.AddWithValue("@dataFim", objeto.dataFim);
                cmd.Parameters.AddWithValue("@id", objeto.id);
                cmd.Parameters.AddWithValue("@pass_comb", objeto.passagem);
                cmd.Parameters.AddWithValue("@alimentacao", objeto.alimentacao);
                cmd.Parameters.AddWithValue("@desconto", objeto.desconto);
                if (objeto.acao != "1") // Somente atualização e exclusão precisam do ID
                {
                    cmd.Parameters.AddWithValue("@codigo", objeto.codigo);
                }

                int linhasAfetadas = cmd.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                {
                    mensagem = objeto.acao == "1" ? "Registo feito com sucesso!"
                              : objeto.acao == "2" ? "Registro atualizado com sucesso!"
                              : "Registro excluído com sucesso!";
                }
                else
                {
                    mensagem = "Nenhuma alteração realizada.";
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro: " + ex.Message);
                throw new Exception("Erro: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return mensagem;
        }

        public DataTable DbuscarAdc_desc(ClasseEntidades objeto)
        {
            string query = "" +
               "SELECT adc_desc.id, data_inicio, data_fim, nome, passagem_combustivel, alimentacao, desconto " +
               "FROM adc_desc " +
               "JOIN funcionarios ON adc_desc.id_funcionario = funcionarios.id " +
               "WHERE nome LIKE @nome " +
               "OR data_inicio LIKE @dataInicio " +
               "OR data_fim LIKE @dataFim";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@dataInicio", objeto.dataInicio + "%");
            cmd.Parameters.AddWithValue("@dataFim", objeto.dataFim + "%");
            cmd.Parameters.AddWithValue("@nome", objeto.nome + "%");
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();


            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        // MÉTODO RELATÓRIO
        public DataTable DlistarRelatorio()
        {
            string query = @"
               SELECT nome AS NOME, veiculos.veiculo AS VEÍCULO, 
               SUM(fc) AS FC,
               SUM(carta) AS CARTA,
               SUM(caixa) AS CAIXA,
               SUM(quantidade_entregas) AS ENTREGAS,
               SUM(quantidade_devolucoes) AS DEVOLUÇÃO,
               COALESCE(adc_desc_total.PASS_COMB, 0) AS PASS_COMB,
               COALESCE(adc_desc_total.ALIMENTAÇÃO, 0) AS ALIMENTAÇÃO,
               COALESCE(adc_desc_total.DESCONTO, 0) AS DESCONTO,
               SUM(
                    CASE WHEN funcionarios.veiculo = 1 THEN entregas.fc * 1 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 1 THEN entregas.carta * 1.7 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 1 THEN entregas.caixa * 2.3 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 3 THEN entregas.fc * 1 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 3 THEN entregas.carta * 1 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 3 THEN entregas.caixa * 2 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 2 THEN entregas.fc * 1 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 2 THEN entregas.carta * 1 ELSE 0 END +
                    CASE WHEN funcionarios.veiculo = 2 THEN entregas.caixa * 2 ELSE 0 END
               ) +
               COALESCE(adc_desc_total.PASS_COMB, 0) + 
               COALESCE(adc_desc_total.ALIMENTAÇÃO, 0) - 
               COALESCE(adc_desc_total.DESCONTO, 0) AS TOTAL
               FROM funcionarios
               JOIN entregas ON funcionarios.id = entregas.id_funcionario
               JOIN veiculos ON funcionarios.veiculo = veiculos.id_veiculo
               LEFT JOIN (
                    SELECT id_funcionario, 
                           SUM(passagem_combustivel) AS PASS_COMB, 
                           SUM(alimentacao) AS ALIMENTAÇÃO, 
                           SUM(desconto) AS DESCONTO
                    FROM adc_desc
                    GROUP BY id_funcionario
               ) AS adc_desc_total ON funcionarios.id = adc_desc_total.id_funcionario
               GROUP BY nome, veiculos.veiculo, adc_desc_total.PASS_COMB, adc_desc_total.ALIMENTAÇÃO, adc_desc_total.DESCONTO";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarFuncionariosMoto()
        {
            string query = @"
                SELECT nome, id
                FROM funcionarios
                WHERE veiculo = 1";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarFuncionariosBicicleta()
        {
            string query = @"
                SELECT nome, id
                FROM funcionarios
                WHERE veiculo = 2";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarFuncionariosCarro()
        {
            string query = @"
                SELECT nome, id
                FROM funcionarios
                WHERE veiculo = 3";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarFuncionariosRelatorio(ClasseEntidades objeto)
        {
            string query = @"
                SELECT nome AS NOME, veiculos.veiculo AS VEÍCULO, 
                   SUM(fc) AS FC,
                   SUM(carta) AS CARTA,
                   SUM(caixa) AS CAIXA,
                   SUM(quantidade_entregas) AS ENTREGAS,
                   SUM(quantidade_devolucoes) AS DEVOLUÇÃO,
                   COALESCE(adc_desc_total.PASS_COMB, 0) AS PASS_COMB,
                   COALESCE(adc_desc_total.ALIMENTAÇÃO, 0) AS ALIMENTAÇÃO,
                   COALESCE(adc_desc_total.DESCONTO, 0) AS DESCONTO,
                   SUM(
                        CASE WHEN funcionarios.veiculo = 1 THEN entregas.fc * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 1 THEN entregas.carta * 1.7 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 1 THEN entregas.caixa * 2.3 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 3 THEN entregas.fc * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 3 THEN entregas.carta * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 3 THEN entregas.caixa * 2 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 2 THEN entregas.fc * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 2 THEN entregas.carta * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 2 THEN entregas.caixa * 2 ELSE 0 END
                   ) +
                   COALESCE(adc_desc_total.PASS_COMB, 0) + 
                   COALESCE(adc_desc_total.ALIMENTAÇÃO, 0) - 
                   COALESCE(adc_desc_total.DESCONTO, 0) AS TOTAL
            FROM funcionarios
            JOIN entregas ON funcionarios.id = entregas.id_funcionario
            JOIN veiculos ON funcionarios.veiculo = veiculos.id_veiculo
            LEFT JOIN (
                 SELECT id_funcionario, 
                        SUM(passagem_combustivel) AS PASS_COMB, 
                        SUM(alimentacao) AS ALIMENTAÇÃO, 
                        SUM(desconto) AS DESCONTO
                 FROM adc_desc
                 GROUP BY id_funcionario
            ) AS adc_desc_total ON funcionarios.id = adc_desc_total.id_funcionario
            WHERE funcionarios.veiculo = @id
            GROUP BY nome, veiculos.veiculo, adc_desc_total.PASS_COMB, adc_desc_total.ALIMENTAÇÃO, adc_desc_total.DESCONTO;
            ";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", objeto.veiculo);
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar resgistros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        public DataTable DlistarFuncionariosFiltro(ClasseEntidades objeto)
        {
            string query = @"
                SELECT nome AS NOME, veiculos.veiculo AS VEÍCULO, 
                   SUM(fc) AS FC,
                   SUM(carta) AS CARTA,
                   SUM(caixa) AS CAIXA,
                   SUM(quantidade_entregas) AS ENTREGAS,
                   SUM(quantidade_devolucoes) AS DEVOLUÇÃO,
                   COALESCE(adc_desc_total.PASS_COMB, 0) AS PASS_COMB,
                   COALESCE(adc_desc_total.ALIMENTAÇÃO, 0) AS ALIMENTAÇÃO,
                   COALESCE(adc_desc_total.DESCONTO, 0) AS DESCONTO,
                   SUM(
                        CASE WHEN funcionarios.veiculo = 1 THEN entregas.fc * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 1 THEN entregas.carta * 1.7 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 1 THEN entregas.caixa * 2.3 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 3 THEN entregas.fc * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 3 THEN entregas.carta * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 3 THEN entregas.caixa * 2 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 2 THEN entregas.fc * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 2 THEN entregas.carta * 1 ELSE 0 END +
                        CASE WHEN funcionarios.veiculo = 2 THEN entregas.caixa * 2 ELSE 0 END
                   ) +
                   COALESCE(adc_desc_total.PASS_COMB, 0) + 
                   COALESCE(adc_desc_total.ALIMENTAÇÃO, 0) - 
                   COALESCE(adc_desc_total.DESCONTO, 0) AS TOTAL
                FROM funcionarios
                JOIN entregas ON funcionarios.id = entregas.id_funcionario
                JOIN veiculos ON funcionarios.veiculo = veiculos.id_veiculo
                LEFT JOIN (
                     SELECT id_funcionario, 
                            SUM(passagem_combustivel) AS PASS_COMB, 
                            SUM(alimentacao) AS ALIMENTAÇÃO, 
                            SUM(desconto) AS DESCONTO
                     FROM adc_desc
                     WHERE adc_desc.data_inicio BETWEEN @dataInicio AND @dataFim
                     AND adc_desc.data_fim BETWEEN @dataInicio AND @dataFim
                     GROUP BY id_funcionario
                ) AS adc_desc_total ON funcionarios.id = adc_desc_total.id_funcionario
                WHERE entregas.data_entrega BETWEEN @dataInicio AND @dataFim";

            // Se veículo não foi selecionado (ou seja, "Todos"), não aplica o filtro
            if (objeto.veiculo != DBNull.Value.ToString())
            {
                query += " AND funcionarios.veiculo = @veiculo";
            }

            query += " GROUP BY nome, veiculos.veiculo;";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@dataInicio", objeto.dataInicio);
            cmd.Parameters.AddWithValue("@dataFim", objeto.dataFim);
            cmd.Parameters.AddWithValue("@veiculo", objeto.veiculo);

            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\log.txt", "Erro ao listar registros: " + ex.Message);
                throw new Exception("Erro ao listar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }
    }
}
