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
                var entrada = await _context.Veiculo.AsNoTracking()
                            .Where(whr => whr.Id == id)
                            .Select(slc => slc.Entrada).ToListAsync();

                foreach (var estacionado in entrada)
                {

                    var subtracaoDatas = DateTime.Now - estacionado;
                    if (subtracaoDatas.Minutes < 30 || subtracaoDatas.Minutes > 59)
                    {
                        valorPagar += 1;
                    }
                    if (subtracaoDatas.Hours == 1 && subtracaoDatas.Minutes <= 10)
                    {
                        valorPagar += 2;
                    }
                    if (subtracaoDatas.Hours == 1 && subtracaoDatas.Minutes >= 15)
                    {
                        valorPagar += 3;
                    }
                    if (subtracaoDatas.Hours == 2 && subtracaoDatas.Minutes <= 5)
                    {
                        valorPagar += 3;
                    }
                    if (subtracaoDatas.Hours == 2 && subtracaoDatas.Minutes >= 15)
                    {
                        valorPagar += 4;
                    }
                }

                var placa = _context.Veiculo
                       .Where(p => p.Id == id)
                       .Select(p => p.Placa)
                       .FirstOrDefault();

                var entradaTime = _context.Veiculo.AsNoTracking()
                            .Where(whr => whr.Id == id)
                            .Select(slc => slc.Entrada).FirstOrDefault();

                TimeSpan horaAtual = DateTime.Now.TimeOfDay;
                var duracao = horaAtual - entradaTime.TimeOfDay;

                veiculoModel.Placa = placa;
                veiculoModel.Entrada = entradaTime;
                veiculoModel.Saida = saida;
                veiculoModel.ValorPagar = valorPagar;
                veiculoModel.Duracao = duracao;

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
