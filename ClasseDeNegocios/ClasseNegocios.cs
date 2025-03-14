using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClasseDeDados;
using ClasseDeEntidades;

namespace ClasseDeNegocios
{
    public class ClasseNegocios
    {
        ClasseDados classeDados = new ClasseDados();
        public DataTable Nlogin(ClasseEntidades objeto)
        {
            return classeDados.Dlogin(objeto);
        }

        public DataTable NlistarEntregas()
        {
            return classeDados.DlistarEntregas();
        }

        public DataTable NlistarFuncionarios()
        {
            return classeDados.DlistarFuncionarios();
        }

        public DataTable NbuscarEntregas(ClasseEntidades objeto)
        {
            return classeDados.DbuscarEntregas(objeto);
        }

        public string NCRUDentregas(ClasseEntidades objeto)
        {
            return classeDados.DmanutencaoEntregas(objeto);
        }

        public DataTable NlistarInfosFuncionarios()
        {
            return classeDados.DlistarInfosFuncionarios();
        }

        public DataTable NlistarVeiculos()
        {
            return classeDados.DlistarVeiculos();
        }

        public DataTable NbuscarFuncionarios(ClasseEntidades objeto)
        {
            return classeDados.DbuscarFuncionarios(objeto);
        }

        public string NCRUDfuncionarios(ClasseEntidades objeto)
        {
            return classeDados.DmanutencaoFuncionarios(objeto);
        }

        public (DataTable, DataTable) NbuscarDadosEntregas(ClasseEntidades objeto)
        {
            DataTable dtTotal = classeDados.DbuscarDadosEntregas(objeto);
            DataTable dtDetalhado = classeDados.DbuscarDadosEntregasDetalhadas(objeto);

            return (dtTotal, dtDetalhado);
        }

        public DataTable NlistarAdcDesc()
        {
            return classeDados.DlistarAdcDesc();
        }

        public string NCRUDadcdesc(ClasseEntidades objeto)
        {
            return classeDados.DmanutencaoAdcdesc(objeto);
        }

        public DataTable NbuscarAdc_desc(ClasseEntidades objeto)
        {
            return classeDados.DbuscarAdc_desc(objeto);
        }

        // MÉTODO RELATÓRIO
        public DataTable NlistarRelatorio()
        {
            return classeDados.DlistarRelatorio();
        }

        public DataTable NlistarFuncionariosMoto()
        {
            return classeDados.DlistarFuncionariosMoto();
        }

        public DataTable NlistarFuncionariosBicicleta()
        {
            return classeDados.DlistarFuncionariosBicicleta();
        }

        public DataTable NlistarFuncionariosCarro()
        {
            return classeDados.DlistarFuncionariosCarro();
        }

        public DataTable NlistarFuncionariosRelatorio(ClasseEntidades objeto)
        {
            return classeDados.DlistarFuncionariosRelatorio(objeto);
        }

        public DataTable NlistarFuncionariosFiltro(ClasseEntidades objeto)
        {
            return classeDados.DlistarFuncionariosFiltro(objeto);
        }
    }
}
