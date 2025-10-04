using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControleOficialVeiculos.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficialVeiculos.Models;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace ProjetoOficialVeiculos.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AbastecimentoController : Controller
    {
        private readonly Contexto _context;
        private readonly ILogger<AbastecimentoController> _logger;

        public AbastecimentoController(Contexto context)
        {
            _context = context;
            
        }

        // GET: Abastecimento

        public async Task<IActionResult> Index(DateTime? dataAgendamento, DateTime? dataConclusao, string status, string caminhaoID)
        {
            var contexto = _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .Include(a => a.material)
                .Include(a => a.motorista)
                .OrderBy(a => a.DataAgendamento)
                .Where(a => a.Status != StatusAgendamento.Faturamento)

                .AsQueryable();

            // Filtro por data de agendamento
            if (dataAgendamento.HasValue)
            {
                contexto = contexto.Where(a => a.DataAgendamento.Date == dataAgendamento.Value.Date);
            }

            if (dataConclusao.HasValue)
            {
                contexto = contexto.Where(a => a.DataConclusao.Date == dataConclusao.Value.Date);
            }


            // Filtro por status
            if (string.IsNullOrEmpty(status))
            {
                status = "Aguardando Liberação";  // valor padrão se o status não for fornecido
            }

            if (Enum.TryParse<StatusAgendamento>(status, true, out var statusEnum))
            {
                contexto = contexto.Where(a => a.Status == statusEnum);
            }

            // Filtro por nome do caminhão
            if (!string.IsNullOrEmpty(caminhaoID))
            {
                contexto = contexto.Where(a => a.caminhao.placa.Contains(caminhaoID)); // Filtro baseado no nome do caminhão
            }

            // Verificando se StatusList está sendo passado corretamente para a View
            ViewBag.StatusList = new SelectList(new List<string>
            {
                 "Pendente",
                 "Concluido",
                 "Carregamento",
                 "Cancelada",
             }, status);

            // Certificando-se de que o ViewBag.Caminhoes está sendo preenchido corretamente
            var caminhões = await _context.Caminhoes.ToListAsync();
            ViewBag.Caminhoes = new SelectList(caminhões, "Id", "Placa");

            // Retornando a lista filtrada
            return View(await contexto.ToListAsync());
        }




        // GET: Abastecimento/Details/5
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

        // GET: Abastecimento/Create
        public IActionResult Create()
        {
            ViewData["caminhaoID"] = new SelectList(_context.Caminhoes, "id", "placa");
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "cnpj");
            ViewData["MaterialID"] = new SelectList(_context.Materiais, "id", "nome");
            ViewData["MotoristaID"] = new SelectList(_context.Motoristas, "id", "nome");
            return View();
        }

        // POST: Abastecimento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,caminhaoID,empresaID,MaterialID,DataAgendamento,PesoCarregado,Status,DataConclusao,OrdemFila,MotoristaID")] Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agendamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["caminhaoID"] = new SelectList(_context.Caminhoes, "id", "placa", agendamento.caminhaoID);
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "cnpj", agendamento.empresaID);
            ViewData["MaterialID"] = new SelectList(_context.Materiais, "id", "nome", agendamento.MaterialID);
            ViewData["MotoristaID"] = new SelectList(_context.Motoristas, "id", "nome", agendamento.MotoristaID);
            return View(agendamento);
        }

        // GET: Abastecimento/Edit/5
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

            // Carregar lista de motoristas
            var motoristas = await _context.Motoristas.ToListAsync();

            // Verificar se o motorista já existe na tabela
            var motoristaExistente = motoristas.FirstOrDefault(m => m.id == agendamento.MotoristaID);

            // Se o motorista não existir, permitir inserir um novo
            if (motoristaExistente == null)
            {
                ViewData["MostrarCampoNovoMotorista"] = true; // Variável para exibir o campo para novo motorista
            }
            else
            {
                ViewData["MostrarCampoNovoMotorista"] = false;
            }

            // Carregar os dados para o dropdown de motoristas
            ViewData["caminhaoID"] = new SelectList(_context.Caminhoes, "id", "placa", agendamento.caminhaoID);
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "cnpj", agendamento.empresaID);
            ViewData["MaterialID"] = new SelectList(_context.Materiais, "id", "nome", agendamento.MaterialID);
            ViewData["MotoristaID"] = new SelectList(_context.Motoristas, "id", "nome", agendamento.MotoristaID);

            return View(agendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,caminhaoID,empresaID,MaterialID,DataAgendamento,PesoCarregado,Status,DataConclusao,OrdemFila,MotoristaID")] Agendamento agendamento, string novoMotoristaNome)
        {
            if (id != agendamento.id)
            {
                return NotFound();
            }

            // Obter o agendamento atual do banco de dados para comparar o status
            var agendamentoAtual = await _context.Agendamentos.AsNoTracking().FirstOrDefaultAsync(a => a.id == id);

            // Se foi informado o nome de um novo motorista, criar o motorista
            if (!string.IsNullOrEmpty(novoMotoristaNome))
            {
                var novoMotorista = new Motorista { nome = novoMotoristaNome };
                _context.Motoristas.Add(novoMotorista);
                await _context.SaveChangesAsync();  // Salvar para gerar o ID do novo motorista

                agendamento.MotoristaID = novoMotorista.id;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendamento);
                    await _context.SaveChangesAsync();

                    // Verificar se o status foi alterado para Faturamento
                    if (agendamentoAtual != null && agendamentoAtual.Status != agendamento.Status &&
                        agendamento.Status == StatusAgendamento.Faturamento)
                    {
                        // Enviar email apenas quando o status for alterado para Faturamento
                        var emailService = new ProjetoOficialVeiculos.Services.EmailService();

                        string placa = _context.Caminhoes
                            .Where(c => c.id == agendamento.caminhaoID)
                            .Select(c => c.placa)
                            .FirstOrDefault();

                        string corpo = $"🚛 Abastecimento Finalizado para Faturamento!\n\n" +
                                       $"📅 Data: {agendamento.DataAgendamento:dd/MM/yyyy HH:mm}\n" +
                                       $"🚚 Caminhão: {placa}\n" +
                                       $"⛽ Peso Carregado: {agendamento.PesoCarregado} Kg\n" +
                                       $"✅ Status: Finalizado Faturamento";

                        await emailService.EnviarEmailAsync("rafael.souza@aguabonita.com.br", "Notificação de Abastecimento Finalizado para Faturamento", corpo);
                    }
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

                return RedirectToAction(nameof(Index));
            }

            // Se ModelState não é válido, recarrega dropdowns e retorna a View para corrigir erros
            ViewData["caminhaoID"] = new SelectList(_context.Caminhoes, "id", "placa", agendamento.caminhaoID);
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "cnpj", agendamento.empresaID);
            ViewData["MaterialID"] = new SelectList(_context.Materiais, "id", "nome", agendamento.MaterialID);
            ViewData["MotoristaID"] = new SelectList(_context.Motoristas, "id", "nome", agendamento.MotoristaID);

            return View(agendamento);
        }


        // GET: Abastecimento/Delete/5
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

        // POST: Abastecimento/Delete/5
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
            return RedirectToAction(nameof(Index));
        }

        private bool AgendamentoExists(int id)
        {
            return _context.Agendamentos.Any(e => e.id == id);
        }

        public IActionResult ChamarParaBalanca(int id)
        {
            var abastecimento = _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .FirstOrDefault(a => a.id == id);

            if (abastecimento == null)
            {
                return NotFound();
            }

            return View(abastecimento);
        }

        public async Task<IActionResult> listAbastecimento(DateTime? dataConclusao, string status)
        {
            var statusPermitidos = new[]
            {
        StatusAgendamento.Concluido,
        StatusAgendamento.Cancelado,
        StatusAgendamento.Carregamento
    };

            var contexto = _context.Agendamentos
                .Include(a => a.caminhao)
                .Include(a => a.empresa)
                .Include(a => a.material)
                .Include(a => a.motorista)
                // filtro fixo: só status permitidos
                .Where(a => statusPermitidos.Contains(a.Status))
                .OrderBy(a => a.DataConclusao)
                .AsQueryable();

            // filtro opcional por data
            if (dataConclusao.HasValue)
            {
                contexto = contexto.Where(a => a.DataConclusao.Date == dataConclusao.Value.Date);
            }

            // filtro opcional por status, só se estiver dentro dos permitidos
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusAgendamento>(status, true, out var statusEnum)
                && statusPermitidos.Contains(statusEnum))
            {
                contexto = contexto.Where(a => a.Status == statusEnum);
            }

            ViewBag.StatusList = new SelectList(new List<string> { "Concluido", "Cancelado", "Carregamento" });

            return View(await contexto.ToListAsync());
        }


        public async Task<IActionResult> EditarAbastecimento(int? id)
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

            // Lista restrita de status
            ViewBag.StatusRestrito = new SelectList(new[]
            {
        new { Value = "Concluido", Text = "Concluído" },
        new { Value = "Carregamento", Text = "Em Carregamento" }
    }, "Value", "Text", agendamento.Status.ToString());

            return View(agendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAbastecimento(int id, [Bind("Status")] Agendamento agendamento)
        {
            var agendamentoExistente = await _context.Agendamentos.FindAsync(id);
            if (agendamentoExistente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    agendamentoExistente.Status = agendamento.Status;

                    // Define a DataConclusao se for concluído
                    if (agendamento.Status == StatusAgendamento.Concluido)
                    {
                        agendamentoExistente.DataConclusao = DateTime.Now;
                    }

                    _context.Update(agendamentoExistente);
                    await _context.SaveChangesAsync();

                    // Mensagem de sucesso
                    TempData["SuccessMessage"] = "Status do abastecimento atualizado com sucesso!";
                    return RedirectToAction("DetalhesAbastecimento", new { id = agendamentoExistente.id });
                }
                catch (DbUpdateException ex)
                {
                    // Log do erro (opcional)
                    _logger.LogError(ex, "Erro ao atualizar status do abastecimento");

                    ModelState.AddModelError("", "Ocorreu um erro ao salvar as alterações. Por favor, tente novamente.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao atualizar status");
                    ModelState.AddModelError("", "Ocorreu um erro inesperado. Contate o suporte técnico.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Por favor, corrija os erros no formulário.");
            }

            // Recarrega a lista restrita se algo falhar
            ViewBag.StatusRestrito = new SelectList(new[]
            {
        new { Value = "Concluido", Text = "Concluído" },
        new { Value = "Carregamento", Text = "Em Carregamento" }
    }, "Value", "Text", agendamento.Status.ToString());

            return View(agendamentoExistente);
        }
        public async Task<IActionResult> DetalhesAbastecimento(int? id)
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



    }


}
