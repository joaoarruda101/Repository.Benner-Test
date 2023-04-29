using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Estacionamento.Models;
using FurquimSite.DBContext;
using System.Data;
using Nest;
using Microsoft.IdentityModel.Tokens;

namespace Estacionamento.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly EstDbContext _context;

        public VeiculoController(EstDbContext context)
        {
            _context = context;
        }

        // GET: Veiculo
        public async Task<IActionResult> Index(string filtro)
        {
            return _context.Veiculo != null ?
                        View(await _context.Veiculo.ToListAsync()) :
                        Problem("Entity set 'EstDbContext.Veiculo'  is null.");
        }

        // GET: Veiculo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculoModel = await _context.Veiculo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculoModel == null)
            {
                return NotFound();
            }

            return View(veiculoModel);
        }

        // GET: Veiculo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veiculo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Placa,Entrada,Saida,Duracao,TempoCobrado,PrecoEstatacionamento,ValorPagar")] VeiculoModel veiculoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(veiculoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(veiculoModel);
        }

        // GET: Veiculo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculoModel = await _context.Veiculo.FindAsync(id);
            if (veiculoModel == null)
            {
                return NotFound();
            }
            return View(veiculoModel);
        }

        // POST: Veiculo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DateTime saida, [Bind("Id,Saida")] VeiculoModel veiculoModel)
        {

            if (id != veiculoModel.Id)
            {
                return NotFound();
            }

            try
            {
                var valorPagar = 0;

                var dados = _context.Veiculo.AsNoTracking()
                            .Where(whr => whr.Id == id).ToList();

                var entradaTime = _context.Veiculo.AsNoTracking()
                            .Where(whr => whr.Id == id)
                            .Select(slc => slc.Entrada).FirstOrDefault();

                foreach (var item in dados)
                {
                    var horas = saida.Hour - item.Entrada.Hour;
                    if (horas >= item.TempoCobrado)
                    {
                        var calculoValor = item.PrecoEstatacionamento * item.TempoCobrado;
                        veiculoModel.ValorPagar = calculoValor;
                    }
                    else
                    {
                        var calculoValor = item.PrecoEstatacionamento * horas;
                        veiculoModel.ValorPagar = calculoValor;
                    }
                }

                var placa = _context.Veiculo
                       .Where(p => p.Id == id)
                       .Select(p => p.Placa)
                       .FirstOrDefault();

                var tempoCobrado = _context.Veiculo
                      .Where(p => p.Id == id)
                      .Select(p => p.TempoCobrado)
                      .FirstOrDefault();

                var preco = _context.Veiculo
                      .Where(p => p.Id == id)
                      .Select(p => p.PrecoEstatacionamento)
                      .FirstOrDefault();

                TimeSpan horaAtual = DateTime.Now.TimeOfDay;
                var duracao = horaAtual - entradaTime.TimeOfDay;

                veiculoModel.Placa = placa;
                veiculoModel.Entrada = entradaTime;
                veiculoModel.Saida = saida;
                veiculoModel.Duracao = duracao;
                veiculoModel.TempoCobrado = tempoCobrado;
                veiculoModel.PrecoEstatacionamento = preco;

                _context.Update(veiculoModel);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VeiculoModelExists(veiculoModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            return View(veiculoModel);
        }

        // GET: Veiculo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Veiculo == null)
            {
                return NotFound();
            }

            var veiculoModel = await _context.Veiculo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculoModel == null)
            {
                return NotFound();
            }

            return View(veiculoModel);
        }

        // POST: Veiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Veiculo == null)
            {
                return Problem("Entity set 'EstDbContext.Veiculo'  is null.");
            }
            var veiculoModel = await _context.Veiculo.FindAsync(id);
            if (veiculoModel != null)
            {
                _context.Veiculo.Remove(veiculoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculoModelExists(int id)
        {
            return (_context.Veiculo?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Pesquisar(string filtro)
        {
            var query = _context.Veiculo.AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(whr => whr.Placa.Contains(filtro));
            }

            var placa = query.ToList();

            return View(placa);
        }
    }
}
