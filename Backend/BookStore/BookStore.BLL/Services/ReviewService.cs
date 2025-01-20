using BookStore.BLL.Services.Base;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using BookStore.DAL.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services
{
    public class ReviewService : BaseService<Review>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Review> _reviewRepository;

        public ReviewService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _reviewRepository = unitOfWork.GenericRepository<Review>();
        }

        public async Task<int> AddAsync(Review entity)
        {
            await _reviewRepository.AddAsync(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Review entity)
        {
            await _reviewRepository.UpdateAsync(entity);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var entity = _reviewRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _reviewRepository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _reviewRepository.GetAllAsync();
        }
    }

}
