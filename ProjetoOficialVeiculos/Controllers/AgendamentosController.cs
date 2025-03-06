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

namespace ProjetoOficialVeiculos.Controllers
{
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
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "cnpj", agendamento?.empresaID);
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
        public async Task<IActionResult> Index(DateTime? dataAgendamento, string status)
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
            if (!string.IsNullOrEmpty(status) && System.Enum.TryParse<StatusAgendamento>(status, true, out var statusEnum))
            {
                contexto = contexto.Where(a => a.Status == statusEnum);
            }

            ViewBag.StatusList = new SelectList(new List<string> { "Pendente", "Concluido", "Cancelado" });

            return View(await contexto.ToListAsync());
        }

        // GET: Agendamentos/Create
        public IActionResult Create()
        {
            var agendamento = new Agendamento();
            PopulateDropDowns();
            return View(agendamento);
        }

        // POST: Agendamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,caminhaoID,empresaID,MaterialID,DataAgendamento,PesoCarregado,Status,DataConclusao,OrdemFila, MotoristaID")] Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
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
    }
}
