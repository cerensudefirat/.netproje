using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDB _context;
        public CommentRepository(ApplicationDB context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commetModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (commetModel == null)
            {
                return null;
            }
            _context.Comments.Remove(commetModel);
            await _context.SaveChangesAsync();
            return commetModel;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(a => a.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(a=>a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment = await _context.Comments.FindAsync(id);
            if (existingComment == null)
            {
                return null;
            }
            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;
            await _context.SaveChangesAsync();
            return existingComment; 
        }
    }
}