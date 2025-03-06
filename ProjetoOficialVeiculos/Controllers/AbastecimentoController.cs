using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControleOficialVeiculos.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficialVeiculos.Models;

namespace ProjetoOficialVeiculos.Controllers
{
    public class AbastecimentoController : Controller
    {
        private readonly Contexto _context;

        public AbastecimentoController(Contexto context)
        {
            _context = context;
        }

        // GET: Abastecimento
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Agendamentos.Include(a => a.caminhao).Include(a => a.empresa).Include(a => a.material).Include(a => a.motorista)
                 .Where(a => a.Status == StatusAgendamento.Pendente); 
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["caminhaoID"] = new SelectList(_context.Caminhoes, "id", "placa", agendamento.caminhaoID);
            ViewData["empresaID"] = new SelectList(_context.Empresas, "id", "cnpj", agendamento.empresaID);
            ViewData["MaterialID"] = new SelectList(_context.Materiais, "id", "nome", agendamento.MaterialID);
            ViewData["MotoristaID"] = new SelectList(_context.Motoristas, "id", "nome", agendamento.MotoristaID);
            return View(agendamento);
        }

        // POST: Abastecimento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,caminhaoID,empresaID,MaterialID,DataAgendamento,PesoCarregado,Status,DataConclusao,OrdemFila,MotoristaID")] Agendamento agendamento)
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
                return RedirectToAction(nameof(Index));
            }
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
    }
}
