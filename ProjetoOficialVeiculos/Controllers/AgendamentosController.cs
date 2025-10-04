using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControleOficialVeiculos.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using ProjetoOficialVeiculos.Models;
using System.Drawing;
using System.Text;
using System.Data;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoOficialVeiculos.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AgendamentosController : Controller
    {
        private readonly Contexto _context;

        public AgendamentosController(Contexto context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // Método para incluir dados relacionados
        private void PopulateDropDowns(Agendamento agendamento = null)
        {
            ViewData["caminhaoID"] = new SelectList(_context.Caminhoes, "id", "placa", agendamento?.caminhaoID);
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "nome", agendamento?.empresaID);
            ViewData["MaterialID"] = new SelectList(_context.Materiais, "id", "nome", agendamento?.MaterialID);
            ViewData["MotoristaID"] = new SelectList(_context.Motoristas, "id", "nome", agendamento?.MotoristaID);
        }

        // Método para exportar para Excel
        public IActionResult Exportar()
        {
            // Obtém os dados
            var dataTable = GetDados();

            // Cria o pacote Excel
            using (var package = new ExcelPackage())
            {
                // Cria a planilha
                var worksheet = package.Workbook.Worksheets.Add("Agendamentos");

                // Preenche a planilha com os dados
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                // Formatação de cabeçalho (em negrito e com fundo colorido)
                using (var range = worksheet.Cells[1, 1, 1, dataTable.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Formatação das células (bordas e ajuste de tamanho das colunas)
                worksheet.Cells.AutoFitColumns();

                // Adiciona bordas nas células da tabela
                using (var range = worksheet.Cells[1, 1, dataTable.Rows.Count + 1, dataTable.Columns.Count])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // Configura o nome do arquivo e a resposta
                var fileBytes = package.GetAsByteArray();
                var fileName = "Agendamentos.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // Método para preencher o DataTable com os dados dos agendamentos
        private DataTable GetDados()
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "Dados Veiculos";

            // Adiciona as colunas no DataTable
            dataTable.Columns.Add("CaminhaoID", typeof(string));
            dataTable.Columns.Add("Empresa", typeof(string));
            dataTable.Columns.Add("Material", typeof(string));
            dataTable.Columns.Add("DataAgendamento", typeof(DateTime));
            dataTable.Columns.Add("PesoCarregado", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataTable.Columns.Add("DataConclusao", typeof(DateTime));
            dataTable.Columns.Add("OrdemFila", typeof(int));
            dataTable.Columns.Add("Motorista", typeof(string));

            // Preenche o DataTable com dados dos agendamentos
            var dados = _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .Include(a => a.material)
                .Include(a => a.motorista)
                .ToList();

            foreach (var agendamento in dados)
            {
                dataTable.Rows.Add(
                    agendamento.caminhao?.placa,
                    agendamento.empresa?.nome,
                    agendamento.material?.nome,
                    agendamento.DataAgendamento,
                    agendamento.PesoCarregado,
                    agendamento.Status.ToString(),
                    agendamento.DataConclusao,
                    agendamento.OrdemFila,
                    agendamento.motorista?.nome
                );
            }

            return dataTable;
        }

        // GET: Agendamentos (com filtros de data e status)
        public async Task<IActionResult> Index(DateTime? dataAgendamento, DateTime? dataConclusao, string status)
        {
            var contexto = _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .Include(a => a.material)
                .Include(a => a.motorista)
                .OrderBy(a => a.DataAgendamento)
                .AsQueryable();

            // Filtro por data, se fornecida
            if (dataAgendamento.HasValue)
            {
                contexto = contexto.Where(a => a.DataAgendamento.Date == dataAgendamento.Value.Date);
            }

            if (dataConclusao.HasValue)
            {
                contexto = contexto.Where(a => a.DataConclusao.Date == dataConclusao.Value.Date);
            }



            // Filtro por status, se fornecido
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusAgendamento>(status, true, out var statusEnum))
            {
                contexto = contexto.Where(a => a.Status == statusEnum);
            }

            ViewBag.StatusList = new SelectList(new List<string> { "Aguardando Liberação", "Concluido", "Cancelado" , "Em Carregamento",  });

            return View(await contexto.ToListAsync());
        }
        
        // GET: Agendamentos/Create
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View(new Agendamento());
        }

        // POST: Agendamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,caminhaoID,empresaID,MaterialID,DataAgendamento,PesoCarregado,Status,DataConclusao,OrdemFila, MotoristaID")] Agendamento agendamento)
        {
            if (ModelState.IsValid)

            {
                if (agendamento.caminhaoID == 0)  // Se o ID do caminhão for zero (não selecionado)
                {
                    // Redireciona para a tela de criação do caminhão
                    return RedirectToAction("Create", "Caminhoes");
                }
                if (agendamento.PesoCarregado == "-1")
                {
                    agendamento.PesoCarregado = null; // Define como nulo se for -1
                }

                if (agendamento.MotoristaID == -1)
                {
                    agendamento.MotoristaID = null; // No banco, ele será tratado como null
                }

                // Verifica a maior ordem existente ou define a ordem inicial como 1
                int ordemMaxima = _context.Agendamentos.Any() ? _context.Agendamentos.Max(a => a.OrdemFila) : 0;
                agendamento.OrdemFila = ordemMaxima + 1;

                // Adiciona o agendamento ao contexto
                _context.Add(agendamento);
                await _context.SaveChangesAsync();

                // Reordena a fila
                await ReordenarFila();

                return RedirectToAction(nameof(Index));
            }

            // Preenche os dropdowns novamente em caso de erro de validação
            PopulateDropDowns(agendamento);
            return View(agendamento);
        }

        // Método para reordenar a fila após cada inserção
        private async Task ReordenarFila()
        {
            // Pega todos os agendamentos ordenados pela data de agendamento
            var agendamentos = await _context.Agendamentos.OrderBy(a => a.DataAgendamento).ToListAsync();

            int i = 1;
            foreach (var agendamento in agendamentos)
            {
                // Reatribui a ordem da fila
                agendamento.OrdemFila = i++;
            }

            // Atualiza os agendamentos no banco
            _context.UpdateRange(agendamentos);
            await _context.SaveChangesAsync();
        }

        // GET: Agendamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
            {
                return NotFound();
            }

            PopulateDropDowns(agendamento); // Popula os dropdowns com os dados existentes
            return View(agendamento);
        }

        // POST: Agendamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,caminhaoID,empresaID,MaterialID,DataAgendamento,PesoCarregado,Status,DataConclusao,OrdemFila, MotoristaID")] Agendamento agendamento)
        {
            if (id != agendamento.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendamentoExists(agendamento.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Reordena a fila após a edição
                await ReordenarFila();

                return RedirectToAction(nameof(Index));
            }

            PopulateDropDowns(agendamento);
            return View(agendamento);
        }

        [HttpPost]
        public async Task<IActionResult> AddCaminhao(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                return BadRequest("Placa inválida.");
            }

            // Cria um novo objeto Caminhao e salva no banco de dados
            var caminhao = new Caminhao
            {
                placa = placa
            };

            _context.Caminhoes.Add(caminhao);
            await _context.SaveChangesAsync();

            // Retorna o novo caminhão com seu ID
            return Json(new { id = caminhao.id, placa = caminhao.placa });
        }


        // GET: Agendamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .Include(a => a.material)
                .Include(a => a.motorista)
                .FirstOrDefaultAsync(m => m.id == id);
            if (agendamento == null)
            {
                return NotFound();
            }

            return View(agendamento);
        }

        // POST: Agendamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
            }

            await _context.SaveChangesAsync();
            await ReordenarFila();
            return RedirectToAction(nameof(Index));
        }

        private bool AgendamentoExists(int id)
        {
            return _context.Agendamentos.Any(e => e.id == id);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.caminhao)       
                .Include(a => a.empresa)       
                .Include(a => a.material)      
                .Include(a => a.motorista)      
                .FirstOrDefaultAsync(m => m.id == id);

            if (agendamento == null)
            {
                return NotFound();
            }

            return View(agendamento);
        }


        public async Task<IActionResult> List(DateTime? dataAgendamento, string status)
        {
            var contexto = _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .Include(a => a.material)
                .Include(a => a.motorista)
                .OrderBy(a => a.DataAgendamento)
                .AsQueryable();

            // Filtro por data, se fornecida
            if (dataAgendamento.HasValue)
            {
                contexto = contexto.Where(a => a.DataAgendamento.Date == dataAgendamento.Value.Date);
            }

            // Filtro por status, se fornecido
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusAgendamento>(status, true, out var statusEnum))
            {
                contexto = contexto.Where(a => a.Status == statusEnum);
            }

            ViewBag.StatusList = new SelectList(new List<string> { "Aguardando Liberação", "Concluido", "Cancelado" , "Em Carregamento", });

            return View(await contexto.ToListAsync());
        }

        public IActionResult Dashboard(List<int> meses, int? ano)
        {
            var anoAtual = ano ?? DateTime.Now.Year;

            // Se nenhum mês for selecionado, considera todos os meses (1 a 12)
            if (meses == null || meses.Count == 0)
            {
                meses = Enumerable.Range(1, 12).ToList();
            }

            // Filtra os agendamentos com base nos meses selecionados e no ano
            var agendamentosMes = _context.Agendamentos
                .Include(a => a.empresa)
                .Where(a => meses.Contains(a.DataAgendamento.Month) && a.DataAgendamento.Year == anoAtual)
                .ToList();

            // Verificar se o filtro trouxe algum dado
            if (!agendamentosMes.Any())
            {
                ViewBag.Message = "Nenhum agendamento encontrado para o filtro selecionado.";
            }

            // Prepara os dados para os gráficos
            ViewBag.StatusData = new
            {
                total = agendamentosMes.Count,
                pendente = agendamentosMes.Count(a => a.Status == StatusAgendamento.Pendente),
                concluido = agendamentosMes.Count(a => a.Status == StatusAgendamento.Concluido),
                cancelado = agendamentosMes.Count(a => a.Status == StatusAgendamento.Cancelado),
                carregamento = agendamentosMes.Count(a => a.Status == StatusAgendamento.Carregamento),
                empresasData = agendamentosMes
                    .GroupBy(a => a.empresa.nome)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Count())
                    .ToArray(),
                empresasLabels = agendamentosMes
                    .GroupBy(a => a.empresa.nome)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Key)
                    .ToArray(),
                porDia = agendamentosMes
                    .GroupBy(a => a.DataAgendamento.Day)
                    .OrderBy(g => g.Key)
                    .Select(g => g.Count())
                    .ToArray()
            };

            // Empresas agrupadas para gráfico de barras
            ViewBag.Empresas = agendamentosMes
                .GroupBy(a => a.empresa.nome)
                .OrderBy(g => g.Key)
                .Select(g => g.Key)
                .ToList();

            // Passa o ano e os meses selecionados para a View
            ViewBag.Ano = anoAtual;
            ViewBag.Meses = meses;

            // Exemplo básico: você pode adaptar essa lógica para armazenar a preferência do modo escuro no banco/cookie/localstorage
            ViewBag.IsDarkMode = false;

            return View();
        }



    }

}
