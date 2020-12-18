﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatriculaWebApplicationEF.ApplicationServices;
using MatriculaWebApplicationEF.DataContext;
using MatriculaWebApplicationEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatriculaWebApplicationEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaController : ControllerBase
    {
        private readonly UniversidadDataContext _baseDatos;
        private readonly MateriaAppService _materiaAppService;
        public MateriaController(UniversidadDataContext context, MateriaAppService materiaAppService)
        {
            _baseDatos = context;
            _materiaAppService = materiaAppService;

            if (_baseDatos.Materias.Count() == 0)
            {
                _baseDatos.Materias.Add(new Materia { Nombre = "Filosofia", CursoId = 1 });
                _baseDatos.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Materia>>> GetMaterias()
        {
            return await _baseDatos.Materias.Include(q => q.Curso).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Materia>> GetMateria(int id)
        {
            var materia = await _baseDatos.Materias.Include(q => q.Curso).FirstOrDefaultAsync(q => q.Id == id);

            if (materia == null)
            {
                return NotFound();
            }

            return materia;
        }

        [HttpPost]
        public async Task<ActionResult<Materia>> PostMateria(Materia materia)
        {
            var respuesta = await _materiaAppService.RegistrarMateria(materia);

            if (respuesta != null)
            {
                return BadRequest(respuesta);
            }

            return CreatedAtAction(nameof(GetMateria), new { id = materia.Id }, materia);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMateria(int id, Materia materia)
        {
            if (id != materia.Id)
            {
                return BadRequest();
            }

            Curso curso = await _baseDatos.Cursos.FirstOrDefaultAsync(q => q.Id == materia.CursoId);
            if (curso == null)
            {
                return NotFound("El curso no existe");
            }

            _baseDatos.Entry(materia).State = EntityState.Modified;
            await _baseDatos.SaveChangesAsync();

            return Ok("success");
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMateria(int id)
        {
            var materia = await _baseDatos.Materias.FindAsync(id);

            if (materia == null)
            {
                return NotFound();
            }

            _baseDatos.Materias.Remove(materia);
            await _baseDatos.SaveChangesAsync();

            return Ok("success");
        }
    }
}