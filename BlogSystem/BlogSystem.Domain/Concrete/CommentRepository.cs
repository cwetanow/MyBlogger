﻿using BlogSystem.Domain.Concrete.Abstract;
using BlogSystem.Domain.Contracts;
using System.Collections.Generic;
using BlogSystem.Domain.Models;
using System.Data.Entity;
using System.Linq;
using BlogSystem.Domain.Contracts.Entities;

namespace BlogSystem.Domain.Concrete
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(IApplicationDbContext dbContext) : base(dbContext)
        {
        }

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

        public IEnumerable<IComment> GetPostCommentsById(int postId)
        {
            var postComments = this.Comments
                .Where(c => !c.IsDeleted && c.Post.PostId == postId)
                .OrderByDescending(x => x.Date);

            return postComments;
        }

        public IEnumerable<IComment> GetCommentsFromIds(IEnumerable<int> commentIds)
        {
            var comments = this.Comments
                .Where(c => !c.IsDeleted && commentIds.Contains(c.CommentId))
                .OrderByDescending(x => x.Date);

            return comments;
        }

        public IEnumerable<IComment> GetUserComments(string userId)
        {
            var comments = this.Comments
                  .Where(p => p.Author.Id == userId && !p.IsDeleted);

            return comments;
        }

        public void WriteComment(Comment comment, int postId, string authorId)
        {
            var post = this.GetPostById(postId);
            comment.Post = post;

            var author = this.GetUserById(authorId);
            comment.Author = author;

            this.context.Comments.Add(comment);
            post.Comments.Add(comment);

            this.SaveChanges();
        }


    }
}
