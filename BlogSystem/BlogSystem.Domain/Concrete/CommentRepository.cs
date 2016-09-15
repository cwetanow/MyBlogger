﻿using BlogSystem.Domain.Concrete.Abstract;
using BlogSystem.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Domain.Models;
using System.Data.Entity;
using BlogSystem.Domain.Contracts.Entities;
using BlogSystem.Domain.Utils;

namespace BlogSystem.Domain.Concrete
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public IEnumerable<IComment> Comments
        {
            get
            {
                return this.context.Comments
                    .Include(c => c.Author)
                    .Include(c => c.Post);
            }
        }

        public void DeleteComment(int commentId)
        {
            var commentToDelete = this.context.Comments.Find(commentId);

            if (commentToDelete != null)
            {
                commentToDelete.IsDeleted = true;
            }

            this.SaveChanges();
        }

        public void WriteComment(Comment comment, int postId, string authorId)
        {
            var post = this.GetPostById(postId);
            var author = this.GetUserById(authorId);

            comment.Post = post;
            comment.Author = author;

            this.context.Comments.Add(comment);
            post.Comments.Add(comment);

            this.SaveChanges();
        }
    }
}
