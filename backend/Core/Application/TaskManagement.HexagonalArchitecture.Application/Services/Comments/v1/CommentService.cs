using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Comments.v1
{
    public class CommentService(IUnitOfWork unitOfWork) : ICommentService
    {
        public async Task<CustomResult<Comment>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(id, cancellationToken);

            if (comment is null)
                return CustomResult<Comment>.Failure(new CustomError("CommentNotFound", "Comment not found."));

            return CustomResult<Comment>.Success(comment);
        }

        public async Task<CustomResult<Comment>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(id, cancellationToken);

            if (comment is null)
                return CustomResult<Comment>.Failure(new CustomError("CommentNotFound", "Comment not found."));

            unitOfWork.Comments.Remove(comment);

            await unitOfWork.SaveChanges(cancellationToken);

            return CustomResult<Comment>.Success(comment);
        }

        public async Task<CustomResult<List<Comment>>> ListByUserIdAsync(Guid id,
            CancellationToken cancellationToken)
        {
            var comments = await unitOfWork.Comments.ListByUserIdAsync(id, cancellationToken);

            if (comments is null or { Count: 0 })
                return CustomResult<List<Comment>>.Failure(new CustomError("CommentsNotFound",
                    "Comments not found."));

            return CustomResult<List<Comment>>.Success(comments);
        }
        
        public async Task<CustomResult<List<Comment>>> ListByAssignmentIdAsync(Guid id,
            CancellationToken cancellationToken)
        {
            var comments = await unitOfWork.Comments.ListByAssignmentIdAsync(id, cancellationToken);

            if (comments is null or { Count: 0 })
                return CustomResult<List<Comment>>.Failure(new CustomError("CommentsNotFound",
                    "Comments not found."));

            return CustomResult<List<Comment>>.Success(comments);
        }

        public async Task<CustomResult<Comment>> UpdateAsync(Guid id, string description,
            CancellationToken cancellationToken)
        {
            var resultGet = await GetAsync(id, cancellationToken);

            if (resultGet.IsFailure)
                return resultGet;

            var comment = resultGet.Value;

            comment.Update(description);

            unitOfWork.Comments.Update(comment);

            await unitOfWork.SaveChanges(cancellationToken);

            return CustomResult<Comment>.Success(comment);
        }

        public async Task<CustomResult<Comment>> CreateAsync(string description, Guid assignmentId, Guid userId,
            CancellationToken cancellationToken)
        {
            var comment = new Comment(description, assignmentId, userId);

            unitOfWork.Comments.Add(comment);

            await unitOfWork.SaveChanges(cancellationToken);

            return CustomResult<Comment>.Success(comment);
        }
    }
}